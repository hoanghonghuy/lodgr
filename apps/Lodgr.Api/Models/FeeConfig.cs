using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lodgr.Api.Models;

[Table("fee_configs")]
public class FeeConfig
{
    [Key]
    [Column("config_id")]
    public long ConfigId { get; set; }

    [Column("building_id")]
    public long BuildingId { get; set; }

    [Column("fee_type")]
    public FeeType FeeType { get; set; }

    [Column("charge_mode")]
    public ChargeMode ChargeMode { get; set; }

    [Column("unit_price", TypeName = "numeric(12,2)")]
    public decimal UnitPrice { get; set; }

    [StringLength(20)]
    [Column("unit")]
    public string? Unit { get; set; }

    [Column("effective_from")]
    public DateOnly EffectiveFrom { get; set; }

    [Column("effective_to")]
    public DateOnly? EffectiveTo { get; set; }

    [StringLength(255)]
    [Column("note")]
    public string? Note { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}

public enum FeeType
{
    ELECTRICITY,
    WATER,
    INTERNET,
    GARBAGE,
    PARKING,
    OTHER,
}

public enum ChargeMode
{
    PER_UNIT,
    PER_PERSON,
    FIXED_MONTHLY,
}
