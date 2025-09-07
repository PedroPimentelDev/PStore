using PStore.Orders.Domain;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Threading;

namespace PStore.Orders.Infrastructure;

public sealed class OrdersDbContext : DbContext, IOrdersDbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> opt) : base(opt) { }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder b) {
        b.Entity<Order>(e => {
            e.ToTable("Orders");
            e.HasKey(x => x.Id);
            e.Property(x => x.Number).IsRequired().HasMaxLength(40);
            e.HasIndex(x => x.Number).IsUnique();
            e.Property(x => x.Total).HasColumnType("decimal(18,2)");
            e.HasMany(x => x.Items).WithOne().HasForeignKey(i => i.OrderId);
        });

        b.Entity<OrderItem>(e => {
            e.ToTable("OrderItems");
            e.HasKey(x => x.Id);
            e.Property(x => x.Sku).IsRequired().HasMaxLength(50);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
        });

        b.Entity<OutboxMessage>(e =>
        {
            e.ToTable("OutboxMessages");
            e.HasKey(o => o.Id);
        });
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        base.SaveChangesAsync(cancellationToken);
}
