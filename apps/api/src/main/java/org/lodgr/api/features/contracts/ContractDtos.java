package org.lodgr.api.features.contracts;

import jakarta.validation.constraints.DecimalMin;
import jakarta.validation.constraints.Max;
import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotNull;
import java.math.BigDecimal;
import java.time.LocalDate;
import java.time.LocalDateTime;
import org.lodgr.api.model.ContractStatus;
import org.lodgr.api.model.DepositStatus;
import org.lodgr.api.model.InvoiceStatus;

public final class ContractDtos {
    private ContractDtos() {
    }

    public record GetContractsQuery(
            @Min(1) Integer pageNumber,
            @Min(1) @Max(100) Integer pageSize,
            Long roomId,
            Long tenantId,
            ContractStatus status,
            DepositStatus depositStatus,
            String keyword) {
    }

    public record GetContractInvoicesQuery(
            @Min(1) Integer pageNumber,
            @Min(1) @Max(100) Integer pageSize,
            InvoiceStatus status) {
    }

    public record CreateContractRequest(
            @NotNull Long roomId,
            @NotNull Long tenantId,
            @NotNull LocalDate startDate,
            LocalDate endDate,
            LocalDate actualEndDate,
            @NotNull @DecimalMin("0") BigDecimal monthlyRent,
            @NotNull @DecimalMin("0") BigDecimal depositAmount,
            @NotNull @DecimalMin("0") BigDecimal depositBalance,
            DepositStatus depositStatus,
            @NotNull @Min(1) Integer currentOccupants,
            @NotNull @Min(1) @Max(28) Integer billingCycleDay,
            ContractStatus status,
            String note) {
    }

    public record UpdateContractRequest(
            @NotNull Long roomId,
            @NotNull Long tenantId,
            @NotNull LocalDate startDate,
            LocalDate endDate,
            LocalDate actualEndDate,
            @NotNull @DecimalMin("0") BigDecimal monthlyRent,
            @NotNull @DecimalMin("0") BigDecimal depositAmount,
            @NotNull @DecimalMin("0") BigDecimal depositBalance,
            @NotNull DepositStatus depositStatus,
            @NotNull @Min(1) Integer currentOccupants,
            @NotNull @Min(1) @Max(28) Integer billingCycleDay,
            @NotNull ContractStatus status,
            String note) {
    }

    public record ContractListItemResponse(
            Long contractId,
            Long roomId,
            Long tenantId,
            LocalDate startDate,
            LocalDate endDate,
            LocalDate actualEndDate,
            BigDecimal monthlyRent,
            BigDecimal depositAmount,
            BigDecimal depositBalance,
            String depositStatus,
            Integer currentOccupants,
            Integer billingCycleDay,
            String status,
            LocalDateTime createdAt,
            LocalDateTime updatedAt) {
    }

    public record ContractDetailResponse(
            Long contractId,
            Long roomId,
            Long tenantId,
            LocalDate startDate,
            LocalDate endDate,
            LocalDate actualEndDate,
            BigDecimal monthlyRent,
            BigDecimal depositAmount,
            BigDecimal depositBalance,
            String depositStatus,
            Integer currentOccupants,
            Integer billingCycleDay,
            String status,
            String note,
            LocalDateTime createdAt,
            LocalDateTime updatedAt) {
    }

    public record ContractInvoiceListItemResponse(
            Long invoiceId,
            LocalDate invoiceMonth,
            LocalDate periodFrom,
            LocalDate periodTo,
            LocalDate issueDate,
            LocalDate dueDate,
            BigDecimal totalAmount,
            BigDecimal paidAmount,
            String status) {
    }
}
