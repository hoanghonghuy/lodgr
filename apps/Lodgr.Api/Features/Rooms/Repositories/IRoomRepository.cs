using Lodgr.Api.Common.Pagination;
using Lodgr.Api.Models;

namespace Lodgr.Api.Features.Rooms;

public interface IRoomRepository
{
    Task<PagedResult<Room>> GetPagedAsync(GetRoomsRequest request, CancellationToken cancellationToken);
    Task<Room?> GetByIdAsync(long roomId, bool includeDeleted, CancellationToken cancellationToken);
    Task AddAsync(Room room, CancellationToken cancellationToken);
    Task<PagedResult<Contract>> GetPagedContractsAsync(long roomId, GetRoomContractsRequest request, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(long roomId, bool includeDeleted, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
