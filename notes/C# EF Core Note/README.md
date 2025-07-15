# C# EF Core Note

- EF Core 筆記
- Entity Framework Core (EF Core) 是 Microsoft 開發的 ORM 框架，用於 .NET 應用程式與資料庫之間的互動，有 Code First 和 Database First 兩種開發模式

## 欄位映射

- EF Core 欄位映射需明確 mapping，不會像 Dapper 一樣自動將沒有對應的欄位設為 C# 屬性的預設，如果使用 `SqlQueryRaw` 寫原生 sql 更要注意映射的實體類型

- 如果指定實體的欄位類型與查詢語法所得的資料類型不符會產生錯誤，例如實體的欄位為 `public string A { get; set; }`，如果查詢所取得的 A 欄位為 null 則會產生 `SqlNullValueException`
![](01.png)

- 如果指定實體的欄位不存在於查詢語法中也會產生錯誤，例如實體的欄位為 `public string? XXX { get; set; }`，查詢語法也需要有對應的結果
![](02.png)

## 變更追蹤機制與 SaveChanges

- EF Core 具有變更追蹤機制，從資料庫載入實體後，EF Core 會自動追蹤這些實體的狀態變化，例如 `dbContext.XXXEntities.Add(entity)`、`dbContext.XXXEntities.Update(entity)`、`dbContext.XXXEntities.Remove(entity)` 等方法
- 這些變更會被暫存在記憶體中，直到呼叫 `SaveChanges()` 方法時，EF Core 才會生成 SQL 語句真正執行對資料庫的操作，此時 EF Core 會自動建立一個 transaction 包住當前的所有「有追蹤的資料庫異動」

## Migrations
