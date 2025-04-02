select Id, (
    select top 1 DocDate from (
        select case 
                   when DocType = 1 then min(DocDate) 
                   else max(DocDate) 
               end as DocDate,
               DocType
          from #tmpDocLog tt
         where Id = m.Id
         group by DocType
    ) as x
    order by DocType desc
) as DocDate
from #tmpDocMain m;