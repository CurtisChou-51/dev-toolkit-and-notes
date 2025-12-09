# C# Refact Note - ExtractCfvExcelData

- 對於 `ExtractCfvExcelData` 方法的重構筆記

## 重構描述

- `ExtractCfvExcelData` 方法用途為解析特定類型 Excel，由於此類型 Excel 資料性質相同但欄位未必完全符合故使用反射轉換，最終將資料轉換成 CfvAccountDto 與 CfvWalletDto 兩種類型

- 效能優化：預先建立屬性 Mapping，減少重複的反射呼叫

- 拆分為小方法，原方法只負責主流程協調：
  - `YieldHeaderCols` 讀取標題列，產生 index + 對應的 property 名稱
  - `InitPropMap` 預先建立屬性字典以優化效能
  - `ConvertRowToDto` 處理反射賦值，並使用泛型支援多種 DTO 類型

- 細部調整：主要流程重構完成後可再次優化 pattern matching 的使用，進一步限制暫時性變數的 Scope 並使程式碼更簡潔

```csharp
foreach ((int colIdx, string key) in headerCols)
{ 
    var prop = typeof(T).GetProperty(key);
    if (prop != null && prop.CanWrite)
        propMap[colIdx] = prop;
}

// 可修改為 pattern matching

foreach ((int colIdx, string key) in headerCols)
    if (typeof(T).GetProperty(key) is PropertyInfo prop && prop.CanWrite)
        propMap[colIdx] = prop;
```

## Before

```csharp
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
```

## After

```csharp
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
```