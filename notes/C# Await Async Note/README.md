# C# Await Async Note

- 背景：以往在開發時大家都說盡可能地使用 `await` 處理非同步是最佳實踐，但一開始不太容易直覺地理解它到底好在哪裡，寫起來也不覺得有什麼差別。在這裡做些個人心得整理

## await 的好處是?

- 在等待 I/O 時釋放執行緒，可以處理其他工作，提高系統吞吐量，**避免浪費執行緒**

- 當 `await` 遇到尚未完成的非同步工作時：
  - 目前使用的執行緒會被釋放回執行緒池，系統可用來處理其他請求或任務
  - 等待的 I/O 完成後，再從執行緒池取出一條執行緒繼續往下執行（可能不是同一條）

```csharp
public async Task<string> FetchDataAsync()
{
    Console.WriteLine($"開始: 執行緒 {Thread.CurrentThread.ManagedThreadId}");

    // 發起非同步操作，執行緒立即被釋放，回到執行緒池
    // 在等待期間，執行緒可以去做其他工作
    var result = await _httpClient.GetStringAsync("https://api.example.com/data");
    
    // 操作完成後，從執行緒池取一個執行緒繼續執行
    Console.WriteLine($"完成: 執行緒 {Thread.CurrentThread.ManagedThreadId}"); // 可能是不同的執行緒
    
    return result;
}
```

## 阻塞 vs 非阻塞

### 阻塞方式

- `GetAwaiter().GetResult()`、`Wait()`、`Result` 都是同步阻塞方式
- 執行緒會在原地等 I/O 完成，什麼事也不能做

```csharp
public string BlockingMethod()
{
    // 執行緒在這裡等2秒，什麼事都不做
    var result = Task.Delay(2000).GetAwaiter().GetResult();
    return "Done";
}
```

### 非阻塞方式

- 使用 `await` 時，執行緒會立即被釋放以供其他使用

```csharp
public async Task<string> NonBlockingMethod()
{
    // 執行緒立即被釋放，可以去做其他工作
    await Task.Delay(2000);
    return "Done";
}
```

## 直觀感受

- 在網頁開發環境為何感受不直觀：
  - 網頁不像 WinForm 的 UI 執行緒被占用時會凍結畫面、沒有回應，使用者端的瀏覽器不會有任何異常表現
  - 開發環境通常只有開發者一人發送請求、負載不高，不會造成執行緒耗盡，即使執行緒被阻塞閒置也沒有明顯感覺