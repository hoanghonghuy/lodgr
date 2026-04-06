using Lodgr.Api.Common.Pagination;

namespace Lodgr.Api.Features.Buildings;

public interface IBuildingService
{
    Task<PagedResult<BuildingListItemResponse>> GetPagedAsync(GetBuildingsRequest request, CancellationToken cancellationToken);
    Task<BuildingDetailResponse?> GetByIdAsync(long buildingId, bool includeDeleted, CancellationToken cancellationToken);
    Task<BuildingDetailResponse> CreateAsync(CreateBuildingRequest request, CancellationToken cancellationToken);
    Task<BuildingDetailResponse?> UpdateAsync(long buildingId, UpdateBuildingRequest request, CancellationToken cancellationToken);
    Task<bool> SoftDeleteAsync(long buildingId, CancellationToken cancellationToken);
    Task<PagedResult<BuildingRoomItemResponse>?> GetPagedRoomsAsync(long buildingId, GetBuildingRoomsRequest request, CancellationToken cancellationToken);
}
