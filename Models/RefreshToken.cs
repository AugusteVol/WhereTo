using System.ComponentModel.DataAnnotations.Schema;

namespace WhereTo.Api.Models;

[Table("refresh_tokens")]
public class RefreshToken
{
    [Column("id")]
    public int Id { get; set; }

    [Column("account_id")]
    public int AccountId { get; set; }

    public Account Account { get; set; } = null!;

    [Column("token_hash")]
    public string TokenHash { get; set; } = string.Empty;

    [Column("expires_at")]
    public DateTime ExpiresAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("revoked_at")]
    public DateTime? RevokedAt { get; set; }
}
