# bat

- 編寫簡易的 Windows 批次檔 (Batch File) 範本

```
@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion
cd /d "%~dp0"
```

## NuGet

- 修改 NuGet 快取路徑和套件路徑
```
setx NUGET_HTTP_CACHE_PATH "D:\.nuget\v3-cache"
setx NUGET_PACKAGES "D:\.nuget\packages"
```