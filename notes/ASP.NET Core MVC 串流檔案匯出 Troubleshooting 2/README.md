# ASP.NET Core MVC 串流檔案匯出 Troubleshooting 2

- 這個 .Net Core 程式將 DataSet 轉為 xml 並使用 GZip 壓縮之後下載，將原先轉出 `byte[]` 的方式 (Query0) 改為直接輸出 `MemoryStream` (由 Query1 修改至 Query5)，過程中遇到不少問題做個紀錄

```csharp
// 原先載入記憶體的下載方式
public IActionResult Query0()
{
    using DataSet ds = GetData();
    using MemoryStream ms = new();
    using GZipStream gzs = new GZipStream(ms, CompressionMode.Compress);
    using StreamWriter sw = new StreamWriter(gzs);
    ds.WriteXml(sw, XmlWriteMode.WriteSchema);
    sw.Flush();

    Response.Headers.Add("Content-Encoding", "gzip");
    return File(ms.ToArray(), "application/xml");
}

public IActionResult Query1()
{
    using DataSet ds = GetData();
    using MemoryStream ms = new();
    using GZipStream gzs = new GZipStream(ms, CompressionMode.Compress);
    using StreamWriter sw = new StreamWriter(gzs);
    ds.WriteXml(sw, XmlWriteMode.WriteSchema);
    sw.Flush();

    Response.Headers.Add("Content-Encoding", "gzip");
    return File(ms, "application/xml");
}

public IActionResult Query2()
{
    using DataSet ds = GetData();
    using MemoryStream ms = new();
    using GZipStream gzs = new GZipStream(ms, CompressionMode.Compress, leaveOpen: true);
    using StreamWriter sw = new StreamWriter(gzs);
    ds.WriteXml(sw, XmlWriteMode.WriteSchema);
    sw.Flush();

    Response.Headers.Add("Content-Encoding", "gzip");
    return File(ms, "application/xml");
}


public IActionResult Query3()
{
    using DataSet ds = GetData();
    MemoryStream ms = new();
    using GZipStream gzs = new GZipStream(ms, CompressionMode.Compress, leaveOpen: true);
    using StreamWriter sw = new StreamWriter(gzs);
    ds.WriteXml(sw, XmlWriteMode.WriteSchema);
    sw.Flush();

    Response.Headers.Add("Content-Encoding", "gzip");
    return File(ms, "application/xml");
}

public IActionResult Query4()
{
    using DataSet ds = GetData();
    MemoryStream ms = new();
    using GZipStream gzs = new GZipStream(ms, CompressionMode.Compress, leaveOpen: true);
    using StreamWriter sw = new StreamWriter(gzs);
    ds.WriteXml(sw, XmlWriteMode.WriteSchema);
    sw.Flush();

    ms.Position = 0;
    Response.Headers.Add("Content-Encoding", "gzip");
    return File(ms, "application/xml");
}

public IActionResult Query5()
{
    using DataSet ds = GetData();
    MemoryStream ms = new();
    using (GZipStream gzs = new GZipStream(ms, CompressionMode.Compress, leaveOpen: true))
    {
        using StreamWriter sw = new StreamWriter(gzs);
        ds.WriteXml(sw, XmlWriteMode.WriteSchema);
        sw.Flush();
    }

    ms.Position = 0;
    Response.Headers.Add("Content-Encoding", "gzip");
    return File(ms, "application/xml");
}
```

## Query1
Status Code：500  
ObjectDisposedException: Cannot access a closed Stream.

### Memo
判斷應該是串流被關閉問題，`GZipStream` 加上 `leaveOpen: true`

---

## Query2
Status Code：500  
ObjectDisposedException: Cannot access a closed Stream.

### Memo
還是一樣串流被關閉問題，這次把 `MemoryStream` 的 using 也移除 (using 移除後 `leaveOpen: true` 也要留著否則還是一樣串流被關閉)

---

## Query3
Status Code：500  
content-length：0  
瀏覽器顯示：這個網頁無法正常運作 (HTTP ERROR 500)

### Memo
串流關閉問題修好了，這個看起來是沒有輸出內容，加上 `ms.Position = 0` 重置串流位置

---

## Query4
Status Code：200  
content-length：645  
瀏覽器 Network 顯示：(failed) net::ERR_CONTENT_DECODING_FAILED

### Memo
看起來可能是檔案損毀，也許是 `GZipStream` 還有內容就修改了 `MemoryStream` 所造成，這次將 using 區塊縮小

---

## Query5
可正常下載

### Memo
但修完 Query5 之後才發現原先的 Query0 也並非完全正確，Query0 在 `GZipStream` 關閉之前就取了 `MemoryStream`，可能會導致某些況狀下會缺失部分內容 (就像 Query4 的狀況一樣)

### 調整總結
- 使用 `MemoryStream` 方案需要注意
  - 不用手動 Dispose (交給框架即可)、檢查串流是否會在其他地方被關閉
  - 包裝層 (如 `GZipStream`、`StreamWriter`) 需要先處理完成，避免內容錯誤
  - 串流位置需要重置 (如 `Position = 0`)，避免請求沒有響應內容

## Response.Body
- 特殊狀況下如架構允許也可再改為直接對 `Response.Body` 輸出，可避開 LOH 與 GC 壓力。但過程不能再改狀態碼，若中途發生例外用戶端拿到的是一個狀態碼 200 但內容不完整的回應。
```csharp
public void Query6()
{
    using DataSet ds = GetData();
    Response.ContentType = "application/xml";
    Response.Headers.Add("Content-Encoding", "gzip");

    using (GZipStream gzs = new GZipStream(Response.Body, CompressionMode.Compress, leaveOpen: true))
    {
        using StreamWriter sw = new StreamWriter(gzs);
        ds.WriteXml(sw, XmlWriteMode.WriteSchema);
        sw.Flush();
    }
}
```