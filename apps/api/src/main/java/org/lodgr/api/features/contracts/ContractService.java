package org.lodgr.api.features.contracts;

import static org.lodgr.api.features.contracts.ContractDtos.*;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.ZoneOffset;
import java.util.List;
import java.util.Optional;
import org.lodgr.api.pagination.PagedResult;
import org.lodgr.api.pagination.PagingUtils;
import org.lodgr.api.features.rooms.RoomRepository;
import org.lodgr.api.model.Contract;
import org.lodgr.api.model.ContractStatus;
import org.lodgr.api.model.DepositStatus;
import org.lodgr.api.model.Invoice;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.data.domain.Sort;
import org.springframework.data.jpa.domain.Specification;
import org.springframework.stereotype.Service;

@Service
public class ContractService {
    private static final LocalDate MAX_DATE = LocalDate.of(9999, 12, 31);

    private final ContractRepository contractRepository;
    private final RoomRepository roomRepository;
    private final TenantRepository tenantRepository;
    private final InvoiceRepository invoiceRepository;

    public ContractService(
            ContractRepository contractRepository,
            RoomRepository roomRepository,
            TenantRepository tenantRepository,
            InvoiceRepository invoiceRepository) {
        this.contractRepository = contractRepository;
        this.roomRepository = roomRepository;
        this.tenantRepository = tenantRepository;
        this.invoiceRepository = invoiceRepository;
    }

    public PagedResult<ContractListItemResponse> getPaged(GetContractsQuery query) {
        int pageNumber = PagingUtils.normalizePageNumber(query.pageNumber());
        int pageSize = PagingUtils.normalizePageSize(query.pageSize());

        Specification<Contract> specification = Specification.where(null);
        if (query.roomId() != null) {
            specification = specification.and((root, q, cb) -> cb.equal(root.get("roomId"), query.roomId()));
        }

        if (query.tenantId() != null) {
            specification = specification.and((root, q, cb) -> cb.equal(root.get("tenantId"), query.tenantId()));
        }

        if (query.status() != null) {
            specification = specification.and((root, q, cb) -> cb.equal(root.get("status"), query.status()));
        }

        if (query.depositStatus() != null) {
            specification = specification
                    .and((root, q, cb) -> cb.equal(root.get("depositStatus"), query.depositStatus()));
        }

        if (query.keyword() != null && !query.keyword().isBlank()) {
            String keyword = "%" + query.keyword().trim().toLowerCase() + "%";
            specification = specification
                    .and((root, q, cb) -> cb.like(cb.lower(cb.coalesce(root.get("note"), "")), keyword));
        }

        Pageable pageable = PageRequest.of(
                pageNumber - 1,
                pageSize,
                Sort.by(Sort.Order.desc("startDate"), Sort.Order.desc("contractId")));

        Page<Contract> page = contractRepository.findAll(specification, pageable);
        List<ContractListItemResponse> items = page.getContent().stream().map(this::mapToListItem).toList();
        return PagedResult.create(items, pageNumber, pageSize, page.getTotalElements());
    }

    public ContractDetailResponse getById(Long contractId) {
        return contractRepository.findById(contractId).map(this::mapToDetail).orElse(null);
    }

