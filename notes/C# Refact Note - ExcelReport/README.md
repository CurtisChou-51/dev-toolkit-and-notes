# C# Refact Note - ExcelReport

- 對於產生報表方法的重構筆記

## 重構描述

- 使用 `GroupJoin` 一次性完成資料分組
  - 以 `codeName` 作為分組的邏輯更清晰直觀
  - 維持原本查無資料時也會為該 `codeName` 建立工作表的設計
  - 避免在迴圈中重複執行 `Where` 查詢提升效能，大量資料時效果更明顯

## Before

```csharp
var results = GetQueryResult(conditionModel);
var codeNames = GetQueryCodeNames(conditionModel);

foreach (var codeName in codeNames)
{
    var resultItem = results.Where(x => x.Code_Name == codeName);
    
    // Header information
    var sheet = workbook.Worksheets.Add(codeName);
    sheet.Cells[0, 0].Value = $"{codeName}_查詢單";
    sheet.Cells[1, 0].Value = $"使用者：{UserName}";
    sheet.Cells[2, 0].Value = $"查詢日期：{conditionModel.St_Date}~{conditionModel.End_Date}";
    sheet.Cells[3, 0].Value = $"查詢項目：{codeName}";
    sheet.Cells[4, 0].Value = $"筆數：{resultItem.Count()}";
    
    int row = 6;
    foreach (var item in resultItem)
    {
        sheet.Cells[row, 0].Value = item.DtlNo;
        sheet.Cells[row, 1].Value = item.Owner;
        // other columns ...
        row++;
    }
}
```

## After

```csharp
var results = GetQueryResult(conditionModel);
var codeNames = GetQueryCodeNames(conditionModel);
var codeGroupedResults = codeNames.GroupJoin(results,
    codeName => codeName,
    result => result.Code_Name,
    (codeName, resultItems) => new { codeName, resultItems }
);

foreach (var g in codeGroupedResults)
{
    // Header information
    var sheet = workbook.Worksheets.Add(g.codeName);
    sheet.Cells[0, 0].Value = $"{g.codeName}_查詢單";
    sheet.Cells[1, 0].Value = $"使用者：{UserName}";
    sheet.Cells[2, 0].Value = $"查詢日期：{conditionModel.St_Date}~{conditionModel.End_Date}";
    sheet.Cells[3, 0].Value = $"查詢項目：{g.codeName}";
    sheet.Cells[4, 0].Value = $"筆數：{g.resultItems.Count()}";
    
    int row = 6;
    foreach (var item in g.resultItems)
    {
        sheet.Cells[row, 0].Value = item.DtlNo;
        sheet.Cells[row, 1].Value = item.Owner;
        // other columns ...
        row++;
    }
}
```
