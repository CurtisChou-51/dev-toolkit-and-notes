using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BlazorAppOutbox
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Outbox> Outboxes { get; set; }
    }

    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class Outbox
    {
        [Key]
        public int Id { get; set; }
        public string? EventType { get; set; }
        public string? Payload { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Status { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}