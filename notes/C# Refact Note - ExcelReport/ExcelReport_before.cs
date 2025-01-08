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