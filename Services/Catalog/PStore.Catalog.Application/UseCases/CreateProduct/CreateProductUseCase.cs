using Microsoft.AspNetCore.Mvc;
using PStore.Catalog.Domain;
using PStore.Catalog.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PStore.Catalog.Application.UseCases.CreateProduct
{
    public class CreateProductUseCase : ICreateProductUseCase
    {
        private readonly ICatalogDbContext _catalogDbContext;

        public CreateProductUseCase(ICatalogDbContext catalogDbContext)
        {
            _catalogDbContext = catalogDbContext;
        }

        public async Task<Product> ExecuteAsync(CreateProductInputModel inputModel)
        {
            var product = new Product 
            { 
                Id = Guid.NewGuid(), 
                Sku = inputModel.Sku, 
                Name = inputModel.Name, 
                Price = inputModel.Price, 
                Stock = inputModel.Stock 
            };

            _catalogDbContext.Add(product);
            await _catalogDbContext.SaveChangesAsync();

            return product;
        }
    }
}
