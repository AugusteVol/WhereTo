using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WhereTo.Api.Models;

[Table("accounts")]
public class Account
{
    [Column("id")]
    public int Id { get; set; }

    [Column("name"), MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Column("email"), MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    public List<Ticket> Tickets { get; set; } = [];
}