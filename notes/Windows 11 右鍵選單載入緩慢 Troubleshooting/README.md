# Windows 11 右鍵選單載入緩慢 Troubleshooting

- 問題背景：Windows 11 檔案總管按右鍵時，選單中段（如：Adobe Acrobat Reader、詢問 Copilot、Notepad++、Bandizip、記事本）會先顯示「載入中」，等一段時間才出現完整選單

## 在程式設定中關閉選單項目無效

- 嘗試在 Bandizip 設定中取消右鍵選單後，Bandizip 選項不再顯示，但按右鍵時仍會先出現「載入中」
- 觀察後發現關閉的只是「顯示」，擴充功能本身仍會被載入，按右鍵時 Windows 先載入各程式的選單擴充功能，並詢問是否顯示，回應之前 Windows 先放「載入中」佔位
- 故要讓選單變快，必須讓 Windows 不去載入該擴充功能

## 稀疏套件 (Sparse Package)

- 傳統安裝的程式（.exe / .msi）若要加入 Windows 11 新右鍵選單，需要 App 身分，因此會額外註冊一個不含程式的空殼套件，這類套件可以用 `Get-AppxPackage` 查到，移除後只會拿掉選單整合，程式本身不受影響

```powershell
Get-AppxPackage *adobe* | Select-Object Name, PackageFullName, InstallLocation
```

## 處理方式

### 1. 有稀疏套件的程式（Adobe、Bandizip、Notepad++）

```powershell
Get-AppxPackage AdobeAcrobatReaderCoreApp | Remove-AppxPackage
Get-AppxPackage *bandi* | Remove-AppxPackage
Get-AppxPackage *NotepadPlusPlus* | Remove-AppxPackage
```

- 先單獨執行 `Get-AppxPackage` 確認清單再接 `Remove-AppxPackage`，萬用字元範圍太大可能誤刪同廠商的其他 App
- 程式更新或修復安裝後可能會再註冊回來，屆時再移除一次

### 2. 重新啟動檔案總管

```powershell
Stop-Process -Name explorer -Force
```

## Get-AppxPackage 常用寫法

- 本身只是查詢，不串上其他指令不會改變任何東西。用來列出目前使用者已安裝的 App 套件（Store App、內建 App、稀疏套件），傳統安裝程式不會出現在結果中

| 指令 | 作用 |
| --- | --- |
| `Get-AppxPackage` | 列出目前使用者的所有套件 |
| `Get-AppxPackage *adobe*` | 以萬用字元篩選名稱 |
| `Get-AppxPackage -AllUsers` | 列出所有使用者的套件（需系統管理員） |
| `... \| Select-Object Name, Version, InstallLocation` | 只顯示指定欄位 |
| `... \| Format-List *` | 顯示所有欄位 |
| `... \| Remove-AppxPackage` | 移除查到的套件 |

- 避免移除 `Microsoft.WindowsStore` 或 `Microsoft.VCLibs`、`Microsoft.UI.Xaml` 這類 Framework 套件
