using Lodgr.Api.Common.Pagination;
using DomainContract = Lodgr.Api.Models.Contract;

namespace Lodgr.Api.Features.Contracts;

public interface IContractRepository
{
    Task<PagedResult<DomainContract>> GetPagedAsync(GetContractsRequest request, CancellationToken cancellationToken);
    Task<DomainContract?> GetByIdAsync(long contractId, CancellationToken cancellationToken);
    Task AddAsync(DomainContract contract, CancellationToken cancellationToken);
    Task<bool> RoomExistsAsync(long roomId, CancellationToken cancellationToken);
    Task<bool> TenantExistsAsync(long tenantId, CancellationToken cancellationToken);
    Task<bool> HasOverlappingContractAsync(long roomId, DateOnly startDate, DateOnly? endDate, DateOnly? actualEndDate, long? excludedContractId, CancellationToken cancellationToken);
    Task<PagedResult<Lodgr.Api.Models.Invoice>> GetPagedInvoicesAsync(long contractId, GetContractInvoicesRequest request, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(long contractId, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
