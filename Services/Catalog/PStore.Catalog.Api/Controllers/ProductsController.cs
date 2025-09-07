using Microsoft.AspNetCore.Mvc;
using PStore.Catalog.Application.UseCases.CreateProduct;
using PStore.Catalog.Application.UseCases.GetAllProducts;
using PStore.Catalog.Application.UseCases.GetAllProductsBySku;
using PStore.Catalog.Application.UseCases.GetProductById;
using PStore.Catalog.Application.UseCases.UpdateProduct;

namespace PStore.Catalog.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromServices] IGetAllProductsUseCase useCase) =>
             await useCase.ExecuteAsync();

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductInputModel inputModel, [FromServices] ICreateProductUseCase useCase)
        {
            var product = await useCase.ExecuteAsync(inputModel);
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductInputModel inputModel, [FromServices] IUpdateProductUseCase useCase) =>
            await useCase.ExecuteAsync(inputModel);

        [HttpGet("by-sku{sku}")]
        public async Task<IActionResult> GetProductBySku(string sku, [FromServices] IGetAllProductsBySkuUseCase useCase)
        {
            var product = await useCase.ExecuteAsync(sku);
            return product is null ? NotFound() : Ok(product);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetProductById(Guid id, [FromServices] IGetProductByIdUseCase useCase) =>
            await useCase.ExecuteAsync(id);
    }
}