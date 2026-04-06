using Lodgr.Api.Common.Pagination;
using Lodgr.Api.Data;
using Lodgr.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Lodgr.Api.Features.Rooms;

public class RoomRepository(LodgrDbContext dbContext) : IRoomRepository
{
    public async Task<PagedResult<Room>> GetPagedAsync(GetRoomsRequest request, CancellationToken cancellationToken)
    {
        var query = dbContext.Rooms.AsNoTracking().AsQueryable();

        if (!request.IncludeDeleted)
        {
            query = query.Where(x => !x.IsDeleted);
        }

        if (request.BuildingId.HasValue)
        {
            query = query.Where(x => x.BuildingId == request.BuildingId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim();
            query = query.Where(x =>
                EF.Functions.ILike(x.RoomNumber, $"%{keyword}%")
                || (x.Description != null && EF.Functions.ILike(x.Description, $"%{keyword}%")));
        }

        if (!string.IsNullOrWhiteSpace(request.OperationalStatus)
            && Enum.TryParse<OperationalStatus>(request.OperationalStatus, true, out var operationalStatus))
        {
            query = query.Where(x => x.OperationalStatus == operationalStatus);
        }

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.Floor)
            .ThenBy(x => x.RoomNumber)
            .Skip(request.Skip)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<Room>.Create(items, request.PageNumber, request.PageSize, totalItems);
    }

    public Task<Room?> GetByIdAsync(long roomId, bool includeDeleted, CancellationToken cancellationToken)
    {
        var query = dbContext.Rooms.AsQueryable();
        if (!includeDeleted)
        {
            query = query.Where(x => !x.IsDeleted);
        }

        return query.FirstOrDefaultAsync(x => x.RoomId == roomId, cancellationToken);
    }

    public Task AddAsync(Room room, CancellationToken cancellationToken)
    {
        return dbContext.Rooms.AddAsync(room, cancellationToken).AsTask();
    }

    public Task<bool> ExistsAsync(long roomId, bool includeDeleted, CancellationToken cancellationToken)
    {
        var query = dbContext.Rooms.AsNoTracking().AsQueryable();
        if (!includeDeleted)
        {
            query = query.Where(x => !x.IsDeleted);
        }

        return query.AnyAsync(x => x.RoomId == roomId, cancellationToken);
    }

    public async Task<PagedResult<Contract>> GetPagedContractsAsync(long roomId, GetRoomContractsRequest request, CancellationToken cancellationToken)
    {
        var query = dbContext.Contracts.AsNoTracking().Where(x => x.RoomId == roomId);

        if (!request.IncludeInactive)
        {
            query = query.Where(x => x.Status == ContractStatus.ACTIVE);
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.StartDate)
            .ThenByDescending(x => x.ContractId)
            .Skip(request.Skip)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<Contract>.Create(items, request.PageNumber, request.PageSize, totalItems);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
