using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhereTo.Api.Data;
using WhereTo.Api.Dtos;
using WhereTo.Api.Models;

namespace WhereTo.Api.Controllers;

[ApiController]
[Route("api/accounts")]
[Produces("application/json")]
public class AccountsController(AppDbContext db) : ControllerBase
{
    /// <summary>Grąžina vieną paskyrą pagal ID.</summary>
    [HttpGet("{accountId:int}")]
    [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AccountResponse>> GetById(int accountId, CancellationToken ct)
    {
        var account = await db.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == accountId, ct);

        return account is null ? NotFoundResult() : Ok(ToResponse(account));
    }

    /// <summary>Sukuria naują paskyrą.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AccountResponse>> Create(AccountRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim();
        if (await db.Accounts.AnyAsync(a => a.Email == email, ct))
        {
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "An account with this email already exists."
            });
        }

        var account = new Account
        {
            Name = request.Name.Trim(),
            Email = email
        };

        db.Accounts.Add(account);
        await db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetById), new { accountId = account.Id }, ToResponse(account));
    }

    /// <summary>Grąžina paskyrai priklausančius bilietus.</summary>
    [HttpGet("{accountId:int}/tickets")]
    [ProducesResponseType(typeof(IEnumerable<TicketResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<TicketResponse>>> GetTickets(int accountId, CancellationToken ct)
    {
        if (!await db.Accounts.AnyAsync(a => a.Id == accountId, ct))
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Account not found."
            });
        }

        var tickets = await db.Tickets
            .AsNoTracking()
            .Where(t => t.AccountId == accountId)
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

        return Ok(tickets);
    }

    private static AccountResponse ToResponse(Account account) => new()
    {
        Id = account.Id,
        Name = account.Name,
        Email = account.Email
    };

    private NotFoundObjectResult NotFoundResult() =>
        NotFound(new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Account not found."
        });
}