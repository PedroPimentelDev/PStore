using Microsoft.AspNetCore.Mvc;
using PStore.Catalog.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PStore.Catalog.Application.UseCases.UpdateProduct
{
    public class UpdateProductUseCase : IUpdateProductUseCase
    {
        private readonly ICatalogDbContext _catalogDbContext;

        public UpdateProductUseCase(ICatalogDbContext catalogDbContext)
        {
            _catalogDbContext = catalogDbContext;
        }


        public async Task<IActionResult> ExecuteAsync(UpdateProductInputModel inputModel)
        {
            var product = await _catalogDbContext.Products.FindAsync(inputModel.Id);
            if (product is null) return new NotFoundResult();

            product.Sku = inputModel.Sku;
            product.Name = inputModel.Name;
            product.Price = inputModel.Price;
            product.Stock = inputModel.Stock;

            await _catalogDbContext.SaveChangesAsync();

            return new OkResult();
        }
    }
}
