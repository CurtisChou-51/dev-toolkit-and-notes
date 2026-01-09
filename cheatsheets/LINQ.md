# LINQ

## Left Join
```csharp
(from u in users
 join o in orders on u.UserId equals o.UserId into userOrders
 from o in userOrders.DefaultIfEmpty()
 select new { u.Name, Product = o?.Product ?? null })
```

## Pairwise
```csharp
users.Zip(users.Skip(1), (u1, u2) => new { u1, u2 })
     .Select(p =>$"{p.u1.Name} - {p.u2.Name}");
```