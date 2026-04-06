using System.ComponentModel.DataAnnotations;
using Lodgr.Api.Common.Pagination;

namespace Lodgr.Api.Features.Rooms;

public class GetRoomsRequest : PagedRequest
{
    public long? BuildingId { get; init; }
    public bool IncludeDeleted { get; init; }
    public string? Keyword { get; init; }
    public string? OperationalStatus { get; init; }
}

public class GetRoomContractsRequest : PagedRequest
{
    public bool IncludeInactive { get; init; } = true;
}

public class CreateRoomRequest
{
    [Required]
    public long BuildingId { get; init; }

    [Required]
    [StringLength(20)]
    public string RoomNumber { get; init; } = string.Empty;

    public int? Floor { get; init; }

    public decimal? Area { get; init; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal BasePrice { get; init; }

    public string? RoomType { get; init; }

    [Range(1, int.MaxValue)]
    public int MaxOccupants { get; init; } = 1;

    public string? OperationalStatus { get; init; }

    public string? Description { get; init; }
}

public class UpdateRoomRequest
{
    [Required]
    [StringLength(20)]
    public string RoomNumber { get; init; } = string.Empty;

    public int? Floor { get; init; }

    public decimal? Area { get; init; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal BasePrice { get; init; }

    public string? RoomType { get; init; }

    [Range(1, int.MaxValue)]
    public int MaxOccupants { get; init; }

    public string? OperationalStatus { get; init; }

    public string? Description { get; init; }
}

public class RoomListItemResponse
{
    public required long RoomId { get; init; }
    public required long BuildingId { get; init; }
    public required string RoomNumber { get; init; }
    public int? Floor { get; init; }
    public decimal? Area { get; init; }
    public decimal BasePrice { get; init; }
    public string? RoomType { get; init; }
    public int MaxOccupants { get; init; }
    public required string OperationalStatus { get; init; }
    public bool IsDeleted { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public class RoomDetailResponse
{
    public required long RoomId { get; init; }
    public required long BuildingId { get; init; }
    public required string RoomNumber { get; init; }
    public int? Floor { get; init; }
    public decimal? Area { get; init; }
    public decimal BasePrice { get; init; }
    public string? RoomType { get; init; }
    public int MaxOccupants { get; init; }
    public required string OperationalStatus { get; init; }
    public string? Description { get; init; }
    public bool IsDeleted { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public class RoomContractListItemResponse
{
    public required long ContractId { get; init; }
    public required long TenantId { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public DateOnly? ActualEndDate { get; init; }
    public decimal MonthlyRent { get; init; }
    public int CurrentOccupants { get; init; }
    public required string Status { get; init; }
    public required string DepositStatus { get; init; }
}
