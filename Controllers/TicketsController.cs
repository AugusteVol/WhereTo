using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WhereTo.Api.Data;
using WhereTo.Api.Dtos;
using WhereTo.Api.Models;

namespace WhereTo.Api.Controllers;

[ApiController]
[Route("api/tickets")]
[Produces("application/json")]
[Authorize]
public class TicketsController(AppDbContext db) : ControllerBase
{
    /// <summary>Grąžina puslapiuotą bilietų sąrašą.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedTicketResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedTicketResponse>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
        {
            ModelState.AddModelError(nameof(page), "Page must be at least 1 and pageSize must be between 1 and 100.");
            return ValidationProblem(ModelState);
        }

        var query = db.Tickets
            .AsNoTracking()
            .OrderBy(t => t.Id);

        var totalCount = await query.CountAsync(ct);
        var tickets = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TicketResponse
            {
                Id = t.Id,
                EventId = t.EventId,
                EventTitle = t.Event.Title,
                Category = t.Event.Category,
                Venue = t.Event.Venue,
                City = t.Event.City,
                Date = t.Event.EventDate,
                Price = t.Price,
                PurchasedAt = t.PurchasedAt
            })
            .ToListAsync(ct);

        foreach (var ticket in tickets)
        {
            ticket.Links = TicketLinks(ticket.Id);
        }

        return Ok(new PagedTicketResponse
        {
            Items = tickets,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            Links = CollectionLinks(page, pageSize, (int)Math.Ceiling(totalCount / (double)pageSize))
        });
    }

    /// <summary>Sukuria naują bilietą.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketResponse>> Create(TicketRequest request, CancellationToken ct)
    {
        if (!await db.Events.AnyAsync(e => e.Id == request.EventId, ct))
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Event not found."
            });
        }

        if (!await db.Accounts.AnyAsync(a => a.Id == request.AccountId, ct))
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Account not found."
            });
        }

        var ticket = new Ticket
        {
            EventId = request.EventId,
            AccountId = request.AccountId,
            Price = request.Price,
            PurchasedAt = request.PurchasedAt
        };

        db.Tickets.Add(ticket);
        await db.SaveChangesAsync(ct);

        var response = await db.Tickets
            .AsNoTracking()
            .Where(t => t.Id == ticket.Id)
            .Select(t => new TicketResponse
            {
                Id = t.Id,
                EventId = t.EventId,
                EventTitle = t.Event.Title,
                Category = t.Event.Category,
                Venue = t.Event.Venue,
                City = t.Event.City,
                Date = t.Event.EventDate,
                Price = t.Price,
                PurchasedAt = t.PurchasedAt
            })
            .SingleAsync(ct);

        response.Links = TicketLinks(response.Id);

        return CreatedAtAction(nameof(GetAll), response);
    }

    /// <summary>Atnaujina bilieto informaciją.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketResponse>> Update(int id, TicketRequest request, CancellationToken ct)
    {
        var ticket = await db.Tickets.FindAsync([id], ct);
        if (ticket is null)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Ticket not found."
            });
        }

        if (!await db.Events.AnyAsync(e => e.Id == request.EventId, ct))
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Event not found."
            });
        }

        if (!await db.Accounts.AnyAsync(a => a.Id == request.AccountId, ct))
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Account not found."
            });
        }

        ticket.EventId = request.EventId;
        ticket.AccountId = request.AccountId;
        ticket.Price = request.Price;
        ticket.PurchasedAt = request.PurchasedAt;

        await db.SaveChangesAsync(ct);

        var response = await db.Tickets
            .AsNoTracking()
            .Where(t => t.Id == ticket.Id)
            .Select(t => new TicketResponse
            {
                Id = t.Id,
                EventId = t.EventId,
                EventTitle = t.Event.Title,
                Category = t.Event.Category,
                Venue = t.Event.Venue,
                City = t.Event.City,
                Date = t.Event.EventDate,
                Price = t.Price,
                PurchasedAt = t.PurchasedAt
            })
            .SingleAsync(ct);

        response.Links = TicketLinks(response.Id);

        return Ok(response);
    }

    /// <summary>Ištrina bilietą.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var ticket = await db.Tickets.FindAsync([id], ct);
        if (ticket is null)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Ticket not found."
            });
        }

        db.Tickets.Remove(ticket);
        await db.SaveChangesAsync(ct);

        return NoContent();
    }

    /// <summary>Grąžina paskyrą, kuriai priklauso bilietas.</summary>
    [HttpGet("{ticketId:int}/owner")]
    [ProducesResponseType(typeof(TicketOwnerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketOwnerResponse>> GetOwner(int ticketId, CancellationToken ct)
    {
        var owner = await db.Tickets
            .AsNoTracking()
            .Where(t => t.Id == ticketId)
            .Select(t => new TicketOwnerResponse
            {
                AccountId = t.AccountId,
                Name = t.Account.Name,
                Email = t.Account.Email
            })
            .FirstOrDefaultAsync(ct);

        return owner is null
            ? NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Ticket not found."
            })
            : Ok(owner);
    }

    private IEnumerable<HypermediaLink> TicketLinks(int ticketId) =>
    [
        new() { Rel = "owner", Href = Url.Action(nameof(GetOwner), "Tickets", new { ticketId })!, Method = "GET" },
        new() { Rel = "update", Href = Url.Action(nameof(Update), "Tickets", new { id = ticketId })!, Method = "PUT" },
        new() { Rel = "delete", Href = Url.Action(nameof(Delete), "Tickets", new { id = ticketId })!, Method = "DELETE" }
    ];

    private IEnumerable<HypermediaLink> CollectionLinks(int page, int pageSize, int totalPages)
    {
        var links = new List<HypermediaLink>
        {
            new()
            {
                Rel = "self",
                Href = Url.Action(nameof(GetAll), "Tickets", new { page, pageSize })!,
                Method = "GET"
            }
        };

        if (page > 1)
        {
            links.Add(new HypermediaLink
            {
                Rel = "previous",
                Href = Url.Action(nameof(GetAll), "Tickets", new { page = page - 1, pageSize })!,
                Method = "GET"
            });
        }

        if (page < totalPages)
        {
            links.Add(new HypermediaLink
            {
                Rel = "next",
                Href = Url.Action(nameof(GetAll), "Tickets", new { page = page + 1, pageSize })!,
                Method = "GET"
            });
        }

        return links;
    }
}