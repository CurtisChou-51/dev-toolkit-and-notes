# bat

## 通用項目

### 檔案格式
- 出現亂碼或是顯示異常檢查：
  - 換行：CRLF
  - 編碼：UTF-8 without BOM

### 批次檔起手式
```
@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion
cd /d "%~dp0"
```

## Fast Settings

### NuGet

- 修改 NuGet 快取路徑和套件路徑
```
setx NUGET_HTTP_CACHE_PATH "D:\.nuget\v3-cache"
setx NUGET_PACKAGES "D:\.nuget\packages"
```