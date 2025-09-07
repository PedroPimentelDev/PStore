using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PStore.Catalog.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PStore.Catalog.Application.UseCases.GetProductById
{
    public class GetProductByIdUseCase : IGetProductByIdUseCase
    {
        private readonly ICatalogDbContext _catalogDbContext;

        public GetProductByIdUseCase(ICatalogDbContext catalogDbContext)
        {
            _catalogDbContext = catalogDbContext;
        }

        public async Task<IActionResult> ExecuteAsync(Guid id)
        {
            var product = await _catalogDbContext.Products.FirstOrDefaultAsync(p => p.Id == id);

            return product == null ? new NotFoundResult() : new OkObjectResult(product);
        }
    }
}