    public OperationResult<ContractDetailResponse> create(CreateContractRequest request) {
        String validationError = validateDateRange(request.startDate(), request.endDate(), request.actualEndDate());
        if (validationError != null) {
            return OperationResult.failure(OperationErrorType.VALIDATION, validationError);
        }

        if (!roomRepository.existsByRoomIdAndIsDeletedFalse(request.roomId())) {
            return OperationResult.failure(OperationErrorType.NOT_FOUND, "Room not found.");
        }

        if (!tenantRepository.existsByTenantId(request.tenantId())) {
            return OperationResult.failure(OperationErrorType.NOT_FOUND, "Tenant not found.");
        }

        LocalDate proposedEnd = coalesceDate(request.actualEndDate(), request.endDate(), MAX_DATE);
        boolean hasOverlap = contractRepository.existsOverlappingContract(
                request.roomId(),
                request.startDate(),
                proposedEnd,
                MAX_DATE,
                null);

        if (hasOverlap) {
            return OperationResult.failure(
                    OperationErrorType.CONFLICT,
                    "Contract date range overlaps with an existing contract in the same room.");
        }

        LocalDateTime now = LocalDateTime.now(ZoneOffset.UTC);

        Contract contract = new Contract();
        contract.setRoomId(request.roomId());
        contract.setTenantId(request.tenantId());
        contract.setStartDate(request.startDate());
        contract.setEndDate(request.endDate());
        contract.setActualEndDate(request.actualEndDate());
        contract.setMonthlyRent(request.monthlyRent());
        contract.setDepositAmount(request.depositAmount());
        contract.setDepositBalance(request.depositBalance());
        contract.setDepositStatus(request.depositStatus() == null ? DepositStatus.UNPAID : request.depositStatus());
        contract.setCurrentOccupants(request.currentOccupants());
        contract.setBillingCycleDay(request.billingCycleDay());
        contract.setStatus(request.status() == null ? ContractStatus.DRAFT : request.status());
        contract.setNote(request.note());
        contract.setCreatedAt(now);
        contract.setUpdatedAt(now);

        Contract saved = contractRepository.save(contract);
        return OperationResult.success(mapToDetail(saved));
    }

    public OperationResult<ContractDetailResponse> update(Long contractId, UpdateContractRequest request) {
        Optional<Contract> optional = contractRepository.findById(contractId);
        if (optional.isEmpty()) {
            return OperationResult.failure(OperationErrorType.NOT_FOUND, "Contract not found.");
        }

        String validationError = validateDateRange(request.startDate(), request.endDate(), request.actualEndDate());
        if (validationError != null) {
            return OperationResult.failure(OperationErrorType.VALIDATION, validationError);
        }

        if (!roomRepository.existsByRoomIdAndIsDeletedFalse(request.roomId())) {
            return OperationResult.failure(OperationErrorType.NOT_FOUND, "Room not found.");
        }

        if (!tenantRepository.existsByTenantId(request.tenantId())) {
            return OperationResult.failure(OperationErrorType.NOT_FOUND, "Tenant not found.");
        }

        LocalDate proposedEnd = coalesceDate(request.actualEndDate(), request.endDate(), MAX_DATE);
        boolean hasOverlap = contractRepository.existsOverlappingContract(
                request.roomId(),
                request.startDate(),
                proposedEnd,
                MAX_DATE,
                contractId);

        if (hasOverlap) {
            return OperationResult.failure(
                    OperationErrorType.CONFLICT,
                    "Contract date range overlaps with an existing contract in the same room.");
        }

        Contract contract = optional.get();
        contract.setRoomId(request.roomId());
        contract.setTenantId(request.tenantId());
        contract.setStartDate(request.startDate());
        contract.setEndDate(request.endDate());
        contract.setActualEndDate(request.actualEndDate());
        contract.setMonthlyRent(request.monthlyRent());
        contract.setDepositAmount(request.depositAmount());
        contract.setDepositBalance(request.depositBalance());
        contract.setDepositStatus(request.depositStatus());
        contract.setCurrentOccupants(request.currentOccupants());
        contract.setBillingCycleDay(request.billingCycleDay());
        contract.setStatus(request.status());
        contract.setNote(request.note());
        contract.setUpdatedAt(LocalDateTime.now(ZoneOffset.UTC));

        Contract saved = contractRepository.save(contract);
        return OperationResult.success(mapToDetail(saved));
    }

