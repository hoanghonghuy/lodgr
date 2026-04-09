package io.github.hoanghonghuy.lodgr.features.building.dto;

import lombok.Data;

@Data
public class BuildingFilter {
    private Long ownerId;
    private String keyword;
    private String wardCode;
    private Boolean includeDeleted;
}
