package org.lodgr.api.features.buildings;

import static org.lodgr.api.features.buildings.BuildingDtos.*;

import java.time.LocalDateTime;
import java.time.ZoneOffset;
import java.util.List;
import java.util.Optional;
import org.lodgr.api.pagination.PagedResult;
import org.lodgr.api.pagination.PagingUtils;
import org.lodgr.api.features.rooms.RoomRepository;
import org.lodgr.api.model.Building;
import org.lodgr.api.model.Room;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.data.domain.Sort;
import org.springframework.data.jpa.domain.Specification;
import org.springframework.stereotype.Service;

@Service
public class BuildingService {
    private final BuildingRepository buildingRepository;
    private final RoomRepository roomRepository;

    public BuildingService(BuildingRepository buildingRepository, RoomRepository roomRepository) {
        this.buildingRepository = buildingRepository;
        this.roomRepository = roomRepository;
    }

    public PagedResult<BuildingListItemResponse> getPaged(GetBuildingsQuery query) {
        int pageNumber = PagingUtils.normalizePageNumber(query.pageNumber());
        int pageSize = PagingUtils.normalizePageSize(query.pageSize());
        boolean includeDeleted = Boolean.TRUE.equals(query.includeDeleted());

        Specification<Building> specification = Specification.where(null);

        if (!includeDeleted) {
            specification = specification.and((root, q, cb) -> cb.isFalse(root.get("isDeleted")));
        }

        if (query.ownerId() != null) {
            specification = specification.and((root, q, cb) -> cb.equal(root.get("ownerId"), query.ownerId()));
        }

        if (query.keyword() != null && !query.keyword().isBlank()) {
            String keyword = "%" + query.keyword().trim().toLowerCase() + "%";
            specification = specification.and((root, q, cb) -> cb.or(
                    cb.like(cb.lower(root.get("name")), keyword),
                    cb.like(cb.lower(root.get("addressDetail")), keyword),
                    cb.like(cb.lower(root.get("wardCode")), keyword)));
        }

        Pageable pageable = PageRequest.of(
                pageNumber - 1,
                pageSize,
                Sort.by(Sort.Order.desc("updatedAt"), Sort.Order.desc("buildingId")));

        Page<Building> page = buildingRepository.findAll(specification, pageable);
        List<BuildingListItemResponse> items = page.getContent().stream().map(this::mapToListItem).toList();

        return PagedResult.create(items, pageNumber, pageSize, page.getTotalElements());
    }

    public BuildingDetailResponse getById(Long buildingId, boolean includeDeleted) {
        Optional<Building> optional = includeDeleted
                ? buildingRepository.findById(buildingId)
                : buildingRepository.findByBuildingIdAndIsDeletedFalse(buildingId);

        return optional.map(this::mapToDetail).orElse(null);
    }

    public BuildingDetailResponse create(CreateBuildingRequest request) {
        LocalDateTime now = LocalDateTime.now(ZoneOffset.UTC);

        Building building = new Building();
        building.setOwnerId(request.ownerId());
        building.setName(request.name().trim());
        building.setTotalFloors(request.totalFloors());
        building.setDescription(request.description());
        building.setAddressDetail(request.addressDetail().trim());
        building.setWardCode(request.wardCode().trim());
        building.setCreatedAt(now);
        building.setUpdatedAt(now);
        building.setIsDeleted(false);

        Building saved = buildingRepository.save(building);
        return mapToDetail(saved);
    }

    public BuildingDetailResponse update(Long buildingId, UpdateBuildingRequest request) {
        Optional<Building> optional = buildingRepository.findById(buildingId);
        if (optional.isEmpty()) {
            return null;
        }

        Building building = optional.get();
        building.setName(request.name().trim());
        building.setTotalFloors(request.totalFloors());
        building.setDescription(request.description());
        building.setAddressDetail(request.addressDetail().trim());
        building.setWardCode(request.wardCode().trim());
        building.setUpdatedAt(LocalDateTime.now(ZoneOffset.UTC));

        Building saved = buildingRepository.save(building);
        return mapToDetail(saved);
    }

    public boolean softDelete(Long buildingId) {
        Optional<Building> optional = buildingRepository.findById(buildingId);
        if (optional.isEmpty()) {
            return false;
        }

        Building building = optional.get();
        if (Boolean.TRUE.equals(building.getIsDeleted())) {
            return true;
        }

        building.setIsDeleted(true);
        building.setUpdatedAt(LocalDateTime.now(ZoneOffset.UTC));
        buildingRepository.save(building);
        return true;
    }

    public PagedResult<BuildingRoomItemResponse> getPagedRooms(Long buildingId, GetBuildingRoomsQuery query) {
        if (!buildingRepository.existsByBuildingId(buildingId)) {
            return null;
        }

        int pageNumber = PagingUtils.normalizePageNumber(query.pageNumber());
        int pageSize = PagingUtils.normalizePageSize(query.pageSize());
        boolean includeDeleted = Boolean.TRUE.equals(query.includeDeleted());

        Specification<Room> specification = (root, q, cb) -> cb.equal(root.get("buildingId"), buildingId);
        if (!includeDeleted) {
            specification = specification.and((root, q, cb) -> cb.isFalse(root.get("isDeleted")));
        }

        Pageable pageable = PageRequest.of(
                pageNumber - 1,
                pageSize,
                Sort.by(Sort.Order.asc("floor"), Sort.Order.asc("roomNumber")));

        Page<Room> page = roomRepository.findAll(specification, pageable);
        List<BuildingRoomItemResponse> items = page.getContent().stream().map(this::mapToRoomItem).toList();

        return PagedResult.create(items, pageNumber, pageSize, page.getTotalElements());
    }

    private BuildingListItemResponse mapToListItem(Building building) {
        return new BuildingListItemResponse(
                building.getBuildingId(),
                building.getOwnerId(),
                building.getName(),
                building.getTotalFloors(),
                building.getAddressDetail(),
                building.getWardCode(),
                building.getIsDeleted(),
                building.getCreatedAt(),
                building.getUpdatedAt());
    }

    private BuildingDetailResponse mapToDetail(Building building) {
        return new BuildingDetailResponse(
                building.getBuildingId(),
                building.getOwnerId(),
                building.getName(),
                building.getTotalFloors(),
                building.getDescription(),
                building.getAddressDetail(),
                building.getWardCode(),
                building.getIsDeleted(),
                building.getCreatedAt(),
                building.getUpdatedAt());
    }

    private BuildingRoomItemResponse mapToRoomItem(Room room) {
        return new BuildingRoomItemResponse(
                room.getRoomId(),
                room.getRoomNumber(),
                room.getFloor(),
                room.getBasePrice(),
                room.getArea(),
                room.getMaxOccupants(),
                room.getOperationalStatus().name(),
                room.getIsDeleted());
    }
}
