
/* 檢查資料表是否存在，不存在則建立 */
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

/* 檢查欄位是否存在，不存在則新增 */
if not exists (select 1 
                 from sys.columns c 
                where c.object_id = object_id('XXXTable') 
                  and c.name = 'XXXColumn')
begin
    alter table XXXTable add XXXColumn Nvarchar(30) not null default '0';
end;

/* 修改欄位 */
alter table XXXTable alter column XXXColumn Nvarchar(30) null;


/* 動態 SQL drop table */
declare @sql nvarchar(max) = 'drop table if exists ' + @TableName;
exec sp_executesql @sql;
