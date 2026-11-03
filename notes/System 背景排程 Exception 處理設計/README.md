# System 背景排程 Exception 處理設計

- 背景：看過兩種不同的 exception 處理設計，DAO 層一律 catch 轉 `Result`、Client 層刻意不 catch 讓它炸出去當斷路器，用下來發現兩種都有不適用的地方，在這裡整理個人心得

## 兩種看過的設計

### 設計一 - DAO 層一律 catch 轉成 Result

- 形式是主張「自己寫的應用層不該丟 exception」，每個 repository 方法都呼叫包了 try/catch 的父類別方法，記完 log 回 `Result<T>`
- 一開始覺得呼叫端永遠拿到值，不用擔心哪裡會炸出來，但實際用一陣子之後，不適用的地方陸續出現：
  - 呼叫端拿到 `Failure` 之後做的事都一樣，都是 rollback 之後回傳失敗訊息給使用者
  - 「無資料更新」也被算入 `Failure`：這種狀況到底算是「業務上的否定答案」還是「技術上的失敗」，需要看呼叫端的業務邏輯決定
  - 如果有人忘記檢查 `Result` 或是不知道這個設計，後續的程式碼還是會執行，此時若是在交易中就會導致資料被錯誤儲存

### 設計二 - Client 層刻意不 catch 達到斷路效果

- 排程逐筆呼叫對外 API，對外呼叫的 Client 刻意不包 try/catch，讓任何的非預期結果直接炸到宿主層整輪就停了
- 這招在某些場合確實可用，但拿它當批次斷路器，不適用的地方也很明顯：
  - 門檻是「第一個倒楣的呼叫」，不是「連續失敗 N 次」，難以自行控制時機
  - 如果要分出「對方掛了」、「程式有 bug」或是更多的狀況，就需要讓呼叫端自行處理 HTTP 的細節
  - 斷路範圍由 try/catch 形狀決定，中間任何一層為了別的原因加了 try/catch 都可能導致斷路就靜默失效

## 呼叫端需不需要那個資訊

- 兩種設計碰到的其實是同一件事的兩面：包不包 try/catch，取決於呼叫端拿到那個資訊之後，做不做得出不一樣的事
- 這也解釋了為什麼兩種設計都是「層」的規則，卻都有些不適合：
  - DAO 層：**技術性失敗**的分類對呼叫端沒有用，最後幾乎都是 rollback
  - Client 層：**排程的呼叫端真的有選擇**（跳下一筆、斷路跳出、重試等等），拋掉就等於要在更上層處理

## 套件不幫分類? 因為套件不認識呼叫端

- `HttpClient`、`SqlConnection` 這些套件的錯誤處理設計都是拋出，因提供者不可能知道呼叫端是誰，所以套件作者唯一能做的正確選擇就是**丟一個誠實的 exception**：語意是「我無法履行契約」，「所以接下來怎麼辦」只有應用端能決定

- 另外一點也值得翻過來想：`HttpRequestException` 本身並不代表「意外」，對整輪排程來說，斷線本來就可以是契約的一部分

## 實現範例

- 分類要用呼叫端的決策語彙，每個值都要能回答呼叫端因此做什麼不一樣的事

| 分類               | 代表什麼                   | 呼叫端該做什麼                 |
| ------------------ | ------------------------ | ---------------------------- |
| `BadRequest`       | 4xx，我送的內容不合        | 重試沒用，通知人去改             |
| `ServerError`      | 5xx，對方內部錯誤          | 可重試                        |
| `Timeout`          | 逾時                     | 可重試，可算連續次數             |
| `Network`          | 斷線                     | 可重試，可能是對方掛了或是網路問題 |
| `UnexpectedFormat` | HTTP 成功但回應讀不懂      | 依業務情境決定如何處理           |

