using Lodgr.Api.Common.Pagination;

namespace Lodgr.Api.Features.Rooms;

public interface IRoomService
{
    Task<PagedResult<RoomListItemResponse>> GetPagedAsync(GetRoomsRequest request, CancellationToken cancellationToken);
    Task<RoomDetailResponse?> GetByIdAsync(long roomId, bool includeDeleted, CancellationToken cancellationToken);
    Task<RoomDetailResponse> CreateAsync(CreateRoomRequest request, CancellationToken cancellationToken);
    Task<RoomDetailResponse?> UpdateAsync(long roomId, UpdateRoomRequest request, CancellationToken cancellationToken);
    Task<bool> SoftDeleteAsync(long roomId, CancellationToken cancellationToken);
    Task<PagedResult<RoomContractListItemResponse>?> GetPagedContractsAsync(long roomId, GetRoomContractsRequest request, CancellationToken cancellationToken);
}
