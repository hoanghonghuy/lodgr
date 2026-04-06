using Lodgr.Api.Common.Pagination;
using Lodgr.Api.Models;

namespace Lodgr.Api.Features.Buildings;

public interface IBuildingRepository
{
    Task<PagedResult<Building>> GetPagedAsync(GetBuildingsRequest request, CancellationToken cancellationToken);
    Task<Building?> GetByIdAsync(long buildingId, bool includeDeleted, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(long buildingId, bool includeDeleted, CancellationToken cancellationToken);
    Task AddAsync(Building building, CancellationToken cancellationToken);
    Task<PagedResult<Room>> GetPagedRoomsAsync(long buildingId, GetBuildingRoomsRequest request, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
