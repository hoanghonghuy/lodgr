package org.lodgr.api.features.contracts;

import static org.lodgr.api.features.contracts.ContractDtos.*;

import jakarta.validation.Valid;
import java.util.Map;
import org.lodgr.api.pagination.PagedResult;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

@RestController
@Validated
@RequestMapping("/api/contracts")
public class ContractsController {
    private final ContractService contractService;

    public ContractsController(ContractService contractService) {
        this.contractService = contractService;
    }

    @GetMapping
    public ResponseEntity<PagedResult<ContractListItemResponse>> getContracts(
            @RequestParam(required = false) Integer pageNumber,
            @RequestParam(required = false) Integer pageSize,
            @RequestParam(required = false) Long roomId,
            @RequestParam(required = false) Long tenantId,
            @RequestParam(required = false) org.lodgr.api.model.ContractStatus status,
            @RequestParam(required = false) org.lodgr.api.model.DepositStatus depositStatus,
            @RequestParam(required = false) String keyword) {
        GetContractsQuery query = new GetContractsQuery(pageNumber, pageSize, roomId, tenantId, status, depositStatus,
                keyword);
        return ResponseEntity.ok(contractService.getPaged(query));
    }

    @GetMapping("/{contractId}")
    public ResponseEntity<ContractDetailResponse> getContractById(@PathVariable Long contractId) {
        ContractDetailResponse result = contractService.getById(contractId);
        if (result == null) {
            return ResponseEntity.notFound().build();
        }

        return ResponseEntity.ok(result);
    }

    @GetMapping("/{contractId}/invoices")
    public ResponseEntity<PagedResult<ContractInvoiceListItemResponse>> getContractInvoices(
            @PathVariable Long contractId,
            @RequestParam(required = false) Integer pageNumber,
            @RequestParam(required = false) Integer pageSize,
            @RequestParam(required = false) org.lodgr.api.model.InvoiceStatus status) {
        GetContractInvoicesQuery query = new GetContractInvoicesQuery(pageNumber, pageSize, status);
        PagedResult<ContractInvoiceListItemResponse> result = contractService.getPagedInvoices(contractId, query);
        if (result == null) {
            return ResponseEntity.notFound().build();
        }

        return ResponseEntity.ok(result);
    }

    @PostMapping
    public ResponseEntity<?> createContract(@Valid @RequestBody CreateContractRequest request) {
        OperationResult<ContractDetailResponse> result = contractService.create(request);
        if (result.isSuccess() && result.getValue() != null) {
            return ResponseEntity.status(HttpStatus.CREATED).body(result.getValue());
        }

        return mapOperationError(result.getError());
    }

    @PutMapping("/{contractId}")
    public ResponseEntity<?> updateContract(
            @PathVariable Long contractId,
            @Valid @RequestBody UpdateContractRequest request) {
        OperationResult<ContractDetailResponse> result = contractService.update(contractId, request);
        if (result.isSuccess() && result.getValue() != null) {
            return ResponseEntity.ok(result.getValue());
        }

        return mapOperationError(result.getError());
    }

    @DeleteMapping("/{contractId}")
    public ResponseEntity<Void> terminateContract(@PathVariable Long contractId) {
        boolean terminated = contractService.terminate(contractId);
        if (!terminated) {
            return ResponseEntity.notFound().build();
        }

        return ResponseEntity.noContent().build();
    }

    private ResponseEntity<?> mapOperationError(OperationResult.OperationError error) {
        if (error == null) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body(Map.of("error", "Unknown error"));
        }

        return switch (error.getType()) {
            case VALIDATION -> ResponseEntity.badRequest().body(Map.of("error", error.getMessage()));
            case NOT_FOUND -> ResponseEntity.status(HttpStatus.NOT_FOUND).body(Map.of("error", error.getMessage()));
            case CONFLICT -> ResponseEntity.status(HttpStatus.CONFLICT).body(Map.of("error", error.getMessage()));
        };
    }
}
