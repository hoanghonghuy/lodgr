package org.lodgr.api.features.contracts;

import org.lodgr.api.model.Tenant;
import org.springframework.data.jpa.repository.JpaRepository;

public interface TenantRepository extends JpaRepository<Tenant, Long> {
    boolean existsByTenantId(Long tenantId);
}
