# C# Refact Note - NotificationService

- 對於推播服務的重構筆記，把貫穿整條管線的 `isSales` 旗標與其五個分支移除，改為拆成兩個類別
- 同批重構中 `filterDocument` 的調整另見 [C# Refact Note - FilterDocument](../C%23%20Refact%20Note%20-%20FilterDocument/README.md)

## 情境

- 推播服務同時服務兩種對象：稽核部與業務部。兩者的內容組成、信件標題、信件樣板、去重檔命名都不同；而兩者共用同一個 Controller 入口與同一個 Service，靠一個 `isSales` 判斷

## 重構描述

- 不改變 Controller 端點簽章狀況下進行調整

### 主要問題

`isSales` 從 Controller 產生後一路往下傳，被判斷了五次：

```csharp
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
```

以呼叫順序來看：
```
Controller.SendMail(sales)
  └ bool isSales = !string.IsNullOrEmpty(sales)
      ① isSales ? FetchNotificationResultSales() : FetchNotificationResult()
      ② FilterNotificationDatas(datas, isSales ? "Sales" : "Audit")
      ③ RenderView(isSales ? "MailTemplateSales" : "MailTemplate")
      ④ if (isSales) SendMailSales(datas) else SendMail(datas)
           └ sendMail(user, data, isSales)          ← 參數鏈往下傳
               └ sendMail(mail, data, isSales)
                   └ ⑤ if (!isSales) title = "稽核部…" else title = "業務部…"
```

- 兩條路線被擠在同一個類別裡：這是兩套流程，並非同一個流程有兩種選項
- 語意隨距離遞減：`isSales` 在 ① 是「要取哪種內容」，到 ⑤ 變成「標題怎麼寫」。同一個變數在五個地方各自被重新解釋，讀到 ⑤ 時已經看不出它原本是什麼
- 中間層被迫夾帶：`sendMail(user, data, isSales)` 從頭到尾沒有用到這個參數，只是往下一層轉手。它出現在簽章上的唯一理由，是它的下游需要

### 重構調整

- 先讓分支需要的值改由資料自帶（消滅 ②⑤ 與參數鏈）

在 Fetch 階段就把值算好放進 `NotificationResult`，下游只讀屬性：

| 新欄位 | 消滅的分支 |
|---|---|
| `DeptCode` | ② —— `FilterNotificationDatas` 的 `customerName` 參數從簽章刪除 |
| `MailTitle` | ⑤ —— 標題的 `if/else` 拆成兩個服務各自的 `buildMailTitle` |

```
去重檔名  {DeptCode}_{GroupName}_{報表區塊}_{yyyy-MM-dd}.txt
               ↑ 兩部門會撈到同一批文件，靠這段區隔，原本由呼叫端傳入 "Sales" / "Audit"
```

`MailTitle` 就位後寄信方法不再需要知道部門，搬到共用的 `NotificationUtility.TrySendMail(user, data)`

- 再把業務部拆成獨立類別（消滅 ①③④）

逐個方法搬進 `ISalesNotificationService`，Controller 的分支點隨之一個個從三元運算改成「呼叫另一個 service」，三個分支點全部指向同一個類別後，整段對折成兩個私有方法，`bool isSales` 旗標參數終於刪除

### 成果

判斷從「中間反覆問五次」變成「入口分一次路」，之後兩條路線各自是一條直線：取得內容 → 去重 → 產生信件內文 → 寄信。

原本擠在同一個 `if/else` 裡的兩種標題規則，成為兩個類別各自的 `buildMailTitle`。要改哪一邊就只會動到那一邊，不必再確認另一邊會不會被波及。

## Before

```csharp
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
```

## After

```csharp
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
                DeptName = "業務部",   //部門資訊由擁有它的類別宣告一次，不再由呼叫端每次指認
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
```

### 後續

後續需求要把業務部再分成一部與二部。經過重構後部門資訊已經是資料而不是分支，只要 Fetch 端多帶一組 `DeptName` / `DeptCode` 即可；即使一部與二部差異更大也可以直接拆成三個類別，不需要再加入更多旗標參數判斷