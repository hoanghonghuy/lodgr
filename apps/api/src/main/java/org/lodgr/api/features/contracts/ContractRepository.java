package org.lodgr.api.features.contracts;

import org.lodgr.api.model.Contract;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.JpaSpecificationExecutor;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

public interface ContractRepository extends JpaRepository<Contract, Long>, JpaSpecificationExecutor<Contract> {
    boolean existsByContractId(Long contractId);

    @Query("""
                select (count(c) > 0) from Contract c
                where c.roomId = :roomId
                  and (:excludedContractId is null or c.contractId <> :excludedContractId)
                  and c.startDate <= :proposedEnd
                  and :startDate <= coalesce(c.actualEndDate, c.endDate, :maxDate)
            """)
    boolean existsOverlappingContract(
            @Param("roomId") Long roomId,
            @Param("startDate") java.time.LocalDate startDate,
            @Param("proposedEnd") java.time.LocalDate proposedEnd,
            @Param("maxDate") java.time.LocalDate maxDate,
            @Param("excludedContractId") Long excludedContractId);
}
