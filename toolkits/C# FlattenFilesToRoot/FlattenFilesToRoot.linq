<Query Kind="Program" />

void Main()
{
    string baseFolder = @"your baseFolder";
    string[] filePaths = Directory.GetFiles(baseFolder, "*", SearchOption.AllDirectories);
    foreach (string filePath in filePaths)
    {
        string newFilePath = Path.Combine(baseFolder, Path.GetRelativePath(baseFolder, filePath).Replace(@"\", "_"));
        File.Move(filePath, newFilePath);
        $"{filePath} -> {newFilePath}".Dump();
    }
}
