using Lodgr.Api.Common.Pagination;
using Lodgr.Api.Models;
using DomainContract = Lodgr.Api.Models.Contract;

namespace Lodgr.Api.Features.Contracts;

public class ContractService(IContractRepository repository) : IContractService
{
    public async Task<PagedResult<ContractListItemResponse>> GetPagedAsync(GetContractsRequest request, CancellationToken cancellationToken)
    {
        var result = await repository.GetPagedAsync(request, cancellationToken);
        var mapped = result.Items.Select(MapToListItem).ToList();

        return PagedResult<ContractListItemResponse>.Create(
            mapped,
            result.PageNumber,
            result.PageSize,
            result.TotalItems);
    }

    public async Task<ContractDetailResponse?> GetByIdAsync(long contractId, CancellationToken cancellationToken)
    {
        var contract = await repository.GetByIdAsync(contractId, cancellationToken);
        return contract is null ? null : MapToDetail(contract);
    }

    public async Task<OperationResult<ContractDetailResponse>> CreateAsync(CreateContractRequest request, CancellationToken cancellationToken)
    {
        var validationError = ValidateDateRange(request.StartDate, request.EndDate, request.ActualEndDate);
        if (validationError is not null)
        {
            return OperationResult<ContractDetailResponse>.Failure(OperationErrorType.Validation, validationError);
        }

        if (!await repository.RoomExistsAsync(request.RoomId, cancellationToken))
        {
            return OperationResult<ContractDetailResponse>.Failure(OperationErrorType.NotFound, "Room not found.");
        }

        if (!await repository.TenantExistsAsync(request.TenantId, cancellationToken))
        {
            return OperationResult<ContractDetailResponse>.Failure(OperationErrorType.NotFound, "Tenant not found.");
        }

        var hasOverlap = await repository.HasOverlappingContractAsync(
            request.RoomId,
            request.StartDate,
            request.EndDate,
            request.ActualEndDate,
            excludedContractId: null,
            cancellationToken);

        if (hasOverlap)
        {
            return OperationResult<ContractDetailResponse>.Failure(OperationErrorType.Conflict, "Contract date range overlaps with an existing contract in the same room.");
        }

        var now = DateTime.UtcNow;
        var contract = new DomainContract
        {
            RoomId = request.RoomId,
            TenantId = request.TenantId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            ActualEndDate = request.ActualEndDate,
            MonthlyRent = request.MonthlyRent,
            DepositAmount = request.DepositAmount,
            DepositBalance = request.DepositBalance,
            DepositStatus = request.DepositStatus ?? DepositStatus.UNPAID,
            CurrentOccupants = request.CurrentOccupants,
            BillingCycleDay = request.BillingCycleDay,
            Status = request.Status ?? ContractStatus.DRAFT,
            Note = request.Note,
            CreatedAt = now,
            UpdatedAt = now,
        };

        await repository.AddAsync(contract, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return OperationResult<ContractDetailResponse>.Success(MapToDetail(contract));
    }

    public async Task<OperationResult<ContractDetailResponse>> UpdateAsync(long contractId, UpdateContractRequest request, CancellationToken cancellationToken)
    {
        var contract = await repository.GetByIdAsync(contractId, cancellationToken);
        if (contract is null)
        {
            return OperationResult<ContractDetailResponse>.Failure(OperationErrorType.NotFound, "Contract not found.");
        }

        var validationError = ValidateDateRange(request.StartDate, request.EndDate, request.ActualEndDate);
        if (validationError is not null)
        {
            return OperationResult<ContractDetailResponse>.Failure(OperationErrorType.Validation, validationError);
        }

        if (!await repository.RoomExistsAsync(request.RoomId, cancellationToken))
        {
            return OperationResult<ContractDetailResponse>.Failure(OperationErrorType.NotFound, "Room not found.");
        }

        if (!await repository.TenantExistsAsync(request.TenantId, cancellationToken))
        {
            return OperationResult<ContractDetailResponse>.Failure(OperationErrorType.NotFound, "Tenant not found.");
        }

        var hasOverlap = await repository.HasOverlappingContractAsync(
            request.RoomId,
            request.StartDate,
            request.EndDate,
            request.ActualEndDate,
            excludedContractId: contractId,
            cancellationToken);

        if (hasOverlap)
        {
            return OperationResult<ContractDetailResponse>.Failure(OperationErrorType.Conflict, "Contract date range overlaps with an existing contract in the same room.");
        }

        contract.RoomId = request.RoomId;
        contract.TenantId = request.TenantId;
        contract.StartDate = request.StartDate;
        contract.EndDate = request.EndDate;
        contract.ActualEndDate = request.ActualEndDate;
        contract.MonthlyRent = request.MonthlyRent;
        contract.DepositAmount = request.DepositAmount;
        contract.DepositBalance = request.DepositBalance;
        contract.DepositStatus = request.DepositStatus;
        contract.CurrentOccupants = request.CurrentOccupants;
        contract.BillingCycleDay = request.BillingCycleDay;
        contract.Status = request.Status;
        contract.Note = request.Note;
        contract.UpdatedAt = DateTime.UtcNow;

        await repository.SaveChangesAsync(cancellationToken);
        return OperationResult<ContractDetailResponse>.Success(MapToDetail(contract));
    }

    public async Task<bool> TerminateAsync(long contractId, CancellationToken cancellationToken)
    {
        var contract = await repository.GetByIdAsync(contractId, cancellationToken);
        if (contract is null)
        {
            return false;
        }

        if (contract.Status == ContractStatus.TERMINATED)
        {
            return true;
        }

        contract.Status = ContractStatus.TERMINATED;
        contract.ActualEndDate ??= DateOnly.FromDateTime(DateTime.UtcNow);
        contract.UpdatedAt = DateTime.UtcNow;

        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<PagedResult<ContractInvoiceListItemResponse>?> GetPagedInvoicesAsync(long contractId, GetContractInvoicesRequest request, CancellationToken cancellationToken)
    {
        var exists = await repository.ExistsAsync(contractId, cancellationToken);
        if (!exists)
        {
            return null;
        }

        var result = await repository.GetPagedInvoicesAsync(contractId, request, cancellationToken);
        var mapped = result.Items.Select(MapToInvoiceListItem).ToList();

        return PagedResult<ContractInvoiceListItemResponse>.Create(
            mapped,
            result.PageNumber,
            result.PageSize,
            result.TotalItems);
    }

    private static string? ValidateDateRange(DateOnly startDate, DateOnly? endDate, DateOnly? actualEndDate)
    {
        if (endDate.HasValue && endDate.Value < startDate)
        {
            return "EndDate must be greater than or equal to StartDate.";
        }

        if (actualEndDate.HasValue && actualEndDate.Value < startDate)
        {
            return "ActualEndDate must be greater than or equal to StartDate.";
        }

        return null;
    }

    private static ContractListItemResponse MapToListItem(DomainContract contract)
    {
        return new ContractListItemResponse
        {
            ContractId = contract.ContractId,
            RoomId = contract.RoomId,
            TenantId = contract.TenantId,
            StartDate = contract.StartDate,
            EndDate = contract.EndDate,
            ActualEndDate = contract.ActualEndDate,
            MonthlyRent = contract.MonthlyRent,
            DepositAmount = contract.DepositAmount,
            DepositBalance = contract.DepositBalance,
            DepositStatus = contract.DepositStatus.ToString(),
            CurrentOccupants = contract.CurrentOccupants,
            BillingCycleDay = contract.BillingCycleDay,
            Status = contract.Status.ToString(),
            CreatedAt = contract.CreatedAt,
            UpdatedAt = contract.UpdatedAt,
        };
    }

    private static ContractDetailResponse MapToDetail(DomainContract contract)
    {
        return new ContractDetailResponse
        {
            ContractId = contract.ContractId,
            RoomId = contract.RoomId,
            TenantId = contract.TenantId,
            StartDate = contract.StartDate,
            EndDate = contract.EndDate,
            ActualEndDate = contract.ActualEndDate,
            MonthlyRent = contract.MonthlyRent,
            DepositAmount = contract.DepositAmount,
            DepositBalance = contract.DepositBalance,
            DepositStatus = contract.DepositStatus.ToString(),
            CurrentOccupants = contract.CurrentOccupants,
            BillingCycleDay = contract.BillingCycleDay,
            Status = contract.Status.ToString(),
            Note = contract.Note,
            CreatedAt = contract.CreatedAt,
            UpdatedAt = contract.UpdatedAt,
        };
    }

    private static ContractInvoiceListItemResponse MapToInvoiceListItem(Invoice invoice)
    {
        return new ContractInvoiceListItemResponse
        {
            InvoiceId = invoice.InvoiceId,
            InvoiceMonth = invoice.InvoiceMonth,
            PeriodFrom = invoice.PeriodFrom,
            PeriodTo = invoice.PeriodTo,
            IssueDate = invoice.IssueDate,
            DueDate = invoice.DueDate,
            TotalAmount = invoice.TotalAmount,
            PaidAmount = invoice.PaidAmount,
            Status = invoice.Status.ToString(),
        };
    }
}
