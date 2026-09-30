namespace WhereTo.Api.Dtos;

/// <summary>Renginio bilietų prieinamumas.</summary>
public class EventAvailabilityResponse
{
    public int EventId { get; set; }
    public int TotalTickets { get; set; }
    public int AvailableTickets { get; set; }
    public int SoldTickets { get; set; }
}