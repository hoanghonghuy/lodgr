using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lodgr.Api.Models;

[Table("contracts")]
public class Contract
{
    [Key]
    [Column("contract_id")]
    public long ContractId { get; set; }

    [Column("room_id")]
    public long RoomId { get; set; }

    [Column("tenant_id")]
    public long TenantId { get; set; }

    [Column("start_date")]
    public DateOnly StartDate { get; set; }

    [Column("end_date")]
    public DateOnly? EndDate { get; set; }

    [Column("actual_end_date")]
    public DateOnly? ActualEndDate { get; set; }

    [Column("monthly_rent", TypeName = "numeric(12,2)")]
    public decimal MonthlyRent { get; set; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    [Column("deposit_amount", TypeName = "numeric(12,2)")]
    public decimal DepositAmount { get; set; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    [Column("deposit_balance", TypeName = "numeric(12,2)")]
    public decimal DepositBalance { get; set; }

    [Column("deposit_status")]
    public DepositStatus DepositStatus { get; set; } = DepositStatus.UNPAID;

    [Range(1, int.MaxValue)]
    [Column("current_occupants")]
    public int CurrentOccupants { get; set; } = 1;

    [Range(1, 28)]
    [Column("billing_cycle_day")]
    public int BillingCycleDay { get; set; } = 1;

    [Column("status")]
    public ContractStatus Status { get; set; } = ContractStatus.DRAFT;

    [Column("note")]
    public string? Note { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

public enum DepositStatus
{
    UNPAID,
    PARTIALLY_PAID,
    HELD,
    PARTIALLY_RETURNED,
    RETURNED,
    FORFEITED,
}

public enum ContractStatus
{
    DRAFT,
    ACTIVE,
    EXPIRED,
    TERMINATED,
}
