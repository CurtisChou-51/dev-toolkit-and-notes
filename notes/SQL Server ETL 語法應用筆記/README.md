# SQL Server ETL 語法應用筆記

## 概述
- 背景：紀錄一些 SQL Server 中常見的 ETL（擷取、轉換、載入）語法與使用情境

## 範例1

- 範例檔：[example.sql](example.sql)

1. `select into temp table`

- 使用 `select ... into` 快速建立臨時表，可以不需預先定義結構

```sql
select Col_1, Col_2
  into #XXXTmpTable
  from XXXTable
 where Col_1 = 'xxx';
```

2. `insert into select`

- 將 `select` 結果批次新增到目標資料表

```sql
insert into XXXTable
  (Col_1, Col_2)
select Col_1, Col_2
  from XXXTable_2
 where Col_1 = 'xxx';
```

3. `drop temp table`

- 安全刪除臨時表，`if exists` 確保不會因表不存在而報錯

```sql
drop table if exists #XXXTmpTable;
```

4. `merge into output`

- 使用 `merge into` 用於批次資料更新
- `output` 可以捕捉被 `insert` 或 `update` 的資料，而 `into #ChangeResult_XXXTable` 需要預先定義結構

```sql
merge into XXXTable T
using
(
    select Col_1, Col_2, Col_3
      from #XXXMergeSrc
) i on i.Col_1 = T.Col_1
when matched then
    update set T.Col_2 = i.Col_2,
               T.Col_3 = i.Col_3
when not matched then
    insert (Col_1, Col_2, Col_3)
    values (i.Col_1, i.Col_2, i.Col_3)
output inserted.Id, inserted.Col_1, inserted.Col_2, inserted.Col_3 into #ChangeResult_XXXTable;
```

5. `values clause`

- `values` 語法可寫多筆資料，不需要寫為多個 `insert`，但是也會有筆數限制

```sql
insert into XXXTable
  (Col_1, Col_2) 
values
  ('1', 'A'),
  ('2', 'B'),
  ('3', 'C');
```

6. `delete from join`

- 可以找出符合條件的資料並刪除，語法也可以加入 `join` 來過濾符合條件的資料

```sql
delete XXXTable
  from XXXTable n
  join XXXTable_2 n2 on n.Id = n2.Id
 where n.Col_1 = 'xxx' and n2.Col_A = 'aaa';
```