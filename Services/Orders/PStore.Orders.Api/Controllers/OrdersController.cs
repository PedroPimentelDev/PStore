using Microsoft.AspNetCore.Mvc;
using PStore.Orders.Application.UseCases.CreateOrder;
using PStore.Orders.Application.UseCases.GetOrder;
using PStore.Orders.Infrastructure;

namespace PStore.Orders.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrdersDbContext _db;
        private readonly IHttpClientFactory _httpFactory;

        public OrdersController(OrdersDbContext db, IHttpClientFactory httpFactory)
        {
            _db = db;
            _httpFactory = httpFactory;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderInputModel inputModel, [FromServices] ICreateOrderUseCase useCase)
        {
            var (isSuccess, result) = await useCase.ExecuteAsync(inputModel);

            if (!isSuccess)
                return result!;

            return result!;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOrder(Guid id, [FromServices] IGetOrderUseCase useCase)
        =>
             await useCase.ExecuteAsync(id);
    }
}

