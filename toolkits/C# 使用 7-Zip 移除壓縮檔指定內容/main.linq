<Query Kind="Program">
  <Namespace>System.IO.Compression</Namespace>
</Query>

void Main()
{
    string filename = "test.zip";  // 目標ZIP檔案名稱
    string zipExePath = @"C:\Program Files\7-Zip\7z.exe";  // 7-Zip執行檔路徑
    var removeEntries = new List<string> {  // 要移除的目錄或檔案列表
        "/A",
        "/B",
        "/C/D/E"
    };
    
    // 複製一份後綴今天日期的壓縮檔
    if (!File.Exists(filename))
    {
        $"檔案不存在 {filename}".Dump();
        return;
    }
    string newfileName = $"{Path.GetFileNameWithoutExtension(filename)}_{DateTime.Now:yyyyMMdd}{Path.GetExtension(filename)}";
    File.Copy(filename, Path.Combine(Environment.CurrentDirectory, newfileName), true);
    
    // 對複製的壓縮檔進行移除
    SevenZipProcessor sevenZipProcessor = new SevenZipProcessor(zipExePath);
    string result = sevenZipProcessor.RemoveEntries(newfileName, removeEntries);
    result.Dump("已移除Entry資訊");

    // 可自行補充檔案後續處理，如複製到 USB 或是同步 OneDrive
}

public class SevenZipProcessor
{
    private string _zipExePath;
    public SevenZipProcessor(string zipExePath)
    {
        _zipExePath = zipExePath;
    }
    
    public string RemoveEntries(string targetFile, List<string> removeEntries)
    {
        string reomveArgString = string.Join(" ", removeEntries.Select(x => $"\"{x}\""));
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = _zipExePath,
            Arguments = $"d \"{targetFile}\" -r {reomveArgString}",
            UseShellExecute = false,
            CreateNoWindow = false,
            RedirectStandardOutput = true,
            StandardOutputEncoding = Encoding.UTF8
        };
        
        using (Process process = Process.Start(psi))
        {
            string output = process.StandardOutput.ReadToEnd();
            string msg = Regex.Match(output, @"Delete data from archive:([^\n]*)\n").Groups[1].Value.Trim();
            process.WaitForExit();
            return msg;
        }
    }
}