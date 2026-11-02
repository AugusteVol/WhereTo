namespace WhereTo.Api.Dtos;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public AccountResponse Account { get; set; } = new();
}
