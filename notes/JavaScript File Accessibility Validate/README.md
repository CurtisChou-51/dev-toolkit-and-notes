# JavaScript File Accessibility Validate

- JavaScript 確認檔案的可存取性

## 問題描述

在網頁使用 xhr 上傳檔案時，可能會遇到以下情境導致上傳失敗：
1. 使用者選擇檔案後，將該檔案移動到其他資料夾
2. 使用者選擇檔案後，將該檔案刪除
3. 使用者選擇檔案後，將該檔案改名
4. 因為其他原因檔案無法讀取

## 不同瀏覽器的行為差異

- **Chrome**：
  - 顯示 `ERR_FILE_NOT_FOUND` 錯誤，xhr 請求沒到達伺服器端
![](01.png)

- **Firefox**：
  - 傳輸量顯示 `NS_ERROR_NET_INTERRUPT` 錯誤，xhr 請求到達伺服器端，但伺服器端給的回應沒有顯示
![](02.png)

## 實作檔案可存取性檢查

- 使用 [JavaScript 函數](validate.js) 來預先驗證檔案的可存取性

1. `validateFormDataFileAccessible(formData)`：
   - 接收 FormData 物件作為參數
   - 遍歷所有欄位，找出檔案類型的欄位進行驗證
   - 回傳 Promise 物件，包含驗證結果

2. `validateFileAccessible(file)`：
   - 接收 File 物件作為參數
   - 使用 FileReader 嘗試讀取檔案的第一個 byte
   - 如果讀取成功，表示檔案可以存取
   - 如果讀取失敗，表示檔案可能已被移動或刪除
