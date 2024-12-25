# SQL Server DDL 語法應用筆記

- 背景：因特定情境需要使用，紀錄一些 SQL Server 的結構變更語法 (Data Definition Language) 應用範例

## 範例1

- 範例檔：[example.sql](example.sql)

### 1. 檢查資料表是否存在，不存在則建立

```sql
if not exists (select 1
                 from sysobjects
                where id = object_id('XXXTable')
                  and type = 'U')
begin
    create table XXXTable (
       Id1         bigint        not null,
       XXXColumn   Nvarchar(30)  null,
       constraint PK_XXXTable primary key (Id1)
    );
end;
```

### 2. 檢查欄位是否存在，不存在則新增

```sql
if not exists (select 1 
                 from sys.columns c 
                where c.object_id = object_id('XXXTable') 
                  and c.name = 'XXXColumn')
begin
    alter table XXXTable add XXXColumn Nvarchar(30) not null default '0';
end;
```

### 3. 修改欄位
```sql
alter table XXXTable alter column XXXColumn Nvarchar(30) null;
```

### 4. 動態 SQL drop table
```sql
declare @sql nvarchar(max) = 'drop table if exists ' + @TableName;
exec sp_executesql @sql;
```