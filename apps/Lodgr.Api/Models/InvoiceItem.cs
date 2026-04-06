using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lodgr.Api.Models;

[Table("invoice_items")]
public class InvoiceItem
{
    [Key]
    [Column("item_id")]
    public long ItemId { get; set; }

    [Column("invoice_id")]
    public long InvoiceId { get; set; }

    [Column("reading_id")]
    public long? ReadingId { get; set; }

    [Column("item_type")]
    public ItemType ItemType { get; set; }

    [StringLength(255)]
    [Column("description")]
    public string? Description { get; set; }

    [Column("quantity", TypeName = "numeric(10,2)")]
    public decimal? Quantity { get; set; }

    [Column("unit_price", TypeName = "numeric(12,2)")]
    public decimal? UnitPrice { get; set; }

    [Column("amount", TypeName = "numeric(12,2)")]
    public decimal Amount { get; set; }
}

public enum ItemType
{
    RENT,
    ELECTRICITY,
    WATER,
    INTERNET,
    GARBAGE,
    PARKING,
    DEPOSIT_CHARGE,
    DEPOSIT_REFUND,
    OTHER,
}
