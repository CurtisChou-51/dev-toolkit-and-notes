# Batch File 指令

## 批次檔起手式說明

```
@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion
cd /d "%~dp0"
```

### `@echo off`
- 關閉指令回顯（不顯示正在執行的每一行指令）
- `@` 代表該行本身也不要顯示

**用途**：讓畫面輸出更乾淨

---

### `chcp 65001 >nul`
- `chcp`：變更命令提示字元的編碼（Code Page）
- `65001`：UTF-8 編碼
- `>nul`：將指令輸出結果丟棄，不顯示「Active code page: 65001」

**用途**：使用 UTF-8 避免中文顯示亂碼

---

### `setlocal enabledelayedexpansion`
- `setlocal`：讓變數作用域僅限於此批次檔，結束後不會污染全域環境
- `enabledelayedexpansion`：啟用「延遲變數展開」，在 `for` 或 `if` 區塊中，使用 `%變數%` 可能會讀到舊值，啟用後可改用 `!變數!` 取得即時更新的值
```bat
setlocal enabledelayedexpansion
set a=1
for %%i in (1 2 3) do (
    set a=%%i
    echo !a!
)
```

**用途**：主要是避免變數污染，啟用延遲展開則是看使用狀況

---

### `cd /d "%~dp0"`

- `cd`：切換目錄
- `/d`：允許切換磁碟機（例如 C: → D:）
- `%~dp0`：取得目前批次檔所在的資料夾路徑

**用途**：確保批次檔執行時的工作目錄為「批次檔所在位置」，避免從其他目錄執行時相對路徑發生錯誤
