using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WhereTo.Api.Models;

[Table("organizations")]
public class Organization
{
    [Column("id")]
    public int Id { get; set; }

    [Column("name"), MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("email"), MaxLength(150)]
    public string? Email { get; set; }

    [Column("website"), MaxLength(255)]
    public string? Website { get; set; }

    [Column("is_approved")]
    public bool IsApproved { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    public List<Event> Events { get; set; } = [];
}
