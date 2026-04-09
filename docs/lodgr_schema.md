# Lodgr - Database Schema

## Tổng quan: 16 bảng

```sql
-- Danh mục địa chỉ (data tĩnh, import 1 lần)
provinces, districts, wards

-- Nghiệp vụ chính
users, buildings, rooms, fee_configs,
tenants, contracts, occupancy_changes,
utility_readings, invoices, invoice_items,
payments, deposit_transactions, notifications
```

---

## Quan hệ tổng thể

```text
users
  \-- buildings (-> wards -> districts -> provinces)
        |-- fee_configs
        \-- rooms
              |-- utility_readings
              \-- contracts
                    |-- tenants
                    |-- occupancy_changes
                    |-- invoices
                    |     |-- invoice_items
                    |     \-- payments
                    |-- deposit_transactions
                    \-- notifications
```

---

## Danh mục địa chỉ (data tĩnh)

### 1. provinces

### 2. districts

### 3. wards

---

## Nghiệp vụ chính

### 4. users
Chủ nhà / quản lý đăng nhập hệ thống.

| Field | Kiểu | Ghi chú |
|---|---|---|
| user_id | BIGSERIAL PK | |
| email | VARCHAR(255) NOT NULL UNIQUE | Dùng để đăng nhập |
| password_hash | VARCHAR(255) NOT NULL | BCrypt/Argon2 |
| full_name | VARCHAR(100) NOT NULL | |
| phone | VARCHAR(20) | |
| role | ENUM(OWNER, MANAGER) NOT NULL DEFAULT OWNER | |
| created_at | TIMESTAMP NOT NULL DEFAULT NOW() | |
| updated_at | TIMESTAMP NOT NULL DEFAULT NOW() | |

---

### 5. buildings
Tòa nhà / dãy trọ do chủ nhà quản lý.

| Field | Kiểu | Ghi chú |
|---|---|---|
| building_id | BIGSERIAL PK | |
| owner_id | BIGINT NOT NULL FK -> users | |
| name | VARCHAR(100) NOT NULL | Tên tòa nhà/dãy trọ |
| total_floors | INT NOT NULL DEFAULT 1 | Số tầng |
| description | TEXT | |
| address_detail | VARCHAR(255) NOT NULL | Số nhà, ngõ, tên đường |
| ward_code | VARCHAR(20) NOT NULL FK -> wards | Ward là nguồn sự thật; district/province suy ra qua join |
| created_at | TIMESTAMP NOT NULL DEFAULT NOW() | |
| updated_at | TIMESTAMP NOT NULL DEFAULT NOW() | |
| is_deleted | BOOLEAN NOT NULL DEFAULT FALSE | Soft delete |

> **Index:** `owner_id`, `ward_code`, `is_deleted`
>
> **Địa chỉ đầy đủ khi hiển thị:** `address_detail` + `wards.full_name` + `districts.full_name` + `provinces.full_name`

---

### 6. rooms
Phòng thuộc một tòa nhà.

| Field | Kiểu | Ghi chú |
|---|---|---|
| room_id | BIGSERIAL PK | |
| building_id | BIGINT NOT NULL FK -> buildings | |
| room_number | VARCHAR(20) NOT NULL | Số phòng (101, A2...) |
| floor | INT | |
| area | DECIMAL(8,2) | Diện tích (m2) |
| base_price | DECIMAL(12,2) NOT NULL | Giá phòng tham chiếu |
| room_type | ENUM(SINGLE, DOUBLE, STUDIO, FAMILY) | |
| max_occupants | INT NOT NULL DEFAULT 1 | Số người tối đa |
| operational_status | ENUM(ACTIVE, MAINTENANCE, BLOCKED) NOT NULL DEFAULT ACTIVE | Trạng thái vận hành; tình trạng có người ở suy ra từ contract active |
| description | TEXT | |
| created_at | TIMESTAMP NOT NULL DEFAULT NOW() | |
| updated_at | TIMESTAMP NOT NULL DEFAULT NOW() | |
| is_deleted | BOOLEAN NOT NULL DEFAULT FALSE | Soft delete |

