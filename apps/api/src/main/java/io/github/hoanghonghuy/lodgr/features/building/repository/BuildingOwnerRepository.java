package io.github.hoanghonghuy.lodgr.features.building.repository;

import io.github.hoanghonghuy.lodgr.entity.User;
import org.springframework.data.jpa.repository.JpaRepository;

public interface BuildingOwnerRepository extends JpaRepository<User, Long> {
}