    public boolean terminate(Long contractId) {
        Optional<Contract> optional = contractRepository.findById(contractId);
        if (optional.isEmpty()) {
            return false;
        }

        Contract contract = optional.get();
        if (contract.getStatus() == ContractStatus.TERMINATED) {
            return true;
        }

        contract.setStatus(ContractStatus.TERMINATED);
        if (contract.getActualEndDate() == null) {
            contract.setActualEndDate(LocalDate.now(ZoneOffset.UTC));
        }
        contract.setUpdatedAt(LocalDateTime.now(ZoneOffset.UTC));
        contractRepository.save(contract);
        return true;
    }

    public PagedResult<ContractInvoiceListItemResponse> getPagedInvoices(Long contractId,
            GetContractInvoicesQuery query) {
        if (!contractRepository.existsByContractId(contractId)) {
            return null;
        }

        int pageNumber = PagingUtils.normalizePageNumber(query.pageNumber());
        int pageSize = PagingUtils.normalizePageSize(query.pageSize());

        Specification<Invoice> specification = (root, q, cb) -> cb.equal(root.get("contractId"), contractId);
        if (query.status() != null) {
            specification = specification.and((root, q, cb) -> cb.equal(root.get("status"), query.status()));
        }

        Pageable pageable = PageRequest.of(
                pageNumber - 1,
                pageSize,
                Sort.by(Sort.Order.desc("issueDate"), Sort.Order.desc("invoiceId")));

        Page<Invoice> page = invoiceRepository.findAll(specification, pageable);
        List<ContractInvoiceListItemResponse> items = page.getContent().stream().map(this::mapToInvoiceListItem)
                .toList();
        return PagedResult.create(items, pageNumber, pageSize, page.getTotalElements());
    }

    private String validateDateRange(LocalDate startDate, LocalDate endDate, LocalDate actualEndDate) {
        if (endDate != null && endDate.isBefore(startDate)) {
            return "EndDate must be greater than or equal to StartDate.";
        }

        if (actualEndDate != null && actualEndDate.isBefore(startDate)) {
            return "ActualEndDate must be greater than or equal to StartDate.";
        }

        return null;
    }

    private LocalDate coalesceDate(LocalDate first, LocalDate second, LocalDate fallback) {
        if (first != null) {
            return first;
        }

        if (second != null) {
            return second;
        }

        return fallback;
    }

    private ContractListItemResponse mapToListItem(Contract contract) {
        return new ContractListItemResponse(
                contract.getContractId(),
                contract.getRoomId(),
                contract.getTenantId(),
                contract.getStartDate(),
                contract.getEndDate(),
                contract.getActualEndDate(),
                contract.getMonthlyRent(),
                contract.getDepositAmount(),
                contract.getDepositBalance(),
                contract.getDepositStatus().name(),
                contract.getCurrentOccupants(),
                contract.getBillingCycleDay(),
                contract.getStatus().name(),
                contract.getCreatedAt(),
                contract.getUpdatedAt());
    }

    private ContractDetailResponse mapToDetail(Contract contract) {
        return new ContractDetailResponse(
                contract.getContractId(),
                contract.getRoomId(),
                contract.getTenantId(),
                contract.getStartDate(),
                contract.getEndDate(),
                contract.getActualEndDate(),
                contract.getMonthlyRent(),
                contract.getDepositAmount(),
                contract.getDepositBalance(),
                contract.getDepositStatus().name(),
                contract.getCurrentOccupants(),
                contract.getBillingCycleDay(),
                contract.getStatus().name(),
                contract.getNote(),
                contract.getCreatedAt(),
                contract.getUpdatedAt());
    }

    private ContractInvoiceListItemResponse mapToInvoiceListItem(Invoice invoice) {
        return new ContractInvoiceListItemResponse(
                invoice.getInvoiceId(),
                invoice.getInvoiceMonth(),
                invoice.getPeriodFrom(),
                invoice.getPeriodTo(),
                invoice.getIssueDate(),
                invoice.getDueDate(),
                invoice.getTotalAmount(),
                invoice.getPaidAmount(),
                invoice.getStatus().name());
    }
}
