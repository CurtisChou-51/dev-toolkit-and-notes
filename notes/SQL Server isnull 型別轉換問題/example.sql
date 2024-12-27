
select cast(null as varchar(100)) as old_name, 
       cast(null as varchar(100)) as new_name 
  into #tmpNameMap 
 where 1 = 0;

with XXXTable as (
  select N'測試' as name
)
select t.name,
       m.new_name,
       isnull(m.new_name, t.name) as display_name
  from XXXTable t
  left join #tmpNameMap m on t.name = m.old_name

drop table if exists #tmpNameMap;