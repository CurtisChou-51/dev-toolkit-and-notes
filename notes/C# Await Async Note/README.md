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

- [WinForm 範例](Form1.cs)，展示阻塞與非阻塞對 UI 執行緒的影響：
  - 阻塞方式會導致 UI 凍結無法回應
  - 非阻塞方式 UI 保持流暢

- 在網頁開發環境為何感受不直觀：
  - 網頁不像 WinForm 的 UI 執行緒被占用時會凍結畫面、沒有回應，使用者端的瀏覽器不會有任何異常表現
  - 開發環境通常只有開發者一人發送請求、負載不高，不會造成執行緒耗盡，即使執行緒被阻塞閒置也沒有明顯感覺

## 高併發場景的差異

- 假設有 200 個併發請求；執行緒池只有 100 個執行緒

```csharp
// 阻塞版本
public IActionResult BlockingEndpoint()
{
    // 100 個執行緒全部被佔用；後續 100 個請求要排隊等待
    var data = _httpClient.GetAsync("https://api.example.com/data").GetAwaiter().GetResult();
    return Ok(data);
}

// 非阻塞版本
public async Task<IActionResult> NonBlockingEndpoint()
{
    // 執行緒在 I/O 期間被釋放，可以處理更多請求
    var data = await _httpClient.GetAsync("https://api.example.com/data");
    return Ok(data);
}
```

### Example of 30 Threads

- 以下範例透過 ThreadPool 設定最小與最大執行緒數量，並分別以 `Thread.Sleep(3000)` 與 `await Task.Delay(3000)` 來模擬耗時的 I/O 操作，可以看出在執行緒數量受限的情況下，阻塞方式整體執行時間高於非阻塞方式，原因就是執行緒被佔用無法處理其他工作

- 範例程式 [Example30Threads.cs](Example30Threads.cs)：
```csharp
ThreadPool.SetMinThreads(30, 1);
ThreadPool.SetMaxThreads(30, 1);

Stopwatch sw = Stopwatch.StartNew();
var tasks = Enumerable.Range(1, 100).Select(_ => Task.Run(DoWork)).ToArray();
Task.WaitAll(tasks);
sw.Stop();
Console.WriteLine($"All tasks completed. Elapsed = {sw.Elapsed}");

sw.Restart();
var asyncTasks = Enumerable.Range(1, 100).Select(_ => Task.Run(DoWorkAsync)).ToArray();
Task.WaitAll(asyncTasks);
sw.Stop();
Console.WriteLine($"All asyncTasks completed. Elapsed = {sw.Elapsed}");

void DoWork()
{
    Thread.Sleep(3000);
}

async Task DoWorkAsync()
{
    await Task.Delay(3000);
}
```

- 範例結果：
```
All tasks completed. Elapsed = 00:00:12.0775677
All asyncTasks completed. Elapsed = 00:00:03.0261486
```

## Avoid Async Void

- 一般來說 `async void` 只能用在事件處理器中，其他情況應避免使用，原因是無法讓呼叫端捕捉例外，如果未處理的例外一路往外拋最後可能會導致網站程式掛掉。參考：[Avoid Async Void - Microsoft](https://learn.microsoft.com/zh-tw/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming#avoid-async-void)

```csharp
try
{
    DoSomething();
}
catch (Exception ex)
{
    // 無法捕捉
}

async void DoSomething()
{
    await Task.Delay(100);
    throw new Exception("Boom!");
}
```

- 改為 `async Task` 後即可 `await` 捕捉例外：

```csharp
try
{
    await DoSomething();
}
catch (Exception ex)
{
    // 可以捕捉
}

async Task DoSomething()
{
    await Task.Delay(100);
    throw new Exception("Boom!");
}
```

