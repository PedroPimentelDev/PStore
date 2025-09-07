using Microsoft.AspNetCore.Mvc;
using PStore.Orders.Application.Dtos;
using PStore.Orders.Domain;
using PStore.Orders.Infrastructure;
using System.Net.Http.Json;
using System.Text.Json;

namespace PStore.Orders.Application.UseCases.CreateOrder
{
    public class CreateOrderUseCase : ICreateOrderUseCase
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IOrdersDbContext _ordersDbContext;

        public CreateOrderUseCase(IHttpClientFactory httpFactory, IOrdersDbContext ordersDbContext)
        {
            _httpFactory = httpFactory;
            _ordersDbContext = ordersDbContext;
        }

        public async Task<(bool IsSuccess, IActionResult? Result)> ExecuteAsync(CreateOrderInputModel inputModel)
        {
            if (inputModel.Items == null || inputModel.Items.Count == 0)
                return (false, new BadRequestObjectResult("Itens obrigatórios."));

            var client = _httpFactory.CreateClient("catalog");
            decimal total = 0m;

            var numberSuffix = Guid.NewGuid().ToString("N")[..6];
            var order = new Order
            {
                Id = Guid.NewGuid(),
                Number = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{numberSuffix}"
            };

            foreach (var it in inputModel.Items)
            {
                var prod = await client.GetFromJsonAsync<ProductDTO>($"/api/products/by-sku/{it.Sku}");
                if (prod == null)
                    return (false, new BadRequestObjectResult($"SKU '{it.Sku}' não encontrado no catálogo."));

                var item = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    Sku = prod.Sku,
                    Name = prod.Name,
                    UnitPrice = prod.Price,
                    Quantity = it.Quantity
                };

                order.Items.Add(item);
                total += item.UnitPrice * item.Quantity;
            }

            order.Total = total;

            var evt = new
            {
                OrderId = order.Id,
                order.Number,
                order.Total,
                order.CreatedAt,
                Items = order.Items.Select(i => new { i.Sku, i.Name, i.UnitPrice, i.Quantity }).ToList()
            };

            var outbox = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                OccurredOnUtc = DateTime.UtcNow,
                Type = "PedidoCriado.v1",
                Content = JsonSerializer.Serialize(evt)
            };

            _ordersDbContext.Orders.Add(order);
            _ordersDbContext.OutboxMessages.Add(outbox);

            await _ordersDbContext.SaveChangesAsync();

            var response = new CreateOrderViewModel(order.Id, order.Number, order.Total, order.Status.ToString());

            var createdResult = new CreatedResult($"/api/orders/{order.Id}", response);

            return (true, createdResult);
        }
    }
}
