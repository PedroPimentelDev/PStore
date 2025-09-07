using Microsoft.EntityFrameworkCore;
using PStore.Catalog.Domain;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace PStore.Catalog.Infrastructure;

public sealed class CatalogDbContext : DbContext, ICatalogDbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder b) {
        b.Entity<Product>(e => {
            e.ToTable("Products");
            e.HasKey(x => x.Id);
            e.Property(x => x.Sku).IsRequired().HasMaxLength(50);
            e.HasIndex(x => x.Sku).IsUnique();
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.Price).HasColumnType("decimal(18,2)");
        });
    }

    public void Add(Product product = default) =>
        base.Add(product);
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        base.SaveChangesAsync();
}
