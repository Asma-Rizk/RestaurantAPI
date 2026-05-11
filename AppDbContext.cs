using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Models;

namespace RestaurantAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<RestaurantTable> Tables => Set<RestaurantTable>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seed Menu Items
        modelBuilder.Entity<MenuItem>().HasData(
            new MenuItem { Id = 1, Name = "Caesar Salad", Description = "Fresh romaine lettuce", Price = 8.99m, Category = "Starters" },
            new MenuItem { Id = 2, Name = "Grilled Chicken", Description = "Marinated grilled chicken breast", Price = 15.99m, Category = "Main" },
            new MenuItem { Id = 3, Name = "Pasta Carbonara", Description = "Classic Italian pasta", Price = 13.99m, Category = "Main" },
            new MenuItem { Id = 4, Name = "Chocolate Lava Cake", Description = "Warm chocolate cake", Price = 7.99m, Category = "Desserts" }
        );

        // Seed Tables
        modelBuilder.Entity<RestaurantTable>().HasData(
            new RestaurantTable { Id = 1, TableNumber = 1, Capacity = 2 },
            new RestaurantTable { Id = 2, TableNumber = 2, Capacity = 4 },
            new RestaurantTable { Id = 3, TableNumber = 3, Capacity = 6 },
            new RestaurantTable { Id = 4, TableNumber = 4, Capacity = 8 }
        );

        // Seed Inventory
        modelBuilder.Entity<InventoryItem>().HasData(
            new InventoryItem { Id = 1, Name = "Chicken Breast", Unit = "kg", Quantity = 20, LowStockThreshold = 5 },
            new InventoryItem { Id = 2, Name = "Pasta", Unit = "kg", Quantity = 15, LowStockThreshold = 3 },
            new InventoryItem { Id = 3, Name = "Chocolate", Unit = "kg", Quantity = 4, LowStockThreshold = 2 },
            new InventoryItem { Id = 4, Name = "Lettuce", Unit = "kg", Quantity = 8, LowStockThreshold = 2 }
        );
    }
}
