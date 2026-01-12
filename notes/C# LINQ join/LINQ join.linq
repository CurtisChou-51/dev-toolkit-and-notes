<Query Kind="Program" />

void Main()
{
    var users = new[]
    {
        new { UserId = 1, Name = "Alice" },
        new { UserId = 2, Name = "Bob" },
        new { UserId = 3, Name = "Charlie" }
    };

    var orders = new[]
    {
        new { OrderId = 101, UserId = 1, Product = "Book" },
        new { OrderId = 102, UserId = 1, Product = "Pen" },
        new { OrderId = 103, UserId = 2, Product = "Laptop" }
    };

    (from u in users
     from o in orders
     select new { u.Name, o.Product }).Dump("cross join");

    (from u in users
     join o in orders on u.UserId equals o.UserId
     select new { u.Name, o.Product}).Dump("join");

    (from u in users
     join o in orders on u.UserId equals o.UserId into userOrders
     select new { u.Name, userOrders = userOrders.ToList() }).Dump("group join");

    (from u in users
     join o in orders on u.UserId equals o.UserId into userOrders
     from o in userOrders.DefaultIfEmpty()
     select new { u.Name, Product = o?.Product ?? null }).Dump("left join");
                
}

