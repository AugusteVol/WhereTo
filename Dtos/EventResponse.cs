namespace WhereTo.Api.Dtos;

/// <summary>Renginys sąrašui.</summary>
public class EventResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Venue { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public decimal Price { get; set; }
}
