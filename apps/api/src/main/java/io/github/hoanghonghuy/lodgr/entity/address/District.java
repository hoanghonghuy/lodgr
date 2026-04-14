package io.github.hoanghonghuy.lodgr.entity.address;

import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;

import java.util.HashSet;
import java.util.Set;

@Entity
@Table(name = "districts")
@Getter
@Setter
public class District {
    @Id
    @Column(name = "code", length = 10, nullable = false)
    private String code;

    @Column(name = "code_name", nullable = false, length = 50)
    private String codeName;

    @Column(name = "code_name_en", length = 50)
    private String codeNameEn;

    @Column(name = "name", length = 100)
    private String name;

    @Column(name = "name_en", length = 100)
    private String nameEn;

    @ManyToOne
    @JoinColumn(name = "province_code")
    private Province province;

    @OneToMany(mappedBy = "district", cascade = CascadeType.ALL, orphanRemoval = true)
    private Set<Ward> wards = new HashSet<Ward>();
}
