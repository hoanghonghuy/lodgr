using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lodgr.Api.Models;

[Table("buildings")]
public class Building
{
    [Key]
    [Column("building_id")]
    public long BuildingId { get; set; }

    [Column("owner_id")]
    public long OwnerId { get; set; }

    [Required]
    [StringLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    [Column("total_floors")]
    public int TotalFloors { get; set; } = 1;

    [Column("description")]
    public string? Description { get; set; }

    [Required]
    [StringLength(255)]
    [Column("address_detail")]
    public string AddressDetail { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [Column("ward_code")]
    public string WardCode { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }
}
