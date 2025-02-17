# Visual Studio 設定

- 紀錄一些 Visual Studio 設定

## 字型設定

- 工具 > 選項 > 環境 > 字型和色彩

![](字型設定/01.png)

![](字型設定/02.png)

## 編碼設定

- 工具 > 自訂 > 命令 > 檔案 > 加入命令 > 檔案 > 進階儲存選項

![](編碼設定/01.png)

![](編碼設定/02.png)

![](編碼設定/03.png)

![](編碼設定/04.png)

![](編碼設定/05.png)

## 編碼設定 - Visual Studio 2022 v17.13

- 工具 > 選項 > 環境 > 文件

![](編碼設定/06.png)

![](編碼設定/07.png)

## editorconfig

- 在專案目錄加入 `.editorconfig` 也可以設定編碼：

```editorconfig
# Code files
[*.{cs,csx,vb,vbx}]
charset = utf-8-bom
```

- 參考 [Roslyn](https://raw.githubusercontent.com/dotnet/roslyn/refs/heads/main/.editorconfig)

## Code Snippet

- 工具 > 程式碼片段管理員 > 語言

![](Code%20Snippet/01.png)

![](Code%20Snippet/02.png)

- Example：
  - C# `prop`：建立自動實作屬性
  - C# `tryf`：建立 try finally 片段
  - JavaScript `iife`：建立立即執行函數