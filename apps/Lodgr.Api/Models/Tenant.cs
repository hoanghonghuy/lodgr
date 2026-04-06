using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lodgr.Api.Models;

[Table("tenants")]
public class Tenant
{
    [Key]
    [Column("tenant_id")]
    public long TenantId { get; set; }

    [Required]
    [StringLength(100)]
    [Column("full_name")]
    public string FullName { get; set; } = string.Empty;

    [StringLength(20)]
    [Column("id_card_number")]
    public string? IdCardNumber { get; set; }

    [StringLength(255)]
    [Column("email")]
    public string? Email { get; set; }

    [StringLength(20)]
    [Column("phone")]
    public string? Phone { get; set; }

    [Column("date_of_birth")]
    public DateOnly? DateOfBirth { get; set; }

    [Column("hometown")]
    public string? Hometown { get; set; }

    [StringLength(100)]
    [Column("occupation")]
    public string? Occupation { get; set; }

    [Column("note")]
    public string? Note { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
