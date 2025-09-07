using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PStore.Catalog.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PStore.Catalog.Application.UseCases.GetAllProducts
{
    public class GetAllProductsUseCase : IGetAllProductsUseCase
    {
        private readonly ICatalogDbContext _catalogDbContext;

        public GetAllProductsUseCase(ICatalogDbContext catalogDbContext) 
        { 
            _catalogDbContext = catalogDbContext;
        }

        public async Task<IActionResult> ExecuteAsync()
        {
            var products = await _catalogDbContext.Products.AsNoTracking().OrderBy(p => p.Name).ToListAsync();

            return products?.Any() == true ? new NotFoundResult() : new OkObjectResult(products);
        }
    }
}
