# C# Refact Note - FilterDocument

- 對於 `filterDocument` 方法的重構筆記，將「組出檔名」與「用檔名做事」拆開

## 情境

- 推播每隔一段時間執行一次，且查詢區間刻意重疊以避免漏撈，因此同一份文件會被重複撈出
- `filterDocument` 負責去重：把已推播過的文件 ID 記在文字檔，下次比對後只留新的
- 檔名由三個維度組成，任何一維漏掉，不同對象就會共用同一個檔而互相過濾掉對方的資料

  ```
  {customerName} _ {groupName} _ {reportName} _ {yyyy-MM-dd} .txt
       部門            群組           區塊          寫入日期
  ```

## 重構描述

### 主要問題
`filterDocument` 參數過於難懂：
- `filterDocument` 承諾的是「過濾文件」。從這個名字出發，維護者預期的參數是「要過濾的資料」和「過濾條件」，不是「要過濾的資料」和「組檔名的三個維度」
- `groupName` 進來三行內就被覆寫成複合值，實際意義已發生改變
- `customerName` 全程只出現一次，就是被串接在 `groupName` 前面，沒有任何邏輯與它互動
- `reportName` 連碰都沒碰只是原封不動往下傳

它們沒有一個是以自己的身分參與運算的，讓人看不懂它們為什麼會在這裡。方法簽章把它們呈現成三個獨立的業務概念，實際上它們是同一個字串的三段

### 重構調整

- 職責分離：`filterDocument` 只負責「過濾文件」，組檔名的邏輯抽出成 `formatFilterFilePrefix`
- 檔名規則單一化：組檔名的邏輯原本分散在三個方法，`filterDocument` 補佔位與拼接前綴，`getFilterDidsFromFile` 與 `addFilterDidToFile` 又各自寫一份。改為抽出 `formatFilterFilePrefix` 統一負責
- 參數順序調整：`customerName` 是簽章的最後一個參數，卻是檔名的最前一段
- 命名調整：參數名 `fileName` 實際上不是檔名（真正的檔名還要接日期與副檔名），改為 `prefix`

> [!NOTE]
> 有意思的是，那三個參數名字沒改、型別沒改、順序只是回歸檔名順序，換個地方放就都懂了。  
> 當一組參數的唯一用途是組成另一個值，就讓那個值本身成為參數。

## Before

```csharp
public List<NotificationResult> FilterNotificationDatas(List<NotificationResult> notificationDatas)
{
    // 清理過期去重檔 ...

    notificationDatas.ForEach(data =>
    {
        string customerName = data.FilterKeyName;
        string groupKey = data.IsManager ? "_Manager" : data.GroupName;
        data.AnalysisResult = filterDocument(groupKey, "AnalysisResult", data.AnalysisResult, customerName);
        data.AnnouncementResult = filterDocument(groupKey, "AnnouncementResult", data.AnnouncementResult, customerName);
        data.NewsResult = filterDocument(groupKey, "NewsResult", data.NewsResult, customerName);
        data.SocialResult = filterDocument(groupKey, "SocialResult", data.SocialResult, customerName);
    });
    return notificationDatas;
}

/// <remarks> 檔名: {customerName}_{groupName}_{reportName}_{yyyy-MM-dd}.txt </remarks>
private SearchResultBean filterDocument(string groupName, string reportName, SearchResultBean searchResult, string customerName)
{
    if (searchResult == null)
        return new SearchResultBean();

    if (string.IsNullOrEmpty(groupName))
        groupName = "_A_D_M_I_N";

    groupName = customerName + "_" + groupName;

    var filterDids = getFilterDidsFromFile(groupName, reportName);
    var newFilterDids = new List<string>();
    var newResultData = new List<Dictionary<string, Object>>();

    foreach (var item in searchResult.ResultData)
    {
        string did = item["doc_id"].ToString();
        if (!filterDids.Contains(did))
        {
            newResultData.Add(item);
            newFilterDids.Add(did);
        }
    }

    addFilterDidToFile(groupName, reportName, newFilterDids.ToArray());

    return new SearchResultBean
    {
        // ...
        ResultData = newResultData
    };
}

private string[] getFilterDidsFromFile(string groupName, string reportName)
{
    List<string> result = new List<string>();

    string[] files = Directory.GetFiles(FileUtility.GetPath(Constants.FolderPath_Filter));
    string fileName = string.Format("{0}_{1}", FileUtility.EscapeFileName(groupName), reportName);
    string[] matchFiles = files.Where(x => x.Contains(fileName)).ToArray();

    foreach (string filePath in matchFiles)
    {
        if (File.Exists(filePath))
            result.AddRange(File.ReadAllLines(filePath));
    }

    return result.Where(x => !string.IsNullOrEmpty(x)).ToArray();
}

private void addFilterDidToFile(string groupName, string reportName, string[] dids)
{
    string folderPath = FileUtility.GetPath(Constants.FolderPath_Filter);
    string fileName = string.Format("{0}_{1}_{2}.txt", FileUtility.EscapeFileName(groupName), reportName, DateTime.Now.ToString("yyyy-MM-dd"));
    File.AppendAllLines(folderPath + @"\" + fileName, dids);
}
```

