# 批次產生差異檔案 WinMerge 報告

- 問題背景：需要提供版本更新前後的程式碼比對，批次產生 WinMerge 報告

## 功能說明

這是一個使用 LINQPad 的工具程式，用於比對兩個目錄下的檔案差異，並使用 WinMerge 產生差異報告。主要功能包括：

1. 比對兩個目錄下所有檔案的差異，使用 SHA256 進行檔案內容比對
2. 針對差異檔案使用 WinMerge 產生 HTML 格式的差異報告

## 使用方式

### 1. 開啟 main.linq

### 2. 設定比對目錄
```csharp
string folderA = @"C:\Path\To\New\Version";    // 新版目錄
string folderB = @"C:\Path\To\Old\Version";    // 舊版目錄
```

### 3. 設定報告產生器
```csharp
Reporter reporter = new Reporter(
    winMergePath: @"C:\Program Files\WinMerge\WinMergeU.exe",   // WinMerge 執行檔路徑
    reportPath: @"C:\Path\To\Report",                           // 報告輸出目錄
    timeout: 1000,                                              // 逾時時間(毫秒)
    excludes: new string[]{ ".mdf", ".ldf" }                    // 要排除的檔案類型
);
```

## 注意事項

### 執行環境需求
- 需安裝 WinMerge
- 需要 LINQPad 執行環境
- 目錄需有讀取權限

### 效能考量
- 大型檔案比對可能耗時
- 設定適當的逾時時間
- 排除不需要產生報告的檔案類型

### 報告產生
- 報告為 WinMerge HTML 格式
- 會自動建立報告目錄結構
- 報告檔案位置會依照原檔案相對路徑放置