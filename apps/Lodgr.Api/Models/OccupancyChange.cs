using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lodgr.Api.Models;

[Table("occupancy_changes")]
public class OccupancyChange
{
    [Key]
    [Column("change_id")]
    public long ChangeId { get; set; }

    [Column("contract_id")]
    public long ContractId { get; set; }

    [Column("effective_date")]
    public DateOnly EffectiveDate { get; set; }

    [Range(1, int.MaxValue)]
    [Column("occupants_count")]
    public int OccupantsCount { get; set; }

    [Column("reason")]
    public Reason Reason { get; set; }

    [Column("note")]
    public string? Note { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("created_by")]
    public long? CreatedBy { get; set; }
}

public enum Reason
{
    MOVE_IN,
    MOVE_OUT,
    UPDATE,
    CORRECTION,
}
