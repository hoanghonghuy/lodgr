using Lodgr.Api.Common.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace Lodgr.Api.Features.Rooms;

[ApiController]
[Route("api/rooms")]
public class RoomsController(IRoomService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<RoomListItemResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<RoomListItemResponse>>> GetRooms(
        [FromQuery] GetRoomsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await service.GetPagedAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{roomId:long}")]
    [ProducesResponseType(typeof(RoomDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoomDetailResponse>> GetRoomById(
        [FromRoute] long roomId,
        [FromQuery] bool includeDeleted,
        CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(roomId, includeDeleted, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("{roomId:long}/contracts")]
    [ProducesResponseType(typeof(PagedResult<RoomContractListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResult<RoomContractListItemResponse>>> GetRoomContracts(
        [FromRoute] long roomId,
        [FromQuery] GetRoomContractsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await service.GetPagedContractsAsync(roomId, request, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(RoomDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoomDetailResponse>> CreateRoom(
        [FromBody] CreateRoomRequest request,
        CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetRoomById), new { roomId = result.RoomId }, result);
    }

    [HttpPut("{roomId:long}")]
    [ProducesResponseType(typeof(RoomDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoomDetailResponse>> UpdateRoom(
        [FromRoute] long roomId,
        [FromBody] UpdateRoomRequest request,
        CancellationToken cancellationToken)
    {
        var result = await service.UpdateAsync(roomId, request, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{roomId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRoom(
        [FromRoute] long roomId,
        CancellationToken cancellationToken)
    {
        var deleted = await service.SoftDeleteAsync(roomId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
