using System.ComponentModel.DataAnnotations;
using Lodgr.Api.Common.Pagination;
using Lodgr.Api.Models;

namespace Lodgr.Api.Features.Contracts;

public class GetContractsRequest : PagedRequest
{
    public long? RoomId { get; init; }
    public long? TenantId { get; init; }
    public ContractStatus? Status { get; init; }
    public DepositStatus? DepositStatus { get; init; }
    public string? Keyword { get; init; }
}

public class GetContractInvoicesRequest : PagedRequest
{
    public InvoiceStatus? Status { get; init; }
}

public class CreateContractRequest
{
    [Required]
    public long RoomId { get; init; }

    [Required]
    public long TenantId { get; init; }

    [Required]
    public DateOnly StartDate { get; init; }

    public DateOnly? EndDate { get; init; }

    public DateOnly? ActualEndDate { get; init; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal MonthlyRent { get; init; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal DepositAmount { get; init; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal DepositBalance { get; init; }

    public DepositStatus? DepositStatus { get; init; }

    [Range(1, int.MaxValue)]
    public int CurrentOccupants { get; init; } = 1;

    [Range(1, 28)]
    public int BillingCycleDay { get; init; } = 1;

    public ContractStatus? Status { get; init; }

    public string? Note { get; init; }
}

public class UpdateContractRequest
{
    [Required]
    public long RoomId { get; init; }

    [Required]
    public long TenantId { get; init; }

    [Required]
    public DateOnly StartDate { get; init; }

    public DateOnly? EndDate { get; init; }

    public DateOnly? ActualEndDate { get; init; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal MonthlyRent { get; init; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal DepositAmount { get; init; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal DepositBalance { get; init; }

    public DepositStatus DepositStatus { get; init; }

    [Range(1, int.MaxValue)]
    public int CurrentOccupants { get; init; }

    [Range(1, 28)]
    public int BillingCycleDay { get; init; }

    public ContractStatus Status { get; init; }

    public string? Note { get; init; }
}

public class ContractListItemResponse
{
    public required long ContractId { get; init; }
    public required long RoomId { get; init; }
    public required long TenantId { get; init; }
    public required DateOnly StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public DateOnly? ActualEndDate { get; init; }
    public required decimal MonthlyRent { get; init; }
    public required decimal DepositAmount { get; init; }
    public required decimal DepositBalance { get; init; }
    public required string DepositStatus { get; init; }
    public required int CurrentOccupants { get; init; }
    public required int BillingCycleDay { get; init; }
    public required string Status { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}

public class ContractDetailResponse
{
    public required long ContractId { get; init; }
    public required long RoomId { get; init; }
    public required long TenantId { get; init; }
    public required DateOnly StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public DateOnly? ActualEndDate { get; init; }
    public required decimal MonthlyRent { get; init; }
    public required decimal DepositAmount { get; init; }
    public required decimal DepositBalance { get; init; }
    public required string DepositStatus { get; init; }
    public required int CurrentOccupants { get; init; }
    public required int BillingCycleDay { get; init; }
    public required string Status { get; init; }
    public string? Note { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}

public class ContractInvoiceListItemResponse
{
    public required long InvoiceId { get; init; }
    public required DateOnly InvoiceMonth { get; init; }
    public required DateOnly PeriodFrom { get; init; }
    public required DateOnly PeriodTo { get; init; }
    public required DateOnly IssueDate { get; init; }
    public DateOnly? DueDate { get; init; }
    public required decimal TotalAmount { get; init; }
    public required decimal PaidAmount { get; init; }
    public required string Status { get; init; }
}

public enum OperationErrorType
{
    Validation,
    NotFound,
    Conflict,
}

public class OperationError
{
    public required OperationErrorType Type { get; init; }
    public required string Message { get; init; }
}

public class OperationResult<T>
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }
    public OperationError? Error { get; init; }

    public static OperationResult<T> Success(T value)
    {
        return new OperationResult<T>
        {
            IsSuccess = true,
            Value = value,
        };
    }

    public static OperationResult<T> Failure(OperationErrorType type, string message)
    {
        return new OperationResult<T>
        {
            IsSuccess = false,
            Error = new OperationError
            {
                Type = type,
                Message = message,
            },
        };
    }
}
