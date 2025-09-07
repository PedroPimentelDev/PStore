using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PStore.Orders.Application.UseCases.GetOrder
{
    public interface IGetOrderUseCase
    {
        public Task<IActionResult> ExecuteAsync(Guid id);
    }
}
