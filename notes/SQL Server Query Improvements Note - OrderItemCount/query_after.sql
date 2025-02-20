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
