using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lodgr.Api.Models;

[Table("notifications")]
public class Notification
{
    [Key]
    [Column("notification_id")]
    public long NotificationId { get; set; }

    [Column("contract_id")]
    public long ContractId { get; set; }

    [Column("invoice_id")]
    public long? InvoiceId { get; set; }

    [Column("type")]
    public NotificationType Type { get; set; }

    [Required]
    [StringLength(255)]
    [Column("recipient_email")]
    public string RecipientEmail { get; set; } = string.Empty;

    [StringLength(255)]
    [Column("subject")]
    public string? Subject { get; set; }

    [Column("status")]
    public NotificationStatus Status { get; set; } = NotificationStatus.PENDING;

    [StringLength(100)]
    [Column("taskgate_job_id")]
    public string? TaskgateJobId { get; set; }

    [Column("sent_at")]
    public DateTime? SentAt { get; set; }

    [Column("error_message")]
    public string? ErrorMessage { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}

public enum NotificationType
{
    INVOICE,
    PAYMENT_REMINDER,
    CONTRACT_EXPIRY,
    PAYMENT_CONFIRMED,
    WELCOME,
}

public enum NotificationStatus
{
    PENDING,
    SENT,
    FAILED,
}
