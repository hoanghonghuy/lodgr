package org.lodgr.api.features.buildings;

import static org.lodgr.api.features.buildings.BuildingDtos.*;

import jakarta.validation.Valid;
import org.lodgr.api.pagination.PagedResult;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

@RestController
@Validated
@RequestMapping("/api/buildings")
public class BuildingsController {
    private final BuildingService buildingService;

    public BuildingsController(BuildingService buildingService) {
        this.buildingService = buildingService;
    }

    @GetMapping
    public ResponseEntity<PagedResult<BuildingListItemResponse>> getBuildings(
            @RequestParam(required = false) Integer pageNumber,
            @RequestParam(required = false) Integer pageSize,
            @RequestParam(required = false) Long ownerId,
            @RequestParam(required = false, defaultValue = "false") Boolean includeDeleted,
            @RequestParam(required = false) String keyword) {
        GetBuildingsQuery query = new GetBuildingsQuery(pageNumber, pageSize, ownerId, includeDeleted, keyword);
        return ResponseEntity.ok(buildingService.getPaged(query));
    }

    @GetMapping("/{buildingId}")
    public ResponseEntity<BuildingDetailResponse> getBuildingById(
            @PathVariable Long buildingId,
            @RequestParam(required = false, defaultValue = "false") boolean includeDeleted) {
        BuildingDetailResponse result = buildingService.getById(buildingId, includeDeleted);
        if (result == null) {
            return ResponseEntity.notFound().build();
        }

        return ResponseEntity.ok(result);
    }

    @GetMapping("/{buildingId}/rooms")
    public ResponseEntity<PagedResult<BuildingRoomItemResponse>> getBuildingRooms(
            @PathVariable Long buildingId,
            @RequestParam(required = false) Integer pageNumber,
            @RequestParam(required = false) Integer pageSize,
            @RequestParam(required = false, defaultValue = "false") Boolean includeDeleted) {
        GetBuildingRoomsQuery query = new GetBuildingRoomsQuery(pageNumber, pageSize, includeDeleted);
        PagedResult<BuildingRoomItemResponse> result = buildingService.getPagedRooms(buildingId, query);

        if (result == null) {
            return ResponseEntity.notFound().build();
        }

        return ResponseEntity.ok(result);
    }

    @PostMapping
    public ResponseEntity<BuildingDetailResponse> createBuilding(@Valid @RequestBody CreateBuildingRequest request) {
        BuildingDetailResponse result = buildingService.create(request);
        return ResponseEntity.status(HttpStatus.CREATED).body(result);
    }

    @PutMapping("/{buildingId}")
    public ResponseEntity<BuildingDetailResponse> updateBuilding(
            @PathVariable Long buildingId,
            @Valid @RequestBody UpdateBuildingRequest request) {
        BuildingDetailResponse result = buildingService.update(buildingId, request);
        if (result == null) {
            return ResponseEntity.notFound().build();
        }

        return ResponseEntity.ok(result);
    }

    @DeleteMapping("/{buildingId}")
    public ResponseEntity<Void> deleteBuilding(@PathVariable Long buildingId) {
        boolean deleted = buildingService.softDelete(buildingId);
        if (!deleted) {
            return ResponseEntity.notFound().build();
        }

        return ResponseEntity.noContent().build();
    }
}
