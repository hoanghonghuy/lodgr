using Lodgr.Api.Common.Pagination;
using Lodgr.Api.Data;
using Lodgr.Api.Models;
using Microsoft.EntityFrameworkCore;
using DomainContract = Lodgr.Api.Models.Contract;

namespace Lodgr.Api.Features.Contracts;

public class ContractRepository(LodgrDbContext dbContext) : IContractRepository
{
    private static readonly DateOnly MaxDate = new(9999, 12, 31);

    public async Task<PagedResult<DomainContract>> GetPagedAsync(GetContractsRequest request, CancellationToken cancellationToken)
    {
        var query = dbContext.Contracts.AsNoTracking().AsQueryable();

        if (request.RoomId.HasValue)
        {
            query = query.Where(x => x.RoomId == request.RoomId.Value);
        }

        if (request.TenantId.HasValue)
        {
            query = query.Where(x => x.TenantId == request.TenantId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        if (request.DepositStatus.HasValue)
        {
            query = query.Where(x => x.DepositStatus == request.DepositStatus.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim();
            query = query.Where(x => x.Note != null && EF.Functions.ILike(x.Note, $"%{keyword}%"));
        }

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.StartDate)
            .ThenByDescending(x => x.ContractId)
            .Skip(request.Skip)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<DomainContract>.Create(items, request.PageNumber, request.PageSize, totalItems);
    }

    public Task<DomainContract?> GetByIdAsync(long contractId, CancellationToken cancellationToken)
    {
        return dbContext.Contracts.FirstOrDefaultAsync(x => x.ContractId == contractId, cancellationToken);
    }

    public Task AddAsync(DomainContract contract, CancellationToken cancellationToken)
    {
        return dbContext.Contracts.AddAsync(contract, cancellationToken).AsTask();
    }

    public Task<bool> RoomExistsAsync(long roomId, CancellationToken cancellationToken)
    {
        return dbContext.Rooms.AsNoTracking().AnyAsync(x => x.RoomId == roomId && !x.IsDeleted, cancellationToken);
    }

    public Task<bool> TenantExistsAsync(long tenantId, CancellationToken cancellationToken)
    {
        return dbContext.Tenants.AsNoTracking().AnyAsync(x => x.TenantId == tenantId, cancellationToken);
    }

    public Task<bool> ExistsAsync(long contractId, CancellationToken cancellationToken)
    {
        return dbContext.Contracts.AsNoTracking().AnyAsync(x => x.ContractId == contractId, cancellationToken);
    }

    public Task<bool> HasOverlappingContractAsync(
        long roomId,
        DateOnly startDate,
        DateOnly? endDate,
        DateOnly? actualEndDate,
        long? excludedContractId,
        CancellationToken cancellationToken)
    {
        var proposedEnd = actualEndDate ?? endDate ?? MaxDate;

        var query = dbContext.Contracts.AsNoTracking().Where(x => x.RoomId == roomId);

        if (excludedContractId.HasValue)
        {
            query = query.Where(x => x.ContractId != excludedContractId.Value);
        }

        query = query.Where(x =>
            x.StartDate <= proposedEnd
            && startDate <= (x.ActualEndDate ?? x.EndDate ?? MaxDate));

        return query.AnyAsync(cancellationToken);
    }

    public async Task<PagedResult<Invoice>> GetPagedInvoicesAsync(long contractId, GetContractInvoicesRequest request, CancellationToken cancellationToken)
    {
        var query = dbContext.Invoices.AsNoTracking().Where(x => x.ContractId == contractId);

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.IssueDate)
            .ThenByDescending(x => x.InvoiceId)
            .Skip(request.Skip)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<Invoice>.Create(items, request.PageNumber, request.PageSize, totalItems);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
