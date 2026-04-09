package io.github.hoanghonghuy.lodgr.features.building.service;

import io.github.hoanghonghuy.lodgr.entity.Building;
import io.github.hoanghonghuy.lodgr.features.building.dto.BuildingFilter;
import jakarta.persistence.criteria.Predicate;
import java.util.ArrayList;
import java.util.List;
import org.springframework.data.jpa.domain.Specification;

public final class BuildingSpecifications {
    private BuildingSpecifications() {
    }

    public static Specification<Building> byFilter(BuildingFilter filter) {
        return (root, query, criteriaBuilder) -> {
            List<Predicate> predicates = new ArrayList<>();
            boolean includeDeleted = filter.getIncludeDeleted() != null && filter.getIncludeDeleted();

            if (!includeDeleted) {
                predicates.add(criteriaBuilder.isFalse(root.get("isDeleted")));
            }

            if (filter.getOwnerId() != null) {
                predicates.add(criteriaBuilder.equal(root.get("owner").get("userId"), filter.getOwnerId()));
            }

            if (filter.getWardCode() != null && !filter.getWardCode().isBlank()) {
                predicates.add(criteriaBuilder.equal(root.get("wardCode"), filter.getWardCode().trim()));
            }

            if (filter.getKeyword() != null && !filter.getKeyword().isBlank()) {
                // Search theo tên + addressDetail, đủ đơn giản để intern dễ theo dõi.
                String keyword = "%" + filter.getKeyword().trim().toLowerCase() + "%";
                Predicate byName = criteriaBuilder.like(criteriaBuilder.lower(root.get("name")), keyword);
                Predicate byAddress = criteriaBuilder.like(criteriaBuilder.lower(root.get("addressDetail")), keyword);
                predicates.add(criteriaBuilder.or(byName, byAddress));
            }

            return criteriaBuilder.and(predicates.toArray(new Predicate[0]));
        };
    }
}
