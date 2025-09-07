using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PStore.Orders.Application.UseCases.CreateOrder
{
    internal class CreateOrderViewModel
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }

        public CreateOrderViewModel(Guid id, string number, decimal total, string status)
        {
            Id = id;
            Number = number;
            Total = total;
            Status = status;
        }
    }
}
