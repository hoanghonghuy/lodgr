package io.github.hoanghonghuy.lodgr.features.building.dto;

import java.time.LocalDateTime;

import lombok.Data;

@Data
public class BuildingResponse {
    private Long buildingId;
    private Long ownerId;
    private String name;
    private Integer totalFloors;
    private String description;
    private String addressDetail;
    private String wardCode;
    private String fullAddress;
    private LocalDateTime createdAt;
    private LocalDateTime updatedAt;
    private Boolean isDeleted;
}
