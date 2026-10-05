using System.ComponentModel.DataAnnotations;

namespace WhereTo.Api.Dtos;

/// <summary>Duomenys bilietui atnaujinti.</summary>
public class TicketRequest
{
    [Range(1, int.MaxValue)]
    public int EventId { get; set; }

    [Range(1, int.MaxValue)]
    public int AccountId { get; set; }

    [Range(typeof(decimal), "0", "1000")]
    public decimal Price { get; set; }

    public DateTime PurchasedAt { get; set; }
}