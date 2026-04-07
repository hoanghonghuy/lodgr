package org.lodgr.api.features.buildings;

import jakarta.validation.constraints.Max;
import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;
import java.math.BigDecimal;
import java.time.LocalDateTime;

public final class BuildingDtos {
    private BuildingDtos() {
    }

    public record CreateBuildingRequest(
            @NotNull Long ownerId,
            @NotBlank @Size(max = 100) String name,
            @NotNull @Min(1) Integer totalFloors,
            String description,
            @NotBlank @Size(max = 255) String addressDetail,
            @NotBlank @Size(max = 20) String wardCode) {
    }

    public record UpdateBuildingRequest(
            @NotBlank @Size(max = 100) String name,
            @NotNull @Min(1) Integer totalFloors,
            String description,
            @NotBlank @Size(max = 255) String addressDetail,
            @NotBlank @Size(max = 20) String wardCode) {
    }

    public record BuildingListItemResponse(
            Long buildingId,
            Long ownerId,
            String name,
            Integer totalFloors,
            String addressDetail,
            String wardCode,
            Boolean isDeleted,
            LocalDateTime createdAt,
            LocalDateTime updatedAt) {
    }

    public record BuildingDetailResponse(
            Long buildingId,
            Long ownerId,
            String name,
            Integer totalFloors,
            String description,
            String addressDetail,
            String wardCode,
            Boolean isDeleted,
            LocalDateTime createdAt,
            LocalDateTime updatedAt) {
    }

    public record BuildingRoomItemResponse(
            Long roomId,
            String roomNumber,
            Integer floor,
            BigDecimal basePrice,
            BigDecimal area,
            Integer maxOccupants,
            String operationalStatus,
            Boolean isDeleted) {
    }

    public record GetBuildingsQuery(
            @Min(1) Integer pageNumber,
            @Min(1) @Max(100) Integer pageSize,
            Long ownerId,
            Boolean includeDeleted,
            String keyword) {
    }

    public record GetBuildingRoomsQuery(
            @Min(1) Integer pageNumber,
            @Min(1) @Max(100) Integer pageSize,
            Boolean includeDeleted) {
    }
}
