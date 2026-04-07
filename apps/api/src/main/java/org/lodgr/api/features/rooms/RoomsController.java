package org.lodgr.api.features.rooms;

import static org.lodgr.api.features.rooms.RoomDtos.*;

import jakarta.validation.Valid;
import org.lodgr.api.pagination.PagedResult;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

@RestController
@Validated
@RequestMapping("/api/rooms")
public class RoomsController {
    private final RoomService roomService;

    public RoomsController(RoomService roomService) {
        this.roomService = roomService;
    }

    @GetMapping
    public ResponseEntity<PagedResult<RoomListItemResponse>> getRooms(
            @RequestParam(required = false) Integer pageNumber,
            @RequestParam(required = false) Integer pageSize,
            @RequestParam(required = false) Long buildingId,
            @RequestParam(required = false, defaultValue = "false") Boolean includeDeleted,
            @RequestParam(required = false) String keyword,
            @RequestParam(required = false) String operationalStatus) {
        GetRoomsQuery query = new GetRoomsQuery(pageNumber, pageSize, buildingId, includeDeleted, keyword,
                operationalStatus);
        return ResponseEntity.ok(roomService.getPaged(query));
    }

    @GetMapping("/{roomId}")
    public ResponseEntity<RoomDetailResponse> getRoomById(
            @PathVariable Long roomId,
            @RequestParam(required = false, defaultValue = "false") boolean includeDeleted) {
        RoomDetailResponse result = roomService.getById(roomId, includeDeleted);
        if (result == null) {
            return ResponseEntity.notFound().build();
        }

        return ResponseEntity.ok(result);
    }

    @GetMapping("/{roomId}/contracts")
    public ResponseEntity<PagedResult<RoomContractListItemResponse>> getRoomContracts(
            @PathVariable Long roomId,
            @RequestParam(required = false) Integer pageNumber,
            @RequestParam(required = false) Integer pageSize,
            @RequestParam(required = false, defaultValue = "true") Boolean includeInactive) {
        GetRoomContractsQuery query = new GetRoomContractsQuery(pageNumber, pageSize, includeInactive);
        PagedResult<RoomContractListItemResponse> result = roomService.getPagedContracts(roomId, query);

        if (result == null) {
            return ResponseEntity.notFound().build();
        }

        return ResponseEntity.ok(result);
    }

    @PostMapping
    public ResponseEntity<RoomDetailResponse> createRoom(@Valid @RequestBody CreateRoomRequest request) {
        RoomDetailResponse result = roomService.create(request);
        return ResponseEntity.status(HttpStatus.CREATED).body(result);
    }

    @PutMapping("/{roomId}")
    public ResponseEntity<RoomDetailResponse> updateRoom(
            @PathVariable Long roomId,
            @Valid @RequestBody UpdateRoomRequest request) {
        RoomDetailResponse result = roomService.update(roomId, request);
        if (result == null) {
            return ResponseEntity.notFound().build();
        }

        return ResponseEntity.ok(result);
    }

    @DeleteMapping("/{roomId}")
    public ResponseEntity<Void> deleteRoom(@PathVariable Long roomId) {
        boolean deleted = roomService.softDelete(roomId);
        if (!deleted) {
            return ResponseEntity.notFound().build();
        }

        return ResponseEntity.noContent().build();
    }
}
