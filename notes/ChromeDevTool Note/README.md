# ChromeDevTool Note

- ChromeDevTool 使用筆記

## 1.保留紀錄

- 可用於快速檢查網站 OAuth 或是登入後重新定向請求狀況
- 勾選 **保留紀錄 (Preserve Log)**，使用 reurl 網址測試後可看到重新定向前的紀錄被保留
![](01.png)

## 2.XHR/Fetch 中斷點
- 可用於攔截所有或特定的 API 請求
- 於 **原始碼** 頁籤右方 **XHR/Fetch 中斷點** 區塊勾選
![](02.png)

## 3.複製 HTML 元素路徑結構
- 可用於爬蟲、資料採集、自動化測試等情境，協助快速取得指定 HTML 元素路徑結構
- 在偵錯工具的元素分頁對 HTML 元素點選右鍵複製，右方子選單可選取類型：
  - XPATH 如：`//*[@id="readme"]/div[2]/article/div[3]`
  - Selector 如：`#readme > div.Box-sc-g0xbh4-0.QkQOb.js-snippet-clipboard-copy-unpositioned.undefined > article > div:nth-child(5)`  
![](03.png)

## 4.尋找 HTML 元素
- 在偵錯工具的元素分頁 `(ctrl + f)` 開啟搜尋框，可使用 XPATH 或是 Selector 搜尋
![](04.png)

## 5.上傳檔案 redirect 偵錯
- 上傳檔案的請求如果回應 redirect 重新定向，偵錯工具的 Network 面板可能無法顯示原始請求的 payload 資訊。如果是開發階段可暫時移除重新定向邏輯以方便偵錯

- 沒有選取檔案的請求可以顯示 payload 資訊
![](05.png)

- 選取了檔案的請求未顯示 payload 資訊
![](06.png)

## 6.覆寫內容

- 可用於修改網頁內容、測試不同的樣式或功能

- 原始碼 > 指定檔案 > 覆寫內容  
![](override/01.png)  

- 選取資料夾  
![](override/02.png)  

- 在檔案系統中可以看到 Chrome 已經在選取的資料夾建立檔案，在覆寫內容的檔案中修改後儲存
![](override/03.png)  

- 覆寫內容已套用  
![](override/04.png)  

## 7.節流
- 可用於模擬不同網路速度，在本地端也可以測試網站在慢速網路下的行為

- 於 **網路** 頁籤右方 **節流** 區塊選取
![](07.png)  

## 8.查看作用域與全域變數
- 於 console 輸入 `debugger` 可進入中斷點，之後可在 **原始碼** 頁籤右方 **作用域** 區塊查看目前作用域與全域變數
![](08.png)  