# NuGet 建立本地來源

- NuGet 建立本地來源，用於網路受限環境開發

## 連網環境下載套件

- 將專案所需的 NuGet 套件還原 (Package Restore)，目的為將套件下載到本機的指定取資料夾中以供後續使用
```
dotnet restore "D:\Projects\XXX.csproj" --packages "D:\MyPackages"
```

## 搬移檔案

- 將 D:\MyPackages 資料夾整個搬移到受限環境的電腦或是網路中

## 受限環境配置來源

- 在 Visual Studio 中開啟 工具 > 選項 > NuGet 套件管理員 > 套件來源