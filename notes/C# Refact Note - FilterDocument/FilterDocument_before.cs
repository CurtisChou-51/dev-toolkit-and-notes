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
