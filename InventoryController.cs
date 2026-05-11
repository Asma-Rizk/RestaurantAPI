using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.DTOs;
using RestaurantAPI.Models;

namespace RestaurantAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly AppDbContext _db;
    public InventoryController(AppDbContext db) => _db = db;

    // GET /api/inventory
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool? lowStock)
    {
        var items = await _db.InventoryItems.ToListAsync();
        var dtos = items
            .Where(i => !lowStock.HasValue || i.IsLowStock == lowStock.Value)
            .Select(i => new InventoryItemDto(i.Id, i.Name, i.Unit, i.Quantity, i.LowStockThreshold, i.IsLowStock))
            .ToList();

        return Ok(dtos);
    }

    // POST /api/inventory
    [HttpPost]
    public async Task<IActionResult> Create(InventoryItemDto dto)
    {
        var item = new InventoryItem
        {
            Name = dto.Name,
            Unit = dto.Unit,
            Quantity = dto.Quantity,
            LowStockThreshold = dto.LowStockThreshold
        };
        _db.InventoryItems.Add(item);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = item.Id },
            new InventoryItemDto(item.Id, item.Name, item.Unit, item.Quantity, item.LowStockThreshold, item.IsLowStock));
    }

    // PATCH /api/inventory/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateQuantity(int id, UpdateInventoryDto dto)
    {
        var item = await _db.InventoryItems.FindAsync(id);
        if (item is null) return NotFound();

        item.Quantity = dto.Quantity;
        await _db.SaveChangesAsync();

        var response = new InventoryItemDto(item.Id, item.Name, item.Unit, item.Quantity, item.LowStockThreshold, item.IsLowStock);
        if (item.IsLowStock)
            return Ok(new { item = response, alert = $"⚠️ Low stock alert: {item.Name} is below threshold!" });

        return Ok(new { item = response });
    }

    // GET /api/inventory/alerts
    [HttpGet("alerts")]
    public async Task<IActionResult> GetAlerts()
    {
        var items = await _db.InventoryItems.ToListAsync();
        var alerts = items
            .Where(i => i.IsLowStock)
            .Select(i => new LowStockAlertDto(i.Id, i.Name, i.Unit, i.Quantity, i.LowStockThreshold))
            .ToList();

        return Ok(alerts);
    }
}
