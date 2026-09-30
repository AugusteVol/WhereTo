namespace WhereTo.Api.Dtos;

/// <summary>Paskyros duomenys.</summary>
public class AccountResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}