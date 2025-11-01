/**
 * DepartmentApi
 * Service quản lý API cho "Bộ phận sử dụng" (Department)
 * - Kế thừa từ lớp BaseApi
 * - Có fallback sang enum nếu API lỗi
 * @createdBy HMTuan (29/10/2025)
 * @updatedBy HMTuan (01/11/2025) - Convert to PascalCase
 */

import { BaseApi } from '@/domains/api/baseApi.js';
import { Department } from '@/domains/models/Department.js';
import { getDepartmentOptions } from '@/domains/enums/departmentEnum.js';

/**
 * @class DepartmentApi
 * @extends BaseApi
 * @classdesc Quản lý các thao tác gọi API liên quan đến "Bộ phận sử dụng".
 */
class DepartmentApi extends BaseApi {
  /**
   * Khởi tạo endpoint `/api/departments`
   */
  constructor() {
    super('departments');
  }

  /**
   * Lấy danh sách bộ phận sử dụng từ API.
   * Nếu API xảy ra lỗi, tự động fallback sang dữ liệu tĩnh trong `departmentEnum`.
   *
   * @async
   * @function getDepartments
   * @returns {Promise<Department[]>} Danh sách đối tượng Department
   * @throws {Error} Nếu cả API và fallback đều lỗi
   */
  async getDepartments() {
    try {
      // Gọi API chính
      const data = await this.getAll();
      return Department.fromApiArray(data);
    } catch (error) {
      console.warn('Lỗi khi gọi API department, dùng enum fallback:', error);

      // Dữ liệu fallback từ enum nếu API lỗi
      const fallbackData = getDepartmentOptions().map(opt => ({
        DepartmentId: null,
        DepartmentCode: opt.DepartmentCode,
        DepartmentName: opt.DepartmentName
      }));

      return Department.fromApiArray(fallbackData);
    }
  }
}

export default new DepartmentApi();
