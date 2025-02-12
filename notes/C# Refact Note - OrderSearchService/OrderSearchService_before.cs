public class OrderSearchService
{
    private readonly IDbAccess _dbAccess;

    public OrderSearchService(IDbAccess dbAccess)
    {
        _dbAccess = dbAccess;
    }

    public PagedPagedOrder GetPagedOrders(UserRole role, string type)
    {
        PagedPagedOrder result = new();
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