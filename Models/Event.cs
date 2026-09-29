using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WhereTo.Api.Models;

[Table("events")]
public class Event
{
    [Column("id")]
    public int Id { get; set; }

    [Column("organization_id")]
    public int? OrganizationId { get; set; }

    public Organization? Organization { get; set; }

    [Column("title"), MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Column("category"), MaxLength(80)]
    public string Category { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("venue"), MaxLength(150)]
    public string Venue { get; set; } = string.Empty;

    [Column("city"), MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [Column("event_date")]
    public DateTime EventDate { get; set; }

    [Column("starting_price")]
    public decimal StartingPrice { get; set; }

    [Column("total_tickets")]
    public int TotalTickets { get; set; }

    [Column("available_tickets")]
    public int AvailableTickets { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }
}
