using System.ComponentModel.DataAnnotations.Schema;

namespace WhereTo.Api.Models;

[Table("tickets")]
public class Ticket
{
    [Column("id")]
    public int Id { get; set; }

    [Column("event_id")]
    public int EventId { get; set; }

    public Event Event { get; set; } = null!;

    [Column("account_id")]
    public int AccountId { get; set; }

    public Account Account { get; set; } = null!;

    [Column("price")]
    public decimal Price { get; set; }

    [Column("purchased_at")]
    public DateTime PurchasedAt { get; set; }
}