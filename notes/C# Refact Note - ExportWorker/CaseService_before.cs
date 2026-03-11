public class CaseService
{
    // ... CaseService 其他方法 ...

    public Stream Export()
    {
        Workbook workbook = new Workbook();
        
        // 必須手動建立、保存一堆 Style
        Style headerStyle = workbook.CreateStyle();
        // ...設定 headerStyle ...
        
        Style borderStyle = workbook.CreateStyle();
        // ...設定 borderStyle ...
        
        Style titleStyle = workbook.CreateStyle();
        // ...設定 titleStyle ...

        Worksheet indexSheet = workbook.Worksheets[0];
        Worksheet dataSheet = workbook.Worksheets.Add("Data");
        
        int currentRow = 0;
        int indexRow = 1;
        List<ReportData> data = FetchReportData();
        foreach (var group in data.GroupBy(x => x.Category))
        {
            WriteIndex(indexSheet, group.Key, dataSheet.Name, ref indexRow, ref currentRow);
            WriteData(dataSheet, group, headerStyle, borderStyle, titleStyle, ref currentRow);
        }

        // ... 呼叫更多的報表輔助方法 ...

        var stream = new MemoryStream();
        workbook.Save(stream, SaveFormat.Xlsx);
        return stream;
    }

    private void WriteIndex(Worksheet indexSheet, string category, string targetSheetName, ref int indexRow, ref int targetRow)
    {
        indexSheet.Cells[indexRow, 0].PutValue(category);
        // 跨 Sheet 超連結，依賴對方傳來的列數
        indexSheet.Hyperlinks.Add(indexRow, 0, 1, 1, $"'{targetSheetName}'!A{targetRow}");
        indexRow++;
    }

    private void WriteData(Worksheet sheet, IGrouping<string, ReportData> data, Style headerStyle, Style borderStyle, Style titleStyle, ref int currentRow)
    {
        sheet.Cells[currentRow, 0].PutValue(data.Key);
        sheet.Cells[currentRow, 0].SetStyle(titleStyle);
        currentRow++;
        
        foreach (var item in data)
        {
            sheet.Cells[currentRow, 0].PutValue(item.Value);
            sheet.Cells[currentRow, 0].SetStyle(borderStyle);
            currentRow++;
        }
    }

    // ... 更多的報表輔助方法 ...
}