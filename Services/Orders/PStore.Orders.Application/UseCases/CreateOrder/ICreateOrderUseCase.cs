using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PStore.Orders.Application.UseCases.CreateOrder
{
    public interface ICreateOrderUseCase
    {
        public Task<(bool IsSuccess, IActionResult? Result)> ExecuteAsync(CreateOrderInputModel inputModel);
    }
}
