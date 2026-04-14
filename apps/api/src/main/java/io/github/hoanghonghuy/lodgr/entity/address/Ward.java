package io.github.hoanghonghuy.lodgr.entity.address;

import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;

@Entity
@Table(name = "wards")
@Getter
@Setter
public class Ward {
    @Id
    @Column(name = "code", length = 10, nullable = false)
    private String code;

    @Column(name = "code_name", length = 50)
    private String codeName;

    @Column(name = "code_name_en", length = 50)
    private String codeNameEn;

    @Column(name = "name", length = 100)
    private String name;

    @Column(name = "name_en", length = 100)
    private String nameEn;

    @ManyToOne
    @JoinColumn(name = "district_code")
    private District district;
}
