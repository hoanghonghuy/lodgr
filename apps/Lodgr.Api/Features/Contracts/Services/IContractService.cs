using Lodgr.Api.Common.Pagination;

namespace Lodgr.Api.Features.Contracts;

public interface IContractService
{
    Task<PagedResult<ContractListItemResponse>> GetPagedAsync(GetContractsRequest request, CancellationToken cancellationToken);
    Task<ContractDetailResponse?> GetByIdAsync(long contractId, CancellationToken cancellationToken);
    Task<OperationResult<ContractDetailResponse>> CreateAsync(CreateContractRequest request, CancellationToken cancellationToken);
    Task<OperationResult<ContractDetailResponse>> UpdateAsync(long contractId, UpdateContractRequest request, CancellationToken cancellationToken);
    Task<bool> TerminateAsync(long contractId, CancellationToken cancellationToken);
    Task<PagedResult<ContractInvoiceListItemResponse>?> GetPagedInvoicesAsync(long contractId, GetContractInvoicesRequest request, CancellationToken cancellationToken);
}
