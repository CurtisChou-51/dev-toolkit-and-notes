select case when 'test' = 'test   ' then 'true' else 'false' end;
select case when 'test   ' = 'test' then 'true' else 'false' end;