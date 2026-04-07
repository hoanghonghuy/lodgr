package org.lodgr.api.features.rooms;

import java.util.Optional;
import org.lodgr.api.model.Room;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.JpaSpecificationExecutor;

public interface RoomRepository extends JpaRepository<Room, Long>, JpaSpecificationExecutor<Room> {
    Optional<Room> findByRoomIdAndIsDeletedFalse(Long roomId);

    boolean existsByRoomId(Long roomId);

    boolean existsByRoomIdAndIsDeletedFalse(Long roomId);
}
