package io.github.hoanghonghuy.lodgr.features.building.validator;

import io.github.hoanghonghuy.lodgr.entity.User;
import io.github.hoanghonghuy.lodgr.features.building.repository.BuildingOwnerRepository;
import io.github.hoanghonghuy.lodgr.features.building.repository.WardRepository;
import io.github.hoanghonghuy.lodgr.features.building.service.InvalidBuildingReferenceException;

import java.util.Objects;

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
        Long id = Objects.requireNonNull(ownerId, "ownerId should not be null");
        return ownerRepository.findById(id)
                .orElseThrow(() -> new InvalidBuildingReferenceException(
                        "Owner with id " + id + " does not exist"));
    }

    public void requireWard(String wardCode) {
        String code = Objects.requireNonNull(wardCode, "wardCode should not be null");
        if (!wardRepository.existsById(code)) {
            throw new InvalidBuildingReferenceException("Ward with code " + code + " does not exist");
        }
    }
}
