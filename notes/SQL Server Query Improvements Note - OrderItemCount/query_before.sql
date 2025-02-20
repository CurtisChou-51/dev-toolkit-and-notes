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