> **Unique constraint:** partial unique `(building_id, room_number)` where `is_deleted = FALSE`

---

### 7. fee_configs
Đơn giá dịch vụ theo từng tòa nhà, có lịch sử hiệu lực.

| Field | Kiểu | Ghi chú |
|---|---|---|
| config_id | BIGSERIAL PK | |
| building_id | BIGINT NOT NULL FK -> buildings | |
| fee_type | ENUM(ELECTRICITY, WATER, INTERNET, GARBAGE, PARKING, OTHER) NOT NULL | |
| charge_mode | ENUM(PER_UNIT, PER_PERSON, FIXED_MONTHLY) NOT NULL | Cách tính phí |
| unit_price | DECIMAL(12,2) NOT NULL | |
| unit | VARCHAR(20) | kWh, m3, người, tháng... |
| effective_from | DATE NOT NULL | |
| effective_to | DATE | Null = còn hiệu lực |
| note | VARCHAR(255) | |
| created_at | TIMESTAMP NOT NULL DEFAULT NOW() | |

> **Business rule:** không cho phép 2 cấu hình cùng `building_id + fee_type` bị overlap thời gian.

---

### 8. tenants
Thông tin người đại diện đứng hợp đồng.

| Field | Kiểu | Ghi chú |
|---|---|---|
| tenant_id | BIGSERIAL PK | |
| full_name | VARCHAR(100) NOT NULL | |
| id_card_number | VARCHAR(20) UNIQUE | Số CCCD |
| email | VARCHAR(255) | Nhận thông báo |
| phone | VARCHAR(20) | |
| date_of_birth | DATE | |
| hometown | TEXT | |
| occupation | VARCHAR(100) | |
| note | TEXT | |
| created_at | TIMESTAMP NOT NULL DEFAULT NOW() | |
| updated_at | TIMESTAMP NOT NULL DEFAULT NOW() | |

---

### 9. contracts
Hợp đồng thuê phòng - trung tâm nghiệp vụ.

| Field | Kiểu | Ghi chú |
|---|---|---|
| contract_id | BIGSERIAL PK | |
| room_id | BIGINT NOT NULL FK -> rooms | |
| tenant_id | BIGINT NOT NULL FK -> tenants | Người đại diện đứng hợp đồng |
| start_date | DATE NOT NULL | Ngày bắt đầu ở |
| end_date | DATE | Ngày kết thúc theo hợp đồng |
| actual_end_date | DATE | Ngày rời phòng thực tế nếu trả sớm/trả giữa kỳ |
| monthly_rent | DECIMAL(12,2) NOT NULL | Giá thuê thỏa thuận |
| deposit_amount | DECIMAL(12,2) NOT NULL DEFAULT 0 | Mức cọc theo hợp đồng |
| deposit_balance | DECIMAL(12,2) NOT NULL DEFAULT 0 | Số tiền cọc đang giữ, suy ra từ sổ giao dịch hoặc cache để query nhanh |
| deposit_status | ENUM(UNPAID, PARTIALLY_PAID, HELD, PARTIALLY_RETURNED, RETURNED, FORFEITED) NOT NULL DEFAULT UNPAID | |
| current_occupants | INT NOT NULL DEFAULT 1 | Số người đang ở hiện tại của hợp đồng |
| billing_cycle_day | INT NOT NULL DEFAULT 1 | Ngày dự kiến chốt kỳ / lập hóa đơn (1-28) |
| status | ENUM(DRAFT, ACTIVE, EXPIRED, TERMINATED) NOT NULL DEFAULT DRAFT | |
| note | TEXT | |
| created_at | TIMESTAMP NOT NULL DEFAULT NOW() | |
| updated_at | TIMESTAMP NOT NULL DEFAULT NOW() | |

