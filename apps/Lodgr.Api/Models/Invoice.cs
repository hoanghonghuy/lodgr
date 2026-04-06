using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lodgr.Api.Models;

[Table("invoices")]
public class Invoice
{
    [Key]
    [Column("invoice_id")]
    public long InvoiceId { get; set; }

    [Column("contract_id")]
    public long ContractId { get; set; }

    [Column("invoice_month")]
    public DateOnly InvoiceMonth { get; set; }

    [Column("period_from")]
    public DateOnly PeriodFrom { get; set; }

    [Column("period_to")]
    public DateOnly PeriodTo { get; set; }

    [Column("issue_date")]
    public DateOnly IssueDate { get; set; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    [Column("total_amount", TypeName = "numeric(12,2)")]
    public decimal TotalAmount { get; set; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    [Column("paid_amount", TypeName = "numeric(12,2)")]
    public decimal PaidAmount { get; set; }

    [Column("due_date")]
    public DateOnly? DueDate { get; set; }

    [Column("status")]
    public InvoiceStatus Status { get; set; } = InvoiceStatus.DRAFT;

    [Column("sent_at")]
    public DateTime? SentAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

public enum InvoiceStatus
{
    DRAFT,
    SENT,
    PARTIALLY_PAID,
    PAID,
    OVERDUE,
    VOID,
}
