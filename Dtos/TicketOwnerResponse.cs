namespace WhereTo.Api.Dtos;

/// <summary>Paskyra, kuriai priklauso bilietas.</summary>
public class TicketOwnerResponse
{
    public int AccountId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}