using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PStore.Catalog.Application.UseCases.UpdateProduct
{
    public interface IUpdateProductUseCase
    {
        public Task<IActionResult> ExecuteAsync(UpdateProductInputModel inputModel);
    }
}
