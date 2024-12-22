# ZIP 套件 Directory Traversal 漏洞

- 使用 Visual Studio 開發時出現套件安全性警告，開啟 [連結](https://github.com/advisories/GHSA-xhg6-9j5j-w4vf) 查看說明為 Directory Traversal，此弱點與 ZIP 檔案解壓縮的處理機制有關

## 攻擊原理

- 建構惡意的 ZIP 檔案，使用特定的 entry name
  - 包含 `..\\` 路徑跳脫符號
  - 絕對路徑
  
- 當系統解壓縮這類 ZIP 檔案時，可能會跳脫預期的目標目錄、寫入系統其他位置的檔案

