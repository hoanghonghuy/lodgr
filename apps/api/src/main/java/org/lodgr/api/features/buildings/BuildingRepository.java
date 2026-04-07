package org.lodgr.api.features.buildings;

import java.util.Optional;
import org.lodgr.api.model.Building;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.JpaSpecificationExecutor;

public interface BuildingRepository extends JpaRepository<Building, Long>, JpaSpecificationExecutor<Building> {
    Optional<Building> findByBuildingIdAndIsDeletedFalse(Long buildingId);

    boolean existsByBuildingId(Long buildingId);
}
