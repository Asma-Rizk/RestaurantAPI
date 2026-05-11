using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.DTOs;
using RestaurantAPI.Models;

namespace RestaurantAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly AppDbContext _db;
    public MenuController(AppDbContext db) => _db = db;

    // GET /api/menu
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? category)
    {
        var query = _db.MenuItems.AsQueryable();
        if (!string.IsNullOrEmpty(category))
            query = query.Where(m => m.Category == category);

        var items = await query
            .Select(m => new MenuItemDto(m.Id, m.Name, m.Description, m.Price, m.Category, m.IsAvailable))
            .ToListAsync();

        return Ok(items);
    }

    // GET /api/menu/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _db.MenuItems.FindAsync(id);
        if (item is null) return NotFound();
        return Ok(new MenuItemDto(item.Id, item.Name, item.Description, item.Price, item.Category, item.IsAvailable));
    }

    // POST /api/menu
    [HttpPost]
    public async Task<IActionResult> Create(CreateMenuItemDto dto)
    {
        var item = new MenuItem
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Category = dto.Category
        };
        _db.MenuItems.Add(item);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.Id },
            new MenuItemDto(item.Id, item.Name, item.Description, item.Price, item.Category, item.IsAvailable));
    }

    // PUT /api/menu/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateMenuItemDto dto)
    {
        var item = await _db.MenuItems.FindAsync(id);
        if (item is null) return NotFound();

        item.Name = dto.Name;
        item.Description = dto.Description;
        item.Price = dto.Price;
        item.Category = dto.Category;
        item.IsAvailable = dto.IsAvailable;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/menu/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.MenuItems.FindAsync(id);
        if (item is null) return NotFound();

        _db.MenuItems.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
