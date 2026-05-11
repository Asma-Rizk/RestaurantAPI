namespace RestaurantAPI.Models;

public class Reservation
{
    public int Id { get; set; }
    public int TableId { get; set; }
    public RestaurantTable Table { get; set; } = null!;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public DateTime ReservationTime { get; set; }
    public int GuestCount { get; set; }
    public bool IsConfirmed { get; set; } = true;
}
