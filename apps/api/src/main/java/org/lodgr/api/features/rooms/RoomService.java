package org.lodgr.api.features.rooms;

import static org.lodgr.api.features.rooms.RoomDtos.*;

import java.time.LocalDateTime;
import java.time.ZoneOffset;
import java.util.List;
import java.util.Optional;
import org.lodgr.api.pagination.PagedResult;
import org.lodgr.api.pagination.PagingUtils;
import org.lodgr.api.features.contracts.ContractRepository;
import org.lodgr.api.model.Contract;
import org.lodgr.api.model.ContractStatus;
import org.lodgr.api.model.OperationalStatus;
import org.lodgr.api.model.Room;
import org.lodgr.api.model.RoomType;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.data.domain.Sort;
import org.springframework.data.jpa.domain.Specification;
import org.springframework.stereotype.Service;

@Service
public class RoomService {
    private final RoomRepository roomRepository;
    private final ContractRepository contractRepository;

    public RoomService(RoomRepository roomRepository, ContractRepository contractRepository) {
        this.roomRepository = roomRepository;
        this.contractRepository = contractRepository;
    }

    public PagedResult<RoomListItemResponse> getPaged(GetRoomsQuery query) {
        int pageNumber = PagingUtils.normalizePageNumber(query.pageNumber());
        int pageSize = PagingUtils.normalizePageSize(query.pageSize());
        boolean includeDeleted = Boolean.TRUE.equals(query.includeDeleted());

        Specification<Room> specification = Specification.where(null);

        if (!includeDeleted) {
            specification = specification.and((root, q, cb) -> cb.isFalse(root.get("isDeleted")));
        }

        if (query.buildingId() != null) {
            specification = specification.and((root, q, cb) -> cb.equal(root.get("buildingId"), query.buildingId()));
        }

        if (query.keyword() != null && !query.keyword().isBlank()) {
            String keyword = "%" + query.keyword().trim().toLowerCase() + "%";
            specification = specification.and((root, q, cb) -> cb.or(
                    cb.like(cb.lower(root.get("roomNumber")), keyword),
                    cb.like(cb.lower(cb.coalesce(root.get("description"), "")), keyword)));
        }

        if (query.operationalStatus() != null && !query.operationalStatus().isBlank()) {
            OperationalStatus status = parseOperationalStatus(query.operationalStatus());
            specification = specification.and((root, q, cb) -> cb.equal(root.get("operationalStatus"), status));
        }

        Pageable pageable = PageRequest.of(
                pageNumber - 1,
                pageSize,
                Sort.by(Sort.Order.asc("floor"), Sort.Order.asc("roomNumber")));

        Page<Room> page = roomRepository.findAll(specification, pageable);
        List<RoomListItemResponse> items = page.getContent().stream().map(this::mapToListItem).toList();

        return PagedResult.create(items, pageNumber, pageSize, page.getTotalElements());
    }

    public RoomDetailResponse getById(Long roomId, boolean includeDeleted) {
        Optional<Room> optional = includeDeleted
                ? roomRepository.findById(roomId)
                : roomRepository.findByRoomIdAndIsDeletedFalse(roomId);

        return optional.map(this::mapToDetail).orElse(null);
    }

    public RoomDetailResponse create(CreateRoomRequest request) {
        LocalDateTime now = LocalDateTime.now(ZoneOffset.UTC);

        Room room = new Room();
        room.setBuildingId(request.buildingId());
        room.setRoomNumber(request.roomNumber().trim());
        room.setFloor(request.floor());
        room.setArea(request.area());
        room.setBasePrice(request.basePrice());
        room.setRoomType(parseRoomType(request.roomType()));
        room.setMaxOccupants(request.maxOccupants());
        room.setOperationalStatus(parseOperationalStatus(request.operationalStatus()));
        room.setDescription(request.description());
        room.setCreatedAt(now);
        room.setUpdatedAt(now);
        room.setIsDeleted(false);

        Room saved = roomRepository.save(room);
        return mapToDetail(saved);
    }

