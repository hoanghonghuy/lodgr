package io.github.hoanghonghuy.lodgr.features.building.repository;

import io.github.hoanghonghuy.lodgr.entity.address.Ward;
import java.util.Collection;
import java.util.List;
import org.springframework.data.jpa.repository.JpaRepository;

public interface WardRepository extends JpaRepository<Ward, String> {
    List<Ward> findByCodeIn(Collection<String> codes);
}
