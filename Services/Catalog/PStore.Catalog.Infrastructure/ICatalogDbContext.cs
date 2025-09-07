using Microsoft.EntityFrameworkCore;
using PStore.Catalog.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PStore.Catalog.Infrastructure
{
    public interface ICatalogDbContext
    {
        public DbSet<Product> Products { get; }
        void Add(Product product = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
