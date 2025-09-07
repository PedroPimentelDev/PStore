namespace PStore.Orders.Domain;


public sealed class Order {
    public Guid Id { get; set; }
    public string Number { get; set; } = default!; 
    public decimal Total { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Created;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<OrderItem> Items { get; set; } = new();
}

public sealed class OrderItem {
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string Sku { get; set; } = default!;
    public string Name { get; set; } = default!;   
    public decimal UnitPrice { get; set; }     
    public int Quantity { get; set; }
    public decimal LineTotal => UnitPrice * Quantity;
}

public enum OrderStatus { Created, Paid, Rejected }