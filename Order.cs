namespace RestaurantAPI.Models;

public enum OrderStatus
{
    Pending,
    Preparing,
    Ready,
    Served,
    Cancelled
}

public class Order
{
    public int Id { get; set; }
    public int TableId { get; set; }
    public RestaurantTable Table { get; set; } = null!;
    public DateTime OrderTime { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public int MenuItemId { get; set; }
    public MenuItem MenuItem { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
