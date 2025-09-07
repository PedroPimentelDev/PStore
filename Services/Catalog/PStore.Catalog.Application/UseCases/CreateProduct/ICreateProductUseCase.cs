using Microsoft.AspNetCore.Mvc;
using PStore.Catalog.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PStore.Catalog.Application.UseCases.CreateProduct
{
    public interface ICreateProductUseCase
    {
        public Task<Product> ExecuteAsync(CreateProductInputModel inputModel);
    }
}