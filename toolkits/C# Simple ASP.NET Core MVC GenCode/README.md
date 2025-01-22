# C# Simple ASP.NET Core MVC GenCode

- 這是一個使用 LINQPad 的簡易程式碼生成工具，用於根據模板檔案生成專案代碼，適合用於需要根據模板生成多個檔案的情況

## 功能說明
- 根據模板資料夾中的檔案生成對應的專案代碼
- 替換檔案名稱和檔案內容中的佔位符
- 自動建立輸出目錄，保持原始目錄結構

## 使用方式

### 1.開啟 GenCode.linq

### 2. 設定參數：
- `projName`：專案名稱
- `funcName`：功能名稱
- `inputDir`：模板資料夾路徑
- `outputDir`：輸出資料夾路徑

### 3. 在模板檔案中使用以下佔位符：
- `[[ProjName]]`：會被替換為專案名稱
- `[[FuncName]]`：會被替換為功能名稱

### 4. 執行後會在輸出資料夾中生成替換後的檔案

## 範例

```csharp
string projName = "MyWebApp";  // 專案
string funcName = "NewFunc";  // 功能名稱
string inputDir = @"C:\Users\user\Desktop\GenCode\BasicTemplate";  // template資料夾
string outputDir = @"C:\Users\user\Desktop\GenCode\Result";  // 輸出資料夾
```

- 如果模板資料夾中有以下檔案：
```
BasicTemplate/
  ├── [[FuncName]]Controller.cs
  └── Models/
      └── [[FuncName]]Model.cs
```

- 執行後會在輸出資料夾生成：
```
Result/
  ├── NewFuncController.cs
  └── Models/
      └── NewFuncModel.cs
```

## 注意事項

- 需要 LINQPad 執行環境，且版本過舊的 LINQPad 不支援 `Path.GetRelativePath` 方法
- 確保模板資料夾中的檔案使用 UTF-8 編碼
