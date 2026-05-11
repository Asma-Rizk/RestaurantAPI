using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.DTOs;
using RestaurantAPI.Models;

namespace RestaurantAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly AppDbContext _db;
    public ReservationsController(AppDbContext db) => _db = db;

    // GET /api/reservations
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var reservations = await _db.Reservations
            .Include(r => r.Table)
            .Select(r => new ReservationDto(
                r.Id, r.Table.TableNumber, r.CustomerName,
                r.CustomerPhone, r.ReservationTime, r.GuestCount, r.IsConfirmed))
            .ToListAsync();

        return Ok(reservations);
    }

    // POST /api/reservations
    [HttpPost]
    public async Task<IActionResult> Create(CreateReservationDto dto)
    {
        var table = await _db.Tables.FindAsync(dto.TableId);
        if (table is null) return BadRequest("Table not found.");
        if (!table.IsAvailable) return BadRequest("Table is not available.");
        if (table.Capacity < dto.GuestCount)
            return BadRequest($"Table capacity ({table.Capacity}) is less than guest count ({dto.GuestCount}).");

        // Check for conflicting reservations (within 2 hours)
        var conflict = await _db.Reservations.AnyAsync(r =>
            r.TableId == dto.TableId &&
            r.IsConfirmed &&
            Math.Abs((r.ReservationTime - dto.ReservationTime).TotalHours) < 2);

        if (conflict) return BadRequest("Table already reserved at this time.");

        var reservation = new Reservation
        {
            TableId = dto.TableId,
            CustomerName = dto.CustomerName,
            CustomerPhone = dto.CustomerPhone,
            ReservationTime = dto.ReservationTime,
            GuestCount = dto.GuestCount
        };

        _db.Reservations.Add(reservation);
        table.IsAvailable = false;
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = reservation.Id },
            new ReservationDto(reservation.Id, table.TableNumber,
                reservation.CustomerName, reservation.CustomerPhone,
                reservation.ReservationTime, reservation.GuestCount, reservation.IsConfirmed));
    }

    // DELETE /api/reservations/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Cancel(int id)
    {
        var reservation = await _db.Reservations.Include(r => r.Table).FirstOrDefaultAsync(r => r.Id == id);
        if (reservation is null) return NotFound();

        reservation.Table.IsAvailable = true;
        _db.Reservations.Remove(reservation);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