## After

```csharp
public List<NotificationResult> FilterNotificationDatas(List<NotificationResult> notificationDatas)
{
    // 清理過期去重檔 ...

    notificationDatas.ForEach(data =>
    {
        string customerName = data.FilterKeyName;
        string groupKey = data.IsManager ? "_Manager" : data.GroupName;
        data.AnalysisResult = filterDocument(data.AnalysisResult, formatFilterFilePrefix(customerName, groupKey, "AnalysisResult"));
        data.AnnouncementResult = filterDocument(data.AnnouncementResult, formatFilterFilePrefix(customerName, groupKey, "AnnouncementResult"));
        data.NewsResult = filterDocument(data.NewsResult, formatFilterFilePrefix(customerName, groupKey, "NewsResult"));
        data.SocialResult = filterDocument(data.SocialResult, formatFilterFilePrefix(customerName, groupKey, "SocialResult"));
    });
    return notificationDatas;
}

/// <summary> 去重檔名前綴。實際檔名為 {前綴}_{yyyy-MM-dd}.txt，讀取時以此前綴比對所有日期的檔案 </summary>
private string formatFilterFilePrefix(string customerName, string groupName, string reportName)
{
    // 無群組名稱者(長官)補一個不會與真實群組衝突的佔位
    if (string.IsNullOrEmpty(groupName))
        groupName = "_A_D_M_I_N";

    return FileUtility.EscapeFileName(string.Format("{0}_{1}_{2}", customerName, groupName, reportName));
}

private SearchResultBean filterDocument(SearchResultBean searchResult, string prefix)
{
    if (searchResult == null)
        return new SearchResultBean();

    var filterDids = getFilterDidsFromFile(prefix);
    var newFilterDids = new List<string>();
    var newResultData = new List<Dictionary<string, Object>>();

    foreach (var item in searchResult.ResultData)
    {
        string did = item["doc_id"].ToString();
        if (!filterDids.Contains(did))
        {
            newResultData.Add(item);
            newFilterDids.Add(did);
        }
    }

    addFilterDidToFile(prefix, newFilterDids.ToArray());

    return new SearchResultBean
    {
        // ...
        ResultData = newResultData
    };
}

// 讀取不限日期，取回保留期內所有同前綴檔案的did
private string[] getFilterDidsFromFile(string prefix)
{
    List<string> result = new List<string>();

    string[] files = Directory.GetFiles(FileUtility.GetPath(Constants.FolderPath_Filter));
    string[] matchFiles = files.Where(x => x.Contains(prefix)).ToArray();

    foreach (string filePath in matchFiles)
    {
        if (File.Exists(filePath))
            result.AddRange(File.ReadAllLines(filePath));
    }

    return result.Where(x => !string.IsNullOrEmpty(x)).ToArray();
}

// 寫入僅針對當天檔案，供保留期清理依檔案時間判斷
private void addFilterDidToFile(string prefix, string[] dids)
{
    string folderPath = FileUtility.GetPath(Constants.FolderPath_Filter);
    string filePath = string.Format("{0}\\{1}_{2}.txt", folderPath, prefix, DateTime.Now.ToString("yyyy-MM-dd"));
    File.AppendAllLines(filePath, dids);
}
```
