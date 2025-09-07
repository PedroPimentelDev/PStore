using Microsoft.EntityFrameworkCore;
using PStore.Orders.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PStore.Orders.Infrastructure
{
    public interface IOrdersDbContext
    {
        DbSet<Order> Orders { get; }
        DbSet<OrderItem> OrderItems { get; }
        DbSet<OutboxMessage> OutboxMessages { get; }
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
