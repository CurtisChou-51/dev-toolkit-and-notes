# C# VisualStudioOpenFile

- 這是一個使用 LINQPad 的簡易工具，用於自動找出專案中特定模組的相關檔案，並用 Visual Studio 開啟它們

## 功能說明

- 依據專案結構客製化，達到直接在現有 Visual Studio 實例中一鍵開啟模組的所有相關檔案的效果
- 以一個 ASP.NET Core MVC 專案為例，搜尋並開啟 Controller、Service、DAO、ViewModel、View 以及前端資源檔案

## 使用方式

### 1. 開啟 VisualStudioOpenFile.linq

### 2. 設定搜尋條件
```csharp
string module = "CaseOverview";  // 目標模組名稱
string projectRoot = @"D:\MyProject\MyProject.Web";  // 專案根目錄
```

### 3. 定義檔案搜尋模式
```csharp
// 要找的檔案
HashSet<string> targetFiles = new([
    "Index.cshtml", 
    "index.js", 
    $"{module}Controller.cs",  // 例如：CaseOverviewController.cs
    $"{module}Srv.cs",
    $"{module}Dao.cs",
    $"{module}ViewModel.cs"
]);

// 在特定資料夾搜尋
string[] searchFolders = { 
    "Controllers", 
    "DataAccess", 
    "Models", 
    "Services", 
    "Views", 
    @"wwwroot\cust" 
};
```

### 4. 執行後會在已開啟的 Visual Studio 中打開相關檔案

## 範例

假設 `module = "CaseOverview"`，會搜尋並開啟：
```
├── Controllers\
│   └── CaseOverviewController.cs
├── Services\
│   └── CaseOverviewSrv.cs
├── DataAccess\
│   └── CaseOverviewDao.cs
├── Models\
│   └── CaseOverviewViewModel.cs
├── Views\
│   └── CaseOverview\
│       └── Index.cshtml
└── wwwroot\cust\
    └── CaseOverview\
        └── index.js
```

## 工作原理

1. **指定搜尋範圍**：只在 Controllers、Services 等特定資料夾搜尋
2. **呼叫 VS**：透過 PowerShell 執行 `devenv /edit` 命令
3. **在現有實例開啟**：使用 `/edit` 參數不會啟動新的 VS 視窗

## 注意事項

- 需要 LINQPad 執行環境
- 用於 Visual Studio（devenv.exe 必須在系統路徑中）