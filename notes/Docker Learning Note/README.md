# Docker Learning Note

## Docker Desktop on Windows

- Docker 是需要一個 Linux 內核來運行的，在 Windows 上使用 Docker，一般是通過以下方式之一來提供 Linux 內核：
  - WSL2 (Windows Subsystem for Linux 2)，這是目前推薦的方式，效能較好
  - Hyper-V 虛擬化，在早期 Docker for Windows 版本中使用

- 安裝 Docker 時會出現選取選項 (目前已預設勾選 WSL2)：
![](安裝/01.png)

- 這兩個方式都需要在 BIOS 中開啟 CPU 虛擬化功能 (Intel VT)，否則啟動時會出現錯誤：
![](安裝/02.png)

- WSL2 需要開啟 **Windows 功能** 中的 **Virtual Machine Platform** 與 **Windows Subsystem for Linux**：
![](安裝/03.png)

## welcome-to-docker 範例

- 在 [Docker Github](https://github.com/docker/welcome-to-docker) 下載範例 repository `welcome-to-docker`

- 啟動 Docker Desktop，因為是首次使用目前沒有任何的 image
![](first_example/01.png)

- 到 `welcome-to-docker` 存放目錄下輸入指令：
```
docker build -t welcome-to-docker-image .
docker run -d -p 8088:3000 --name welcome-to-docker-container welcome-to-docker-image
```

![](first_example/02.png)

- `docker build -t welcome-to-docker-image .` 指令說明：
  - `docker build`：告訴 Docker 讀取 Dockerfile 建立一個新的 image
  - `-t`：使用 `-t` 參數來指定 image 名稱為 welcome-to-docker-image
  - `-f`：使用 `-f` 指定讀取的 Dockerfile 檔，此處沒有指定故預設讀取 Dockerfile
  - `.`：此參數指定 PATH 或 URL 中的檔案，表示**context(建置上下文)**，建置過程可以引用 context 中的任何檔案，所使用到的 `COPY` 指令也是基於此 context，此處設定為當前目錄

- `docker run -d -p 8088:3000 --name welcome-to-docker-container welcome-to-docker-image` 指令說明：
  - `docker run`：告訴 Docker 建立並啟動一個新的容器
  - `-d`：detach 模式，讓容器在背景執行，而不會佔用當前的終端機
  - `-p 8088:3000`：port 對應，在瀏覽器中訪問 `http://localhost:8088`，實際上是透過 8088 port 訪問容器內的 3000 port
  - `--name welcome-to-docker-container`：指定容器名稱
  - `welcome-to-docker-image`：指定用此 image 來建立容器
  
- image 建立後可以在 Docker Desktop 看到建立的 image
![](first_example/03.png)

- 容器啟動後可以使用瀏覽器開啟 `http://localhost:8088` 看到頁面
![](first_example/04.png)

## Visual Studio Docker 支援

- Visual Studio 的 Docker 支援提供 Container Debugging，讓開發者可以在容器環境中開發與偵錯，確保開發環境與生產環境一致
- 建立一個 Web 專案 WebApplicationDockerLearning，在 Visual Studio 對專案點選右鍵，加入 > Docker 支援
![](VS支援/01.png)
![](VS支援/02.png)

- 確認後可產生 Dockerfile 檔案
![](VS支援/03.png)

### 修改檔案或配置

- Visual Studio 啟用 Docker 支援除了產生 Dockerfile 外，還會修改其他相關檔案或配置以確保容器環境正常運作
  - launchSettings.json
  - 專案檔
  - 引入 NuGet 套件
  - docker-compose（如啟用 Compose）
  - .dockerignore

### 使用 Dockerfile

- Visual Studio 產生的 Dockerfile 也可用於自己建立 image
- 開啟終端機，或使用 Visual Studio 的 工具 > 命令列 > 開發人員命令提示字元
- 注意 COPY 指令與 context 的相對路徑正確，如 Visual Studio 產生的 Dockerfile 為：
```
COPY ["WebApplicationDockerLearning/WebApplicationDockerLearning.csproj", "WebApplicationDockerLearning/"]
```

- 如果 Dockerfile 檔案位置不在當前資料夾，可透過 `-f` 參數指定檔案路徑
```
docker build -f WebApplicationDockerLearning/Dockerfile -t ex1-image .
```
![](VS支援/04.png)

## 資料的保存

### container 可替換的設計理念

- 在一個運行中的 container 內部修改檔案時，這些變更會被保存在 container 的可寫層(writable layer)中。但是實際上我們常常將 container 視為無狀態(stateless)的執行環境，而不是一個持久化儲存的方案，基於以下觀點：
  - container 的設計理念為可替換的，隨時可以銷毀並重新創建，而不是進行修改
  - 當需要更新應用程式時，不是修改現有 container，而是構建新 container 並替換
  - 水平擴展需求，在負載增加時可以快速創建多個相同的 container，管理工具(如Kubernetes)也會自動管理 container 的生命週期
  - 效能考量，container 的可寫層不適合高頻率的寫入操作
