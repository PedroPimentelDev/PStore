using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PStore.Catalog.Application.UseCases.GetProductById
{
    public interface IGetProductByIdUseCase
    {
        public Task<IActionResult> ExecuteAsync(Guid id);
    }
}
