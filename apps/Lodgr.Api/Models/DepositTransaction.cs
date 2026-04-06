using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lodgr.Api.Models;

[Table("deposit_transactions")]
public class DepositTransaction
{
    [Key]
    [Column("deposit_txn_id")]
    public long DepositTxnId { get; set; }

    [Column("contract_id")]
    public long ContractId { get; set; }

    [Column("txn_type")]
    public TxnType TxnType { get; set; }

    [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
    [Column("amount", TypeName = "numeric(12,2)")]
    public decimal Amount { get; set; }

    [Column("transaction_date")]
    public DateOnly TransactionDate { get; set; }

    [Column("payment_method")]
    public PaymentMethod? PaymentMethod { get; set; }

    [StringLength(100)]
    [Column("reference_code")]
    public string? ReferenceCode { get; set; }

    [Column("recorded_by")]
    public long? RecordedBy { get; set; }

    [Column("note")]
    public string? Note { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}

public enum TxnType
{
    COLLECT,
    REFUND,
    DEDUCTION,
    ADJUSTMENT,
}
