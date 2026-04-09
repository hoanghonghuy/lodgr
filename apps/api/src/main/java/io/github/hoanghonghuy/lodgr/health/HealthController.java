package io.github.hoanghonghuy.lodgr.health;

import java.util.Map;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class HealthController {
    private final JdbcTemplate jdbcTemplate;

    public HealthController(JdbcTemplate jdbcTemplate) {
        this.jdbcTemplate = jdbcTemplate;
    }

    @GetMapping("/health")
    public Map<String, String> health() {
        return Map.of("status", "ok", "service", "lodgr-api");
    }

    @GetMapping("/health/db")
    public ResponseEntity<Map<String, String>> dbHealth() {
        try {
            Integer status = jdbcTemplate.queryForObject("select 1", Integer.class);
            if (status != null && status == 1) {
                return ResponseEntity.ok(Map.of("status", "ok", "database", "reachable"));
            }
        } catch (Exception ignored) {
            return ResponseEntity.status(HttpStatus.SERVICE_UNAVAILABLE)
                    .body(Map.of("status", "error", "database", "unreachable"));
        }

        return ResponseEntity.status(HttpStatus.SERVICE_UNAVAILABLE)
                .body(Map.of("status", "error", "database", "unreachable"));
    }
}
