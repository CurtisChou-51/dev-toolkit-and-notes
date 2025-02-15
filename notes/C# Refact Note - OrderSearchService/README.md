# C# Refact Note - OrderSearchService

- 對於 `OrderSearchService` 類別的重構筆記

## 重構描述

- `OrderSearchService` 類別用途為根據不同使用者角色搜尋 Order 分頁資料及計數資料

- 結構調整：
  - 使用策略模式(Strategy Pattern)，將各角色的搜尋邏輯拆分到獨立策略類別中
  - 建立 `IOrderSearchStrategy` 介面統一定義各策略類別行為
  - `OrderSearchService` 改為依賴各策略類別集合
  - 新增或是修改角色的搜尋邏輯時更改對應策略類別即可

## Before

```csharp
public class OrderSearchService
{
    private readonly IDbAccess _dbAccess;

    public OrderSearchService(IDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public PagedOrderModel GetPagedOrders(UserRole role, string type)
    {
        PagedOrderModel result = new();
        if (role == UserRole.Normal)
        {
            if (type == "Replied")
                result = _dbAccess.GetPagedOrders(role, OrderStatus.Replied);
            else if (type == "Processing")
                result = _dbAccess.GetPagedOrders(role, OrderStatus.WaitingForReply);
            else if (type == "Rejected")
                result = _dbAccess.GetPagedOrders(role, OrderStatus.Rejected);
        }
        else if (role == UserRole.Manager)
        {
            if (type == "Unreviewed")
                result = _dbAccess.GetPagedReviewOrders(role, OrderStatus.UnderReview, [ProcessDtlStatus.UnderReview1]);
            else if (type == "Reviewed")
                result = _dbAccess.GetPagedReviewOrders(role, OrderStatus.UnderReview, [ProcessDtlStatus.UnderReview2, ProcessDtlStatus.UnderReview3]);
        }
        else if (role == UserRole.Auditor)
        {
            if (type == "Unreviewed")
                result = _dbAccess.GetPagedReviewOrders(role, OrderStatus.UnderReview, [ProcessDtlStatus.UnderReview1]);
            else if (type == "ToBeReceived")
                result = _dbAccess.GetPagedReviewOrders(role, OrderStatus.Replied, [ProcessDtlStatus.ManualReplied, ProcessDtlStatus.SystemReplied]);
            else if (type == "Received")
                result = _dbAccess.GetPagedReviewOrders(role, OrderStatus.Received, [ProcessDtlStatus.ManualReplied, ProcessDtlStatus.SystemReplied]);
        }
        return result;
    }

    public int GetOrderCount(UserRole role, string type)
    {
        int result = 0;
        if (role == UserRole.Normal)
        {
            if (type == "Replied")
                result = _dbAccess.GetOrderCount(role, OrderStatus.Replied);
            else if (type == "Processing")
                result = _dbAccess.GetOrderCount(role, OrderStatus.WaitingForReply);
            else if (type == "Rejected")
                result = _dbAccess.GetOrderCount(role, OrderStatus.Rejected);
        }
        else if (role == UserRole.Manager)
        {
            if (type == "Unreviewed")
                result = _dbAccess.GetReviewOrderCount(role, OrderStatus.UnderReview, [ProcessDtlStatus.UnderReview1]);
            else if (type == "Reviewed")
                result = _dbAccess.GetReviewOrderCount(role, OrderStatus.UnderReview, [ProcessDtlStatus.UnderReview2, ProcessDtlStatus.UnderReview3]);
        }
        else if (role == UserRole.Auditor)
        {
            if (type == "Unreviewed")
                result = _dbAccess.GetReviewOrderCount(role, OrderStatus.UnderReview, [ProcessDtlStatus.UnderReview1]);
            else if (type == "ToBeReceived")
                result = _dbAccess.GetReviewOrderCount(role, OrderStatus.Replied, [ProcessDtlStatus.ManualReplied, ProcessDtlStatus.SystemReplied]);
            else if (type == "Received")
                result = _dbAccess.GetReviewOrderCount(role, OrderStatus.Received, [ProcessDtlStatus.ManualReplied, ProcessDtlStatus.SystemReplied]);
        }
        return result;
    }
}
```

## After

