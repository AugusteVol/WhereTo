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

    [Column("password_hash"), MaxLength(500)]
    public string PasswordHash { get; set; } = string.Empty;

    [Column("role"), MaxLength(50)]
    public string Role { get; set; } = "User";

    public List<Ticket> Tickets { get; set; } = [];
}