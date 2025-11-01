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
            var department = _departmentRepository.GetByCode(dto.department_code);
            if (department == null)
            {
                throw new NotFoundException($"Không tìm thấy bộ phận với mã: {dto.department_code}");
            }

            // Lấy thông tin loại tài sản
            var category = _categoryRepository.GetByCode(dto.fixed_asset_category_code);
            if (category == null)
            {
                throw new NotFoundException($"Không tìm thấy loại tài sản với mã: {dto.fixed_asset_category_code}");
            }

            // Map DTO sang Entity
            var entity = new FixedAsset
            {
                fixed_asset_id = Guid.NewGuid(),
                fixed_asset_code = dto.fixed_asset_code,
                fixed_asset_name = dto.fixed_asset_name,

                department_id = department.department_id,
                department_code = department.department_code,
                department_name = department.department_name,

                fixed_asset_category_id = category.fixed_asset_category_id,
                fixed_asset_category_code = category.fixed_asset_category_code,
                fixed_asset_category_name = category.fixed_asset_category_name,

                purchase_date = dto.purchase_date,
                production_year = dto.purchase_date.Year,
                tracked_year = dto.purchase_date.Year,

                life_time = category.life_time,
                depreciation_rate = category.depreciation_rate,

                quantity = dto.quantity,
                cost = dto.cost,
                depreciation_value = dto.cost * category.depreciation_rate / 100,

                description = dto.description,
                is_active = true,
                created_date = DateTime.Now,
                modified_date = DateTime.Now
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
            var department = _departmentRepository.GetByCode(dto.department_code);
            if (department == null)
            {
                throw new NotFoundException($"Không tìm thấy bộ phận với mã: {dto.department_code}");
            }

            // Lấy thông tin loại tài sản mới (nếu có thay đổi)
            var category = _categoryRepository.GetByCode(dto.fixed_asset_category_code);
            if (category == null)
            {
                throw new NotFoundException($"Không tìm thấy loại tài sản với mã: {dto.fixed_asset_category_code}");
            }

            // Update entity
            existing.fixed_asset_name = dto.fixed_asset_name;
            existing.department_id = department.department_id;
            existing.department_code = department.department_code;
            existing.department_name = department.department_name;
            existing.fixed_asset_category_id = category.fixed_asset_category_id;
            existing.fixed_asset_category_code = category.fixed_asset_category_code;
            existing.fixed_asset_category_name = category.fixed_asset_category_name;
            existing.quantity = dto.quantity;
            existing.cost = dto.cost;
            existing.purchase_date = dto.purchase_date;
            existing.production_year = dto.purchase_date.Year;
            existing.tracked_year = dto.purchase_date.Year;
            existing.life_time = category.life_time;
            existing.depreciation_rate = category.depreciation_rate;
            existing.depreciation_value = dto.cost * category.depreciation_rate / 100;
            existing.description = dto.description;
            existing.modified_date = DateTime.Now;

            return Update(id, existing);
        }

        /// <summary>
        /// Nhân bản tài sản (copy và tự động tạo mã mới)
        /// CreatedBy: HMTuan (28/10/2025)
        /// </summary>
        /// <param name="id">ID tài sản gốc</param>
        /// <returns>Số dòng affected</returns>
        public int Duplicate(Guid id)
        {
            var original = GetById(id);

            // Tạo mã mới tự động
            var newCode = _fixedAssetRepository.GenerateNewCode();

            // Copy entity với mã mới
            var duplicated = new FixedAsset
            {
                fixed_asset_id = Guid.NewGuid(),
                fixed_asset_code = newCode,
                fixed_asset_name = original.fixed_asset_name + " (Copy)",

                department_id = original.department_id,
                department_code = original.department_code,
                department_name = original.department_name,

                fixed_asset_category_id = original.fixed_asset_category_id,
                fixed_asset_category_code = original.fixed_asset_category_code,
                fixed_asset_category_name = original.fixed_asset_category_name,

                purchase_date = DateTime.Now,
                production_year = DateTime.Now.Year,
                tracked_year = DateTime.Now.Year,

                life_time = original.life_time,
                depreciation_rate = original.depreciation_rate,

                quantity = original.quantity,
                cost = original.cost,
                depreciation_value = original.depreciation_value,

                description = original.description,
                is_active = true,
                created_date = DateTime.Now,
                modified_date = DateTime.Now
            };

            return Create(duplicated);
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
            if (_fixedAssetRepository.CheckCodeExists(entity.fixed_asset_code))
            {
                throw new ConflictException($"Mã tài sản '{entity.fixed_asset_code}' đã tồn tại trong hệ thống");
            }

            // Validate các trường bắt buộc
            if (string.IsNullOrWhiteSpace(entity.fixed_asset_code))
                errors.Add("Mã tài sản không được để trống");

            if (string.IsNullOrWhiteSpace(entity.fixed_asset_name))
                errors.Add("Tên tài sản không được để trống");

            if (entity.quantity <= 0)
                errors.Add("Số lượng phải lớn hơn 0");

            if (entity.cost <= 0)
                errors.Add("Nguyên giá phải lớn hơn 0");

            // Ngày mua không được lớn hơn ngày hiện tại
            if (entity.purchase_date > DateTime.Now)
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
            if (string.IsNullOrWhiteSpace(entity.fixed_asset_name))
                errors.Add("Tên tài sản không được để trống");

            if (entity.quantity <= 0)
                errors.Add("Số lượng phải lớn hơn 0");

            if (entity.cost <= 0)
                errors.Add("Nguyên giá phải lớn hơn 0");

            // Ngày mua không được lớn hơn ngày hiện tại
            if (entity.purchase_date > DateTime.Now)
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

            if (string.IsNullOrWhiteSpace(dto.fixed_asset_code))
                errors.Add("Mã tài sản không được để trống");

            if (string.IsNullOrWhiteSpace(dto.fixed_asset_name))
                errors.Add("Tên tài sản không được để trống");

            if (string.IsNullOrWhiteSpace(dto.department_code))
                errors.Add("Mã bộ phận sử dụng không được để trống");

            if (string.IsNullOrWhiteSpace(dto.fixed_asset_category_code))
                errors.Add("Mã loại tài sản không được để trống");

            if (dto.quantity <= 0)
                errors.Add("Số lượng phải lớn hơn 0");

            if (dto.cost <= 0)
                errors.Add("Nguyên giá phải lớn hơn 0");
            // Ngày mua không được lớn hơn ngày hiện tại
            if (dto.purchase_date > DateTime.Now)
                errors.Add("Ngày mua không được lớn hơn ngày hiện tại");

            // Tỷ lệ hao mòn phải bằng 1 / Số năm sử dụng
            if (dto.life_time > 0 && dto.depreciation_rate > 0)
            {
                if (dto.depreciation_rate > 1m)
                {
                    dto.depreciation_rate /= 100;
                    decimal expectedRate = Math.Round(1m / dto.life_time, 5);
                    if (Math.Abs(dto.depreciation_rate - expectedRate) > 0.99999999m)
                    {
                        errors.Add("Tỷ lệ hao mòn phải bằng 1/Số năm sử dụng");
                    }
                }
                else
                {
                    decimal expectedRate = Math.Round(1m / dto.life_time, 5);
                    if (Math.Abs(dto.depreciation_rate - expectedRate) > 0.99999999m)
                    {
                        errors.Add("Tỷ lệ hao mòn phải bằng 1/Số năm sử dụng");
                    }
                }

            }

            // Hao mòn năm không được lớn hơn nguyên giá
            if (dto.depreciation_value > dto.cost)
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

            if (string.IsNullOrWhiteSpace(dto.fixed_asset_name))
                errors.Add("Tên tài sản không được để trống");

            if (string.IsNullOrWhiteSpace(dto.department_code))
                errors.Add("Mã bộ phận sử dụng không được để trống");

            if (string.IsNullOrWhiteSpace(dto.fixed_asset_category_code))
                errors.Add("Mã loại tài sản không được để trống");

            if (dto.quantity <= 0)
                errors.Add("Số lượng phải lớn hơn 0");

            if (dto.cost <= 0)
                errors.Add("Nguyên giá phải lớn hơn 0");

            // Ngày mua không được lớn hơn ngày hiện tại
            if (dto.purchase_date > DateTime.Now)
                errors.Add("Ngày mua không được lớn hơn ngày hiện tại");

            // Tỷ lệ hao mòn phải bằng 1 / Số năm sử dụng
            if (dto.life_time > 0 && dto.depreciation_rate > 0)
            {
                if (dto.depreciation_rate > 1m)
                {
                    dto.depreciation_rate /= 100;
                    Console.WriteLine(dto.depreciation_rate);
                    decimal expectedRate = Math.Round(1m / dto.life_time, 2);
                    if (Math.Abs(dto.depreciation_rate - expectedRate) > 0.99999999m)
                    {
                        errors.Add("Tỷ lệ hao mòn phải bằng 1/Số năm sử dụng");
                    }
                }
                else
                {
                    decimal expectedRate = Math.Round(1m / dto.life_time, 2);
                    if (Math.Abs(dto.depreciation_rate - expectedRate) > 0.99999999m)
                    {
                        errors.Add("Tỷ lệ hao mòn phải bằng 1/Số năm sử dụng");
                    }
                }
            }

            // Hao mòn năm không được lớn hơn nguyên giá
            if (dto.depreciation_value > dto.cost)
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
