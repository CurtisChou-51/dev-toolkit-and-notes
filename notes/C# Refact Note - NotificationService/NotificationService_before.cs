// ========== Controller ==========

//sales = 業務部, null = 稽核部
public ActionResult SendMail(string sales = "")
{
    bool isSales = !string.IsNullOrEmpty(sales);

    var datas = isSales ? _notificationService.FetchNotificationResultSales() : _notificationService.FetchNotificationResult();
    datas = _notificationService.FilterNotificationDatas(datas, isSales ? "Sales" : "Audit");
    datas.ForEach(data => data.HtmlResult = this.RenderView(isSales ? "MailTemplateSales" : "MailTemplate", data));
    if (isSales)
        _notificationService.SendMailSales(datas);
    else
        _notificationService.SendMail(datas);

    // ...
}


// ========== NotificationService：一個類別服務兩種對象 ==========

//去重檔名的部門段，由呼叫端傳入 "Sales" / "Audit" 字面值
public List<NotificationResult> FilterNotificationDatas(List<NotificationResult> datas, string customerName)
{
    datas.ForEach(data =>
    {
        // 檔名: {customerName}_{GroupName}_{報表區塊}_{yyyy-MM-dd}.txt
        data.NewsResult = filterDocument(data.GroupName, "NewsResult", data.NewsResult, customerName);
        // ... 其餘報表區塊
    });
    return datas;
}

public void SendMailSales(List<NotificationResult> notificationDatas)
{
    // ... 取得收件人
    sendMail(user, data, true);
}

public void SendMail(List<NotificationResult> notificationDatas)
{
    // ... 取得收件人
    sendMail(user, data, false);
}

//旗標在此只是轉手，本方法完全沒有用到
private void sendMail(SendMailUserInfo userInfo, NotificationResult data, bool isSales)
{
    // ... 逐一寄送並記錄
    sendMail(email, data, isSales);
}

private void sendMail(string mailAddr, NotificationResult data, bool isSales)
{
    string title = string.Empty;
    if (!isSales)
        title = string.IsNullOrEmpty(data.GroupName) ? "稽核部整點推播—" : "稽核部" + data.GroupName + "整點推播—";
    else
        title = "業務部" + (!string.IsNullOrEmpty(data.GroupName) ? "[" + data.GroupName + "]" : "") + (data.QueryDate.Hour >= 11 ? "下午" : "上午") + "推播—";

    title += data.QueryDate.ToString("yyyy/MM/dd HH:mm:ss");

    // ... 寄送
}
