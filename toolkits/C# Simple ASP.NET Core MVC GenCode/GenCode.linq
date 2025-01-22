<Query Kind="Program" />

void Main()
{
    string projName = "MyWebApp";  // 專案
    string funcName = "NewFunc";  // 功能名稱
    string inputDir = @"C:\Users\user\Desktop\GenCode\BasicTemplate";  // template資料夾
    string outputDir = @"C:\Users\user\Desktop\GenCode\Result";  // 輸出資料夾
    
    var filepaths = Directory.GetFiles(inputDir, "*", SearchOption.AllDirectories);
    foreach (var filepath in filepaths)
    {
        // 根據模板資料夾的相對路徑生成對應的輸出路徑，並替換檔案名稱中的佔位符
        string outputPath = Path.Combine(outputDir, Path.GetRelativePath(inputDir, filepath.Replace("[[FuncName]]", funcName)));
        ReplaceText(filepath, outputPath, projName, funcName);
    }
}

void ReplaceText(string inputPath, string outputPath, string projName, string funcName)
{
    string content = File.ReadAllText(inputPath);
    string replacedContent = content.Replace("[[ProjName]]", projName).Replace("[[FuncName]]", funcName);  // 替換檔案內容中的佔位符
    Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
    File.WriteAllText(outputPath, replacedContent, Encoding.UTF8);
}