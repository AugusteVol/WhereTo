using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhereTo.Api.Data;
using WhereTo.Api.Dtos;
using WhereTo.Api.Models;

namespace WhereTo.Api.Controllers;

[ApiController]
[Route("api/events")]
[Produces("application/json")]
public class EventsController(AppDbContext db) : ControllerBase
{
    /// <summary>Grąžina visus renginius.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EventResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EventResponse>>> GetUpcoming(CancellationToken ct)
    {
        var events = await db.Events
            .AsNoTracking()
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

    /// <summary>Grąžina miestus, kuriuose vyksta renginiai.</summary>
    [HttpGet("cities")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<string>>> GetCities(CancellationToken ct)
    {
        var cities = await db.Events
            .AsNoTracking()
            .Where(e => e.City != string.Empty)
            .Select(e => e.City)
            .Distinct()
            .ToListAsync(ct);

        return Ok(cities);
    }

    /// <summary>Grąžina renginio bilietų prieinamumą.</summary>
    [HttpGet("{eventId:int}/availability")]
    [ProducesResponseType(typeof(EventAvailabilityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventAvailabilityResponse>> GetAvailability(int eventId, CancellationToken ct)
    {
        var availability = await db.Events
            .AsNoTracking()
            .Where(e => e.Id == eventId)
            .Select(e => new EventAvailabilityResponse
            {
                EventId = e.Id,
                TotalTickets = e.TotalTickets,
                AvailableTickets = e.AvailableTickets,
                SoldTickets = e.TotalTickets - e.AvailableTickets
            })
            .FirstOrDefaultAsync(ct);

        return availability is null
            ? NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Event not found."
            })
            : Ok(availability);
    }

    /// <summary>Grąžina vieną renginį pagal ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventResponse>> GetById(int id, CancellationToken ct)
    {
        var item = await db.Events
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, ct);

        return item is null ? NotFoundResult() : Ok(ToResponse(item));
    }

    /// <summary>Sukuria naują renginį.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventResponse>> Create(EventRequest request, CancellationToken ct)
    {
        if (!await OrganizationExists(request.OrganizationId, ct))
        {
            return NotFoundOrganizationResult();
        }

        var item = new Event();
        Apply(request, item);

        db.Events.Add(item);
        await db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, ToResponse(item));
    }

    /// <summary>Atnaujina renginį.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventResponse>> Update(int id, EventRequest request, CancellationToken ct)
    {
        var item = await db.Events.FindAsync([id], ct);
        if (item is null)
        {
            return NotFoundResult();
        }

        if (!await OrganizationExists(request.OrganizationId, ct))
        {
            return NotFoundOrganizationResult();
        }

        Apply(request, item);
        await db.SaveChangesAsync(ct);

        return Ok(ToResponse(item));
    }

    /// <summary>Ištrina renginį.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        if (!await db.Events.AnyAsync(e => e.Id == id, ct))
        {
            return NotFoundResult();
        }

        if (await db.Tickets.AnyAsync(t => t.EventId == id, ct))
        {
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Event cannot be deleted.",
                Detail = "The event has tickets. Delete or move those tickets before deleting the event."
            });
        }

        var deleted = await db.Events.Where(e => e.Id == id).ExecuteDeleteAsync(ct);
        return deleted == 0 ? NotFoundResult() : NoContent();
    }

    private async Task<bool> OrganizationExists(int? organizationId, CancellationToken ct) =>
        organizationId is null || await db.Organizations.AnyAsync(o => o.Id == organizationId, ct);

    private static void Apply(EventRequest request, Event item)
    {
        item.OrganizationId = request.OrganizationId;
        item.Title = request.Title.Trim();
        item.Category = request.Category.Trim();
        item.Description = request.Description;
        item.Venue = request.Venue.Trim();
        item.City = request.City.Trim();
        item.EventDate = request.Date;
        item.StartingPrice = request.Price;
        item.TotalTickets = request.TotalTickets;
        item.AvailableTickets = request.AvailableTickets;
    }

    private static EventResponse ToResponse(Event item) => new()
    {
        Id = item.Id,
        Title = item.Title,
        Category = item.Category,
        Venue = item.Venue,
        City = item.City,
        Date = item.EventDate,
        Price = item.StartingPrice
    };

    private NotFoundObjectResult NotFoundResult() =>
        NotFound(new ProblemDetails { Status = StatusCodes.Status404NotFound, Title = "Event not found." });

    private NotFoundObjectResult NotFoundOrganizationResult() =>
        NotFound(new ProblemDetails { Status = StatusCodes.Status404NotFound, Title = "Organization not found." });
}
