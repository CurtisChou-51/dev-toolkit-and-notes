public class OrderSearchService
{
    private readonly IEnumerable<IOrderSearchStrategy> _strategies;

    public OrderSearchService(IEnumerable<IOrderSearchStrategy> strategies)
    {
        _strategies = strategies;
    }

    public PagedPagedOrder GetPagedOrders(UserRole role, string type)
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
    public PagedPagedOrder GetPagedOrders(string type);
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

    public PagedPagedOrder GetPagedOrders(string type)
    {
        return type switch
        {
            "Replied" => _dbAccess.GetPagedOrders(UserRole, OrderStatus.Replied),
            "Processing" => _dbAccess.GetPagedOrders(UserRole, OrderStatus.WaitingForReply),
            "Rejected" => _dbAccess.GetPagedOrders(UserRole, OrderStatus.Rejected),
            _ => new PagedPagedOrder()
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

    public PagedPagedOrder GetPagedOrders(string type)
    {
        return type switch
        {
            "Unreviewed" => _dbAccess.GetPagedReviewOrders(UserRole, OrderStatus.UnderReview, [ProcessDtlStatus.UnderReview1]),
            "Reviewed" => _dbAccess.GetPagedReviewOrders(UserRole, OrderStatus.UnderReview, [ProcessDtlStatus.UnderReview2, ProcessDtlStatus.UnderReview3]),
            _ => new PagedPagedOrder()
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

    public PagedPagedOrder GetPagedOrders(string type)
    {
        return type switch
        {
            "Unreviewed" => _dbAccess.GetPagedReviewOrders(UserRole, OrderStatus.UnderReview, [ProcessDtlStatus.UnderReview1]),
            "ToBeReceived" => _dbAccess.GetPagedReviewOrders(UserRole, OrderStatus.Replied, [ProcessDtlStatus.ManualReplied, ProcessDtlStatus.SystemReplied]),
            "Received" => _dbAccess.GetPagedReviewOrders(UserRole, OrderStatus.Received, [ProcessDtlStatus.ManualReplied, ProcessDtlStatus.SystemReplied]),
            _ => new PagedPagedOrder()
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