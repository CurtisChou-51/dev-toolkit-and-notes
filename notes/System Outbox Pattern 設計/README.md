# System Outbox Pattern 設計

- System Outbox Pattern 設計

## 基本概念
- Outbox Pattern 是一種**確保資料庫狀態與對外事件/API一致性**的設計
- 核心精神：
  - 至少一次可靠執行對外行為
  - 允許重送，但不允許遺失

> [!NOTE]
> 先紀錄，再做事，做完再更新  
> 事情可以晚點做，但紀錄一定要對

## 實現範例
1. 將要發送的事件先寫入資料庫的 Outbox 表中，與主要業務流程在同一個交易（Transaction）中完成
2. 使用獨立的背景程序檢查 Outbox 表中「未發送」的紀錄，並將事件發送到外部系統（如 API 或消息隊列）
3. 發送成功後，將 Outbox 表中的事件標記為已處理；失敗則保留，等待下次重試

### 範例 - 主要業務流程
```csharp
public async Task Handle(CreateOrderCommand cmd)
{
    using var transaction = await _dbContext.Database.BeginTransactionAsync();
    try {
        var order = new Order { /* ... */ };
        _dbContext.Orders.Add(order);

        var outboxEvt = new OutboxEvent
        {
            MessageType = "OrderCreated",
            Content = JsonSerializer.Serialize(new { OrderId = order.Id }),
            CreatedAt = DateTime.UtcNow,
            Status = Status.Pending
        };
        _dbContext.OutboxEvents.Add(outboxEvt);

        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}
```

### 範例 - 背景服務
```csharp
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    _logger.LogInformation("Outbox 掃描服務啟動...");

    while (!stoppingToken.IsCancellationRequested)
    {
        try
        {
            await ProcessOutboxEvents(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "處理 Outbox 時發生錯誤");
        }
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
    }
}

private async Task ProcessOutboxEvents(CancellationToken ct)
{
    using var scope = _serviceProvider.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var events = await dbContext.OutboxEvents
        .Where(e => e.Status != Status.Complete)
        .OrderBy(e => e.CreatedAt)
        .Take(10)
        .ToListAsync(ct);

    foreach (var evt in events)
    {
        // 模擬發送到外部系統
        // await _messageBus.Publish(evt.Content); 
        await Task.Delay(100);

        evt.Status = Status.Complete;
    }

    if (events.Any())
        await dbContext.SaveChangesAsync(ct);
}
```

### 範例專案 - BlazorAppOutbox
[BlazorAppOutbox](BlazorAppOutbox)

## 常見場景
常用於對外不可控、不穩定的 API 呼叫，並且不要求即時回應，但外部 API 掛了也不能影響主要寫入流程
  