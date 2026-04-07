package org.lodgr.api.features.rooms;

import jakarta.validation.constraints.DecimalMin;
import jakarta.validation.constraints.Max;
import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;
import java.math.BigDecimal;
import java.time.LocalDate;
import java.time.LocalDateTime;

public final class RoomDtos {
    private RoomDtos() {
    }

    public record CreateRoomRequest(
            @NotNull Long buildingId,
            @NotBlank @Size(max = 20) String roomNumber,
            Integer floor,
            BigDecimal area,
            @NotNull @DecimalMin("0") BigDecimal basePrice,
            String roomType,
            @NotNull @Min(1) Integer maxOccupants,
            String operationalStatus,
            String description) {
    }

    public record UpdateRoomRequest(
            @NotBlank @Size(max = 20) String roomNumber,
            Integer floor,
            BigDecimal area,
            @NotNull @DecimalMin("0") BigDecimal basePrice,
            String roomType,
            @NotNull @Min(1) Integer maxOccupants,
            String operationalStatus,
            String description) {
    }

    public record RoomListItemResponse(
            Long roomId,
            Long buildingId,
            String roomNumber,
            Integer floor,
            BigDecimal area,
            BigDecimal basePrice,
            String roomType,
            Integer maxOccupants,
            String operationalStatus,
            Boolean isDeleted,
            LocalDateTime createdAt,
            LocalDateTime updatedAt) {
    }

    public record RoomDetailResponse(
            Long roomId,
            Long buildingId,
            String roomNumber,
            Integer floor,
            BigDecimal area,
            BigDecimal basePrice,
            String roomType,
            Integer maxOccupants,
            String operationalStatus,
            String description,
            Boolean isDeleted,
            LocalDateTime createdAt,
            LocalDateTime updatedAt) {
    }

    public record RoomContractListItemResponse(
            Long contractId,
            Long tenantId,
            LocalDate startDate,
            LocalDate endDate,
            LocalDate actualEndDate,
            BigDecimal monthlyRent,
            Integer currentOccupants,
            String status,
            String depositStatus) {
    }

    public record GetRoomsQuery(
            @Min(1) Integer pageNumber,
            @Min(1) @Max(100) Integer pageSize,
            Long buildingId,
            Boolean includeDeleted,
            String keyword,
            String operationalStatus) {
    }

    public record GetRoomContractsQuery(
            @Min(1) Integer pageNumber,
            @Min(1) @Max(100) Integer pageSize,
            Boolean includeInactive) {
    }
}
