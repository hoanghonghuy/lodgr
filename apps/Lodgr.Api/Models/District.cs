using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lodgr.Api.Models;

[Table("districts")]
public class District
{
    [Key]
    [StringLength(20)]
    [Column("code")]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [Column("province_code")]
    public string ProvinceCode { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    [Column("full_name")]
    public string FullName { get; set; } = string.Empty;

    [StringLength(50)]
    [Column("administrative_unit")]
    public string? AdministrativeUnit { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
