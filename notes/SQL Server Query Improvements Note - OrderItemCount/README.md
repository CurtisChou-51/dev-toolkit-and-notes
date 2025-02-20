# SQL Server Query Improvements Note - OrderItemCount

- 對於 `OrderItemCount` 查詢語法效能改善筆記

## 重構描述

- 此查詢用途為統計不同項目(ItemA-ItemF)的數量

- 語法調整：
  - 查詢主體 join CodeMain，避免每個計數項目都要重新查詢 CodeMain
  - 子查詢中的計數使用 `CASE WHEN` 語法只需掃描一次資料，避免每次子查詢都需要再次檢索
  - 在資料庫語法有支援時可以使用 `PIVOT` 語法

## Before

```sql
with temp as (
    Select CodeId
      from vOrderData v
     where v.OStatus <> 99 and v.StDate > @StDate
)
Select 
    (Select count(*) from temp where CodeId in (Select CodeId from CodeMain where ParentCode = 'OrderItem' and CodeName = 'ItemA')) as ACount,
	(Select count(*) from temp where CodeId in (Select CodeId from CodeMain where ParentCode = 'OrderItem' and CodeName = 'ItemB')) as BCount,
	(Select count(*) from temp where CodeId in (Select CodeId from CodeMain where ParentCode = 'OrderItem' and CodeName = 'ItemC')) as CCount,
	(Select count(*) from temp where CodeId in (Select CodeId from CodeMain where ParentCode = 'OrderItem' and CodeName = 'ItemD')) as DCount,
	(Select count(*) from temp where CodeId in (Select CodeId from CodeMain where ParentCode = 'OrderItem' and CodeName = 'ItemE')) as ECount,
	(Select count(*) from temp where CodeId in (Select CodeId from CodeMain where ParentCode = 'OrderItem' and CodeName = 'ItemF')) as FCount;

```

## After

```sql
select count(case when CodeName = 'ItemA' then 1 end) as ACount,
       count(case when CodeName = 'ItemB' then 1 end) as BCount,
       count(case when CodeName = 'ItemC' then 1 end) as CCount,
       count(case when CodeName = 'ItemD' then 1 end) as DCount,
       count(case when CodeName = 'ItemE' then 1 end) as ECount,
       count(case when CodeName = 'ItemF' then 1 end) as FCount
from (
    Select v.Case_Id, t.CodeName
      from vOrderData v
      join CodeMain t on ParentCode = 'OrderItem' and v.CodeId = t.CodeId
     where v.OStatus <> 99 and v.StDate > @StDate
) t;

/* 或是使用 pivot */
select [ItemA] as ACount, 
       [ItemB] as BCount, 
       [ItemC] as CCount, 
       [ItemD] as DCount, 
       [ItemE] as ECount, 
       [ItemF] as FCount
from (
    Select v.Case_Id, t.CodeName
      from vOrderData v
      join CodeMain t on ParentCode = 'OrderItem' and v.CodeId = t.CodeId
     where v.OStatus <> 99 and v.StDate > @StDate
) a
pivot (
    count(Case_Id) for CodeName in ([ItemA], [ItemB], [ItemC], [ItemD], [ItemE], [ItemF])
) p;
```