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