using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lodgr.Api.Models;

[Table("payments")]
public class Payment
{
    [Key]
    [Column("payment_id")]
    public long PaymentId { get; set; }

    [Column("invoice_id")]
    public long InvoiceId { get; set; }

    [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
    [Column("amount", TypeName = "numeric(12,2)")]
    public decimal Amount { get; set; }

    [Column("payment_date")]
    public DateOnly PaymentDate { get; set; }

    [Column("payment_method")]
    public PaymentMethod PaymentMethod { get; set; }

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

public enum PaymentMethod
{
    CASH,
    BANK_TRANSFER,
    OTHER,
}
