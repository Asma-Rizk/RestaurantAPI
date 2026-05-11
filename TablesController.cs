using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.DTOs;

namespace RestaurantAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TablesController : ControllerBase
{
    private readonly AppDbContext _db;
    public TablesController(AppDbContext db) => _db = db;

    // GET /api/tables
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool? available)
    {
        var query = _db.Tables.AsQueryable();
        if (available.HasValue)
            query = query.Where(t => t.IsAvailable == available.Value);

        var tables = await query
            .Select(t => new TableDto(t.Id, t.TableNumber, t.Capacity, t.IsAvailable))
            .ToListAsync();

        return Ok(tables);
    }

    // PATCH /api/tables/{id}/availability
    [HttpPatch("{id}/availability")]
    public async Task<IActionResult> SetAvailability(int id, [FromBody] bool isAvailable)
    {
        var table = await _db.Tables.FindAsync(id);
        if (table is null) return NotFound();

        table.IsAvailable = isAvailable;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
