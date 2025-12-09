private CfvExtractResult ExtractCfvExcelData(Workbook wb)
{
    List<CfvAccountDto> accountList = new();
    List<CfvWalletDto> walletList = new();

    foreach (Worksheet worksheet in wb.Worksheets)
    {
        var headerCols = YieldHeaderCols(worksheet).ToArray();
        var accountPropMap = InitPropMap<CfvAccountDto>(headerCols);
        var walletPropMap = InitPropMap<CfvWalletDto>(headerCols);
        for (int rowIdx = 1; rowIdx <= worksheet.Cells.MaxDataRow; rowIdx++)
        {
            Aspose.Cells.Row row = worksheet.Cells.GetRow(rowIdx);

            CfvAccountDto account = ConvertRowToDto<CfvAccountDto>(row, accountPropMap);
            if (!string.IsNullOrEmpty(account.Name))
                accountList.Add(account);

            CfvWalletDto wallet = ConvertRowToDto<CfvWalletDto>(row, walletPropMap);
            if (!string.IsNullOrEmpty(wallet.Address))
                walletList.Add(wallet);
        }
    }
    return new CfvExtractResult { Accounts = accountList, Wallets = walletList };
}

private IEnumerable<(int colIdx, string key)> YieldHeaderCols(Worksheet worksheet)
{
    for (int colIdx = 0; colIdx <= worksheet.Cells.MaxDataColumn; colIdx++)
        if (worksheet.Cells[0, colIdx].Value?.ToString() is string headerText && _utility.ConvertName(headerText) is string key)
            yield return (colIdx, key);
}

private static Dictionary<int, PropertyInfo> InitPropMap<T>((int colIdx, string key)[] headerCols)
{
    Dictionary<int, PropertyInfo> propMap = new();
    foreach ((int colIdx, string key) in headerCols)
        if (typeof(T).GetProperty(key) is PropertyInfo prop && prop.CanWrite)
            propMap[colIdx] = prop;
    return propMap;
}

private static T ConvertRowToDto<T>(Aspose.Cells.Row row, Dictionary<int, PropertyInfo> propMap) where T : new()
{
    T dto = new();
    foreach (var kv in propMap)
        if (row[kv.Key].Value?.ToString() is string value)
            kv.Value.SetValue(dto, value);
    return dto;
}