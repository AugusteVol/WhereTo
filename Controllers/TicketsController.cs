using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhereTo.Api.Data;
using WhereTo.Api.Dtos;

namespace WhereTo.Api.Controllers;

[ApiController]
[Route("api/tickets")]
[Produces("application/json")]
public class TicketsController(AppDbContext db) : ControllerBase
{
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
}