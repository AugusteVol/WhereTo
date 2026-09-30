namespace WhereTo.Api.Dtos;

/// <summary>Bilietas, priklausantis paskyrai.</summary>
public class TicketResponse
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public string EventTitle { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Venue { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public decimal Price { get; set; }
    public DateTime PurchasedAt { get; set; }
}