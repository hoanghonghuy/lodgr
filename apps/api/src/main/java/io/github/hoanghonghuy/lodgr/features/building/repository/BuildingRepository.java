package io.github.hoanghonghuy.lodgr.features.building.repository;

import io.github.hoanghonghuy.lodgr.entity.Building;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.JpaSpecificationExecutor;

public interface BuildingRepository extends JpaRepository<Building, Long>, JpaSpecificationExecutor<Building> {
}
