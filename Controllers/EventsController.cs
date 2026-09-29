using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhereTo.Api.Data;
using WhereTo.Api.Dtos;

namespace WhereTo.Api.Controllers;

[ApiController]
[Route("api/events")]
[Produces("application/json")]
public class EventsController(AppDbContext db) : ControllerBase
{
    /// <summary>Grąžina artėjančius renginius (nuo šiandien), seniausius pirmiausia.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EventResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EventResponse>>> GetUpcoming(CancellationToken ct)
    {
        var today = DateTime.Today;

        var events = await db.Events
            .AsNoTracking()
            .Where(e => e.EventDate >= today)
            .OrderBy(e => e.EventDate)
            .Select(e => new EventResponse
            {
                Id = e.Id,
                Title = e.Title,
                Category = e.Category,
                Venue = e.Venue,
                City = e.City,
                Date = e.EventDate,
                Price = e.StartingPrice
            })
            .ToListAsync(ct);

        return Ok(events);
    }
}
