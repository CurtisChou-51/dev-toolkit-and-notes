# bat

- 編寫簡易的 Windows 批次檔 (Batch File) 範本

```
@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion
cd /d "%~dp0"
```
