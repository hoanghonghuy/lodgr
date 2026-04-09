package io.github.hoanghonghuy.lodgr.features.building.validator;

import io.github.hoanghonghuy.lodgr.entity.User;
import io.github.hoanghonghuy.lodgr.features.building.repository.BuildingOwnerRepository;
import io.github.hoanghonghuy.lodgr.features.building.repository.WardRepository;
import io.github.hoanghonghuy.lodgr.features.building.service.InvalidBuildingReferenceException;
import org.springframework.stereotype.Component;

@Component
public class BuildingReferenceValidator {
    private final BuildingOwnerRepository ownerRepository;
    private final WardRepository wardRepository;

    public BuildingReferenceValidator(
            BuildingOwnerRepository ownerRepository,
            WardRepository wardRepository) {
        this.ownerRepository = ownerRepository;
        this.wardRepository = wardRepository;
    }

    public User requireOwner(Long ownerId) {
        return ownerRepository.findById(ownerId)
                .orElseThrow(() -> new InvalidBuildingReferenceException(
                        "Owner with id " + ownerId + " does not exist"));
    }

    public void requireWard(String wardCode) {
        if (!wardRepository.existsById(wardCode)) {
            throw new InvalidBuildingReferenceException("Ward with code " + wardCode + " does not exist");
        }
    }
}
