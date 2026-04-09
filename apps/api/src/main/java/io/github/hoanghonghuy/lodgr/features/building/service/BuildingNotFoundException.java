package io.github.hoanghonghuy.lodgr.features.building.service;

public class BuildingNotFoundException extends RuntimeException {
    public BuildingNotFoundException(Long buildingId) {
        super("Building with id " + buildingId + " was not found");
    }
}
