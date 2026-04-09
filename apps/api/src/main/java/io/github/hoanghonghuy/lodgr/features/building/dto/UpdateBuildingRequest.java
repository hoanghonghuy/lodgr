package io.github.hoanghonghuy.lodgr.features.building.dto;

import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;
import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
public class UpdateBuildingRequest {
    @NotBlank(message = "name is required")
    @Size(max = 100, message = "name must be at most 100 characters")
    private String name;

    @NotNull(message = "totalFloors is required")
    @Min(value = 1, message = "totalFloors must be at least 1")
    private Integer totalFloors;

    @Size(max = 4000, message = "description must be at most 4000 characters")
    private String description;

    @NotBlank(message = "addressDetail is required")
    @Size(max = 255, message = "addressDetail must be at most 255 characters")
    private String addressDetail;

    @NotBlank(message = "wardCode is required")
    @Size(max = 20, message = "wardCode must be at most 20 characters")
    private String wardCode;
}