> **Business rule:** không cho phép 2 contract overlap thời gian trên cùng `room_id`
>
> **Khuyến nghị PostgreSQL:** dùng exclusion constraint với `daterange(start_date, COALESCE(actual_end_date, end_date, 'infinity'))`

---

### 10. occupancy_changes
Lịch sử thay đổi số người ở trong một hợp đồng.

| Field | Kiểu | Ghi chú |
|---|---|---|
| change_id | BIGSERIAL PK | |
| contract_id | BIGINT NOT NULL FK -> contracts | |
| effective_date | DATE NOT NULL | Ngày bắt đầu áp dụng số người mới |
| occupants_count | INT NOT NULL | Số người ở từ ngày này |
| reason | ENUM(MOVE_IN, MOVE_OUT, UPDATE, CORRECTION) NOT NULL | |
| note | TEXT | |
| created_at | TIMESTAMP NOT NULL DEFAULT NOW() | |
| created_by | BIGINT FK -> users | |

> **Business rule:** record mới nhất theo `effective_date` là cơ sở để đồng bộ `contracts.current_occupants`

---

### 11. utility_readings
Kỳ ghi chỉ số điện nước linh hoạt theo thực tế, không khóa cứng theo "tháng dương lịch".

| Field | Kiểu | Ghi chú |
|---|---|---|
| reading_id | BIGSERIAL PK | |
| room_id | BIGINT NOT NULL FK -> rooms | |
| contract_id | BIGINT FK -> contracts | Null khi ghi chỉ số lúc phòng trống hoặc chỉ để chuyển giao |
| reading_type | ENUM(REGULAR, MOVE_IN, MOVE_OUT, FINAL, ADJUSTMENT) NOT NULL DEFAULT REGULAR | |
| period_from | DATE NOT NULL | Đầu kỳ tính tiêu thụ |
| period_to | DATE NOT NULL | Cuối kỳ tính tiêu thụ |
| closing_date | DATE NOT NULL | Ngày đi ghi chỉ số thực tế |
| electricity_old | DECIMAL(10,2) | Chỉ số điện đầu kỳ |
| electricity_new | DECIMAL(10,2) | Chỉ số điện cuối kỳ |
| water_old | DECIMAL(10,2) | Chỉ số nước đầu kỳ |
| water_new | DECIMAL(10,2) | Chỉ số nước cuối kỳ |
| recorded_by | BIGINT FK -> users | |
| recorded_at | TIMESTAMP NOT NULL DEFAULT NOW() | |
| note | TEXT | |

> **Unique constraint:** `(room_id, period_from, period_to, reading_type)`
>
> **Business rule:** `period_to > period_from`, `closing_date >= period_to`, và chỉ số mới không nhỏ hơn chỉ số cũ

---

### 12. invoices
Hóa đơn cho từng hợp đồng theo một kỳ tính tiền thực tế.

| Field | Kiểu | Ghi chú |
|---|---|---|
| invoice_id | BIGSERIAL PK | |
| contract_id | BIGINT NOT NULL FK -> contracts | |
| invoice_month | DATE NOT NULL | Tháng dùng để group/reporting, thường là tháng của `period_to` |
| period_from | DATE NOT NULL | Đầu kỳ tính tiền |
| period_to | DATE NOT NULL | Cuối kỳ tính tiền |
| issue_date | DATE NOT NULL DEFAULT CURRENT_DATE | Ngày lập hóa đơn |
| total_amount | DECIMAL(12,2) NOT NULL | Tổng phải thu |
| paid_amount | DECIMAL(12,2) NOT NULL DEFAULT 0 | Tổng đã thu |
| due_date | DATE | Hạn thanh toán |
| status | ENUM(DRAFT, SENT, PARTIALLY_PAID, PAID, OVERDUE, VOID) NOT NULL DEFAULT DRAFT | |
| sent_at | TIMESTAMP | |
| created_at | TIMESTAMP NOT NULL DEFAULT NOW() | |
| updated_at | TIMESTAMP NOT NULL DEFAULT NOW() | |

