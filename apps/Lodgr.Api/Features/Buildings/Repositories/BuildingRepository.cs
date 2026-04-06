using Lodgr.Api.Common.Pagination;
using Lodgr.Api.Data;
using Lodgr.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Lodgr.Api.Features.Buildings;

public class BuildingRepository(LodgrDbContext dbContext) : IBuildingRepository
{
    public async Task<PagedResult<Building>> GetPagedAsync(GetBuildingsRequest request, CancellationToken cancellationToken)
    {
        var query = dbContext.Buildings.AsNoTracking();

        if (!request.IncludeDeleted)
        {
            query = query.Where(x => !x.IsDeleted);
        }

        if (request.OwnerId.HasValue)
        {
            query = query.Where(x => x.OwnerId == request.OwnerId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim();
            query = query.Where(x =>
                EF.Functions.ILike(x.Name, $"%{keyword}%")
                || EF.Functions.ILike(x.AddressDetail, $"%{keyword}%")
                || EF.Functions.ILike(x.WardCode, $"%{keyword}%"));
        }

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.UpdatedAt)
            .ThenByDescending(x => x.BuildingId)
            .Skip(request.Skip)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<Building>.Create(items, request.PageNumber, request.PageSize, totalItems);
    }

    public Task<Building?> GetByIdAsync(long buildingId, bool includeDeleted, CancellationToken cancellationToken)
    {
        var query = dbContext.Buildings.AsQueryable();
        if (!includeDeleted)
        {
            query = query.Where(x => !x.IsDeleted);
        }

        return query.FirstOrDefaultAsync(x => x.BuildingId == buildingId, cancellationToken);
    }

    public Task<bool> ExistsAsync(long buildingId, bool includeDeleted, CancellationToken cancellationToken)
    {
        var query = dbContext.Buildings.AsNoTracking().AsQueryable();
        if (!includeDeleted)
        {
            query = query.Where(x => !x.IsDeleted);
        }

        return query.AnyAsync(x => x.BuildingId == buildingId, cancellationToken);
    }

    public Task AddAsync(Building building, CancellationToken cancellationToken)
    {
        return dbContext.Buildings.AddAsync(building, cancellationToken).AsTask();
    }

    public async Task<PagedResult<Room>> GetPagedRoomsAsync(long buildingId, GetBuildingRoomsRequest request, CancellationToken cancellationToken)
    {
        var query = dbContext.Rooms.AsNoTracking().Where(x => x.BuildingId == buildingId);

        if (!request.IncludeDeleted)
        {
            query = query.Where(x => !x.IsDeleted);
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

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
