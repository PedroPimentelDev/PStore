using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PStore.Orders.Application.UseCases.CreateOrder
{
    public class CreateOrderInputModel
    {
        public List<OrderItemInput> Items { get; set; }
    }

    public class OrderItemInput()
    {
        public string Sku { get; set; }
        public int Quantity { get; set; }
    }
}
