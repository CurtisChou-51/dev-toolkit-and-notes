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


## 執行 PowerShell
```
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0xxx.ps1" %*
```

**用途**：用批次檔包住 PowerShell 腳本，雙擊即可執行，不必修改系統的執行原則

---

### `-NoProfile`
- 不載入使用者的 PowerShell 設定檔（`$PROFILE`）

**用途**：啟動較快，且避免設定檔內的別名、函式影響腳本行為

---

### `-ExecutionPolicy Bypass`
- 執行原則（Execution Policy）預設會擋下 `.ps1` 腳本
- `Bypass`：不做任何限制，也不顯示警告
- 只對這一次啟動的 PowerShell 有效，不會改到系統或使用者的設定

**用途**：不需事先執行 `Set-ExecutionPolicy` 就能跑腳本

---

### `-File "%~dp0xxx.ps1"`
- `-File`：指定要執行的腳本檔，必須放在 PowerShell 參數的最後，其後的內容全部視為腳本的參數
- `%~dp0`：這個 .bat 所在資料夾的完整絕對路徑（結尾已帶有反斜線），像是展開成 `D:\tools\xxx.ps1`
- 外層雙引號：路徑含空白時（例如 `D:\My Tools\`）不會被切成兩個參數

**用途**：不論從哪個資料夾執行批次檔，都能找到與它放在一起的腳本

---

### `%*`
- 代表呼叫批次檔時帶入的所有參數，原封不動轉交給腳本
- `%1`、`%2` 則是個別取第 1、第 2 個參數，腳本沒有定義 `param(...)` 時，`%*` 展開後是空的，留著不影響
