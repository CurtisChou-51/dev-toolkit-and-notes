# C# Aspose Excel 讀取轉換

- 上次 [重構解析特定類型 Excel](https://github.com/CurtisChou-51/dev-toolkit-and-notes/tree/main/notes/C%23%20Refact%20Note%20-%20ExtractCfvExcelData) 之後進一步提取模組，依舊使用 [Aspose.Cells](https://docs.aspose.com/cells/net/) 讀取檔案配合反射轉換，將 Excel 資料轉換為強型別的 C# 物件

## 主要特點

### 動態表頭對應
- 透過自訂的 `headerMapper` 方法處理各種表頭格式，例如清除空白或特殊字元

範例：
```csharp
static string? HeaderMapper(string? header)
{
    string? normalizedHeader = header is null ? null : string.Concat(header.Where(c => !char.IsControl(c))).Trim();
    return normalizedHeader switch
    {
        "銀行代碼" => nameof(BankTransDto.BankCode),
        "帳號" => nameof(BankTransDto.AccountNumber),
        "帳戶名稱" => nameof(BankTransDto.AccountName),
        "交易日期" => nameof(BankTransDto.TransactionDate),
        "交易金額" => nameof(BankTransDto.Amount),
        "幣別" => nameof(BankTransDto.Currency),
        "交易說明" => nameof(BankTransDto.Description),
        "交易類型" => nameof(BankTransDto.TransactionType),
        _ => normalizedHeader
    };
}
```

### 記憶體優化
- 改善資料讀取，直接跳過空白列，並使用 `GetCellOrNull()` 方法避免不必要的物件建立

範例：
```csharp
// 以前的寫法，可能會建立多餘的 Cell 物件
Cell cell = row.Cells[colIdx];

// 改善後的寫法，避免建立多餘的 Cell 物件
Cell? cell = row.GetCellOrNull(colIdx);
```

### 延遲載入
- 使用 `IEnumerable yield return`，不會一次載入所有資料，可以由呼叫端控制
範例：
```csharp
foreach (var batch in converter.YieldDatas().Chunk(1000))
{
    ProcessBatch(batch);
}
```

### 表頭檢查
- 提供 `YieldMappedColumnNames` 方法，可在讀取資料前檢查表頭對應是否正確

範例：
```csharp
// 格式檢查
string[] mappedCols = converter.YieldMappedColumnNames().ToArray();
string[] requiredCols =
[
    nameof(BankTransDto.BankCode),
    nameof(BankTransDto.AccountNumber),
    nameof(BankTransDto.TransactionDate),
    nameof(BankTransDto.Amount)
];

string[] missingCols = requiredCols.Except(mappedCols).ToArray();
if (missingCols.Length != 0)
    throw new Exception($"缺少必要欄位 {string.Join(", ", missingCols)}");

// 正式讀取
foreach (var item in converter.YieldDatas())
{
    // ...
}
```

## 範例
- 範例專案：[ExcelConsoleApp](ExcelConsoleApp)
- 主要類別：[AsposeSheetConverter.cs](ExcelConsoleApp/AsposeSheetConverter.cs)

## 注意事項
1. Aspose.Cells 為商業授權套件，需自行取得授權
2. 預設表頭在第 0 列，如有不同需求需調整類別建構子參數
3. 反射轉換有一定效能成本，轉換失敗的例外也會影響效能