    public RoomDetailResponse update(Long roomId, UpdateRoomRequest request) {
        Optional<Room> optional = roomRepository.findById(roomId);
        if (optional.isEmpty()) {
            return null;
        }

        Room room = optional.get();
        room.setRoomNumber(request.roomNumber().trim());
        room.setFloor(request.floor());
        room.setArea(request.area());
        room.setBasePrice(request.basePrice());
        room.setRoomType(parseRoomType(request.roomType()));
        room.setMaxOccupants(request.maxOccupants());
        room.setOperationalStatus(parseOperationalStatus(request.operationalStatus()));
        room.setDescription(request.description());
        room.setUpdatedAt(LocalDateTime.now(ZoneOffset.UTC));

        Room saved = roomRepository.save(room);
        return mapToDetail(saved);
    }

    public boolean softDelete(Long roomId) {
        Optional<Room> optional = roomRepository.findById(roomId);
        if (optional.isEmpty()) {
            return false;
        }

        Room room = optional.get();
        if (Boolean.TRUE.equals(room.getIsDeleted())) {
            return true;
        }

        room.setIsDeleted(true);
        room.setUpdatedAt(LocalDateTime.now(ZoneOffset.UTC));
        roomRepository.save(room);
        return true;
    }

    public PagedResult<RoomContractListItemResponse> getPagedContracts(Long roomId, GetRoomContractsQuery query) {
        if (!roomRepository.existsByRoomId(roomId)) {
            return null;
        }

        int pageNumber = PagingUtils.normalizePageNumber(query.pageNumber());
        int pageSize = PagingUtils.normalizePageSize(query.pageSize());
        boolean includeInactive = query.includeInactive() == null || query.includeInactive();

        Specification<Contract> specification = (root, q, cb) -> cb.equal(root.get("roomId"), roomId);
        if (!includeInactive) {
            specification = specification.and((root, q, cb) -> cb.equal(root.get("status"), ContractStatus.ACTIVE));
        }

        Pageable pageable = PageRequest.of(
                pageNumber - 1,
                pageSize,
                Sort.by(Sort.Order.desc("startDate"), Sort.Order.desc("contractId")));

        Page<Contract> page = contractRepository.findAll(specification, pageable);
        List<RoomContractListItemResponse> items = page.getContent().stream().map(this::mapToContractListItem).toList();
        return PagedResult.create(items, pageNumber, pageSize, page.getTotalElements());
    }

    private RoomType parseRoomType(String value) {
        if (value == null || value.isBlank()) {
            return null;
        }

        try {
            return RoomType.valueOf(value.trim().toUpperCase());
        } catch (IllegalArgumentException ignored) {
            return null;
        }
    }

    private OperationalStatus parseOperationalStatus(String value) {
        if (value == null || value.isBlank()) {
            return OperationalStatus.ACTIVE;
        }

        try {
            return OperationalStatus.valueOf(value.trim().toUpperCase());
        } catch (IllegalArgumentException ignored) {
            return OperationalStatus.ACTIVE;
        }
    }

    private RoomListItemResponse mapToListItem(Room room) {
        return new RoomListItemResponse(
                room.getRoomId(),
                room.getBuildingId(),
                room.getRoomNumber(),
                room.getFloor(),
                room.getArea(),
                room.getBasePrice(),
                room.getRoomType() == null ? null : room.getRoomType().name(),
                room.getMaxOccupants(),
                room.getOperationalStatus().name(),
                room.getIsDeleted(),
                room.getCreatedAt(),
                room.getUpdatedAt());
    }

    private RoomDetailResponse mapToDetail(Room room) {
        return new RoomDetailResponse(
                room.getRoomId(),
                room.getBuildingId(),
                room.getRoomNumber(),
                room.getFloor(),
                room.getArea(),
                room.getBasePrice(),
                room.getRoomType() == null ? null : room.getRoomType().name(),
                room.getMaxOccupants(),
                room.getOperationalStatus().name(),
                room.getDescription(),
                room.getIsDeleted(),
                room.getCreatedAt(),
                room.getUpdatedAt());
    }

    private RoomContractListItemResponse mapToContractListItem(Contract contract) {
        return new RoomContractListItemResponse(
                contract.getContractId(),
                contract.getTenantId(),
                contract.getStartDate(),
                contract.getEndDate(),
                contract.getActualEndDate(),
                contract.getMonthlyRent(),
                contract.getCurrentOccupants(),
                contract.getStatus().name(),
                contract.getDepositStatus().name());
    }
}