```csharp
public class OrderSearchService
{
    private readonly IEnumerable<IOrderSearchStrategy> _strategies;

    public OrderSearchService(IEnumerable<IOrderSearchStrategy> strategies)
    {
        _strategies = strategies;
    }

    public PagedOrderModel GetPagedOrders(UserRole role, string type)
    {
        IOrderSearchStrategy? strategy = _strategies.FirstOrDefault(x => x.UserRole == role);
        return strategy?.GetPagedOrders(type) ?? new();
    }

    public int GetOrderCount(UserRole role, string type)
    {
        IOrderSearchStrategy? strategy = _strategies.FirstOrDefault(x => x.UserRole == role);
        return strategy?.GetOrderCount(type) ?? 0;
    }
}

public interface IOrderSearchStrategy
{
    public UserRole UserRole { get; }
    public PagedOrderModel GetPagedOrders(string type);
    public int GetOrderCount(string type);
}

public class NormalOrderSearchStrategy : IOrderSearchStrategy
{
    private readonly IDbAccess _dbAccess;

    public NormalOrderSearchStrategy(IDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public UserRole UserRole => UserRole.Normal;

    public PagedOrderModel GetPagedOrders(string type)
    {
        return type switch
        {
            "Replied" => _dbAccess.GetPagedOrders(UserRole, OrderStatus.Replied),
            "Processing" => _dbAccess.GetPagedOrders(UserRole, OrderStatus.WaitingForReply),
            "Rejected" => _dbAccess.GetPagedOrders(UserRole, OrderStatus.Rejected),
            _ => new PagedOrderModel()
        };
    }

    public int GetOrderCount(string type)
    {
        return type switch
        {
            "Replied" => _dbAccess.GetOrderCount(UserRole, OrderStatus.Replied),
            "Processing" => _dbAccess.GetOrderCount(UserRole, OrderStatus.WaitingForReply),
            "Rejected" => _dbAccess.GetOrderCount(UserRole, OrderStatus.Rejected),
            _ => 0
        };
    }
}

public class ManagerOrderSearchStrategy : IOrderSearchStrategy
{
    private readonly IDbAccess _dbAccess;

    public ManagerOrderSearchStrategy(IDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public UserRole UserRole => UserRole.Manager;

    public PagedOrderModel GetPagedOrders(string type)
    {
        return type switch
        {
            "Unreviewed" => _dbAccess.GetPagedReviewOrders(UserRole, OrderStatus.UnderReview, [ProcessDtlStatus.UnderReview1]),
            "Reviewed" => _dbAccess.GetPagedReviewOrders(UserRole, OrderStatus.UnderReview, [ProcessDtlStatus.UnderReview2, ProcessDtlStatus.UnderReview3]),
            _ => new PagedOrderModel()
        };
    }

    public int GetOrderCount(string type)
    {
        return type switch
        {
            "Unreviewed" => _dbAccess.GetReviewOrderCount(UserRole, OrderStatus.UnderReview, [ProcessDtlStatus.UnderReview1]),
            "Reviewed" => _dbAccess.GetReviewOrderCount(UserRole, OrderStatus.UnderReview, [ProcessDtlStatus.UnderReview2, ProcessDtlStatus.UnderReview3]),
            _ => 0
        };
    }
}

public class AuditorOrderSearchStrategy : IOrderSearchStrategy
{
    private readonly IDbAccess _dbAccess;

    public AuditorOrderSearchStrategy(IDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public UserRole UserRole => UserRole.Auditor;

    public PagedOrderModel GetPagedOrders(string type)
    {
        return type switch
        {
            "Unreviewed" => _dbAccess.GetPagedReviewOrders(UserRole, OrderStatus.UnderReview, [ProcessDtlStatus.UnderReview1]),
            "ToBeReceived" => _dbAccess.GetPagedReviewOrders(UserRole, OrderStatus.Replied, [ProcessDtlStatus.ManualReplied, ProcessDtlStatus.SystemReplied]),
            "Received" => _dbAccess.GetPagedReviewOrders(UserRole, OrderStatus.Received, [ProcessDtlStatus.ManualReplied, ProcessDtlStatus.SystemReplied]),
            _ => new PagedOrderModel()
        };
    }

    public int GetOrderCount(string type)
    {
        return type switch
        {
            "Unreviewed" => _dbAccess.GetReviewOrderCount(UserRole, OrderStatus.UnderReview, [ProcessDtlStatus.UnderReview1]),
            "ToBeReceived" => _dbAccess.GetReviewOrderCount(UserRole, OrderStatus.Replied, [ProcessDtlStatus.ManualReplied, ProcessDtlStatus.SystemReplied]),
            "Received" => _dbAccess.GetReviewOrderCount(UserRole, OrderStatus.Received, [ProcessDtlStatus.ManualReplied, ProcessDtlStatus.SystemReplied]),
            _ => 0
        };
    }
}
```