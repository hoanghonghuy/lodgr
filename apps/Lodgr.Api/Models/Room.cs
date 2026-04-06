using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lodgr.Api.Models;

[Table("rooms")]
public class Room
{
    [Key]
    [Column("room_id")]
    public long RoomId { get; set; }

    [Column("building_id")]
    public long BuildingId { get; set; }

    [Required]
    [StringLength(20)]
    [Column("room_number")]
    public string RoomNumber { get; set; } = string.Empty;

    [Column("floor")]
    public int? Floor { get; set; }

    [Column("area", TypeName = "numeric(8,2)")]
    public decimal? Area { get; set; }

    [Column("base_price", TypeName = "numeric(12,2)")]
    public decimal BasePrice { get; set; }

    [Column("room_type")]
    public RoomType? RoomType { get; set; }

    [Range(1, int.MaxValue)]
    [Column("max_occupants")]
    public int MaxOccupants { get; set; } = 1;

    [Column("operational_status")]
    public OperationalStatus OperationalStatus { get; set; } = OperationalStatus.ACTIVE;

    [Column("description")]
    public string? Description { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }
}

public enum RoomType
{
    SINGLE,
    DOUBLE,
    STUDIO,
    FAMILY,
}

public enum OperationalStatus
{
    ACTIVE,
    MAINTENANCE,
    BLOCKED,
}
