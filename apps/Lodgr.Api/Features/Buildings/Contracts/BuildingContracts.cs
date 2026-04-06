using System.ComponentModel.DataAnnotations;
using Lodgr.Api.Common.Pagination;

namespace Lodgr.Api.Features.Buildings;

public class GetBuildingsRequest : PagedRequest
{
    public long? OwnerId { get; init; }
    public bool IncludeDeleted { get; init; }
    public string? Keyword { get; init; }
}

public class GetBuildingRoomsRequest : PagedRequest
{
    public bool IncludeDeleted { get; init; }
}

public class CreateBuildingRequest
{
    [Required]
    public long OwnerId { get; init; }

    [Required]
    [StringLength(100)]
    public string Name { get; init; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int TotalFloors { get; init; } = 1;

    public string? Description { get; init; }

    [Required]
    [StringLength(255)]
    public string AddressDetail { get; init; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string WardCode { get; init; } = string.Empty;
}

public class UpdateBuildingRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; init; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int TotalFloors { get; init; }

    public string? Description { get; init; }

    [Required]
    [StringLength(255)]
    public string AddressDetail { get; init; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string WardCode { get; init; } = string.Empty;
}

public class BuildingListItemResponse
{
    public required long BuildingId { get; init; }
    public required long OwnerId { get; init; }
    public required string Name { get; init; }
    public required int TotalFloors { get; init; }
    public required string AddressDetail { get; init; }
    public required string WardCode { get; init; }
    public required bool IsDeleted { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}

public class BuildingDetailResponse
{
    public required long BuildingId { get; init; }
    public required long OwnerId { get; init; }
    public required string Name { get; init; }
    public required int TotalFloors { get; init; }
    public string? Description { get; init; }
    public required string AddressDetail { get; init; }
    public required string WardCode { get; init; }
    public required bool IsDeleted { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}

public class BuildingRoomItemResponse
{
    public required long RoomId { get; init; }
    public required string RoomNumber { get; init; }
    public int? Floor { get; init; }
    public decimal BasePrice { get; init; }
    public decimal? Area { get; init; }
    public int MaxOccupants { get; init; }
    public required string OperationalStatus { get; init; }
    public bool IsDeleted { get; init; }
}
