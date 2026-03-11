# C# Refact Note - ExportWorker

- 以「產生客製化複雜報表」為例，針對巨型方法進行拆分，解決「參數傳遞地獄」重構筆記。

## 重構描述

- 產生高度客製化的報表時可能會伴隨大量的樣式、框線的設定，導致主方法變得非常龐大。此時即使拆分成多個小方法也會面臨困境：
  - **選項1 (傳遞參數)**：將共用變數作為參數在方法間傳遞，會導致方法簽章膨脹，甚至可能需要使用 `ref` 參數來處理。
  - **選項2 (類別層級私有變數)**：將共用變數宣告為 Service 的 Private Fields 會汙染原 Service，對於 DI 為 Singleton 的 Service 更是會產生資料錯亂。

對巨型方法進行拆分時，可以將選項1的參數再包裝為一個「參數物件 (Parameter Object)」類別來傳遞，但依然會在原 Service 殘留多個與核心職責不太相關的方法。如果仔細觀察，會發現這些被拆散的輔助方法其實彼此有著極高內聚的特性（頻繁共用相同的輔助資料、樣式設定與行數指標等狀態）。

比起只抽出參數，這時就非常適合將「參數」與「輔助方法」一併打包，抽出到一個完全獨立的 `Worker` 類別中。這麼做不僅對原 Service 隱藏了繁雜的實作細節，更能利用物件的特性封裝了那些原本必須四處傳遞的共用變數，且在每次建構時都能確保執行環境的乾淨與獨立。

> [!NOTE]  
> 這個案例的重構與 Refactoring 的 `Replace Method with Method Object` 是類似的概念

## Before

```csharp
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
```

## After

```csharp
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
```
