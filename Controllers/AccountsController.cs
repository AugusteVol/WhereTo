using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using WhereTo.Api.Data;
using WhereTo.Api.Dtos;
using WhereTo.Api.Models;
using WhereTo.Api.Security;

namespace WhereTo.Api.Controllers;

[ApiController]
[Route("api/accounts")]
[Produces("application/json")]
[Authorize]
public class AccountsController(AppDbContext db, JwtTokenService tokens) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken ct)
    {
        var account = await db.Accounts
            .SingleOrDefaultAsync(a => a.Email == request.Email.Trim(), ct);

        if (account is null || !PasswordService.Verify(request.Password, account.PasswordHash))
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Invalid email or password."
            });
        }

        return Ok(tokens.Create(account));
    }

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
    [AllowAnonymous]
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
            Email = email,
            PasswordHash = PasswordService.Hash(request.Password),
            Role = "User"
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