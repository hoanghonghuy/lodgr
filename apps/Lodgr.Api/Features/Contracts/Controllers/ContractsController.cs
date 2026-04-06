using Lodgr.Api.Common.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace Lodgr.Api.Features.Contracts;

[ApiController]
[Route("api/contracts")]
public class ContractsController(IContractService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ContractListItemResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ContractListItemResponse>>> GetContracts(
        [FromQuery] GetContractsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await service.GetPagedAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{contractId:long}")]
    [ProducesResponseType(typeof(ContractDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContractDetailResponse>> GetContractById(
        [FromRoute] long contractId,
        CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(contractId, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("{contractId:long}/invoices")]
    [ProducesResponseType(typeof(PagedResult<ContractInvoiceListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResult<ContractInvoiceListItemResponse>>> GetContractInvoices(
        [FromRoute] long contractId,
        [FromQuery] GetContractInvoicesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await service.GetPagedInvoicesAsync(contractId, request, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ContractDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ContractDetailResponse>> CreateContract(
        [FromBody] CreateContractRequest request,
        CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(request, cancellationToken);
        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(nameof(GetContractById), new { contractId = result.Value.ContractId }, result.Value);
        }

        return HandleOperationError(result.Error);
    }

    [HttpPut("{contractId:long}")]
    [ProducesResponseType(typeof(ContractDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ContractDetailResponse>> UpdateContract(
        [FromRoute] long contractId,
        [FromBody] UpdateContractRequest request,
        CancellationToken cancellationToken)
    {
        var result = await service.UpdateAsync(contractId, request, cancellationToken);
        if (result.IsSuccess && result.Value is not null)
        {
            return Ok(result.Value);
        }

        return HandleOperationError(result.Error);
    }

    [HttpDelete("{contractId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TerminateContract(
        [FromRoute] long contractId,
        CancellationToken cancellationToken)
    {
        var terminated = await service.TerminateAsync(contractId, cancellationToken);
        return terminated ? NoContent() : NotFound();
    }

    private ActionResult HandleOperationError(OperationError? error)
    {
        if (error is null)
        {
            return Problem(title: "Unknown error", statusCode: StatusCodes.Status500InternalServerError);
        }

        return error.Type switch
        {
            OperationErrorType.Validation => BadRequest(new { error = error.Message }),
            OperationErrorType.NotFound => NotFound(new { error = error.Message }),
            OperationErrorType.Conflict => Conflict(new { error = error.Message }),
            _ => Problem(title: error.Message, statusCode: StatusCodes.Status500InternalServerError),
        };
    }
}
