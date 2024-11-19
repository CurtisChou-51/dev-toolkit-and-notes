drop table if exists #Sales;
CREATE TABLE #Sales (
    Subsidiaries varchar(15),
    Department varchar(15),
    ProductId varchar(15),
    Quantity decimal(10,0)
);

-- 測試資料
insert into #Sales (Subsidiaries, Department, ProductId, Quantity) values
    ('SubA', 'DeptA', '1', 1000),
    ('SubA', 'DeptB', '2', 1500),
    ('SubA', 'DeptC', '3', 800),
    ('SubB', 'DeptA', '1', 500),
    ('SubB', 'DeptB', '5', 600),
    ('SubB', 'DeptC', '6', 700),
    ('SubA', 'DeptA', '1', 1200),
    ('SubA', 'DeptA', '2', 900),
    ('SubB', 'DeptB', '5', 800),
    ('SubA', 'DeptC', '6', 800),
    ('SubB', 'DeptC', '6', 500),
    ('SubA', 'DeptB', '5', 700),
    ('SubA', 'DeptB', '2', 1000),
    ('SubB', 'DeptA', '6', 1600),
    ('SubB', 'DeptB', '2', 600),
    ('SubB', 'DeptB', '3', 800),
    ('SubB', 'DeptC', '5', 1200)
;

select
    case grouping_id(Subsidiaries, Department, ProductId)
      when 1 then '***小計*** 部門'
      when 3 then '***小計*** 子公司'
      when 7 then '***總計***'
      else ''
    end as Description,
    isNull(Subsidiaries, '') as Subsidiaries,
    isNull(Department, '') as Department,
    isNull(ProductId, '') as ProductId,
    sum(Quantity) as Quantity
from #Sales
group by grouping sets (
    (Subsidiaries, Department, ProductId),
    (Subsidiaries, Department),
    (Subsidiaries),
    ()
);

/* 小計-總計 的使用情境可以寫為 ROLLUP 更簡潔 */
/*
select
    case grouping_id(Subsidiaries, Department, ProductId)
      when 1 then '***小計*** 部門'
      when 3 then '***小計*** 子公司'
      when 7 then '***總計***'
      else ''
    end as Description,
    isNull(Subsidiaries, '') as Subsidiaries,
    isNull(Department, '') as Department,
    isNull(ProductId, '') as ProductId,
    sum(Quantity) as Quantity
from #Sales
group by rollup (Subsidiaries, Department, ProductId);
*/

drop table if exists #Sales;