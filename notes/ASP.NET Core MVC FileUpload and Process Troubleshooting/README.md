# ASP.NET Core MVC FileUpload and Process Troubleshooting

- 問題背景：ASP.NET Core MVC 網站會將上傳的檔案暫存，之後使用 `FFMpeg` 嘗試讀取剛上傳的檔案進行剪輯轉檔，發生轉檔錯誤。但是程式停止後手動對相同的檔案轉檔卻成功。

![](01.png)

## 示意程式

```csharp
public IActionResult SendFile(SendFileViewModel vm)
{
    // ...
    using var stream = new FileStream(tmpSave, FileMode.Create);
    uploadFile.CopyTo(stream);
    TrimAudioByFFMpeg(tmpSave, tmpConvert, startTime, endTime);
    // ...
}
```

## 處理過程

- 在 `TrimAudioByFFMpeg` 執行前中斷程式執行觀察暫存檔，發現此時檔案大小確實與原檔案不一致，直到`SendFile` 方法結束為止。
- `using` 會在整個 scope 結束後呼叫 `Dispose()`，而此時使用 `using ... ;` 語法糖的 scope 為 `SendFile` 方法。
- 可知在 `TrimAudioByFFMpeg` 執行時尚未將 stream `Dispose()`，檔案仍被占用或資料未完整寫入，導致判定為格式錯誤而失敗。

- `FFMpeg` 處理時檔案
![](02.png)

- `SendFile` 方法結束時檔案
![](03.png)

## 調整程式

- 明定 `FileStream` 的 scope，讓檔案寫入完成後立刻呼叫 `Dispose()`。

```csharp
using (var stream = new FileStream(tmpSave, FileMode.Create))
{
    uploadFile.CopyTo(stream);
}
TrimAudioByFFMpeg(tmpSave, tmpConvert, startTime, endTime);
```

> [!NOTE]
> `using ... ;` 語法糖簡化了寫法，但也必須注意 scope 範圍
> 如果資源會跨多段程式使用，或需要明確控制釋放時機
> 使用傳統的 `using (...) {}` 語法是個好選擇
