using Lodgr.Api.Common.Pagination;
using Lodgr.Api.Models;

namespace Lodgr.Api.Features.Buildings;

public class BuildingService(IBuildingRepository repository) : IBuildingService
{
    public async Task<PagedResult<BuildingListItemResponse>> GetPagedAsync(GetBuildingsRequest request, CancellationToken cancellationToken)
    {
        var result = await repository.GetPagedAsync(request, cancellationToken);

        var mapped = result.Items.Select(MapToListItem).ToList();
        return PagedResult<BuildingListItemResponse>.Create(
            mapped,
            result.PageNumber,
            result.PageSize,
            result.TotalItems);
    }

    public async Task<BuildingDetailResponse?> GetByIdAsync(long buildingId, bool includeDeleted, CancellationToken cancellationToken)
    {
        var building = await repository.GetByIdAsync(buildingId, includeDeleted, cancellationToken);
        return building is null ? null : MapToDetail(building);
    }

    public async Task<BuildingDetailResponse> CreateAsync(CreateBuildingRequest request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var building = new Building
        {
            OwnerId = request.OwnerId,
            Name = request.Name.Trim(),
            TotalFloors = request.TotalFloors,
            Description = request.Description,
            AddressDetail = request.AddressDetail.Trim(),
            WardCode = request.WardCode.Trim(),
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false,
        };

        await repository.AddAsync(building, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MapToDetail(building);
    }

    public async Task<BuildingDetailResponse?> UpdateAsync(long buildingId, UpdateBuildingRequest request, CancellationToken cancellationToken)
    {
        var building = await repository.GetByIdAsync(buildingId, includeDeleted: true, cancellationToken);
        if (building is null)
        {
            return null;
        }

        building.Name = request.Name.Trim();
        building.TotalFloors = request.TotalFloors;
        building.Description = request.Description;
        building.AddressDetail = request.AddressDetail.Trim();
        building.WardCode = request.WardCode.Trim();
        building.UpdatedAt = DateTime.UtcNow;

        await repository.SaveChangesAsync(cancellationToken);
        return MapToDetail(building);
    }

    public async Task<bool> SoftDeleteAsync(long buildingId, CancellationToken cancellationToken)
    {
        var building = await repository.GetByIdAsync(buildingId, includeDeleted: true, cancellationToken);
        if (building is null)
        {
            return false;
        }

        if (building.IsDeleted)
        {
            return true;
        }

        building.IsDeleted = true;
        building.UpdatedAt = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<PagedResult<BuildingRoomItemResponse>?> GetPagedRoomsAsync(long buildingId, GetBuildingRoomsRequest request, CancellationToken cancellationToken)
    {
        var exists = await repository.ExistsAsync(buildingId, includeDeleted: true, cancellationToken);
        if (!exists)
        {
            return null;
        }

        var result = await repository.GetPagedRoomsAsync(buildingId, request, cancellationToken);

        var mapped = result.Items.Select(MapToRoomItem).ToList();
        return PagedResult<BuildingRoomItemResponse>.Create(
            mapped,
            result.PageNumber,
            result.PageSize,
            result.TotalItems);
    }

    private static BuildingListItemResponse MapToListItem(Building building)
    {
        return new BuildingListItemResponse
        {
            BuildingId = building.BuildingId,
            OwnerId = building.OwnerId,
            Name = building.Name,
            TotalFloors = building.TotalFloors,
            AddressDetail = building.AddressDetail,
            WardCode = building.WardCode,
            IsDeleted = building.IsDeleted,
            CreatedAt = building.CreatedAt,
            UpdatedAt = building.UpdatedAt,
        };
    }

    private static BuildingDetailResponse MapToDetail(Building building)
    {
        return new BuildingDetailResponse
        {
            BuildingId = building.BuildingId,
            OwnerId = building.OwnerId,
            Name = building.Name,
            TotalFloors = building.TotalFloors,
            Description = building.Description,
            AddressDetail = building.AddressDetail,
            WardCode = building.WardCode,
            IsDeleted = building.IsDeleted,
            CreatedAt = building.CreatedAt,
            UpdatedAt = building.UpdatedAt,
        };
    }

    private static BuildingRoomItemResponse MapToRoomItem(Room room)
    {
        return new BuildingRoomItemResponse
        {
            RoomId = room.RoomId,
            RoomNumber = room.RoomNumber,
            Floor = room.Floor,
            BasePrice = room.BasePrice,
            Area = room.Area,
            MaxOccupants = room.MaxOccupants,
            OperationalStatus = room.OperationalStatus.ToString(),
            IsDeleted = room.IsDeleted,
        };
    }
}
