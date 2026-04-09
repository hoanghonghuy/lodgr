package io.github.hoanghonghuy.lodgr.entity;

import io.github.hoanghonghuy.lodgr.enums.RoleType;
import jakarta.persistence.*;
import jakarta.validation.constraints.Email;
import lombok.Getter;
import lombok.Setter;
import org.hibernate.annotations.CreationTimestamp;
import org.hibernate.annotations.UpdateTimestamp;

import java.time.LocalDateTime;

@Entity
@Table(name = "users")
@Getter
@Setter
public class User {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long userId;

    @Column(name = "user_name", length = 50, unique = true)
    private String userName;

    @Column(name = "email", length = 100)
    @Email
    private String email;

    @Column(name = "full_name", length = 50)
    private String fullName;

    @Column(name = "password_hash", nullable = false)
    private String passwordHash;

    @Column(name = "phone", nullable = false)
    private Integer phone;

    @Enumerated(EnumType.STRING)
    @Column(name = "role", nullable = false)
    private RoleType role;

    @Column(name = "created_at", nullable = false, updatable = false)
    @CreationTimestamp
    private LocalDateTime createdAt;

    @Column(name = "updated_at", nullable = false)
    @UpdateTimestamp
    private LocalDateTime updatedAt;
    
}
