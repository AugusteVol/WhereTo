namespace WhereTo.Api.Dtos;

/// <summary>Autentifikacijos tokenai ir paskyros informacija.</summary>
public sealed class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiresAt { get; set; }
    public AccountResponse Account { get; set; } = new();
}
