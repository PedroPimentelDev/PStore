using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PStore.Catalog.Application.UseCases.GetAllProductsBySku
{
    public interface IGetAllProductsBySkuUseCase
    {
        public Task<IActionResult> ExecuteAsync(string sku);
    }
}
