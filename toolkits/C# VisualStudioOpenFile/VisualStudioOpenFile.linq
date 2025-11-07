<Query Kind="Program" />


void Main()
{
    string module = "CaseOverview";
    string projectRoot = @"D:\MyProject\MyProject.Web";

    HashSet<string> targetFiles = new(["Index.cshtml", "index.js", $"{module}Controller.cs", $"{module}Srv.cs", $"{module}Dao.cs", $"{module}ViewModel.cs"], StringComparer.OrdinalIgnoreCase);
    string[] searchFolders = { "Controllers", "DataAccess", "Models", "Services", "Views", @"wwwroot\cust"  };
    var files = searchFolders
        .SelectMany(p => Directory.EnumerateFiles(Path.Combine(projectRoot, p), "*", SearchOption.AllDirectories))
        .Where(f => f.Contains(module, StringComparison.OrdinalIgnoreCase))
        .Where(f => targetFiles.Contains(Path.GetFileName(f)));

    openFiles(files);

}

void openFiles(IEnumerable<string> files)
{
     string fileArgs = string.Join(", ", files.Select(f => $"\"{f}\""));
     string cmd = @$"Start-Process devenv -ArgumentList @( ""/edit"", {fileArgs} )";


    var psi = new ProcessStartInfo()
    {
        FileName = "powershell.exe",
        Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{cmd.Replace("\"", "\\\"")}\"",
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        CreateNoWindow = true
    };

    using (var process = Process.Start(psi))
    {
        process.WaitForExit();
        string error = process.StandardError.ReadToEnd();
        if (!string.IsNullOrEmpty(error))
            Console.WriteLine("Error: " + error);
    }
}
