# C# Refact Note - ExportPartitionData

- 對於匯出分組資料的重構筆記

## 重構描述

- 資料結構調整：
  - 將原本相似的欄位處理邏輯統一化
  - 使用 `List<(typeId, content)>` 取代多個獨立的 List
  - 明確指定產生檔案順序為 `typeId`

- 資料流調整：
  - 將依照筆數拆分檔案的邏輯移出為獨立的擴充方法 `Partition`
  - 使用 Pipeline 式處理：資料擷取 → 排序 → 分批 → 產生內容 → 產生檔案

## Before

```csharp
public class SomeService
{
    // other ...

    /// <summary> 匯出ZIP </summary>
    public byte[] Export(long id)
    {
        int targetCount = 50;
        var data = GetDetail(id);
        if (data != null)
        {
            int count = 0;
            int file_count = 1;

            List<ZipRequestDto> files = new List<ZipRequestDto>();
            List<string> data1 = new List<string>();
            List<string> data2 = new List<string>();
            List<string> data3 = new List<string>();
            List<string> data4 = new List<string>();
            List<string> data5 = new List<string>();
            List<string> data6 = new List<string>();
            
            var source_data1 = data.Where(c => c.Column1 != null).Select(c => c.Column1);
            foreach (var item in source_data1)
            {
                data1.Add(item);
                count++;
                if (count == targetCount)
                {
                    files.Add(GetZipRequestDto($"word_{file_count++}", data1, data2, data3, data4, data5, data6));
                    data1.Clear();
                    count = 0;
                }
            }

            var source_data2 = data.Where(c => c.Column2 != null).Select(c => c.Column2);
            foreach (var item in source_data2)
            {
                data2.Add(item);
                count++;
                if (count == targetCount)
                {
                    files.Add(GetZipRequestDto($"word_{file_count++}", data1, data2, data3, data4, data5, data6));
                    data1.Clear();
                    data2.Clear();
                    count = 0;
                }
            }

            var source_data3 = data.Where(c => c.Column3 != null).Select(c => c.Column3);
            foreach (var item in source_data3)
            {
                data3.Add(item);
                count++;
                if (count == targetCount)
                {
                    files.Add(GetZipRequestDto($"word_{file_count++}", data1, data2, data3, data4, data5, data6));
                    data1.Clear();
                    data2.Clear();
                    data3.Clear();
                    count = 0;
                }
            }

            var source_data4 = data.Where(c => c.Column4 != null).Select(c => c.Column4);
            foreach (var item in source_data4)
            {
                data4.Add(item);
                count++;
                if (count == targetCount)
                {
                    files.Add(GetZipRequestDto($"word_{file_count++}", data1, data2, data3, data4, data5, data6));
                    data1.Clear();
                    data2.Clear();
                    data3.Clear();
                    data4.Clear();
                    count = 0;
                }
            }

            var source_data5 = data.Where(c => c.Column5 != null).Select(c => c.Column5);
            foreach (var item in source_data5)
            {
                data5.Add(item);
                count++;
                if (count == targetCount)
                {
                    files.Add(GetZipRequestDto($"word_{file_count++}", data1, data2, data3, data4, data5, data6));
                    data1.Clear();
                    data2.Clear();
                    data3.Clear();
                    data4.Clear();
                    data5.Clear();
                    count = 0;
                }
            }

            var source_data6 = data.Where(c => c.Column6 != null).Select(c => c.Column6);
            foreach (var item in source_data6)
            {
                data6.Add(item);
                count++;
                if (count == targetCount)
                {
                    files.Add(GetZipRequestDto($"word_{file_count++}", data1, data2, data3, data4, data5, data6));
                    data1.Clear();
                    data2.Clear();
                    data3.Clear();
                    data4.Clear();
                    data5.Clear();
                    data6.Clear();
                    count = 0;
                }
            }

            if (data1.Count() > 0 || data2.Count() > 0 || data3.Count() > 0 || data4.Count() > 0 || data5.Count() > 0 || data6.Count() > 0)
            {
                files.Add(GetZipRequestDto($"word_{file_count++}", data1, data2, data3, data4, data5, data6));
            }

            return _zipUtil.CreateZip(files);
        }
        return Array.Empty<byte>();
    }

    private ZipRequestDto GetZipRequestDto(string fileName, List<string> data1, List<string> data2, List<string> data3, List<string> data4, List<string> data5, List<string> data6)
    {
        List<string> titles = new() { "1.foo", "2.bar", "3.ooo", "4.xxx", "5.abc", "6.123" };
        List<List<string>> datas = new() { data1, data2, data3, data4, data5, data6 };
        var contents = titles.Zip(datas, (title, text) => $"{title}:{string.Join(',', text)}");

        return new ZipRequestDto
        {
            FileName = $"{fileName}.docx",
            FileContent = _wordUtil.CreateFileContent(contents).ToArray()
        };
    }

}
```

## After

```csharp
public class SomeService
{
    // other ...

    /// <summary> 匯出ZIP </summary>
    public byte[] Export(long id)
    {
        int targetCount = 50;
        var data = GetDetail(id);
        if (data == null)
            return Array.Empty<byte>();

        var zipRequestDtos = ExtractDataForExport(data)
            .OrderBy(x => x.typeId)
            .Partition(targetCount)
            .Select(DataForExportToContents)
            .Select((contents, partIdx) => new ZipRequestDto
            {
                FileName = $"word_{partIdx + 1}.docx",
                FileContent = _wordUtil.CreateFileContent(contents).ToArray()
            });
        return _zipUtil.CreateZip(zipRequestDtos);
    }

    /// <summary> 擷取匯出用資料 </summary>
    private static IEnumerable<(int typeId, string content)> ExtractDataForExport(IEnumerable<XXXEntity> data)
    {
        foreach (var item in data)
        {
            if (item.Column1 != null)
                yield return new(1, item.Column1);
            if (item.Column2 != null)
                yield return new(2, item.Column2);
            if (item.Column3 != null)
                yield return new(3, item.Column3);
            if (item.Column4 != null)
                yield return new(4, item.Column4);
            if (item.Column5 != null)
                yield return new(5, item.Column5);
            if (item.Column6 != null)
                yield return new(6, item.Column6);
        }
    }

    /// <summary> 將匯出用資料轉換成內容 </summary>
    private static IEnumerable<string> DataForExportToContents(IEnumerable<(int typeId, string content)> dataForExport)
    {
        var typeNames = new List<(int typeId, string typeName)>
        {
            (1, "1.foo"), (2, "2.bar"), (3, "3.ooo"), (4, "4.xxx"), (5, "5.abc"), (6, "6.123")
        };

        return typeNames.GroupJoin(
            dataForExport,
            tn => tn.typeId,
            de => de.typeId,
            (tn, de) => $"{tn.typeName}:{string.Join(",", de.Select(x => x.content))}"
        );
    }
}

public static class EnumerableExtd
{
    public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> src, int size)
    {
        List<T> tmp = new List<T>();
        foreach (var item in src)
        {
            tmp.Add(item);
            if (tmp.Count == size)
            {
                yield return tmp.ToList();
                tmp.Clear();
            }
        }
        if (tmp.Count > 0)
            yield return tmp.ToList();
    }
}
```
