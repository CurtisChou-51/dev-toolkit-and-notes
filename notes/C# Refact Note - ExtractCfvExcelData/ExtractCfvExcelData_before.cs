private CfvExtractResult ExtractCfvExcelData(Workbook wb)
{
    List<CfvAccountDto> accountList = new();
    List<CfvWalletDto> walletList = new();
    Type accountType = typeof(CfvAccountDto);
    Type walletType = typeof(CfvWalletDto);

    foreach (Worksheet worksheet in wb.Worksheets)
    {
        for (int i = 1; i <= worksheet.Cells.MaxDataRow; i++)
        {
            CfvAccountDto account = new();
            CfvWalletDto wallet = new();

            for (int j = 0; j <= worksheet.Cells.MaxDataColumn; j++)
            {
                if (worksheet.Cells[0, j].Value != null)
                {
                    string? key = _utility.ConvertName(worksheet.Cells[0, j].Value.ToString());
                    if (key != null)
                    {
                        PropertyInfo? accProperty = accountType.GetProperty(key);
                        PropertyInfo? walletProperty = walletType.GetProperty(key);

                        if (accProperty != null && accProperty.CanWrite && worksheet.Cells[i, j].Value != null)
                            accProperty.SetValue(account, worksheet.Cells[i, j].Value.ToString());

                        if (walletProperty != null && walletProperty.CanWrite && worksheet.Cells[i, j].Value != null)
                            walletProperty.SetValue(wallet, worksheet.Cells[i, j].Value.ToString());
                    }
                }
            }
            accountList.Add(account);
            walletList.Add(wallet);
        }
    }

    return new CfvExtractResult
    {
        Accounts = accountList.Where(x => !string.IsNullOrEmpty(x.Name)).ToList(),
        Wallets = walletList.Where(x => !string.IsNullOrEmpty(x.Address)).ToList()
    };
}