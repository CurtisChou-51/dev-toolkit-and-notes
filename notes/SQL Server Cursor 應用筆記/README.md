# SQL Server Cursor 應用筆記

- 背景：因特定情境需要使用，需要對一個範圍內的月份逐一執行 SQL 語法

## 範例

- 範例檔：[example.sql](example.sql)

## 語法

- 使用 cte 產生所有月份

```sql
declare @StartDate date = '2024-05-01';
declare @EndDate date = '2024-06-01';
declare @monthChar varchar(6);

;with DateSequence AS (
    select @StartDate as [Date]
     union all
    select dateadd(month, 1, [Date])
      from DateSequence
     where dateadd(month, 1, [Date]) <= @EndDate
),
months as (
    select format([Date], 'yyyyMM') as monthChar from DateSequence
)
select * into #tmpMonths from months;
```

- 使用 cursor 逐一執行 SQL 語法

```sql
declare table_cursor cursor for
  select monthChar from #tmpMonths;
open table_cursor;

fetch next from table_cursor into @monthChar;
while @@FETCH_STATUS = 0
begin
    set @sql = 'drop table if exists XXXTable_' + @monthChar;
    exec sp_executesql @sql;
    exec [dbo].[sp_ETL_XXXTable] @monthChar;
    fetch next from table_cursor into @monthChar;
    print 'complete: ' + @monthChar;
end;

close table_cursor;
deallocate table_cursor;
drop table if exists #tmpMonths;
```
