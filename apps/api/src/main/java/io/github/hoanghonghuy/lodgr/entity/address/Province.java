package io.github.hoanghonghuy.lodgr.entity.address;

import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;

import java.util.HashSet;
import java.util.Set;

@Entity
@Table(name = "provinces")
@Getter
@Setter
public class Province {
    @Id
    @Column(name = "code", length = 10)
    private String code;

    @Column(name = "code_name", nullable = false, length = 50)
    private String codeName;

    @Column(name = "code_name_en", length = 50)
    private String codeNameEn;

    @Column(name = "name", length = 100)
    private String name;

    @Column(name = "name_en", length = 100)
    private String nameEn;

    // 1 entity Province co n District
    @OneToMany(mappedBy = "province", // map voi private Province province ben District
            cascade = CascadeType.ALL,
            orphanRemoval = true) // khi remove o collection => delete trong db
    private Set<District> districts = new HashSet<District>();
}
