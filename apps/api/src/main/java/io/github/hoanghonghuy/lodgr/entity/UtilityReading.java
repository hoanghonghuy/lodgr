package io.github.hoanghonghuy.lodgr.entity;

import io.github.hoanghonghuy.lodgr.enums.UtilityReadingType;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.EnumType;
import jakarta.persistence.Enumerated;
import jakarta.persistence.FetchType;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import jakarta.persistence.Table;
import lombok.Getter;
import lombok.Setter;

import java.math.BigDecimal;
import java.time.LocalDate;
import java.time.LocalDateTime;

@Entity
@Table(name = "utility_readings")
@Getter
@Setter
public class UtilityReading {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "reading_id")
    private Long readingId;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "room_id", nullable = false)
    private Room room;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "contract_id")
    private Contract contract;

    @Enumerated(EnumType.STRING)
    @Column(name = "reading_type", nullable = false)
    private UtilityReadingType readingType;

    @Column(name = "period_from", nullable = false)
    private LocalDate periodFrom;

    @Column(name = "period_to", nullable = false)
    private LocalDate periodTo;

    @Column(name = "closing_date", nullable = false)
    private LocalDate closingDate;

    @Column(name = "electricity_old", precision = 10, scale = 2)
    private BigDecimal electricityOld;

    @Column(name = "electricity_new", precision = 10, scale = 2)
    private BigDecimal electricityNew;

    @Column(name = "water_old", precision = 10, scale = 2)
    private BigDecimal waterOld;

    @Column(name = "water_new", precision = 10, scale = 2)
    private BigDecimal waterNew;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "recorded_by")
    private User recordedBy;

    @Column(name = "recorded_at", nullable = false)
    private LocalDateTime recordedAt;

    @Column(name = "note")
    private String note;
}
