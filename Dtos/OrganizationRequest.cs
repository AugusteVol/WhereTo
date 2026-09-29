using System.ComponentModel.DataAnnotations;

namespace WhereTo.Api.Dtos;

/// <summary>Duomenys organizacijai sukurti arba atnaujinti.</summary>
public class OrganizationRequest
{
    /// <example>Vilniaus kultūros centras</example>
    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    /// <example>Kultūros renginių organizatorius.</example>
    public string? Description { get; set; }

    /// <example>info@kultura.example</example>
    [EmailAddress, StringLength(150)]
    public string? Email { get; set; }

    /// <example>https://kultura.example</example>
    [Url, StringLength(255)]
    public string? Website { get; set; }

    /// <example>false</example>
    public bool IsApproved { get; set; }
}
