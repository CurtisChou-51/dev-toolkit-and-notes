# SQL Server Query Improvements Note - TakeLogDate

- 對於 `TakeLogDate` 查詢語法效能改善筆記

## 重構描述

- 此查詢用途為查詢主表資料的特定一筆 LogDate，並依照 DocType 採用邏輯：
  - DocType = 1：min
  - DocType = 0：max
- 當同一個 Id 有不同 DocType 時，優先使用 DocType = 1 的日期


- 語法調整：
  - 原查詢的子查詢每筆主表記錄都會重複執行
  - 移除子查詢 + `top 1`，改用 `LEFT JOIN` 配合 `ROW_NUMBER`
  - 在資料庫語法有支援時可以使用 `PIVOT` 語法，配合 `coalesce` 實現優先性的選擇邏輯

- [模擬資料](test_data.sql)

## Before

```sql
select Id, (
    select top 1 DocDate from (
        select case 
                   when DocType = 1 then min(DocDate) 
                   else max(DocDate) 
               end as DocDate,
               DocType
          from #tmpDocLog tt
         where Id = m.Id
         group by DocType
    ) as x
    order by DocType desc
) as DocDate
from #tmpDocMain m;
```

## After

```sql
select m.Id, DocDate
  from #tmpDocMain m
  left join (
      select *, row_number() over(partition by Id order by DocType desc) as rn 
        from (
            select Id, DocType,
                   case 
                       when DocType = 1 then min(DocDate) 
                       else max(DocDate) 
                   end as DocDate
              from #tmpDocLog
             group by Id, DocType
        ) q
  ) s on m.Id = s.Id and rn = 1;

/* 或是使用 pivot + coalesce */
select m.Id, coalesce(s.[1], s.[0]) as DocDate
  from #tmpDocMain m
  left join (
      select * from (
          select Id, DocType,
                 case 
                     when DocType = 1 then min(DocDate) 
                     else max(DocDate) 
                 end as DocDate
            from #tmpDocLog tt
           group by Id, DocType
      ) s
      pivot (
          max(DocDate) for DocType in ([0], [1])
      ) p
  ) s on m.Id = s.Id;
```