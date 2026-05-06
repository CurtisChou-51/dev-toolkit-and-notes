# SQL Greatest-Per-Group

- 除了找出 max() 的值之外，還要找出其他欄位的值

## 用法

### 1. 相關子查詢
- 容易理解但效能差，對於分組的處理是靠關聯條件，通常用於快速撰寫臨時腳本
```sql
select *
from employees e
where salary = (
    select max(salary)
    from employees
    where deptid = e.deptid
)
```

### 2. Self-Join
- 經典 join 語法，max() 的語意明確，語法相容性較好，但需要掃描兩次
```sql
select e.* 
from employees e
inner join (
    select deptid, max(salary) as max_val
    from employees
    group by deptid
) m on e.deptid = m.deptid and e.salary = m.max_val
```

### 3. Window Functions
- 效能較好，較舊的資料庫可能不支援，看上去不像 max() 的語意
```sql
select *
from (
    select *, 
           rank() over (partition by deptid order by salary desc) as rnk
    from employees
) ranked
where rnk = 1
```

### 4. Cross Apply
- 需要特定資料庫的支援，當查詢需要參考外部資料表時可使用
```sql
select e.*
from (select distinct deptid from employees) d
cross apply (
    select top 1 *
    from employees e
    where e.deptid = d.deptid
    order by salary desc
) e
```

> [!NOTE]
> Cross Apply：如果子查詢沒有回傳結果（例如該部門沒員工），則該部門資料不會出現在最終結果中（類似 inner join）
> Outer Apply：如果子查詢沒有回傳結果，外層資料仍會保留欄位顯示為 null（類似 LEFT join）