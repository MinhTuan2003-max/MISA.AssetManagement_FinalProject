using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Services
{
    using MISA.Core.DTOs;
    using MISA.Core.Entities;
    using MISA.Core.Exceptions;
    using MISA.Core.Interfaces;
    using MISA.Core.Interfaces.Repository;
    using MISA.Core.Interfaces.Service;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Service xử lý nghiệp vụ tài sản cố định
    /// CreatedBy: HMTuan (28/10/2025)
    /// </summary>
    public class FixedAssetService : BaseService<FixedAsset>, IFixedAssetService
    {
        private readonly IFixedAssetRepository _fixedAssetRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IFixedAssetCategoryRepository _categoryRepository;

        public FixedAssetService(
            IFixedAssetRepository fixedAssetRepository,
            IDepartmentRepository departmentRepository,
            IFixedAssetCategoryRepository categoryRepository)
            : base(fixedAssetRepository)
        {
            _fixedAssetRepository = fixedAssetRepository;
            _departmentRepository = departmentRepository;
            _categoryRepository = categoryRepository;
        }

        /// <summary>
        /// Lấy danh sách tài sản có phân trang và lọc
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="filter">DTO filter</param>
        /// <returns>Kết quả phân trang</returns>
        public PagingResult<FixedAssetDto> GetPaging(FixedAssetFilterDto filter)
        {
            return _fixedAssetRepository.GetPaging(filter);
        }

        /// <summary>
        /// Lấy dữ liệu để nhân bản (có mã mới) - dùng để hiển thị forÌ
        /// CreatedBy: HMTuan (02/11/2025)
        /// </summary>
        /// <param name="id">ID tài sản gốc</param>
        /// <returns>DTO để hiển thị form tạo mới</returns>
        public FixedAssetCreateDto GetDuplicateData(Guid id)
        {
            var original = GetById(id);

            // Tạo mã mới tự động
            var newCode = _fixedAssetRepository.GenerateNewCode();

            // Trả về DTO với dữ liệu copy từ bản gốc
            return new FixedAssetCreateDto
            {
                FixedAssetCode = newCode,
                FixedAssetName = original.FixedAssetName + " (Copy)",

                DepartmentCode = original.DepartmentCode,
                FixedAssetCategoryCode = original.FixedAssetCategoryCode,

                // Ngày mua = ngày hiện tại
                PurchaseDate = DateTime.Now,

                LifeTime = original.LifeTime,
                DepreciationRate = original.DepreciationRate,

                Quantity = original.Quantity,
                Cost = original.Cost,
                DepreciationValue = original.DepreciationValue,

                Description = original.Description
            };
        }

        /// <summary>
        /// Tạo mới tài sản từ DTO
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="dto">DTO tạo mới</param>
        /// <returns>Số dòng affected</returns>
        public int CreateFromDto(FixedAssetCreateDto dto)
        {
            // Validate DTO
            ValidateCreateDto(dto);

            // Lấy thông tin bộ phận
            var department = _departmentRepository.GetByCode(dto.DepartmentCode);
            if (department == null)
            {
                throw new NotFoundException($"Không tìm thấy bộ phận với mã: {dto.DepartmentCode}");
            }

            // Lấy thông tin loại tài sản
            var category = _categoryRepository.GetByCode(dto.FixedAssetCategoryCode);
            if (category == null)
            {
                throw new NotFoundException($"Không tìm thấy loại tài sản với mã: {dto.FixedAssetCategoryCode}");
            }

            // Map DTO sang Entity
            var entity = new FixedAsset
            {
                // Khóa chính - tự sinh GUID mới cho tài sản
                FixedAssetId = Guid.NewGuid(),

                // Thông tin cơ bản từ form người dùng nhập
                FixedAssetCode = dto.FixedAssetCode,
                FixedAssetName = dto.FixedAssetName,

                // Thông tin bộ phận sử dụng (lấy từ bảng Department)
                DepartmentId = department.DepartmentId,
                DepartmentCode = department.DepartmentCode,
                DepartmentName = department.DepartmentName,

                // Thông tin loại tài sản (lấy từ bảng FixedAssetCategory)
                FixedAssetCategoryId = category.FixedAssetCategoryId,
                FixedAssetCategoryCode = category.FixedAssetCategoryCode,
                FixedAssetCategoryName = category.FixedAssetCategoryName,

                // Ngày mua và năm theo dõi (dựa theo ngày mua)
                PurchaseDate = dto.PurchaseDate,
                ProductionYear = dto.PurchaseDate.Year, // Năm sản xuất = năm mua
                TrackedYear = dto.PurchaseDate.Year,    // Năm theo dõi = năm mua

                // Thông tin khấu hao (theo loại tài sản)
                LifeTime = category.LifeTime,
                DepreciationRate = category.DepreciationRate,

                // Giá trị tài sản và tính toán khấu hao ban đầu
                Quantity = dto.Quantity,
                Cost = dto.Cost,
                DepreciationValue = dto.Cost * category.DepreciationRate / 100, // Giá trị hao mòn năm đầu

                // Mô tả thêm (nếu có)
                Description = dto.Description,

                // Trạng thái & audit info
                IsActive = true,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };

            return Create(entity);
        }

        /// <summary>
        /// Cập nhật tài sản từ DTO
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="id">ID tài sản</param>
        /// <param name="dto">DTO cập nhật</param>
        /// <returns>Số dòng affected</returns>
        public int UpdateFromDto(Guid id, FixedAssetUpdateDto dto)
        {
            // Lấy entity hiện tại
            var existing = GetById(id);

            // Validate DTO
            ValidateUpdateDto(dto);

            // Lấy thông tin bộ phận mới (nếu có thay đổi)
            var department = _departmentRepository.GetByCode(dto.DepartmentCode);
            if (department == null)
            {
                throw new NotFoundException($"Không tìm thấy bộ phận với mã: {dto.DepartmentCode}");
            }

            // Lấy thông tin loại tài sản mới (nếu có thay đổi)
            var category = _categoryRepository.GetByCode(dto.FixedAssetCategoryCode);
            if (category == null)
            {
                throw new NotFoundException($"Không tìm thấy loại tài sản với mã: {dto.FixedAssetCategoryCode}");
            }

            // Update entity
            existing.FixedAssetName = dto.FixedAssetName;
            existing.DepartmentId = department.DepartmentId;
            existing.DepartmentCode = department.DepartmentCode;
            existing.DepartmentName = department.DepartmentName;
            existing.FixedAssetCategoryId = category.FixedAssetCategoryId;
            existing.FixedAssetCategoryCode = category.FixedAssetCategoryCode;
            existing.FixedAssetCategoryName = category.FixedAssetCategoryName;
            existing.Quantity = dto.Quantity;
            existing.Cost = dto.Cost;
            existing.PurchaseDate = dto.PurchaseDate;
            existing.ProductionYear = dto.PurchaseDate.Year;
            existing.TrackedYear = dto.PurchaseDate.Year;
            existing.LifeTime = category.LifeTime;
            existing.DepreciationRate = category.DepreciationRate;
            existing.DepreciationValue = dto.Cost * category.DepreciationRate / 100;
            existing.Description = dto.Description;
            existing.ModifiedDate = DateTime.Now;

            return Update(id, existing);
        }

        /// <summary>
        /// Validate trước khi tạo mới
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="entity">Entity cần validate</param>
        protected override void ValidateBeforeCreate(FixedAsset entity)
        {
            var errors = new List<string>();

            // Kiểm tra mã tài sản không trùng
            if (_fixedAssetRepository.CheckCodeExists(entity.FixedAssetCode))
            {
                throw new ConflictException($"Mã tài sản '{entity.FixedAssetCode}' đã tồn tại trong hệ thống");
            }

            // Validate các trường bắt buộc
            if (string.IsNullOrWhiteSpace(entity.FixedAssetCode))
                errors.Add("Mã tài sản không được để trống");

            if (string.IsNullOrWhiteSpace(entity.FixedAssetName))
                errors.Add("Tên tài sản không được để trống");

            if (entity.Quantity <= 0)
                errors.Add("Số lượng phải lớn hơn 0");

            if (entity.Cost <= 0)
                errors.Add("Nguyên giá phải lớn hơn 0");

            // Ngày mua không được lớn hơn ngày hiện tại
            if (entity.PurchaseDate > DateTime.Now)
                errors.Add("Ngày mua không được lớn hơn ngày hiện tại");

            if (errors.Any())
            {
                throw new ValidateException(string.Join("; ", errors));
            }
        }

        /// <summary>
        /// Validate trước khi cập nhật
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="id">ID entity</param>
        /// <param name="entity">Entity cần validate</param>
        protected override void ValidateBeforeUpdate(Guid id, FixedAsset entity)
        {
            var errors = new List<string>();

            // Validate các trường bắt buộc
            if (string.IsNullOrWhiteSpace(entity.FixedAssetName))
                errors.Add("Tên tài sản không được để trống");

            if (entity.Quantity <= 0)
                errors.Add("Số lượng phải lớn hơn 0");

            if (entity.Cost <= 0)
                errors.Add("Nguyên giá phải lớn hơn 0");

            // Ngày mua không được lớn hơn ngày hiện tại
            if (entity.PurchaseDate > DateTime.Now)
                errors.Add("Ngày mua không được lớn hơn ngày hiện tại");

            if (errors.Any())
            {
                throw new ValidateException(string.Join("; ", errors));
            }
        }

        /// <summary>
        /// Validate DTO tạo mới
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="dto">DTO cần validate</param>
        private void ValidateCreateDto(FixedAssetCreateDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.FixedAssetCode))
                errors.Add("Mã tài sản không được để trống");

            if (string.IsNullOrWhiteSpace(dto.FixedAssetName))
                errors.Add("Tên tài sản không được để trống");

            if (string.IsNullOrWhiteSpace(dto.DepartmentCode))
                errors.Add("Mã bộ phận sử dụng không được để trống");

            if (string.IsNullOrWhiteSpace(dto.FixedAssetCategoryCode))
                errors.Add("Mã loại tài sản không được để trống");

            if (dto.Quantity <= 0)
                errors.Add("Số lượng phải lớn hơn 0");

            if (dto.Cost <= 0)
                errors.Add("Nguyên giá phải lớn hơn 0");
            // Ngày mua không được lớn hơn ngày hiện tại
            if (dto.PurchaseDate > DateTime.Now)
                errors.Add("Ngày mua không được lớn hơn ngày hiện tại");

            // Tỷ lệ hao mòn phải bằng 1 / Số năm sử dụng
            if (dto.LifeTime > 0 && dto.DepreciationRate > 0)
            {
                if (dto.DepreciationRate > 1m)
                {
                    dto.DepreciationRate /= 100;
                    decimal expectedRate = Math.Round(1m / dto.LifeTime, 5);
                    if (Math.Abs(dto.DepreciationRate - expectedRate) > 0.99999999m)
                    {
                        errors.Add("Tỷ lệ hao mòn phải bằng 1/Số năm sử dụng");
                    }
                }
                else
                {
                    decimal expectedRate = Math.Round(1m / dto.LifeTime, 5);
                    if (Math.Abs(dto.DepreciationRate - expectedRate) > 0.99999999m)
                    {
                        errors.Add("Tỷ lệ hao mòn phải bằng 1/Số năm sử dụng");
                    }
                }

            }

            // Hao mòn năm không được lớn hơn nguyên giá
            if (dto.DepreciationValue > dto.Cost)
            {
                errors.Add("Hao mòn năm phải nhỏ hơn hoặc bằng nguyên giá");
            }

            if (errors.Any())
            {
                throw new ValidateException(string.Join("; ", errors));
            }
        }

        /// <summary>
        /// Validate DTO cập nhật
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="dto">DTO cần validate</param>
        private void ValidateUpdateDto(FixedAssetUpdateDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.FixedAssetName))
                errors.Add("Tên tài sản không được để trống");

            if (string.IsNullOrWhiteSpace(dto.DepartmentCode))
                errors.Add("Mã bộ phận sử dụng không được để trống");

            if (string.IsNullOrWhiteSpace(dto.FixedAssetCategoryCode))
                errors.Add("Mã loại tài sản không được để trống");

            if (dto.Quantity <= 0)
                errors.Add("Số lượng phải lớn hơn 0");

            if (dto.Cost <= 0)
                errors.Add("Nguyên giá phải lớn hơn 0");
            // Ngày mua không được lớn hơn ngày hiện tại
            if (dto.PurchaseDate > DateTime.Now)
                errors.Add("Ngày mua không được lớn hơn ngày hiện tại");

            // Tỷ lệ hao mòn phải bằng 1 / Số năm sử dụng
            if (dto.LifeTime > 0 && dto.DepreciationRate > 0)
            {
                if (dto.DepreciationRate > 1m)
                {
                    dto.DepreciationRate /= 100;
                    decimal expectedRate = Math.Round(1m / dto.LifeTime, 5);
                    if (Math.Abs(dto.DepreciationRate - expectedRate) > 0.99999999m)
                    {
                        errors.Add("Tỷ lệ hao mòn phải bằng 1/Số năm sử dụng");
                    }
                }
                else
                {
                    decimal expectedRate = Math.Round(1m / dto.LifeTime, 5);
                    if (Math.Abs(dto.DepreciationRate - expectedRate) > 0.99999999m)
                    {
                        errors.Add("Tỷ lệ hao mòn phải bằng 1/Số năm sử dụng");
                    }
                }

            }

            // Hao mòn năm không được lớn hơn nguyên giá
            if (dto.DepreciationValue > dto.Cost)
            {
                errors.Add("Hao mòn năm phải nhỏ hơn hoặc bằng nguyên giá");
            }

            if (errors.Any())
            {
                throw new ValidateException(string.Join("; ", errors));
            }
        }
    }
}