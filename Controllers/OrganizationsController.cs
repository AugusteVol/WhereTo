using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhereTo.Api.Data;
using WhereTo.Api.Dtos;
using WhereTo.Api.Models;

namespace WhereTo.Api.Controllers;

[ApiController]
[Route("api/organizations")]
[Produces("application/json")]
public class OrganizationsController(AppDbContext db) : ControllerBase
{
    /// <summary>Grąžina visas organizacijas (pagal pavadinimą).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrganizationResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrganizationResponse>>> GetAll(CancellationToken ct)
    {
        var items = await db.Organizations
            .AsNoTracking()
            .OrderBy(o => o.Name)
            .ToListAsync(ct);

        return Ok(items.Select(OrganizationResponse.FromEntity));
    }

    /// <summary>Grąžina vieną organizaciją pagal ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OrganizationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrganizationResponse>> GetById(int id, CancellationToken ct)
    {
        var organization = await db.Organizations
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id, ct);

        return organization is null ? NotFoundResult() : Ok(OrganizationResponse.FromEntity(organization));
    }

    /// <summary>Sukuria naują organizaciją.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrganizationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrganizationResponse>> Create(OrganizationRequest request, CancellationToken ct)
    {
        var organization = new Organization();
        Apply(request, organization);

        db.Organizations.Add(organization);
        await db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetById), new { id = organization.Id }, OrganizationResponse.FromEntity(organization));
    }

    /// <summary>Atnaujina organizaciją.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(OrganizationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrganizationResponse>> Update(int id, OrganizationRequest request, CancellationToken ct)
    {
        var organization = await db.Organizations.FindAsync([id], ct);
        if (organization is null)
        {
            return NotFoundResult();
        }

        Apply(request, organization);
        await db.SaveChangesAsync(ct);

        return Ok(OrganizationResponse.FromEntity(organization));
    }

    /// <summary>Ištrina organizaciją (jos renginiams organization_id tampa NULL).</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var deleted = await db.Organizations.Where(o => o.Id == id).ExecuteDeleteAsync(ct);
        return deleted == 0 ? NotFoundResult() : NoContent();
    }

    private static void Apply(OrganizationRequest request, Organization organization)
    {
        organization.Name = request.Name.Trim();
        organization.Description = request.Description;
        organization.Email = request.Email;
        organization.Website = request.Website;
        organization.IsApproved = request.IsApproved;
    }

    private NotFoundObjectResult NotFoundResult() =>
        NotFound(new ProblemDetails { Status = StatusCodes.Status404NotFound, Title = "Organization not found." });
}
