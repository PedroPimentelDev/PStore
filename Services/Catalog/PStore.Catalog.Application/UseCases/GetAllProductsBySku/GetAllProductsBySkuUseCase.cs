using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PStore.Catalog.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PStore.Catalog.Application.UseCases.GetAllProductsBySku
{
    public class GetAllProductsBySkuUseCase : IGetAllProductsBySkuUseCase
    {
        private readonly ICatalogDbContext _catalogDbContext;

        public GetAllProductsBySkuUseCase(ICatalogDbContext catalogDbContext)
        {
            _catalogDbContext = catalogDbContext;
        }

        public async Task<IActionResult> ExecuteAsync(string sku)
        {
            var product = await _catalogDbContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Sku == sku);
            return product == null ? new NotFoundResult() : new OkObjectResult(product);
        }
    }
}
