using Lodgr.Api.Common.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace Lodgr.Api.Features.Buildings;

[ApiController]
[Route("api/buildings")]
public class BuildingsController(IBuildingService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<BuildingListItemResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<BuildingListItemResponse>>> GetBuildings(
        [FromQuery] GetBuildingsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await service.GetPagedAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{buildingId:long}")]
    [ProducesResponseType(typeof(BuildingDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BuildingDetailResponse>> GetBuildingById(
        [FromRoute] long buildingId,
        [FromQuery] bool includeDeleted,
        CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(buildingId, includeDeleted, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("{buildingId:long}/rooms")]
    [ProducesResponseType(typeof(PagedResult<BuildingRoomItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResult<BuildingRoomItemResponse>>> GetBuildingRooms(
        [FromRoute] long buildingId,
        [FromQuery] GetBuildingRoomsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await service.GetPagedRoomsAsync(buildingId, request, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(BuildingDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BuildingDetailResponse>> CreateBuilding(
        [FromBody] CreateBuildingRequest request,
        CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetBuildingById), new { buildingId = result.BuildingId }, result);
    }

    [HttpPut("{buildingId:long}")]
    [ProducesResponseType(typeof(BuildingDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BuildingDetailResponse>> UpdateBuilding(
        [FromRoute] long buildingId,
        [FromBody] UpdateBuildingRequest request,
        CancellationToken cancellationToken)
    {
        var result = await service.UpdateAsync(buildingId, request, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{buildingId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBuilding(
        [FromRoute] long buildingId,
        CancellationToken cancellationToken)
    {
        var deleted = await service.SoftDeleteAsync(buildingId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
