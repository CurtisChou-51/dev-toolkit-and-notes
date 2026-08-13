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
