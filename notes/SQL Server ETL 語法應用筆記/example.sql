
/* select into temp table */
select Col_1, Col_2
  into #XXXTmpTable
  from XXXTable
 where Col_1 = 'xxx';


/* insert into select */
insert into XXXTable
  (Col_1, Col_2)
select Col_1, Col_2
  from XXXTable_2
 where Col_1 = 'xxx';


/* drop temp table */
drop table if exists #XXXTmpTable;


/* merge into output */
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


/* values clause */
insert into XXXTable
  (Col_1, Col_2) 
values
  ('1', 'A'),
  ('2', 'B'),
  ('3', 'C');


/* delete from join */
delete XXXTable
  from XXXTable n
  join XXXTable_2 n2 on n.Id = n2.Id
 where n.Col_1 = 'xxx' and n2.Col_A = 'aaa';
