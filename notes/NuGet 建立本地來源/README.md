# NuGet 建立本地來源

- NuGet 建立本地來源，用於網路受限環境開發

## 連網環境 - 準備專案結構

- 如果連網環境已有專案結構或是 SourceCode 可以直接跳到下一步，否則需要先建立專案結構
- 要取得專案結構，除了整包複製 SourceCode 或是逐一手動選取外，可以使用 `robocopy` 由 SourceCode 中將專案的解決方案檔 (.sln) 及專案檔 (.csproj) 提取出來
- 這個指令取出的結果**只會包含 .sln 及 .csproj 檔案、保持原有的資料夾結構、不會複製空資料夾 (因為 /S 參數)**，可以說是快速又乾淨
```
robocopy "D:\SourceCode" "D:\Projects" *.sln *.csproj /S
```

## 連網環境 - 下載套件

- 需要安裝 dotnet SDK，將專案所需的 NuGet 套件還原 (Package Restore)，目的為將套件下載到本機的指定取資料夾中以供後續使用
- `restore` 後的參數指向剛剛建立的專案結構；`--packages` 參數指定套件下載的目標資料夾
```
dotnet restore "D:\Projects\XXX.sln" --packages "D:\MyPackages"
```

## 搬移檔案

- 將 D:\MyPackages 資料夾整個搬移到受限環境的電腦或是網路中

## 受限環境 - 配置來源

- 在 Visual Studio 中開啟 工具 > 選項 > NuGet 套件管理員 > 套件來源