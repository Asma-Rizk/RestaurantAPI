namespace RestaurantAPI.Models;

public class InventoryItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty; // e.g. "kg", "liters", "pieces"
    public double Quantity { get; set; }
    public double LowStockThreshold { get; set; }
    public bool IsLowStock => Quantity <= LowStockThreshold;
}
