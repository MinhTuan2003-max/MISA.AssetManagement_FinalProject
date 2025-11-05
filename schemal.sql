-- =====================================================
-- DATABASE: FIXED ASSET MANAGEMENT
-- =====================================================
-- Author: HMTuan
-- Create date: 28/10/2025
-- Description: Cơ sở dữ liệu quản lý tài sản cố định
-- Character Set: utf8mb4
-- Collation: utf8mb4_0900_as_ci
-- =====================================================

-- Tạo database nếu chưa tồn tại
CREATE DATABASE IF NOT EXISTS misa_fixed_asset_management_development
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_0900_as_ci;

-- Sử dụng database vừa tạo
USE misa_fixed_asset_management_development;

-- =====================================================
-- TABLE: DEPARTMENT (Phòng ban)
-- =====================================================
-- Author: HMTuan
-- Create date: 28/10/2025
-- Description: Danh sách các phòng ban trong trường học
-- =====================================================

CREATE TABLE IF NOT EXISTS department (
    -- Khóa chính: UUID duy nhất cho mỗi phòng ban
    department_id CHAR(36) NOT NULL COMMENT 'UUID duy nhất cho mỗi phòng ban',

    -- Mã phòng ban: Dùng để nhận diện nhanh (duy nhất trong hệ thống)
    department_code VARCHAR(20) NOT NULL UNIQUE COMMENT 'Mã phòng ban (duy nhất)',

    -- Tên phòng ban
    department_name VARCHAR(255) NOT NULL COMMENT 'Tên phòng ban',

    -- Tên viết tắt phòng ban (tiếng Việt)
    department_short_name VARCHAR(50) COMMENT 'Tên viết tắt phòng ban (VD: BGH, HCQT, TV, ĐT, TCM)',

    -- Mô tả chi tiết về phòng ban
    description VARCHAR(255) COMMENT 'Mô tả chi tiết về phòng ban',

    -- Trạng thái hoạt động: 1 - Hoạt động, 0 - Không hoạt động
    is_active TINYINT NOT NULL DEFAULT 1 COMMENT 'Trạng thái hoạt động (1: Hoạt động, 0: Không hoạt động)',

    -- Ngày tạo bản ghi
    created_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Ngày tạo bản ghi',

    -- Người tạo bản ghi
    created_by VARCHAR(100) NOT NULL COMMENT 'Người tạo bản ghi',

    -- Ngày chỉnh sửa lần cuối
    modified_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'Ngày chỉnh sửa lần cuối',

    -- Người chỉnh sửa lần cuối
    modified_by VARCHAR(100) COMMENT 'Người chỉnh sửa lần cuối',

    PRIMARY KEY (department_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_as_ci COMMENT='Bảng quản lý phòng ban';

-- Index cho department_code để tìm kiếm nhanh
CREATE UNIQUE INDEX idx_department_code ON department(department_code) COMMENT 'Index tìm kiếm nhanh theo mã phòng ban';

-- =====================================================
-- TABLE: FIXED_ASSET_CATEGORY (Phân loại tài sản cố định)
-- =====================================================
-- Author: HMTuan
-- Create date: 28/10/2025
-- Description: Danh mục các loại tài sản cố định theo quy định kế toán
-- =====================================================

CREATE TABLE IF NOT EXISTS fixed_asset_category (
    -- Khóa chính: UUID duy nhất cho mỗi loại tài sản
    fixed_asset_category_id CHAR(36) NOT NULL COMMENT 'UUID duy nhất cho mỗi loại tài sản',

    -- Mã loại tài sản: Dùng để nhận diện nhanh (duy nhất trong hệ thống)
    fixed_asset_category_code VARCHAR(20) NOT NULL UNIQUE COMMENT 'Mã loại tài sản (duy nhất)',

    -- Tên loại tài sản
    fixed_asset_category_name VARCHAR(255) NOT NULL COMMENT 'Tên loại tài sản',

    -- Tên viết tắt loại tài sản (tiếng Việt)
    fixed_asset_category_short_name VARCHAR(50) COMMENT 'Tên viết tắt loại tài sản (VD: NCTXD, VKT, XÔT, MMTB)',

    -- Thời gian sử dụng (năm): Độ tuổi tài sản theo quy định
    life_time INT NOT NULL COMMENT 'Thời gian sử dụng dự kiến (năm)',

    -- Tỷ lệ hao mòn hàng năm (%)
    -- Ví dụ: 1.25 = 1.25% hao mòn/năm
    depreciation_rate DECIMAL(18, 4) NOT NULL DEFAULT 0 COMMENT 'Tỷ lệ hao mòn hàng năm (%)',

    -- Mô tả chi tiết về loại tài sản
    description VARCHAR(255) COMMENT 'Mô tả chi tiết về loại tài sản',

    -- Trạng thái hoạt động: 1 - Hoạt động, 0 - Không hoạt động
    is_active TINYINT NOT NULL DEFAULT 1 COMMENT 'Trạng thái hoạt động (1: Hoạt động, 0: Không hoạt động)',

    -- Ngày tạo bản ghi
    created_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Ngày tạo bản ghi',

    -- Người tạo bản ghi
    created_by VARCHAR(100) NOT NULL COMMENT 'Người tạo bản ghi',

    -- Ngày chỉnh sửa lần cuối
    modified_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'Ngày chỉnh sửa lần cuối',

    -- Người chỉnh sửa lần cuối
    modified_by VARCHAR(100) COMMENT 'Người chỉnh sửa lần cuối',

    PRIMARY KEY (fixed_asset_category_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_as_ci COMMENT='Bảng phân loại tài sản cố định';

-- Index cho fixed_asset_category_code để tìm kiếm nhanh
CREATE UNIQUE INDEX idx_fixed_asset_category_code ON fixed_asset_category(fixed_asset_category_code) COMMENT 'Index tìm kiếm nhanh theo mã loại tài sản';

-- =====================================================
-- TABLE: FIXED_ASSET (Tài sản cố định)
-- =====================================================
-- Author: HMTuan
-- Create date: 28/10/2025
-- Description: Danh sách các tài sản cố định của trường học
-- =====================================================

CREATE TABLE IF NOT EXISTS fixed_asset (
    -- Khóa chính: UUID duy nhất cho mỗi tài sản
    fixed_asset_id CHAR(36) NOT NULL COMMENT 'UUID duy nhất cho mỗi tài sản',

    -- Mã tài sản: Dùng để nhận diện nhanh (duy nhất trong hệ thống)
    -- Định dạng: TS + 6 chữ số (ví dụ: TS000001, TS000002, ...)
    fixed_asset_code VARCHAR(20) NOT NULL UNIQUE COMMENT 'Mã tài sản (duy nhất) - Định dạng: TS000001',

    -- Tên tài sản
    fixed_asset_name VARCHAR(255) NOT NULL COMMENT 'Tên tài sản',

    -- Khóa ngoại: Liên kết đến bảng department
    department_id CHAR(36) COMMENT 'Khóa ngoại liên kết đến bảng department',

    -- Mã phòng ban (denormalize để tìm kiếm nhanh)
    department_code VARCHAR(20) COMMENT 'Mã phòng ban (denormalize)',

    -- Tên phòng ban (denormalize để hiển thị)
    department_name VARCHAR(255) COMMENT 'Tên phòng ban (denormalize)',

    -- Tên viết tắt phòng ban (denormalize để hiển thị)
    department_short_name VARCHAR(50) COMMENT 'Tên viết tắt phòng ban (denormalize)',

    -- Khóa ngoại: Liên kết đến bảng fixed_asset_category
    fixed_asset_category_id CHAR(36) COMMENT 'Khóa ngoại liên kết đến bảng fixed_asset_category',

    -- Mã loại tài sản (denormalize để tìm kiếm nhanh)
    fixed_asset_category_code VARCHAR(20) COMMENT 'Mã loại tài sản (denormalize)',

    -- Tên loại tài sản (denormalize để hiển thị)
    fixed_asset_category_name VARCHAR(255) COMMENT 'Tên loại tài sản (denormalize)',

    -- Tên viết tắt loại tài sản (denormalize để hiển thị)
    fixed_asset_category_short_name VARCHAR(50) COMMENT 'Tên viết tắt loại tài sản (denormalize)',

    -- Số lượng tài sản
    quantity DECIMAL(18, 4) NOT NULL DEFAULT 1 COMMENT 'Số lượng tài sản',

    -- Nguyên giá (giá mua)
    cost DECIMAL(22, 4) NOT NULL DEFAULT 0 COMMENT 'Nguyên giá (giá mua)',

    -- Ngày mua tài sản
    purchase_date DATE COMMENT 'Ngày mua tài sản',

    -- Năm sản xuất/sử dụng: Được tính từ purchase_date bằng trigger
    production_year INT COMMENT 'Năm sản xuất/sử dụng (tự động tính từ purchase_date)',

    -- Năm bắt đầu theo dõi: Được tính từ purchase_date bằng trigger
    tracked_year INT COMMENT 'Năm bắt đầu theo dõi (tự động tính từ purchase_date)',

    -- Thời gian sử dụng dự kiến (năm): Lấy từ loại tài sản
    life_time INT COMMENT 'Thời gian sử dụng dự kiến (năm) - Lấy từ loại tài sản',

    -- Tỷ lệ hao mòn hàng năm (%): Lấy từ loại tài sản
    depreciation_rate DECIMAL(18, 4) NOT NULL DEFAULT 0 COMMENT 'Tỷ lệ hao mòn hàng năm (%) - Lấy từ loại tài sản',

    -- Giá trị hao mòn trong năm: Được tính bằng trigger (cost * depreciation_rate / 100)
    depreciation_value DECIMAL(22, 4) NOT NULL DEFAULT 0 COMMENT 'Giá trị hao mòn trong năm (tự động tính = cost * depreciation_rate / 100)',

    -- Mô tả chi tiết về tài sản
    description VARCHAR(255) COMMENT 'Mô tả chi tiết về tài sản',

    -- Trạng thái hoạt động: 1 - Hoạt động, 0 - Không hoạt động
    is_active TINYINT NOT NULL DEFAULT 1 COMMENT 'Trạng thái hoạt động (1: Hoạt động, 0: Không hoạt động)',

    -- Ngày tạo bản ghi
    created_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Ngày tạo bản ghi',

    -- Người tạo bản ghi
    created_by VARCHAR(100) NOT NULL COMMENT 'Người tạo bản ghi',

    -- Ngày chỉnh sửa lần cuối
    modified_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'Ngày chỉnh sửa lần cuối',

    -- Người chỉnh sửa lần cuối
    modified_by VARCHAR(100) COMMENT 'Người chỉnh sửa lần cuối',

    PRIMARY KEY (fixed_asset_id),

    -- Khóa ngoại: Liên kết với bảng department
    CONSTRAINT fk_fixed_asset_department FOREIGN KEY (department_id) 
        REFERENCES department(department_id) ON DELETE SET NULL ON UPDATE RESTRICT,

    -- Khóa ngoại: Liên kết với bảng fixed_asset_category
    CONSTRAINT fk_fixed_asset_category FOREIGN KEY (fixed_asset_category_id) 
        REFERENCES fixed_asset_category(fixed_asset_category_id) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_as_ci COMMENT='Bảng quản lý tài sản cố định';

-- Index cho fixed_asset_code để tìm kiếm nhanh
CREATE UNIQUE INDEX idx_fixed_asset_code ON fixed_asset(fixed_asset_code) COMMENT 'Index tìm kiếm nhanh theo mã tài sản';

-- Index cho department_code để filter theo phòng ban
CREATE INDEX idx_fixed_asset_department_code ON fixed_asset(department_code) COMMENT 'Index filter theo phòng ban';

-- Index cho fixed_asset_category_code để filter theo loại tài sản
CREATE INDEX idx_fixed_asset_category_code ON fixed_asset(fixed_asset_category_code) COMMENT 'Index filter theo loại tài sản';

-- Index cho created_date để sort theo ngày tạo
CREATE INDEX idx_fixed_asset_created_date ON fixed_asset(created_date DESC) COMMENT 'Index sắp xếp theo ngày tạo';

-- =====================================================
-- TRIGGER: trg_fixed_asset_insert
-- =====================================================
-- Author: HMTuan
-- Create date: 31/10/2025
-- Description: Tự động tính giá trị hao mòn năm, năm sử dụng 
--              và năm bắt đầu theo dõi khi thêm mới tài sản cố định
-- Logic:
--   - depreciation_value = cost * depreciation_rate / 100
--   - production_year = YEAR(purchase_date)
--   - tracked_year = YEAR(purchase_date)
-- Test: 
--   INSERT INTO fixed_asset (fixed_asset_id, fixed_asset_code, 
--   fixed_asset_name, cost, depreciation_rate, purchase_date, created_by) 
--   VALUES (UUID(), 'TS000001', 'Máy tính', 10000000, 20.00, '2025-01-01', 'system');
-- =====================================================

DELIMITER //

CREATE TRIGGER trg_fixed_asset_insert
BEFORE INSERT ON fixed_asset
FOR EACH ROW
BEGIN
    -- Tự động tính giá trị hao mòn năm dựa trên nguyên giá và tỷ lệ hao mòn
    -- Công thức: depreciation_value = cost * depreciation_rate / 100
    SET NEW.depreciation_value = NEW.cost * NEW.depreciation_rate / 100;

    -- Tự động set năm sử dụng = năm từ ngày mua
    SET NEW.production_year = YEAR(NEW.purchase_date);

    -- Tự động set năm bắt đầu theo dõi = năm từ ngày mua
    SET NEW.tracked_year = YEAR(NEW.purchase_date);
END //

DELIMITER ;

-- =====================================================
-- TRIGGER: trg_fixed_asset_update
-- =====================================================
-- Author: HMTuan
-- Create date: 31/10/2025
-- Description: Tự động tính lại giá trị hao mòn năm và cập nhật 
--              năm sử dụng, năm bắt đầu theo dõi khi ngày mua thay đổi
-- Logic:
--   - Luôn tính lại: depreciation_value = cost * depreciation_rate / 100
--   - Nếu purchase_date thay đổi: cập nhật production_year và tracked_year
-- Test:
--   UPDATE fixed_asset SET purchase_date = '2025-06-01', cost = 15000000 
--   WHERE fixed_asset_code = 'TS000001';
-- =====================================================

DELIMITER //

CREATE TRIGGER trg_fixed_asset_update
BEFORE UPDATE ON fixed_asset
FOR EACH ROW
BEGIN
    -- Tự động tính lại giá trị hao mòn năm dựa trên nguyên giá và tỷ lệ hao mòn mới
    -- Công thức: depreciation_value = cost * depreciation_rate / 100
    SET NEW.depreciation_value = NEW.cost * NEW.depreciation_rate / 100;

    -- Nếu ngày mua thay đổi, tự động cập nhật năm sử dụng và năm bắt đầu theo dõi
    IF NEW.purchase_date != OLD.purchase_date THEN
        SET NEW.production_year = YEAR(NEW.purchase_date);
        SET NEW.tracked_year = YEAR(NEW.purchase_date);
    END IF;
END //

DELIMITER ;

-- =====================================================
-- DỮ LIỆU MẪU: DEPARTMENT
-- =====================================================
-- Author: HMTuan
-- Create date: 28/10/2025
-- Description: Thêm dữ liệu ban đầu cho các phòng ban trong trường học
--              Bao gồm 5 phòng ban chính
-- Test: 
--   SELECT * FROM department WHERE is_active = 1;
-- =====================================================

INSERT INTO department (department_id, department_code, department_name, department_short_name, description, created_by) VALUES
-- Phòng ban 1: Ban Giám hiệu - Bộ phận lãnh đạo cao nhất
(UUID(), '01', 'Ban Giám hiệu', 'BGH', 'Bộ phận lãnh đạo cao nhất', 'system'),

-- Phòng ban 2: Phòng Hành chính - Quản trị - Quản lý hành chính và quản trị
(UUID(), '02', 'Phòng Hành chính - Quản trị', 'HCQT', 'Quản lý hành chính và quản trị', 'system'),

-- Phòng ban 3: Phòng Tài vụ - Quản lý tài chính và kế toán
(UUID(), '03', 'Phòng Tài vụ', 'TV', 'Quản lý tài chính và kế toán', 'system'),

-- Phòng ban 4: Phòng Đào tạo - Quản lý đào tạo và giảng dạy
(UUID(), '04', 'Phòng Đào tạo', 'ĐT', 'Quản lý đào tạo và giảng dạy', 'system'),

-- Phòng ban 5: Tổ chuyên môn - Giáo viên các bộ môn chuyên môn
(UUID(), '05', 'Tổ chuyên môn (Giáo viên bộ môn)', 'TCM', 'Giáo viên các bộ môn chuyên môn', 'system');

-- =====================================================
-- DỮ LIỆU MẪU: FIXED_ASSET_CATEGORY
-- =====================================================
-- Author: HMTuan
-- Create date: 28/10/2025
-- Description: Thêm dữ liệu các loại tài sản cố định theo quy định kế toán
--              Bao gồm 7 loại tài sản cố định chính
-- Quy định:
--   - life_time: Thời gian sử dụng (năm)
--   - depreciation_rate: Tỷ lệ hao mòn hàng năm (%)
-- Test:
--   SELECT * FROM fixed_asset_category WHERE is_active = 1 
--   ORDER BY fixed_asset_category_code;
-- =====================================================

INSERT INTO fixed_asset_category (fixed_asset_category_id, fixed_asset_category_code, fixed_asset_category_name, fixed_asset_category_short_name, life_time, depreciation_rate, created_by) VALUES
-- Loại 1: Nhà, công trình xây dựng - Thời gian sử dụng: 80 năm, Hao mòn: 1.25%/năm
(UUID(), '1', 'Nhà, công trình xây dựng', 'NCTXD', 80, 1.25, 'system'),

-- Loại 2: Vật kiến trúc - Thời gian sử dụng: 20 năm, Hao mòn: 5.00%/năm
(UUID(), '2', 'Vật kiến trúc', 'VKT', 20, 5.00, 'system'),

-- Loại 3: Xe ô tô - Thời gian sử dụng: 15 năm, Hao mòn: 6.67%/năm
(UUID(), '3', 'Xe ô tô', 'XÔT', 15, 6.67, 'system'),

-- Loại 4: Phương tiện vận tải khác (ngoài xe ô tô) - Thời gian sử dụng: 20 năm, Hao mòn: 5.00%/năm
(UUID(), '4', 'Phương tiện vận tải khác (ngoài xe ô tô)', 'PTVTK', 20, 5.00, 'system'),

-- Loại 5: Máy móc, thiết bị - Thời gian sử dụng: 5 năm, Hao mòn: 20.00%/năm
(UUID(), '5', 'Máy móc, thiết bị', 'MMTB', 5, 20.00, 'system'),

-- Loại 6: Cây lâu năm, súc vật làm việc và/hoặc cho sản phẩm - Thời gian sử dụng: 4 năm, Hao mòn: 25.00%/năm
(UUID(), '6', 'Cây lâu năm, súc vật làm việc và/hoặc cho sản phẩm', 'CLNSV', 4, 25.00, 'system'),

-- Loại 7: Tài sản cố định hữu hình khác - Thời gian sử dụng: 10 năm, Hao mòn: 10.00%/năm
(UUID(), '7', 'Tài sản cố định hữu hình khác', 'TSCDHHK', 10, 10.00, 'system');

INSERT INTO fixed_asset (
    fixed_asset_id, 
    fixed_asset_code, 
    fixed_asset_name, 
    department_id,
    department_code, 
    department_name,
    fixed_asset_category_id,
    fixed_asset_category_code, 
    fixed_asset_category_name,
    quantity, 
    cost, 
    purchase_date,
    life_time,
    depreciation_rate,
    created_by
) VALUES

-- ===== PHÒNG BAN GIÁM HIỆU (01) - 15 tài sản =====
(UUID(), 'TS000001', 'Máy tính Dell Optiplex 7080', 
 (SELECT department_id FROM department WHERE department_code = '01'), '01', 'Ban Giám hiệu',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 1, 20000000, '2024-01-15', 5, 20.00, 'system'),

(UUID(), 'TS000002', 'Máy in HP LaserJet Pro M404dn', 
 (SELECT department_id FROM department WHERE department_code = '01'), '01', 'Ban Giám hiệu',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 1, 8500000, '2024-01-20', 5, 20.00, 'system'),

(UUID(), 'TS000003', 'Máy photocopy Ricoh MP 2555', 
 (SELECT department_id FROM department WHERE department_code = '01'), '01', 'Ban Giám hiệu',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 1, 45000000, '2023-12-10', 5, 20.00, 'system'),

(UUID(), 'TS000004', 'Bàn làm việc Giám đốc gỗ Hòa Phát', 
 (SELECT department_id FROM department WHERE department_code = '01'), '01', 'Ban Giám hiệu',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 1, 15000000, '2023-11-05', 10, 10.00, 'system'),

(UUID(), 'TS000005', 'Tủ tài liệu cao cấp 2 cánh', 
 (SELECT department_id FROM department WHERE department_code = '01'), '01', 'Ban Giám hiệu',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 3, 7500000, '2023-11-05', 10, 10.00, 'system'),

(UUID(), 'TS000006', 'Điều hòa Daikin Inverter 2HP', 
 (SELECT department_id FROM department WHERE department_code = '01'), '01', 'Ban Giám hiệu',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 2, 18000000, '2024-02-01', 5, 20.00, 'system'),

(UUID(), 'TS000007', 'Máy chiếu Epson EB-2250U', 
 (SELECT department_id FROM department WHERE department_code = '01'), '01', 'Ban Giám hiệu',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 1, 28000000, '2024-03-10', 5, 20.00, 'system'),

(UUID(), 'TS000008', 'Màn hình chiếu điện 120 inch', 
 (SELECT department_id FROM department WHERE department_code = '01'), '01', 'Ban Giám hiệu',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 1, 12000000, '2024-03-10', 10, 10.00, 'system'),

(UUID(), 'TS000009', 'Hệ thống âm thanh hội trường TOA', 
 (SELECT department_id FROM department WHERE department_code = '01'), '01', 'Ban Giám hiệu',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 1, 75000000, '2023-09-15', 5, 20.00, 'system'),

(UUID(), 'TS000010', 'Xe ô tô Toyota Camry 2.5Q', 
 (SELECT department_id FROM department WHERE department_code = '01'), '01', 'Ban Giám hiệu',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '3'), '3', 'Xe ô tô',
 1, 1250000000, '2023-06-20', 15, 6.67, 'system'),

(UUID(), 'TS000011', 'Laptop Dell Latitude 7420', 
 (SELECT department_id FROM department WHERE department_code = '01'), '01', 'Ban Giám hiệu',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 2, 25000000, '2024-04-01', 5, 20.00, 'system'),

(UUID(), 'TS000012', 'Két sắt chống cháy Welko 150kg', 
 (SELECT department_id FROM department WHERE department_code = '01'), '01', 'Ban Giám hiệu',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 1, 18000000, '2023-10-15', 10, 10.00, 'system'),

(UUID(), 'TS000013', 'Bộ bàn ghế tiếp khách sofa da', 
 (SELECT department_id FROM department WHERE department_code = '01'), '01', 'Ban Giám hiệu',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 1, 35000000, '2023-11-05', 10, 10.00, 'system'),

(UUID(), 'TS000014', 'Camera an ninh hệ thống 16 mắt Hikvision', 
 (SELECT department_id FROM department WHERE department_code = '01'), '01', 'Ban Giám hiệu',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 1, 45000000, '2024-01-05', 5, 20.00, 'system'),

(UUID(), 'TS000015', 'Tivi Samsung 65 inch 4K', 
 (SELECT department_id FROM department WHERE department_code = '01'), '01', 'Ban Giám hiệu',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 1, 22000000, '2024-02-20', 5, 20.00, 'system'),

-- ===== PHÒNG HÀNH CHÍNH - QUẢN TRỊ (02) - 20 tài sản =====
(UUID(), 'TS000016', 'Máy tính Dell Vostro 3510', 
 (SELECT department_id FROM department WHERE department_code = '02'), '02', 'Phòng Hành chính - Quản trị',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 5, 16000000, '2024-01-10', 5, 20.00, 'system'),

(UUID(), 'TS000017', 'Máy in HP LaserJet Pro M203dn', 
 (SELECT department_id FROM department WHERE department_code = '02'), '02', 'Phòng Hành chính - Quản trị',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 2, 5500000, '2024-01-15', 5, 20.00, 'system'),

(UUID(), 'TS000018', 'Bàn làm việc nhân viên 1m4', 
 (SELECT department_id FROM department WHERE department_code = '02'), '02', 'Phòng Hành chính - Quản trị',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 8, 4500000, '2023-12-01', 10, 10.00, 'system'),

(UUID(), 'TS000019', 'Ghế văn phòng lưng lưới', 
 (SELECT department_id FROM department WHERE department_code = '02'), '02', 'Phòng Hành chính - Quản trị',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 8, 2500000, '2023-12-01', 10, 10.00, 'system'),

(UUID(), 'TS000020', 'Tủ tài liệu 4 ngăn kéo', 
 (SELECT department_id FROM department WHERE department_code = '02'), '02', 'Phòng Hành chính - Quản trị',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 4, 5500000, '2023-12-01', 10, 10.00, 'system'),

(UUID(), 'TS000021', 'Điều hòa Panasonic Inverter 1.5HP', 
 (SELECT department_id FROM department WHERE department_code = '02'), '02', 'Phòng Hành chính - Quản trị',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 3, 12000000, '2024-01-20', 5, 20.00, 'system'),

(UUID(), 'TS000022', 'Máy scan Canon DR-M160II', 
 (SELECT department_id FROM department WHERE department_code = '02'), '02', 'Phòng Hành chính - Quản trị',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 1, 18000000, '2024-02-01', 5, 20.00, 'system'),

(UUID(), 'TS000023', 'Máy hủy tài liệu Silicon PS-890C', 
 (SELECT department_id FROM department WHERE department_code = '02'), '02', 'Phòng Hành chính - Quản trị',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 1, 5500000, '2024-02-05', 5, 20.00, 'system'),

(UUID(), 'TS000024', 'Máy chấm công vân tay Ronald Jack X628-C', 
 (SELECT department_id FROM department WHERE department_code = '02'), '02', 'Phòng Hành chính - Quản trị',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 2, 3500000, '2024-01-10', 5, 20.00, 'system'),

(UUID(), 'TS000025', 'Tủ sắt locker 10 ngăn', 
 (SELECT department_id FROM department WHERE department_code = '02'), '02', 'Phòng Hành chính - Quản trị',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 2, 8500000, '2023-12-05', 10, 10.00, 'system'),

(UUID(), 'TS000026', 'Bộ bàn ghế họp 10 người', 
 (SELECT department_id FROM department WHERE department_code = '02'), '02', 'Phòng Hành chính - Quản trị',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 1, 25000000, '2023-11-15', 10, 10.00, 'system'),

(UUID(), 'TS000027', 'Máy lọc nước RO Kangaroo KG10A3', 
 (SELECT department_id FROM department WHERE department_code = '02'), '02', 'Phòng Hành chính - Quản trị',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 2, 8500000, '2024-01-05', 5, 20.00, 'system'),

(UUID(), 'TS000028', 'Tủ lạnh Sanky 90L', 
 (SELECT department_id FROM department WHERE department_code = '02'), '02', 'Phòng Hành chính - Quản trị',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 1, 4500000, '2024-01-05', 5, 20.00, 'system'),

(UUID(), 'TS000029', 'Bảng từ trắng 1.2m x 2.4m', 
 (SELECT department_id FROM department WHERE department_code = '02'), '02', 'Phòng Hành chính - Quản trị',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 2, 2500000, '2023-12-10', 10, 10.00, 'system'),

(UUID(), 'TS000030', 'Máy photocopy Canon iR2006N', 
 (SELECT department_id FROM department WHERE department_code = '02'), '02', 'Phòng Hành chính - Quản trị',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 1, 35000000, '2023-12-15', 5, 20.00, 'system'),

(UUID(), 'TS000031', 'Xe máy Honda Wave RSX', 
 (SELECT department_id FROM department WHERE department_code = '02'), '02', 'Phòng Hành chính - Quản trị',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '4'), '4', 'Phương tiện vận tải khác (ngoài xe ô tô)',
 2, 28000000, '2024-02-15', 20, 5.00, 'system'),

(UUID(), 'TS000032', 'Máy fax Panasonic KX-FT987', 
 (SELECT department_id FROM department WHERE department_code = '02'), '02', 'Phòng Hành chính - Quản trị',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 1, 3500000, '2024-01-20', 5, 20.00, 'system'),

(UUID(), 'TS000033', 'Máy đóng sách nhiệt GBC', 
 (SELECT department_id FROM department WHERE department_code = '02'), '02', 'Phòng Hành chính - Quản trị',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 1, 12000000, '2024-02-10', 5, 20.00, 'system'),

(UUID(), 'TS000034', 'Máy ép plastic khổ A3', 
 (SELECT department_id FROM department WHERE department_code = '02'), '02', 'Phòng Hành chính - Quản trị',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 1, 8500000, '2024-02-10', 5, 20.00, 'system'),

(UUID(), 'TS000035', 'Quạt đứng công nghiệp Asia D16003', 
 (SELECT department_id FROM department WHERE department_code = '02'), '02', 'Phòng Hành chính - Quản trị',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 4, 1500000, '2024-03-01', 5, 20.00, 'system'),

-- ===== PHÒNG TÀI VỤ (03) - 15 tài sản =====
(UUID(), 'TS000036', 'Máy tính HP EliteDesk 800 G6', 
 (SELECT department_id FROM department WHERE department_code = '03'), '03', 'Phòng Tài vụ',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 4, 18000000, '2024-01-08', 5, 20.00, 'system'),

(UUID(), 'TS000037', 'Máy in hóa đơn Epson LQ-310', 
 (SELECT department_id FROM department WHERE department_code = '03'), '03', 'Phòng Tài vụ',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 2, 4500000, '2024-01-12', 5, 20.00, 'system'),

(UUID(), 'TS000038', 'Két sắt văn phòng Welko 80kg', 
 (SELECT department_id FROM department WHERE department_code = '03'), '03', 'Phòng Tài vụ',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 2, 12000000, '2023-11-20', 10, 10.00, 'system'),

(UUID(), 'TS000039', 'Máy đếm tiền Silicon MC-9900N', 
 (SELECT department_id FROM department WHERE department_code = '03'), '03', 'Phòng Tài vụ',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 2, 8500000, '2024-01-15', 5, 20.00, 'system'),

(UUID(), 'TS000040', 'Máy tính tiền Casio SE-G1', 
 (SELECT department_id FROM department WHERE department_code = '03'), '03', 'Phòng Tài vụ',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 1, 12000000, '2024-01-18', 5, 20.00, 'system'),

(UUID(), 'TS000041', 'Bàn làm việc kế toán 1.6m', 
 (SELECT department_id FROM department WHERE department_code = '03'), '03', 'Phòng Tài vụ',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 5, 6500000, '2023-12-01', 10, 10.00, 'system'),

(UUID(), 'TS000042', 'Ghế văn phòng cao cấp', 
 (SELECT department_id FROM department WHERE department_code = '03'), '03', 'Phòng Tài vụ',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 5, 3500000, '2023-12-01', 10, 10.00, 'system'),

(UUID(), 'TS000043', 'Tủ hồ sơ 6 ngăn kéo', 
 (SELECT department_id FROM department WHERE department_code = '03'), '03', 'Phòng Tài vụ',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 3, 7500000, '2023-12-01', 10, 10.00, 'system'),

(UUID(), 'TS000044', 'Điều hòa LG Inverter 2HP', 
 (SELECT department_id FROM department WHERE department_code = '03'), '03', 'Phòng Tài vụ',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 2, 15000000, '2024-01-25', 5, 20.00, 'system'),

(UUID(), 'TS000045', 'Máy scan tài liệu Fujitsu fi-7160', 
 (SELECT department_id FROM department WHERE department_code = '03'), '03', 'Phòng Tài vụ',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 1, 22000000, '2024-02-01', 5, 20.00, 'system'),

(UUID(), 'TS000046', 'Máy photocopy Toshiba e-Studio 2508A', 
 (SELECT department_id FROM department WHERE department_code = '03'), '03', 'Phòng Tài vụ',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 1, 38000000, '2024-01-05', 5, 20.00, 'system'),

(UUID(), 'TS000047', 'Máy hủy tài liệu công suất lớn', 
 (SELECT department_id FROM department WHERE department_code = '03'), '03', 'Phòng Tài vụ',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 1, 8500000, '2024-02-05', 5, 20.00, 'system'),

(UUID(), 'TS000048', 'Máy in mã vạch Zebra ZT230', 
 (SELECT department_id FROM department WHERE department_code = '03'), '03', 'Phòng Tài vụ',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 1, 15000000, '2024-02-10', 5, 20.00, 'system'),

(UUID(), 'TS000049', 'Máy đóng dấu tự động Shiny S-843', 
 (SELECT department_id FROM department WHERE department_code = '03'), '03', 'Phòng Tài vụ',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 2, 2500000, '2024-02-15', 5, 20.00, 'system'),

(UUID(), 'TS000050', 'Laptop Dell Inspiron 15 3520', 
 (SELECT department_id FROM department WHERE department_code = '03'), '03', 'Phòng Tài vụ',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 2, 16000000, '2024-03-01', 5, 20.00, 'system'),

-- ===== PHÒNG ĐÀO TẠO (04) - 25 tài sản =====
(UUID(), 'TS000051', 'Máy tính Dell OptiPlex 5090', 
 (SELECT department_id FROM department WHERE department_code = '04'), '04', 'Phòng Đào tạo',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 6, 17000000, '2024-01-10', 5, 20.00, 'system'),

(UUID(), 'TS000052', 'Máy in HP LaserJet Pro M404dn', 
 (SELECT department_id FROM department WHERE department_code = '04'), '04', 'Phòng Đào tạo',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 3, 8500000, '2024-01-15', 5, 20.00, 'system'),

(UUID(), 'TS000053', 'Máy chiếu Epson EB-X06', 
 (SELECT department_id FROM department WHERE department_code = '04'), '04', 'Phòng Đào tạo',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 8, 12000000, '2024-02-01', 5, 20.00, 'system'),

(UUID(), 'TS000054', 'Màn chiếu điện 100 inch', 
 (SELECT department_id FROM department WHERE department_code = '04'), '04', 'Phòng Đào tạo',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 8, 8500000, '2024-02-01', 10, 10.00, 'system'),

(UUID(), 'TS000055', 'Bảng tương tác thông minh 85 inch', 
 (SELECT department_id FROM department WHERE department_code = '04'), '04', 'Phòng Đào tạo',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 4, 45000000, '2024-03-01', 5, 20.00, 'system'),

(UUID(), 'TS000056', 'Bàn làm việc giáo viên', 
 (SELECT department_id FROM department WHERE department_code = '04'), '04', 'Phòng Đào tạo',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 10, 5500000, '2023-12-05', 10, 10.00, 'system'),

(UUID(), 'TS000057', 'Ghế văn phòng giáo viên', 
 (SELECT department_id FROM department WHERE department_code = '04'), '04', 'Phòng Đào tạo',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 10, 2800000, '2023-12-05', 10, 10.00, 'system'),

(UUID(), 'TS000058', 'Tủ tài liệu giáo án', 
 (SELECT department_id FROM department WHERE department_code = '04'), '04', 'Phòng Đào tạo',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 6, 6500000, '2023-12-05', 10, 10.00, 'system'),

(UUID(), 'TS000059', 'Điều hòa Mitsubishi Inverter 2HP', 
 (SELECT department_id FROM department WHERE department_code = '04'), '04', 'Phòng Đào tạo',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 4, 16000000, '2024-02-10', 5, 20.00, 'system'),

(UUID(), 'TS000060', 'Máy photocopy Canon iR2425', 
 (SELECT department_id FROM department WHERE department_code = '04'), '04', 'Phòng Đào tạo',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 1, 42000000, '2024-01-20', 5, 20.00, 'system'),

(UUID(), 'TS000061', 'Máy scan tài liệu Canon DR-C225W', 
 (SELECT department_id FROM department WHERE department_code = '04'), '04', 'Phòng Đào tạo',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 2, 16000000, '2024-02-05', 5, 20.00, 'system'),

(UUID(), 'TS000062', 'Laptop HP ProBook 450 G9', 
 (SELECT department_id FROM department WHERE department_code = '04'), '04', 'Phòng Đào tạo',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 4, 18000000, '2024-03-10', 5, 20.00, 'system'),

(UUID(), 'TS000063', 'Máy in màu Brother HL-L3230CDW', 
 (SELECT department_id FROM department WHERE department_code = '04'), '04', 'Phòng Đào tạo',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 2, 7500000, '2024-03-15', 5, 20.00, 'system'),

(UUID(), 'TS000064', 'Bộ bàn ghế học sinh 40 bộ/phòng', 
 (SELECT department_id FROM department WHERE department_code = '04'), '04', 'Phòng Đào tạo',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 240, 1500000, '2023-11-10', 10, 10.00, 'system'),

(UUID(), 'TS000065', 'Bảng từ trắng lớp học 1.2m x 3.6m', 
 (SELECT department_id FROM department WHERE department_code = '04'), '04', 'Phòng Đào tạo',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 15, 4500000, '2023-11-10', 10, 10.00, 'system'),

(UUID(), 'TS000066', 'Tủ đựng đồ dùng học tập', 
 (SELECT department_id FROM department WHERE department_code = '04'), '04', 'Phòng Đào tạo',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 15, 3500000, '2023-11-10', 10, 10.00, 'system'),

(UUID(), 'TS000067', 'Quạt trần lớp học', 
 (SELECT department_id FROM department WHERE department_code = '04'), '04', 'Phòng Đào tạo',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 45, 1200000, '2023-11-15', 5, 20.00, 'system'),

(UUID(), 'TS000068', 'Loa trợ giảng không dây cho giáo viên', 
 (SELECT department_id FROM department WHERE department_code = '04'), '04', 'Phòng Đào tạo',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 20, 2500000, '2024-02-20', 5, 20.00, 'system'),

(UUID(), 'TS000069', 'Máy lọc nước RO cho học sinh', 
 (SELECT department_id FROM department WHERE department_code = '04'), '04', 'Phòng Đào tạo',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 8, 7500000, '2024-01-25', 5, 20.00, 'system'),

(UUID(), 'TS000070', 'Tivi Samsung 55 inch cho phòng họp', 
 (SELECT department_id FROM department WHERE department_code = '04'), '04', 'Phòng Đào tạo',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 3, 15000000, '2024-03-05', 5, 20.00, 'system'),

(UUID(), 'TS000071', 'Tủ sắt đựng đề thi', 
 (SELECT department_id FROM department WHERE department_code = '04'), '04', 'Phòng Đào tạo',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 2, 8500000, '2023-12-10', 10, 10.00, 'system'),

(UUID(), 'TS000072', 'Máy chấm điểm danh học sinh', 
 (SELECT department_id FROM department WHERE department_code = '04'), '04', 'Phòng Đào tạo',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 15, 3500000, '2024-02-01', 5, 20.00, 'system'),

(UUID(), 'TS000073', 'Máy tính bảng iPad Gen 9 cho giáo viên', 
 (SELECT department_id FROM department WHERE department_code = '04'), '04', 'Phòng Đào tạo',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 10, 9000000, '2024-03-20', 5, 20.00, 'system'),

(UUID(), 'TS000074', 'Hệ thống âm thanh lớp học TOA', 
 (SELECT department_id FROM department WHERE department_code = '04'), '04', 'Phòng Đào tạo',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 10, 12000000, '2024-01-30', 5, 20.00, 'system'),

(UUID(), 'TS000075', 'Bộ bàn ghế họp phụ huynh 20 người', 
 (SELECT department_id FROM department WHERE department_code = '04'), '04', 'Phòng Đào tạo',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 1, 35000000, '2023-12-01', 10, 10.00, 'system'),

-- ===== TỔ CHUYÊN MÔN (05) - 25 tài sản =====
(UUID(), 'TS000076', 'Máy tính All-in-One HP 24-df1000', 
 (SELECT department_id FROM department WHERE department_code = '05'), '05', 'Tổ chuyên môn (Giáo viên bộ môn)',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 30, 15000000, '2024-01-12', 5, 20.00, 'system'),

(UUID(), 'TS000077', 'Laptop Asus Vivobook 15', 
 (SELECT department_id FROM department WHERE department_code = '05'), '05', 'Tổ chuyên môn (Giáo viên bộ môn)',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 15, 14000000, '2024-02-15', 5, 20.00, 'system'),

(UUID(), 'TS000078', 'Kính hiển vi sinh học Olympus CX23', 
 (SELECT department_id FROM department WHERE department_code = '05'), '05', 'Tổ chuyên môn (Giáo viên bộ môn)',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 20, 8500000, '2024-01-20', 5, 20.00, 'system'),

(UUID(), 'TS000079', 'Bộ thí nghiệm hóa học cơ bản', 
 (SELECT department_id FROM department WHERE department_code = '05'), '05', 'Tổ chuyên môn (Giáo viên bộ môn)',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 10, 25000000, '2024-01-25', 5, 20.00, 'system'),

(UUID(), 'TS000080', 'Tủ đựng hóa chất phòng thí nghiệm', 
 (SELECT department_id FROM department WHERE department_code = '05'), '05', 'Tổ chuyên môn (Giáo viên bộ môn)',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 5, 18000000, '2023-12-15', 10, 10.00, 'system'),

(UUID(), 'TS000081', 'Bàn thí nghiệm hóa học chuyên dụng', 
 (SELECT department_id FROM department WHERE department_code = '05'), '05', 'Tổ chuyên môn (Giáo viên bộ môn)',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 15, 12000000, '2023-12-15', 10, 10.00, 'system'),

(UUID(), 'TS000082', 'Bộ dụng cụ vật lý điện học', 
 (SELECT department_id FROM department WHERE department_code = '05'), '05', 'Tổ chuyên môn (Giáo viên bộ môn)',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 8, 15000000, '2024-02-01', 5, 20.00, 'system'),

(UUID(), 'TS000083', 'Máy chiếu 3 trục tọa độ cho toán học', 
 (SELECT department_id FROM department WHERE department_code = '05'), '05', 'Tổ chuyên môn (Giáo viên bộ môn)',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 5, 18000000, '2024-02-10', 5, 20.00, 'system'),

(UUID(), 'TS000084', 'Bộ mô hình giải phẫu cơ thể người', 
 (SELECT department_id FROM department WHERE department_code = '05'), '05', 'Tổ chuyên môn (Giáo viên bộ môn)',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 3, 35000000, '2024-01-15', 10, 10.00, 'system'),

(UUID(), 'TS000085', 'Bộ mô hình địa lý địa hình Việt Nam', 
 (SELECT department_id FROM department WHERE department_code = '05'), '05', 'Tổ chuyên môn (Giáo viên bộ môn)',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 2, 28000000, '2024-01-20', 10, 10.00, 'system'),

(UUID(), 'TS000086', 'Quả địa cầu điện tử tương tác', 
 (SELECT department_id FROM department WHERE department_code = '05'), '05', 'Tổ chuyên môn (Giáo viên bộ môn)',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 5, 12000000, '2024-02-05', 5, 20.00, 'system'),

(UUID(), 'TS000087', 'Đàn piano điện Yamaha P-125', 
 (SELECT department_id FROM department WHERE department_code = '05'), '05', 'Tổ chuyên môn (Giáo viên bộ môn)',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 3, 18000000, '2024-03-01', 5, 20.00, 'system'),

(UUID(), 'TS000088', 'Bộ nhạc cụ dân tộc (đàn tranh, đàn bầu)', 
 (SELECT department_id FROM department WHERE department_code = '05'), '05', 'Tổ chuyên môn (Giáo viên bộ môn)',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 5, 22000000, '2024-03-05', 10, 10.00, 'system'),

(UUID(), 'TS000089', 'Bộ dụng cụ thể dục thể thao đa năng', 
 (SELECT department_id FROM department WHERE department_code = '05'), '05', 'Tổ chuyên môn (Giáo viên bộ môn)',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 20, 5500000, '2024-01-10', 10, 10.00, 'system'),

(UUID(), 'TS000090', 'Máy chạy bộ điện tử Mofit MF-200', 
 (SELECT department_id FROM department WHERE department_code = '05'), '05', 'Tổ chuyên môn (Giáo viên bộ môn)',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 3, 15000000, '2024-02-20', 5, 20.00, 'system'),

(UUID(), 'TS000091', 'Xà đơn xà kép thể dục', 
 (SELECT department_id FROM department WHERE department_code = '05'), '05', 'Tổ chuyên môn (Giáo viên bộ môn)',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '2'), '2', 'Vật kiến trúc',
 2, 25000000, '2023-11-20', 20, 5.00, 'system'),

(UUID(), 'TS000092', 'Bộ máy tính bảng học tiếng Anh cho học sinh', 
 (SELECT department_id FROM department WHERE department_code = '05'), '05', 'Tổ chuyên môn (Giáo viên bộ môn)',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 40, 6500000, '2024-03-15', 5, 20.00, 'system'),

(UUID(), 'TS000093', 'Tủ sách thư viện lớp học', 
 (SELECT department_id FROM department WHERE department_code = '05'), '05', 'Tổ chuyên môn (Giáo viên bộ môn)',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 20, 4500000, '2023-12-10', 10, 10.00, 'system'),

(UUID(), 'TS000094', 'Máy photocopy Canon iR2006N cho tổ bộ môn', 
 (SELECT department_id FROM department WHERE department_code = '05'), '05', 'Tổ chuyên môn (Giáo viên bộ môn)',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 2, 35000000, '2024-01-18', 5, 20.00, 'system'),

(UUID(), 'TS000095', 'Máy in 3D Creality Ender-3 V2', 
 (SELECT department_id FROM department WHERE department_code = '05'), '05', 'Tổ chuyên môn (Giáo viên bộ môn)',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 2, 12000000, '2024-03-20', 5, 20.00, 'system'),

(UUID(), 'TS000096', 'Robot lập trình giáo dục mBot', 
 (SELECT department_id FROM department WHERE department_code = '05'), '05', 'Tổ chuyên môn (Giáo viên bộ môn)',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 15, 3500000, '2024-03-25', 5, 20.00, 'system'),

(UUID(), 'TS000097', 'Bộ kit Arduino cho STEM', 
 (SELECT department_id FROM department WHERE department_code = '05'), '05', 'Tổ chuyên môn (Giáo viên bộ môn)',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 20, 2500000, '2024-03-25', 5, 20.00, 'system'),

(UUID(), 'TS000098', 'Máy cắt giấy công nghiệp A3', 
 (SELECT department_id FROM department WHERE department_code = '05'), '05', 'Tổ chuyên môn (Giáo viên bộ môn)',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 2, 8500000, '2024-02-25', 5, 20.00, 'system'),

(UUID(), 'TS000099', 'Điều hòa Daikin Inverter 1.5HP cho phòng thí nghiệm', 
 (SELECT department_id FROM department WHERE department_code = '05'), '05', 'Tổ chuyên môn (Giáo viên bộ môn)',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '5'), '5', 'Máy móc, thiết bị',
 6, 13000000, '2024-02-28', 5, 20.00, 'system'),

(UUID(), 'TS000100', 'Tủ đựng thiết bị dạy học tổ chuyên môn', 
 (SELECT department_id FROM department WHERE department_code = '05'), '05', 'Tổ chuyên môn (Giáo viên bộ môn)',
 (SELECT fixed_asset_category_id FROM fixed_asset_category WHERE fixed_asset_category_code = '7'), '7', 'Tài sản cố định hữu hình khác',
 10, 6500000, '2023-12-20', 10, 10.00, 'system');