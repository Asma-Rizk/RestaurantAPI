using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.DTOs;
using RestaurantAPI.Models;

namespace RestaurantAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly AppDbContext _db;
    public ReportsController(AppDbContext db) => _db = db;

    // GET /api/reports/daily-sales?date=2024-01-15
    [HttpGet("daily-sales")]
    public async Task<IActionResult> DailySales([FromQuery] DateTime? date)
    {
        var targetDate = date?.Date ?? DateTime.UtcNow.Date;

        var orders = await _db.Orders
            .Where(o => o.OrderTime.Date == targetDate && o.Status == OrderStatus.Served)
            .ToListAsync();

        var report = new DailySalesDto(
            targetDate,
            orders.Count,
            orders.Sum(o => o.TotalAmount)
        );

        return Ok(report);
    }

    // GET /api/reports/popular-items
    [HttpGet("popular-items")]
    public async Task<IActionResult> PopularItems()
    {
        var popular = await _db.OrderItems
            .Include(oi => oi.MenuItem)
            .GroupBy(oi => new { oi.MenuItemId, oi.MenuItem.Name })
            .Select(g => new
            {
                MenuItemId = g.Key.MenuItemId,
                Name = g.Key.Name,
                TotalOrdered = g.Sum(oi => oi.Quantity)
            })
            .OrderByDescending(x => x.TotalOrdered)
            .Take(5)
            .ToListAsync();

        return Ok(popular);
    }
}
