package io.github.hoanghonghuy.lodgr.features.building.controller;

import io.github.hoanghonghuy.lodgr.features.building.dto.BuildingFilter;
import io.github.hoanghonghuy.lodgr.features.building.dto.BuildingResponse;
import io.github.hoanghonghuy.lodgr.features.building.dto.CreateBuildingRequest;
import io.github.hoanghonghuy.lodgr.features.building.dto.UpdateBuildingRequest;
import io.github.hoanghonghuy.lodgr.features.building.service.BuildingService;
import io.github.hoanghonghuy.lodgr.pagination.PagedResult;
import jakarta.validation.Valid;
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
@RequestMapping("/api/v1/buildings")
@Validated
public class BuildingController {
    private final BuildingService buildingService;

    public BuildingController(BuildingService buildingService) {
        this.buildingService = buildingService;
    }

    @GetMapping
    public PagedResult<BuildingResponse> getPaged(
            @RequestParam(required = false) Long ownerId,
            @RequestParam(required = false) String keyword,
            @RequestParam(required = false) String wardCode,
            @RequestParam(required = false, defaultValue = "false") Boolean includeDeleted,
            @RequestParam(required = false) Integer pageNumber,
            @RequestParam(required = false) Integer pageSize) {
        BuildingFilter filter = new BuildingFilter();
        filter.setOwnerId(ownerId);
        filter.setKeyword(keyword);
        filter.setWardCode(wardCode);
        filter.setIncludeDeleted(includeDeleted);

        return buildingService.getPaged(filter, pageNumber, pageSize);
    }

    @GetMapping("/{buildingId}")
    public BuildingResponse getById(
            @PathVariable Long buildingId,
            @RequestParam(required = false, defaultValue = "false") boolean includeDeleted) {
        return buildingService.getById(buildingId, includeDeleted);
    }

    @PostMapping
    public ResponseEntity<BuildingResponse> create(@Valid @RequestBody CreateBuildingRequest request) {
        BuildingResponse response = buildingService.create(request);
        return ResponseEntity.status(HttpStatus.CREATED).body(response);
    }

    @PutMapping("/{buildingId}")
    public BuildingResponse update(@PathVariable Long buildingId, @Valid @RequestBody UpdateBuildingRequest request) {
        return buildingService.update(buildingId, request);
    }

    @DeleteMapping("/{buildingId}")
    public ResponseEntity<Void> softDelete(@PathVariable Long buildingId) {
        buildingService.softDelete(buildingId);
        return ResponseEntity.noContent().build();
    }
}
