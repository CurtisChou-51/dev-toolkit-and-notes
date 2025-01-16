# C# 使用 Netstat

- 用途：執行 Windows 系統的 `netstat -ano` 指令取得 port 使用狀況，之後使用 C# 程式進行其他處理

## 主要功能

- 列出所有 `netstat -ano` 指令取得的 port 使用狀況，也可透過命令列參數搜尋特定 port
```bash
# 顯示所有 port
Program.exe

# 只顯示 80 port
Program.exe 80
```

- 輸出格式為以逗號分隔的文字：
```
"Port", "Process Name", "Protocol", "State", "Foreign Address"
```

## 運作流程
1. 執行 `netstat -ano` 命令取得原始資料
2. 解析命令輸出的文字內容，轉換自訂型別 `PortInfo`
3. 使用 `Process.GetProcesses()` 取得處理程序名稱
4. 根據輸入的 port 參數進行過濾
5. 結果依 port 排序後輸出

