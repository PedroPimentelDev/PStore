using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PStore.Orders.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PStore.Orders.Application.UseCases.GetOrder
{
    public class GetOrderUseCase : IGetOrderUseCase
    {
        private readonly IOrdersDbContext _ordersDbContext;

        public GetOrderUseCase(IOrdersDbContext ordersDbContext)
        {
            _ordersDbContext = ordersDbContext;
        }

        public async Task<IActionResult> ExecuteAsync(Guid id)
        {
            var order = await _ordersDbContext.Orders.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id);

            return order is null ? new NotFoundResult() : new OkObjectResult(order);
        }
    }
}