> **Unique constraint:** `(contract_id, period_from, period_to)`
>
> **Business rule:** `period_to > period_from`, `paid_amount >= 0`, `paid_amount <= total_amount`

---

### 13. invoice_items
Chi tiết từng khoản trong hóa đơn.

| Field | Kiểu | Ghi chú |
|---|---|---|
| item_id | BIGSERIAL PK | |
| invoice_id | BIGINT NOT NULL FK -> invoices | |
| reading_id | BIGINT FK -> utility_readings | Null với tiền phòng, cọc, phí cố định |
| item_type | ENUM(RENT, ELECTRICITY, WATER, INTERNET, GARBAGE, PARKING, DEPOSIT_CHARGE, DEPOSIT_REFUND, OTHER) NOT NULL | |
| description | VARCHAR(255) | |
| quantity | DECIMAL(10,2) | Số lượng (kWh, m3, người, tháng...) |
| unit_price | DECIMAL(12,2) | |
| amount | DECIMAL(12,2) NOT NULL | Thành tiền |

---

### 14. payments
Ghi nhận từng lần thanh toán cho hóa đơn.

| Field | Kiểu | Ghi chú |
|---|---|---|
| payment_id | BIGSERIAL PK | |
| invoice_id | BIGINT NOT NULL FK -> invoices | |
| amount | DECIMAL(12,2) NOT NULL | Số tiền thanh toán |
| payment_date | DATE NOT NULL | |
| payment_method | ENUM(CASH, BANK_TRANSFER, OTHER) NOT NULL | |
| reference_code | VARCHAR(100) | Mã giao dịch/chuyển khoản |
| recorded_by | BIGINT FK -> users | |
| note | TEXT | |
| created_at | TIMESTAMP NOT NULL DEFAULT NOW() | |

> **Business rule:** `amount > 0`

---

### 15. deposit_transactions
Sổ giao dịch tiền cọc theo hợp đồng.

| Field | Kiểu | Ghi chú |
|---|---|---|
| deposit_txn_id | BIGSERIAL PK | |
| contract_id | BIGINT NOT NULL FK -> contracts | |
| txn_type | ENUM(COLLECT, REFUND, DEDUCTION, ADJUSTMENT) NOT NULL | |
| amount | DECIMAL(12,2) NOT NULL | Luôn lưu số dương; ý nghĩa tăng/giảm nằm ở `txn_type` |
| transaction_date | DATE NOT NULL | |
| payment_method | ENUM(CASH, BANK_TRANSFER, OTHER) | |
| reference_code | VARCHAR(100) | |
| recorded_by | BIGINT FK -> users | |
| note | TEXT | Lý do thu cọc, hoàn cọc, trừ vào tiền nợ, bồi thường... |
| created_at | TIMESTAMP NOT NULL DEFAULT NOW() | |

> **Business rule:** `amount > 0`
>
> **Công thức tham chiếu:** `deposit_balance = COLLECT + ADJUSTMENT - REFUND - DEDUCTION`

---

### 16. notifications
Lịch sử thông báo đã gửi cho khách thuê.

| Field | Kiểu | Ghi chú |
|---|---|---|
| notification_id | BIGSERIAL PK | |
| contract_id | BIGINT NOT NULL FK -> contracts | |
| invoice_id | BIGINT FK -> invoices | Null với welcome/expiry reminder |
| type | ENUM(INVOICE, PAYMENT_REMINDER, CONTRACT_EXPIRY, PAYMENT_CONFIRMED, WELCOME) NOT NULL | |
| recipient_email | VARCHAR(255) NOT NULL | |
| subject | VARCHAR(255) | |
| status | ENUM(PENDING, SENT, FAILED) NOT NULL DEFAULT PENDING | |
| taskgate_job_id | VARCHAR(100) | Job ID từ Taskgate |
| sent_at | TIMESTAMP | |
| error_message | TEXT | |
| created_at | TIMESTAMP NOT NULL DEFAULT NOW() | |

