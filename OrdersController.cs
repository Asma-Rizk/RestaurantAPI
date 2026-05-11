using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.DTOs;
using RestaurantAPI.Models;

namespace RestaurantAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _db;
    public OrdersController(AppDbContext db) => _db = db;

    // GET /api/orders
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? status)
    {
        var query = _db.Orders
            .Include(o => o.Table)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, true, out var parsedStatus))
            query = query.Where(o => o.Status == parsedStatus);

        var orders = await query.Select(o => MapOrderDto(o)).ToListAsync();
        return Ok(orders);
    }

    // GET /api/orders/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _db.Orders
            .Include(o => o.Table)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.MenuItem)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null) return NotFound();
        return Ok(MapOrderDto(order));
    }

    // POST /api/orders
    [HttpPost]
    public async Task<IActionResult> PlaceOrder(CreateOrderDto dto)
    {
        var table = await _db.Tables.FindAsync(dto.TableId);
        if (table is null) return BadRequest("Table not found.");

        var order = new Order
        {
            TableId = dto.TableId,
            Notes = dto.Notes,
            OrderItems = new List<OrderItem>()
        };

        decimal total = 0;
        foreach (var item in dto.Items)
        {
            var menuItem = await _db.MenuItems.FindAsync(item.MenuItemId);
            if (menuItem is null || !menuItem.IsAvailable)
                return BadRequest($"Menu item {item.MenuItemId} not available.");

            var orderItem = new OrderItem
            {
                MenuItemId = item.MenuItemId,
                Quantity = item.Quantity,
                UnitPrice = menuItem.Price
            };
            order.OrderItems.Add(orderItem);
            total += menuItem.Price * item.Quantity;
        }

        order.TotalAmount = total;
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, MapOrderDto(order));
    }

    // PATCH /api/orders/{id}/status
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateOrderStatusDto dto)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order is null) return NotFound();

        if (!Enum.TryParse<OrderStatus>(dto.Status, true, out var newStatus))
            return BadRequest("Invalid status. Valid values: Pending, Preparing, Ready, Served, Cancelled");

        order.Status = newStatus;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static OrderDto MapOrderDto(Order o) => new(
        o.Id,
        o.TableId,
        o.OrderTime,
        o.Status.ToString(),
        o.TotalAmount,
        o.Notes,
        o.OrderItems.Select(oi => new OrderItemDto(
            oi.MenuItemId,
            oi.MenuItem?.Name ?? "",
            oi.Quantity,
            oi.UnitPrice,
            oi.UnitPrice * oi.Quantity
        )).ToList()
    );
}
