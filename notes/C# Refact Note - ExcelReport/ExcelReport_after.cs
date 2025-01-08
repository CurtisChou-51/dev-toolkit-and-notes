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