# C# LINQ join

- 以往都偏向使用 lambda 來進行 LINQ 操作，但最近發現 linq query syntax 在 join 使用上更為簡潔

## cross join
```csharp
(from u in users
 from o in orders
 select new { u.Name, o.Product })
```

## inner join
```csharp
(from u in users
 join o in orders on u.Id equals o.UserId
 select new { u.Name, o.Product })
```

## group join
```csharp
(from u in users
 join o in orders on u.UserId equals o.UserId into userOrders
 select new { u.Name, userOrders = userOrders.ToList() })
```

## left join
```csharp
(from u in users
 join o in orders on u.UserId equals o.UserId into userOrders
 from o in userOrders.DefaultIfEmpty()
 select new { u.Name, Product = o?.Product ?? null })
```
