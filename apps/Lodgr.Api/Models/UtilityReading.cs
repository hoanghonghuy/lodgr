using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lodgr.Api.Models;

[Table("utility_readings")]
public class UtilityReading
{
    [Key]
    [Column("reading_id")]
    public long ReadingId { get; set; }

    [Column("room_id")]
    public long RoomId { get; set; }

    [Column("contract_id")]
    public long? ContractId { get; set; }

    [Column("reading_type")]
    public ReadingType ReadingType { get; set; } = ReadingType.REGULAR;

    [Column("period_from")]
    public DateOnly PeriodFrom { get; set; }

    [Column("period_to")]
    public DateOnly PeriodTo { get; set; }

    [Column("closing_date")]
    public DateOnly ClosingDate { get; set; }

    [Column("electricity_old", TypeName = "numeric(10,2)")]
    public decimal? ElectricityOld { get; set; }

    [Column("electricity_new", TypeName = "numeric(10,2)")]
    public decimal? ElectricityNew { get; set; }

    [Column("water_old", TypeName = "numeric(10,2)")]
    public decimal? WaterOld { get; set; }

    [Column("water_new", TypeName = "numeric(10,2)")]
    public decimal? WaterNew { get; set; }

    [Column("recorded_by")]
    public long? RecordedBy { get; set; }

    [Column("recorded_at")]
    public DateTime RecordedAt { get; set; }

    [Column("note")]
    public string? Note { get; set; }
}

public enum ReadingType
{
    REGULAR,
    MOVE_IN,
    MOVE_OUT,
    FINAL,
    ADJUSTMENT,
}
