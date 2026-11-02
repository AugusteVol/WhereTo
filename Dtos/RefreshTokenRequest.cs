using System.ComponentModel.DataAnnotations;

namespace WhereTo.Api.Dtos;

/// <summary>Prašymas atnaujinti prieigos tokeną.</summary>
public sealed class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
