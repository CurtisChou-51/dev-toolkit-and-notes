// ========== Controller：入口分一次路，之後不再有旗標 ==========

//sales = 業務部, null = 稽核部
public ActionResult SendMail(string sales = "")
{
    if (string.IsNullOrEmpty(sales))
        SendMailAudit();
    else
        SendMailSales();

    // ...
}

/// <summary> 稽核部推播：取得內容 → 去重 → 產生信件內文 → 寄信 </summary>
private void SendMailAudit()
{
    var datas = _auditNotificationService.FetchNotificationResult();
    datas = _auditNotificationService.FilterNotificationDatas(datas);
    datas.ForEach(data => data.HtmlResult = this.RenderView("MailTemplate", data));
    _auditNotificationService.SendMail(datas);
}

/// <summary> 業務部推播：取得內容 → 去重 → 產生信件內文 → 寄信 </summary>
private void SendMailSales()
{
    var datas = _salesNotificationService.FetchNotificationResult();
    datas = _salesNotificationService.FilterNotificationDatas(datas);
    datas.ForEach(data => data.HtmlResult = this.RenderView("MailTemplateSales", data));
    _salesNotificationService.SendMail(datas);
}


// ========== NotificationResult：分支需要的值改由資料自帶 ==========

public class NotificationResult
{
    /// <summary> 部門名稱，信件標題用 </summary>
    public string DeptName { get; set; }

    /// <summary> 部門代號，去重檔名用 </summary>
    public string DeptCode { get; set; }

    /// <summary> 信件標題，於 Fetch 階段由各服務自己組出 </summary>
    public string MailTitle { get; set; }

    // ...
}


// ========== 去重：檔名的部門段改由 DeptCode 提供，簽章不再需要呼叫端指定 ==========

public List<NotificationResult> FilterNotificationDatas(List<NotificationResult> datas)
{
    datas.ForEach(data =>
    {
        // 檔名: {DeptCode}_{GroupName}_{報表區塊}_{yyyy-MM-dd}.txt
        data.NewsResult = filterDocument(data.NewsResult, formatFilterFilePrefix(data.DeptCode, data.GroupName, "NewsResult"));
        // ... 其餘報表區塊
    });
    return datas;
}


// ========== 兩個類別各自實作，原本的 if/else 成為兩份獨立程式碼 ==========

public class SalesNotificationService : ISalesNotificationService
{
    public List<NotificationResult> FetchNotificationResult()
    {
        List<NotificationResult> result = new List<NotificationResult>();

        foreach (var group in _groupService.FetchGroupSales())
        {
            result.Add(new NotificationResult
            {
                GroupName = group.GroupName,
                DeptName = "業務部",
                DeptCode = "Sales",
                QueryDate = queryDate,
                // ... 內容欄位
            });
        }

        //標題於此組好，寄信端不必再判斷部門
        result.ForEach(x => x.MailTitle = buildMailTitle(x));
        return result;
    }

    /// <summary> 業務部信件標題 </summary>
    private string buildMailTitle(NotificationResult data)
    {
        return data.DeptName
             + (!string.IsNullOrEmpty(data.GroupName) ? "[" + data.GroupName + "]" : "")
             + (data.QueryDate.Hour >= 11 ? "下午" : "上午") + "推播—"
             + data.QueryDate.ToString("yyyy/MM/dd HH:mm:ss");
    }
}

public class AuditNotificationService : IAuditNotificationService
{
    public List<NotificationResult> FetchNotificationResult()
    {
        List<NotificationResult> result = new List<NotificationResult>();

        // 長官
        result.Add(new NotificationResult
        {
            DeptName = "稽核部",
            DeptCode = "Audit",
            QueryDate = queryDate,
            // ... 內容欄位
        });

        foreach (var group in _groupService.FetchGroupAudit())
        {
            result.Add(new NotificationResult
            {
                GroupName = group.GroupName,
                DeptName = "稽核部",
                DeptCode = "Audit",
                QueryDate = queryDate,
                // ... 內容欄位
            });
        }

        result.ForEach(x => x.MailTitle = buildMailTitle(x));
        return result;
    }

    /// <summary> 稽核部信件標題 </summary>
    private string buildMailTitle(NotificationResult data)
    {
        return (string.IsNullOrEmpty(data.GroupName)
                    ? "稽核部整點推播—"
                    : "稽核部" + data.GroupName + "整點推播—")
             + data.QueryDate.ToString("yyyy/MM/dd HH:mm:ss");
    }
}


// ========== NotificationUtility：兩者共用，簽章不再有旗標 ==========

public static void TrySendMail(SendMailUserInfo userInfo, NotificationResult data)
{
    // ... 逐一寄送並記錄，標題直接取 data.MailTitle
}
