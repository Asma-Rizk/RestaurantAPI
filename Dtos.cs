namespace RestaurantAPI.DTOs;

// Menu DTOs
public record MenuItemDto(int Id, string Name, string Description, decimal Price, string Category, bool IsAvailable);
public record CreateMenuItemDto(string Name, string Description, decimal Price, string Category);
public record UpdateMenuItemDto(string Name, string Description, decimal Price, string Category, bool IsAvailable);

// Table DTOs
public record TableDto(int Id, int TableNumber, int Capacity, bool IsAvailable);

// Order DTOs
public record CreateOrderItemDto(int MenuItemId, int Quantity);
public record CreateOrderDto(int TableId, List<CreateOrderItemDto> Items, string? Notes);
public record OrderItemDto(int MenuItemId, string MenuItemName, int Quantity, decimal UnitPrice, decimal Subtotal);
public record OrderDto(int Id, int TableId, DateTime OrderTime, string Status, decimal TotalAmount, string? Notes, List<OrderItemDto> Items);
public record UpdateOrderStatusDto(string Status);

// Reservation DTOs
public record CreateReservationDto(int TableId, string CustomerName, string CustomerPhone, DateTime ReservationTime, int GuestCount);
public record ReservationDto(int Id, int TableNumber, string CustomerName, string CustomerPhone, DateTime ReservationTime, int GuestCount, bool IsConfirmed);

// Inventory DTOs
public record InventoryItemDto(int Id, string Name, string Unit, double Quantity, double LowStockThreshold, bool IsLowStock);
public record UpdateInventoryDto(double Quantity);

// Report DTOs
public record DailySalesDto(DateTime Date, int TotalOrders, decimal TotalRevenue);
public record LowStockAlertDto(int Id, string Name, string Unit, double Quantity, double Threshold);
