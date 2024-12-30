select Id_A, Col_A
  into #TableA
  from ( values 
      (1, 'a1'), (2, 'a2'), (3, 'a3')
  ) t (Id_A, Col_A);

select Id_B, Col_B
  into #TableB
  from ( values 
      (1, 'b1'), (5, 'b5'), (6, 'b6')
  ) t (Id_B, Col_B);

/* 以下語法可執行 */
select * from #TableA
 where Id_A in (select Id_A from #TableB where Col_B = 'b1');

/* 語法錯誤 */
-- select Id_A from #TableB where Col_B = 1;

drop table if exists #TableA;
drop table if exists #TableB;