---

## Các constraint quan trọng nên có trong DB

1. `rooms`: unique `(building_id, room_number)` với partial index `WHERE is_deleted = FALSE`.
2. `contracts`: `billing_cycle_day BETWEEN 1 AND 28`, `current_occupants >= 1`, `deposit_amount >= 0`, `deposit_balance >= 0`.
3. `contracts`: `end_date IS NULL OR end_date >= start_date`; `actual_end_date IS NULL OR actual_end_date >= start_date`.
4. `contracts`: cấm overlap 2 khoảng thời gian ở cùng `room_id`.
5. `fee_configs`: cấm overlap khoảng hiệu lực cho cùng `building_id + fee_type`.
6. `utility_readings`: `period_to > period_from`; `closing_date >= period_to`; chỉ số mới >= chỉ số cũ.
7. `invoices`: `period_to > period_from`; `paid_amount BETWEEN 0 AND total_amount`.
8. `payments` và `deposit_transactions`: `amount > 0`.

---

## Tổng kết quan hệ

| Bảng | Quan hệ chính |
|---|---|
| provinces / districts / wards | Danh mục địa chỉ tĩnh |
| users | Sở hữu buildings, ghi nhận payments/readings/deposit transactions |
| buildings | Thuộc users; tham chiếu duy nhất đến ward |
| rooms | Thuộc buildings; có utility_readings và contracts |
| fee_configs | Thuộc buildings; định nghĩa đơn giá theo thời gian |
| tenants | Người đại diện đứng hợp đồng |
| contracts | Nối rooms <-> tenants; lưu giá thuê, thông tin cọc, số người hiện tại |
| occupancy_changes | Lịch sử thay đổi số người ở |
| utility_readings | Kỳ ghi chỉ số linh hoạt theo thực tế |
| invoices | Thuộc contracts; tính theo period_from/period_to |
| invoice_items | Chi tiết khoản tiền của invoices, có thể link reading |
| payments | Ghi nhận thanh toán của invoices |
| deposit_transactions | Sổ giao dịch tiền cọc của contracts |
| notifications | Lịch sử email/thông báo cho contracts và invoices |

---

## Flyway Migration Order

```text
V1__create_address_catalog.sql      <- provinces, districts, wards
V2__import_provinces.sql            <- 34 tỉnh/thành
V3__import_districts.sql            <- ~700 quận/huyện
V4__import_wards.sql                <- ~3,321 phường/xã
V5__create_core_schema.sql          <- users, buildings, rooms, fee_configs
V6__create_tenant_contract.sql      <- tenants, contracts, occupancy_changes
V7__create_billing.sql              <- utility_readings, invoices, invoice_items, payments
V8__create_deposit_transactions.sql <- deposit_transactions
V9__create_notifications.sql        <- notifications
```

---

## Ghi chú thiết kế

1. `contracts.tenant_id` chỉ đại diện cho người đứng hợp đồng; schema này chưa quản lý danh sách từng thành viên ở cùng phòng.
2. `contracts.current_occupants` là giá trị hiện tại để query nhanh; nếu cần audit thì đọc thêm từ `occupancy_changes`.
3. `utility_readings` và `invoices` đã đổi sang kỳ linh hoạt (`period_from`, `period_to`) để xử lý được trường hợp chốt ngày 5, ngày 6, hoặc trả phòng giữa tháng.
4. `deposit_transactions` là sổ giao dịch chi tiết; `contracts.deposit_balance` và `deposit_status` dùng để hiển thị nhanh và lọc dữ liệu.
