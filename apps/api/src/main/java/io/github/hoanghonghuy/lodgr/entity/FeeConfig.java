package io.github.hoanghonghuy.lodgr.entity;

import io.github.hoanghonghuy.lodgr.enums.ChargeMode;
import io.github.hoanghonghuy.lodgr.enums.FeeType;
import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;
import org.hibernate.annotations.CreationTimestamp;
import org.hibernate.annotations.UpdateTimestamp;

import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.Date;

@Entity
@Table(name = "fee_configs")
@Getter
@Setter
public class FeeConfig {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long feeConfigId;

    @ManyToOne
    @JoinColumn(name = "building_id", nullable = false)
    private Building building;

    @Enumerated(EnumType.STRING)
    @Column(name = "fee_type")
    private FeeType feeType;

    @Enumerated(EnumType.STRING)
    @Column(name = "charge_mode")
    private ChargeMode chargeMode;

    @Column(name = "unit_price", nullable = false, precision = 12, scale = 2)
    private BigDecimal unitPrice;

    @Column(name = "unit")
    private String unit;

    @Column(name = "effective_from")
    private Date effectiveFrom;

    @Column(name = "effective_to")
    private Date effectiveTo; // null => con hieu luc

    @Column(name = "note")
    private String note;

    @Column(name = "created_at", nullable = false, updatable = false)
    @CreationTimestamp
    private LocalDateTime createdAt;

    @Column(name = "updated_at", nullable = false)
    @UpdateTimestamp
    private LocalDateTime updatedAt;
}
