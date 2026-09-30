using System.ComponentModel.DataAnnotations;

namespace WhereTo.Api.Dtos;

/// <summary>Duomenys paskyrai sukurti.</summary>
public class AccountRequest
{
    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;
}