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