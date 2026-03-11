public class CaseService
{
    // ... CaseService 其他方法 ...

    public Stream Export()
    {
        List<ReportData> data = FetchReportData();
        using var worker = new ExportWorker();
        return worker.Generate(data);
    }
    
    private class ExportWorker : IDisposable
    {
        private readonly Workbook _workbook;
        private readonly Worksheet _indexSheet;
        private readonly Worksheet _dataSheet;
        private readonly Style _headerStyle;
        private readonly Style _borderStyle;
        private readonly Style _titleStyle;

        private int _currentRow = 0; 
        private int _indexRow = 1;

        public ExportWorker()
        {
            _workbook = new Workbook();
            _indexSheet = _workbook.Worksheets[0];
            _dataSheet = _workbook.Worksheets.Add("Data");

            _headerStyle = CreateHeaderStyle(_workbook); 
            _borderStyle = CreateBorderStyle(_workbook);
            _titleStyle = CreateTitleStyle(_workbook);
        }

        public Stream Generate(List<ReportData> data)
        {
            foreach (var group in data.GroupBy(x => x.Category))
            {
                WriteIndex(group.Key);
                WriteData(group);
            }

            // ... 呼叫更多的報表輔助方法 ...

            var stream = new MemoryStream();
            _workbook.Save(stream, SaveFormat.Xlsx);
            return stream;
        }

        private void WriteIndex(string category)
        {
            _indexSheet.Cells[_indexRow, 0].PutValue(category);
            string targetLink = $"'{_dataSheet.Name}'!A{_currentRow}";
            _indexSheet.Hyperlinks.Add(_indexRow, 0, 1, 1, targetLink);
            _indexRow++;
        }

        private void WriteData(IGrouping<string, ReportData> data)
        {
            _dataSheet.Cells[_currentRow, 0].PutValue(data.Key);
            _dataSheet.Cells[_currentRow, 0].SetStyle(_titleStyle);
            _currentRow++;
            
            foreach (var item in data)
            {
                _dataSheet.Cells[_currentRow, 0].PutValue(item.Value);
                _dataSheet.Cells[_currentRow, 0].SetStyle(_borderStyle);
                _currentRow++;
            }
        }

        public void Dispose()
        {
            _workbook?.Dispose();
        }
        
        // ... (Helper Methods for creating styles) ...

        // ... 更多的報表輔助方法 ...
    }
}