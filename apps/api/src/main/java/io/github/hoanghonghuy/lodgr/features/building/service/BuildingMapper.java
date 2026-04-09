package io.github.hoanghonghuy.lodgr.features.building.service;

import io.github.hoanghonghuy.lodgr.entity.Building;
import io.github.hoanghonghuy.lodgr.entity.address.District;
import io.github.hoanghonghuy.lodgr.entity.address.Province;
import io.github.hoanghonghuy.lodgr.entity.address.Ward;
import io.github.hoanghonghuy.lodgr.features.building.dto.BuildingResponse;
import java.util.ArrayList;
import java.util.List;

public final class BuildingMapper {
    private BuildingMapper() {
    }

    public static BuildingResponse toResponse(Building building, Ward ward) {
        BuildingResponse response = new BuildingResponse();
        response.setBuildingId(building.getBuildingId());
        response.setOwnerId(building.getOwner() != null ? building.getOwner().getUserId() : null);
        response.setName(building.getName());
        response.setTotalFloors(building.getTotalFloors());
        response.setDescription(building.getDescription());
        response.setAddressDetail(building.getAddressDetail());
        response.setWardCode(building.getWardCode());
        response.setFullAddress(formatFullAddress(building.getAddressDetail(), ward));
        response.setCreatedAt(building.getCreatedAt());
        response.setUpdatedAt(building.getUpdatedAt());
        response.setIsDeleted(building.getIsDeleted());
        return response;
    }

    private static String formatFullAddress(String addressDetail, Ward ward) {
        List<String> parts = new ArrayList<>();

        if (addressDetail != null && !addressDetail.isBlank()) {
            parts.add(addressDetail.trim());
        }

        if (ward != null && ward.getName() != null && !ward.getName().isBlank()) {
            parts.add(ward.getName().trim());
        }

        District district = ward != null ? ward.getDistrict() : null;
        if (district != null && district.getName() != null && !district.getName().isBlank()) {
            parts.add(district.getName().trim());
        }

        Province province = district != null ? district.getProvince() : null;
        if (province != null && province.getName() != null && !province.getName().isBlank()) {
            parts.add(province.getName().trim());
        }

        return String.join(", ", parts);
    }
}