```csharp
public enum CallFailure
{
    None = 0,
    BadRequest,
    ServerError,
    Timeout,
    Network,
    UnexpectedFormat,
}

public sealed record CallResult<T>(T? Value, int? StatusCode, string? ErrorMessage, CallFailure Failure)
{
    public bool Succeeded => Failure == CallFailure.None;

    /// <summary> 重試有機會成功。4xx 與格式錯不在其中：重送同一個請求不會變成功 </summary>
    public bool IsRetryable => Failure is CallFailure.ServerError or CallFailure.Timeout or CallFailure.Network;

    /// <summary> 對方或線路整體不通，繼續跑下一筆是沒意義的 </summary>
    public bool IsTransportDown => Failure is CallFailure.Timeout or CallFailure.Network;

    // ... 其他屬性
}
```

```csharp
private async Task<CallResult<T>> SendAsync<T>(string path, Func<HttpRequestMessage> buildRequest, Func<JsonElement, T> read, CancellationToken ct)
{
    var startedAt = _clock.GetUtcNow();
    var sw = Stopwatch.StartNew();
    var method = "?";

    try
    {
        using var request = buildRequest();
        method = request.Method.Method;

        using var response = await _http.SendAsync(request, ct);
        var body = await response.Content.ReadAsStringAsync(ct);
        var status = (int)response.StatusCode;

        if (!response.IsSuccessStatusCode)
        {
            var kind = status is >= 400 and < 500 ? CallFailure.BadRequest : CallFailure.ServerError;
            var message = TryReadErrorMessage(body);
            Audit(path, method, status, "FAILED", message, sw, startedAt);
            return CallResult<T>.Fail(kind, status, message);
        }

        try
        {
            using var document = JsonDocument.Parse(body);
            var value = read(document.RootElement);
            Audit(path, method, status, "SUCCESS", null, sw, startedAt);
            return CallResult<T>.Ok(value, status);
        }
        // 回應讀不懂：分類成 UnexpectedFormat 讓呼叫端知道「重試沒用」
        catch (Exception ex) when (ex is JsonException or KeyNotFoundException or FormatException or InvalidOperationException)
        {
            Audit(path, method, status, "EXCEPTION", $"回應格式非預期：{ex.GetType().Name}", sw, startedAt);
            return CallResult<T>.Fail(CallFailure.UnexpectedFormat, status);
        }
    }
    catch (TaskCanceledException) when (!ct.IsCancellationRequested)
    {
        Audit(path, method, 0, "FAILED", "逾時", sw, startedAt);
        return CallResult<T>.Fail(CallFailure.Timeout);
    }
    catch (HttpRequestException ex)
    {
        Audit(path, method, 0, "FAILED", $"連線失敗：{ex.HttpRequestError}", sw, startedAt);
        return CallResult<T>.Fail(CallFailure.Network);
    }
}
```

```csharp
int consecutiveDown = 0;

foreach (var item in pending)
{
    ct.ThrowIfCancellationRequested();

    try
    {
        var result = await _client.SendAsync(Map(item), ct);

        if (result.Succeeded)
        {
            await MarkSentAsync(item, result.Value!, ct);
            sent++;
            consecutiveDown = 0;
            continue;
        }

        await RecordFailureAsync(item, result, ct);

        // 全局決定：線路不通，後面每一筆都會重演
        if (result.IsTransportDown && ++consecutiveDown >= TransportDownThreshold)
        {
            _logger.LogWarning("對外線路連續失敗 {N} 次，本輪提早結束（已送 {Sent} 筆、剩 {Left} 筆），下一輪再跑", consecutiveDown, sent, pending.Count - sent - skipped);
            break;
        }

        // 局部決定：這筆資料有問題，重試沒用
        if (!result.IsRetryable)
            await NotifyUnrecoverableAsync(item, result, ct);
        skipped++;
    }
    catch (Exception ex) when (ex is not OperationCanceledException)
    {
        _logger.LogError(ex, "處理 {Id} 未預期失敗，跳過這筆，下一輪再試", item.Id);
        skipped++;
    }
}
```

```csharp
while (await timer.WaitForNextTickAsync(stoppingToken))
{
    try
    {
        await runner.RunOnceAsync(stoppingToken);
    }
    catch (Exception ex) when (ex is not OperationCanceledException)
    {
        _logger.LogError(ex, "排程執行失敗");
    }
}
```

