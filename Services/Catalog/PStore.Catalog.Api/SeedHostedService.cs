using Microsoft.EntityFrameworkCore;
using PStore.Catalog.Domain;
using PStore.Catalog.Infrastructure;

namespace PStore.Catalog.Api {
    public class SeedHostedService : IHostedService {
        private readonly IServiceProvider _sp;
        public SeedHostedService(IServiceProvider sp) => _sp = sp;

        public async Task StartAsync(CancellationToken ct) {
            using var scope = _sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

            if (!await db.Products.AnyAsync(ct)) {
                db.Products.AddRange(
                    new Product { Id = Guid.NewGuid(), Sku = "SKU-001", Name = "Camisa PStore", Price = 99.90m, Stock = 50 },
                    new Product { Id = Guid.NewGuid(), Sku = "SKU-002", Name = "Tênis Z", Price = 299.00m, Stock = 20 }
                );
                await db.SaveChangesAsync(ct);
            }
        }

        public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
    }
}
