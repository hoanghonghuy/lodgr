using Lodgr.Api.Common.Pagination;
using Lodgr.Api.Models;

namespace Lodgr.Api.Features.Rooms;

public class RoomService(IRoomRepository repository) : IRoomService
{
    public async Task<PagedResult<RoomListItemResponse>> GetPagedAsync(GetRoomsRequest request, CancellationToken cancellationToken)
    {
        var result = await repository.GetPagedAsync(request, cancellationToken);
        var mapped = result.Items.Select(MapToListItem).ToList();

        return PagedResult<RoomListItemResponse>.Create(
            mapped,
            result.PageNumber,
            result.PageSize,
            result.TotalItems);
    }

    public async Task<RoomDetailResponse?> GetByIdAsync(long roomId, bool includeDeleted, CancellationToken cancellationToken)
    {
        var room = await repository.GetByIdAsync(roomId, includeDeleted, cancellationToken);
        return room is null ? null : MapToDetail(room);
    }

    public async Task<RoomDetailResponse> CreateAsync(CreateRoomRequest request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var room = new Room
        {
            BuildingId = request.BuildingId,
            RoomNumber = request.RoomNumber.Trim(),
            Floor = request.Floor,
            Area = request.Area,
            BasePrice = request.BasePrice,
            RoomType = ParseRoomType(request.RoomType),
            MaxOccupants = request.MaxOccupants,
            OperationalStatus = ParseOperationalStatus(request.OperationalStatus),
            Description = request.Description,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false,
        };

        await repository.AddAsync(room, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return MapToDetail(room);
    }

    public async Task<RoomDetailResponse?> UpdateAsync(long roomId, UpdateRoomRequest request, CancellationToken cancellationToken)
    {
        var room = await repository.GetByIdAsync(roomId, includeDeleted: true, cancellationToken);
        if (room is null)
        {
            return null;
        }

        room.RoomNumber = request.RoomNumber.Trim();
        room.Floor = request.Floor;
        room.Area = request.Area;
        room.BasePrice = request.BasePrice;
        room.RoomType = ParseRoomType(request.RoomType);
        room.MaxOccupants = request.MaxOccupants;
        room.OperationalStatus = ParseOperationalStatus(request.OperationalStatus);
        room.Description = request.Description;
        room.UpdatedAt = DateTime.UtcNow;

        await repository.SaveChangesAsync(cancellationToken);
        return MapToDetail(room);
    }

    public async Task<bool> SoftDeleteAsync(long roomId, CancellationToken cancellationToken)
    {
        var room = await repository.GetByIdAsync(roomId, includeDeleted: true, cancellationToken);
        if (room is null)
        {
            return false;
        }

        if (room.IsDeleted)
        {
            return true;
        }

        room.IsDeleted = true;
        room.UpdatedAt = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<PagedResult<RoomContractListItemResponse>?> GetPagedContractsAsync(long roomId, GetRoomContractsRequest request, CancellationToken cancellationToken)
    {
        var exists = await repository.ExistsAsync(roomId, includeDeleted: true, cancellationToken);
        if (!exists)
        {
            return null;
        }

        var result = await repository.GetPagedContractsAsync(roomId, request, cancellationToken);
        var mapped = result.Items.Select(MapToContractListItem).ToList();

        return PagedResult<RoomContractListItemResponse>.Create(
            mapped,
            result.PageNumber,
            result.PageSize,
            result.TotalItems);
    }

    private static RoomType? ParseRoomType(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return Enum.TryParse<RoomType>(value.Trim(), true, out var roomType)
            ? roomType
            : null;
    }

    private static OperationalStatus ParseOperationalStatus(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return OperationalStatus.ACTIVE;
        }

        return Enum.TryParse<OperationalStatus>(value.Trim(), true, out var status)
            ? status
            : OperationalStatus.ACTIVE;
    }

    private static RoomListItemResponse MapToListItem(Room room)
    {
        return new RoomListItemResponse
        {
            RoomId = room.RoomId,
            BuildingId = room.BuildingId,
            RoomNumber = room.RoomNumber,
            Floor = room.Floor,
            Area = room.Area,
            BasePrice = room.BasePrice,
            RoomType = room.RoomType?.ToString(),
            MaxOccupants = room.MaxOccupants,
            OperationalStatus = room.OperationalStatus.ToString(),
            IsDeleted = room.IsDeleted,
            CreatedAt = room.CreatedAt,
            UpdatedAt = room.UpdatedAt,
        };
    }

    private static RoomDetailResponse MapToDetail(Room room)
    {
        return new RoomDetailResponse
        {
            RoomId = room.RoomId,
            BuildingId = room.BuildingId,
            RoomNumber = room.RoomNumber,
            Floor = room.Floor,
            Area = room.Area,
            BasePrice = room.BasePrice,
            RoomType = room.RoomType?.ToString(),
            MaxOccupants = room.MaxOccupants,
            OperationalStatus = room.OperationalStatus.ToString(),
            Description = room.Description,
            IsDeleted = room.IsDeleted,
            CreatedAt = room.CreatedAt,
            UpdatedAt = room.UpdatedAt,
        };
    }

    private static RoomContractListItemResponse MapToContractListItem(Contract contract)
    {
        return new RoomContractListItemResponse
        {
            ContractId = contract.ContractId,
            TenantId = contract.TenantId,
            StartDate = contract.StartDate,
            EndDate = contract.EndDate,
            ActualEndDate = contract.ActualEndDate,
            MonthlyRent = contract.MonthlyRent,
            CurrentOccupants = contract.CurrentOccupants,
            Status = contract.Status.ToString(),
            DepositStatus = contract.DepositStatus.ToString(),
        };
    }
}
