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
            .Partition(targetCount)  /* .NET 6 以上可使用 .Chunk */
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