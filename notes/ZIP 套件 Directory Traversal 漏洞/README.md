# ZIP 套件 Directory Traversal 漏洞

- 使用 Visual Studio 開發時出現套件安全性警告，經查看 [GitHub 安全公告](https://github.com/advisories/GHSA-xhg6-9j5j-w4vf) 得知這是一個與 ZIP 檔案解壓縮處理機制相關的 Directory Traversal 漏洞。相關的修正方式和測試案例可參考 [ProDotNetZip](https://github.com/mihula/ProDotNetZip) 專案
- 原先並沒有想到 ZIP 套件與 Directory Traversal 漏洞的關聯性，看了測試案例之後清楚了許多

## 攻擊原理

- 攻擊者可以建構惡意的 ZIP 檔案利用此漏洞，如使用特定的 entry name：
  - 包含 `..\\` 路徑跳脫符號
  - 絕對路徑
  
- 當系統解壓縮這類 ZIP 檔案時，可能會跳脫預期的目標目錄、寫入系統其他位置的檔案

## 筆記

- 在進行檔案系統操作時，我們習慣對外部參數進行驗證
- 而這個範例提醒了我們也別忘了 ZIP 檔案的 entry name 也應被視為外部參數，需要進行相同的驗證和防護
