# C# Refact Note - FilterData

- 對於 `FilterData` 方法的重構筆記

## 重構描述

- `FilterData` 方法用途為將輸入資料進行篩選，去除重複之後轉為指定類型

- 資料流調整：拆分為小方法並使用 Pipeline 式處理，減少中間集合的產生

- 去重複方法調整：
  - 對於輸入資料，使用 Linq `GroupBy` 與 `First` 更具表達性
  - 對於已存在資料，利用匿名型別自動生成 `GetHashCode` 與 `Equals` 的特性簡化去重複判斷，並修正字串拼接可能判斷錯誤的潛在問題，同時使用 `HashSet` 增進檢查效能

## Before

```csharp
private List<EventCsvDto> FilterData(List<BankRawDataDto> rawDatas, string fileName, DateTime filterDate)
{
    var existData = GetBankData(filterDate);

    List<string> listindex = new List<string>();
    List<EventCsvDto> listCsv = new List<EventCsvDto>();

    foreach (BankRawDataDto rawData in rawDatas)
    {
        if (string.IsNullOrEmpty(rawData.CaseNumber))
            continue;
        if (string.IsNullOrEmpty(rawData.BankCode))
            continue;
        if (string.IsNullOrEmpty(rawData.BankAccount))
            continue;        
        if (string.IsNullOrEmpty(rawData.BankIdno))
            continue;
        if (rawData.FraudMethod != "fakeinvestment" && rawData.FraudMethod != "fakefriends")
            continue;

        EventCsvDto EventCsvDto = new()
        {
            SrcFileName = fileName,
            CaseNumber = rawData.CaseNumber,
            WarningDatetime = ToDateTime(rawData.WarningDate),
            EventType = ConvertEventType(rawData.FraudMethod),
            BankCode = rawData.BankCode,
            BankAccount = rawData.BankAccount,
            BankIdno = rawData.BankIdno,
            BankUserName = rawData.BankUserName?.Replace("　", "")?.Trim() ?? ""
        };

        string strindex = EventCsvDto.CaseNumber + EventCsvDto.BankCode + EventCsvDto.BankAccount + EventCsvDto.BankIdno;
        if (listindex.Contains(strindex))
            continue;

        int cnt = existData.Where(p => p.CaseNumber == EventCsvDto.CaseNumber
                                && p.BankCode == EventCsvDto.BankCode
                                && p.BankAccount == EventCsvDto.BankAccount
                                && p.BankIdno == EventCsvDto.BankIdno).ToList().Count;
        if (cnt > 0)
            continue;

        listindex.Add(strindex);
        listCsv.Add(EventCsvDto);
    }

    return listCsv;
}
```

## After

```csharp
private List<EventCsvDto> FilterData(List<BankRawDataDto> rawDatas, string fileName, DateTime filterDate)
{
    var existData = GetBankData(filterDate)
        .Select(x => new { x.CaseNumber, x.BankCode, x.BankAccount, x.BankIdno })
        .ToHashSet();

    return rawDatas.Where(IsValidData)
        .GroupBy(x => new { x.CaseNumber, x.BankCode, x.BankAccount, x.BankIdno })
        .Where(g => !existData.Contains(g.Key))
        .Select(g => g.First())
        .Select(x => ConvertToEventCsvDto(fileName, x))
        .ToList();
}

private static EventCsvDto ConvertToEventCsvDto(string fileName, BankRawDataDto rawData)
{
    return new EventCsvDto
    {
        SrcFileName = fileName,
        CaseNumber = rawData.CaseNumber,
        WarningDatetime = ToDateTime(rawData.WarningDate),
        EventType = ConvertEventType(rawData.FraudMethod),
        BankCode = rawData.BankCode,
        BankAccount = rawData.BankAccount,
        BankIdno = rawData.BankIdno,
        BankUserName = rawData.BankUserName?.Replace("　", "")?.Trim() ?? ""
    };
}

private static bool IsValidData(BankRawDataDto rawData)
{
    return !string.IsNullOrEmpty(rawData.CaseNumber)
        && !string.IsNullOrEmpty(rawData.BankCode)
        && !string.IsNullOrEmpty(rawData.BankAccount)
        && !string.IsNullOrEmpty(rawData.BankIdno)
        && rawData.FraudMethod is ("fakeinvestment" or "fakefriends");
}
```
