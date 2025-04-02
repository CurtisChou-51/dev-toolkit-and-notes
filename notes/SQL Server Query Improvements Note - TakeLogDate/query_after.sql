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