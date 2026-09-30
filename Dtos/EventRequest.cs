using System.ComponentModel.DataAnnotations;

namespace WhereTo.Api.Dtos;

/// <summary>Duomenys renginiui sukurti arba atnaujinti.</summary>
public class EventRequest
{
    public int? OrganizationId { get; set; }

    [Required, StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string Category { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required, StringLength(150)]
    public string Venue { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string City { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int TotalTickets { get; set; }

    [Range(0, int.MaxValue)]
    public int AvailableTickets { get; set; }
}