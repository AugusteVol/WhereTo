using WhereTo.Api.Models;

namespace WhereTo.Api.Dtos;

/// <summary>Organizacija, grąžinama klientui.</summary>
public class OrganizationResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public bool IsApproved { get; set; }
    public DateTime? CreatedAt { get; set; }

    public static OrganizationResponse FromEntity(Organization o) => new()
    {
        Id = o.Id,
        Name = o.Name,
        Description = o.Description,
        Email = o.Email,
        Website = o.Website,
        IsApproved = o.IsApproved,
        CreatedAt = o.CreatedAt
    };
}
