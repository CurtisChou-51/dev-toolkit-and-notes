<Query Kind="Program" />


void Main()
{
    string module = "CaseOverview";
    string projectRoot = @"D:\MyProject\MyProject.Web";

    string[] patterns = { "*Controller.cs", "*Srv.cs", "*Dao.cs", "*Model.cs", "index.cshtml", "index.js" };

    var files = patterns
        .SelectMany(p => Directory.GetFiles(projectRoot, p, SearchOption.AllDirectories))
        .Where(f => !f.Contains(@"\bin\") && !f.Contains(@"\obj\"))
        .Where(f => Path.GetFullPath(f).Contains(module, StringComparison.OrdinalIgnoreCase))
        .ToList();

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
