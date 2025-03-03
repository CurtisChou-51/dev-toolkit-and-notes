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