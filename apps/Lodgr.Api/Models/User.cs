using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lodgr.Api.Models;

[Table("users")]
public class User
{
	[Key]
	[Column("user_id")]
	public long UserId { get; set; }

	[Required]
	[StringLength(255)]
	[Column("email")]
	public string Email { get; set; } = string.Empty;

	[Required]
	[StringLength(255)]
	[Column("password_hash")]
	public string PasswordHash { get; set; } = string.Empty;

	[Required]
	[StringLength(100)]
	[Column("full_name")]
	public string FullName { get; set; } = string.Empty;

	[StringLength(20)]
	[Column("phone")]
	public string? Phone { get; set; }

	[Column("role")]
	public Role Role { get; set; }

	[Column("created_at")]
	public DateTime CreatedAt { get; set; }

	[Column("updated_at")]
	public DateTime UpdatedAt { get; set; }
}

public enum Role
{
	OWNER,
	MANAGER,
}
