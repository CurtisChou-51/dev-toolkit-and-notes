<Query Kind="Program">
  <Namespace>System.Security.Cryptography</Namespace>
</Query>

void Main()
{
    string folderA = @"C:\Users\user\Desktop\xxx\new\main";  //目前新版的
    string folderB = @"C:\Users\user\Desktop\xxx\old\main";  //上一版
    
    // 找出差異檔案
    $"{folderA}、{folderB}".Dump("開始比對差異檔案");
    var diffItems = CompareDirectories(folderA, folderB).ToList();
    diffItems.Dump("差異檔案");
    
    Reporter reporter = new Reporter(
        winMergePath: @"C:\Program Files\WinMerge\WinMergeU.exe",
        reportPath: @"C:\Users\user\Desktop\report",
        timeout: 1000,
        excludes: new string[]{ ".mdf", ".ldf" }
    );
    
    foreach (var item in diffItems)
        reporter.CreateReport(item);
}

public class Reporter
{
    private string _winMergePath;
    private string _reportPath;
    private int _timeout = -1;
    private string[] _excludes = new string[]{ };
    
    public Reporter(string winMergePath, string reportPath, int timeout, string[] excludes)
    {
        _winMergePath = winMergePath;
        _reportPath = reportPath;
        _timeout = timeout;
        _excludes = excludes;
    }
    
    public void CreateReport(DiffItem item)
    {
        string ext = Path.GetExtension(item.PathA);
        if (_excludes.Contains(ext))
        {
            item.PathA.Dump("skip");
            return;
        }
        
        string savePath = Path.Combine(_reportPath, item.RelativePath) + ".html";
        Directory.CreateDirectory(Path.GetDirectoryName(savePath));
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = _winMergePath,
            Arguments = $"\"{item.PathA}\" \"{item.PathB}\" -or \"{savePath}\" -noninteractive",
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = Process.Start(psi))
        {
            process.WaitForExit(_timeout);
            if (!process.HasExited)
                process.Kill();
        }
    }
}

public class DiffItem
{
    public string PathA { get; set; }
    public string PathB { get; set; }
    public string RelativePath { get; set; }
}

static IEnumerable<DiffItem> CompareDirectories(string pathA, string pathB)
{
    var filesA = Directory.GetFiles(pathA, "*", SearchOption.AllDirectories);
    var filesB = Directory.GetFiles(pathB, "*", SearchOption.AllDirectories);

    foreach (var fileA in filesA)
    {
        string relativePath = fileA.Substring(pathA.Length + 1);
        string correspondingFileB = Path.Combine(pathB, relativePath);

        if (!File.Exists(correspondingFileB))
        {
            // 文件只存在於A文件夾
            yield return new DiffItem { PathA = fileA, PathB = "", RelativePath = relativePath };
        }
        else if (!CompareFileHashes(fileA, correspondingFileB))
        {
            // 文件內容不同
            yield return new DiffItem { PathA = fileA, PathB = correspondingFileB, RelativePath = relativePath };
        }
    }
}

static bool CompareFileHashes(string file1, string file2)
{
    using (var sha256 = SHA256.Create())
    {
        using (var stream1 = File.OpenRead(file1))
        using (var stream2 = File.OpenRead(file2))
        {
            byte[] hash1 = sha256.ComputeHash(stream1);
            byte[] hash2 = sha256.ComputeHash(stream2);
            return BitConverter.ToString(hash1) == BitConverter.ToString(hash2);
        }
    }
}