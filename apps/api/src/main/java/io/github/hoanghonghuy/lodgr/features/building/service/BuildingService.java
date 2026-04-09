package io.github.hoanghonghuy.lodgr.features.building.service;

import io.github.hoanghonghuy.lodgr.entity.Building;
import io.github.hoanghonghuy.lodgr.entity.User;
import io.github.hoanghonghuy.lodgr.entity.address.Ward;
import io.github.hoanghonghuy.lodgr.features.building.dto.BuildingFilter;
import io.github.hoanghonghuy.lodgr.features.building.dto.BuildingResponse;
import io.github.hoanghonghuy.lodgr.features.building.dto.CreateBuildingRequest;
import io.github.hoanghonghuy.lodgr.features.building.dto.UpdateBuildingRequest;
import io.github.hoanghonghuy.lodgr.features.building.repository.BuildingOwnerRepository;
import io.github.hoanghonghuy.lodgr.features.building.repository.BuildingRepository;
import io.github.hoanghonghuy.lodgr.features.building.repository.WardRepository;
import io.github.hoanghonghuy.lodgr.features.building.validator.BuildingReferenceValidator;
import io.github.hoanghonghuy.lodgr.pagination.PagedResult;
import io.github.hoanghonghuy.lodgr.pagination.PagingUtils;
import jakarta.transaction.Transactional;
import java.time.LocalDateTime;
import java.util.Collection;
import java.util.Collections;
import java.util.List;
import java.util.Map;
import java.util.Objects;
import java.util.Set;
import java.util.function.Function;
import java.util.stream.Collectors;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.data.domain.Sort;
import org.springframework.stereotype.Service;

@Service
public class BuildingService {
    private final BuildingRepository buildingRepository;
    private final BuildingOwnerRepository ownerRepository;
    private final WardRepository wardRepository;
    private final BuildingReferenceValidator buildingReferenceValidator;

    public BuildingService(
            BuildingRepository buildingRepository,
            BuildingOwnerRepository ownerRepository,
            WardRepository wardRepository,
            BuildingReferenceValidator buildingReferenceValidator) {
        this.buildingRepository = buildingRepository;
        this.ownerRepository = ownerRepository;
        this.wardRepository = wardRepository;
        this.buildingReferenceValidator = buildingReferenceValidator;
    }

    public PagedResult<BuildingResponse> getPaged(BuildingFilter filter, Integer pageNumber, Integer pageSize) {
        int normalizedPageNumber = PagingUtils.normalizePageNumber(pageNumber);
        int normalizedPageSize = PagingUtils.normalizePageSize(pageSize);

        Pageable pageable = PageRequest.of(
                normalizedPageNumber - 1,
                normalizedPageSize,
                Sort.by(Sort.Direction.DESC, "createdAt"));

        Page<Building> page = buildingRepository.findAll(BuildingSpecifications.byFilter(filter), pageable);
        Map<String, Ward> wardsByCode = getWardMapByCode(page.getContent());

        List<BuildingResponse> responses = page.getContent().stream()
                .map(building -> BuildingMapper.toResponse(building, wardsByCode.get(building.getWardCode())))
                .toList();

        return PagedResult.create(responses, normalizedPageNumber, normalizedPageSize, page.getTotalElements());
    }

    public BuildingResponse getById(Long buildingId, boolean includeDeleted) {
        Building building = buildingRepository.findById(buildingId)
                .orElseThrow(() -> new BuildingNotFoundException(buildingId));

        if (!includeDeleted && Boolean.TRUE.equals(building.getIsDeleted())) {
            throw new BuildingNotFoundException(buildingId);
        }

        Ward ward = wardRepository.findById(building.getWardCode()).orElse(null);
        return BuildingMapper.toResponse(building, ward);
    }

    @Transactional
    public BuildingResponse create(CreateBuildingRequest request) {
        User ownerRef = buildingReferenceValidator.requireOwner(request.getOwnerId());
        buildingReferenceValidator.requireWard(request.getWardCode());

        Building building = new Building();
        building.setOwner(ownerRef);
        applyMutableFields(building, request.getName(), request.getTotalFloors(), request.getDescription(),
                request.getAddressDetail(), request.getWardCode());
        building.setCreatedAt(LocalDateTime.now());
        building.setUpdatedAt(LocalDateTime.now());
        building.setIsDeleted(false);

        Building saved = buildingRepository.save(building);
        Ward ward = wardRepository.findById(saved.getWardCode()).orElse(null);
        return BuildingMapper.toResponse(saved, ward);
    }

    @Transactional
    public BuildingResponse update(Long buildingId, UpdateBuildingRequest request) {
        Building building = buildingRepository.findById(buildingId)
                .orElseThrow(() -> new BuildingNotFoundException(buildingId));

        if (Boolean.TRUE.equals(building.getIsDeleted())) {
            throw new BuildingNotFoundException(buildingId);
        }

        buildingReferenceValidator.requireWard(request.getWardCode());

        applyMutableFields(building, request.getName(), request.getTotalFloors(), request.getDescription(),
                request.getAddressDetail(), request.getWardCode());
        building.setUpdatedAt(LocalDateTime.now());

        Building saved = buildingRepository.save(building);
        Ward ward = wardRepository.findById(saved.getWardCode()).orElse(null);
        return BuildingMapper.toResponse(saved, ward);
    }

    @Transactional
    public void softDelete(Long buildingId) {
        Building building = buildingRepository.findById(buildingId)
                .orElseThrow(() -> new BuildingNotFoundException(buildingId));

        if (!Boolean.TRUE.equals(building.getIsDeleted())) {
            building.setIsDeleted(true);
            building.setUpdatedAt(LocalDateTime.now());
            buildingRepository.save(building);
        }
    }

    private void applyMutableFields(
            Building building,
            String name,
            Integer totalFloors,
            String description,
            String addressDetail,
            String wardCode) {
        building.setName(normalize(name));
        building.setTotalFloors(totalFloors);
        building.setDescription(normalizeNullable(description));
        building.setAddressDetail(normalize(addressDetail));
        building.setWardCode(normalize(wardCode));
    }

    private Map<String, Ward> getWardMapByCode(List<Building> buildings) {
        Set<String> wardCodes = buildings.stream()
                .map(Building::getWardCode)
                .filter(Objects::nonNull)
                .collect(Collectors.toSet());

        if (wardCodes.isEmpty()) {
            return Collections.emptyMap();
        }

        Collection<Ward> wards = wardRepository.findByCodeIn(wardCodes);
        return wards.stream().collect(Collectors.toMap(Ward::getCode, Function.identity()));
    }

    private String normalize(String value) {
        return value == null ? null : value.trim();
    }

    private String normalizeNullable(String value) {
        if (value == null) {
            return null;
        }

        String trimmed = value.trim();
        return trimmed.isEmpty() ? null : trimmed;
    }
}
