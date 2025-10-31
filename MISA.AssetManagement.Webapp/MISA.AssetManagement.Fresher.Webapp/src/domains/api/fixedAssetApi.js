import { BaseApi } from './baseApi';
import { FixedAsset } from '@/domains/models/FixedAsset.js';

/**
 * API Service cho Tài sản cố định (FixedAsset)
 * Cung cấp các phương thức CRUD và filter cho tài sản
 * CreatedBy: HMTuan (29/10/2025)
 */
class FixedAssetApi extends BaseApi {
  constructor() {
    super('fixed-assets');
  }

  /**
   * Lấy danh sách tài sản với filter và phân trang
   * @param {Object} filterDto - DTO chứa bộ lọc và thông tin phân trang
   * @param {string} filterDto.keyword - từ khóa tìm kiếm
   * @param {string} filterDto.department_code - mã phòng ban
   * @param {string} filterDto.fixed_asset_category_code - mã loại tài sản
   * @param {number} filterDto.page_number - số trang
   * @param {number} filterDto.page_size - số bản ghi/trang
   * @returns {Promise<Object>} dữ liệu trả về gồm: assets (mảng FixedAsset), totalRecords, totalPages, currentPage, pageSize
   * CreatedBy: HMTuan (29/10/2025)
   */
  async getFixedAssets(filterDto = {}) {
    try {
      const params = {
        page_number: filterDto.page_number || 1,
        page_size: filterDto.page_size || 20
      };

      if (filterDto.keyword && filterDto.keyword.trim()) {
        params.keyword = filterDto.keyword.trim();
      }

      if (filterDto.department_code) {
        params.department_code = filterDto.department_code;
      }

      if (filterDto.fixed_asset_category_code) {
        params.fixed_asset_category_code = filterDto.fixed_asset_category_code;
      }

      const response = await this.getFiltered(params);

      return {
        assets: FixedAsset.fromApiArray(response.data || []),
        totalRecords: response.totalRecords || 0,
        totalPages: response.totalPages || 1,
        currentPage: response.currentPage || 1,
        pageSize: response.pageSize || 20
      };
    } catch (error) {
      console.error('Error fetching fixed assets:', error);
      throw error;
    }
  }

  /**
   * Gọi endpoint /filter để lọc tài sản
   * @param {Object} params - tham số filter
   * @returns {Promise<Object>} dữ liệu raw trả về từ API
   * CreatedBy: HMTuan (29/10/2025)
   */
  async getFiltered(params = {}) {
    // Axios tự động serialize params thành query string
    const res = await this.axiosInstance.get(`/${this.resource}/filter`, {
      params: params
    });
    return res.data;
  }

  /**
   * Lấy chi tiết một tài sản theo ID
   * @param {string|number} id - ID tài sản
   * @returns {Promise<FixedAsset>} đối tượng FixedAsset
   * CreatedBy: HMTuan (29/10/2025)
   */
  async getFixedAssetById(id) {
    const data = await this.getById(id);
    return FixedAsset.fromApi(data);
  }

  /**
   * Thêm mới một tài sản
   * @param {Object} assetData - dữ liệu tài sản
   * @returns {Promise<FixedAsset>} tài sản vừa tạo
   * CreatedBy: HMTuan (29/10/2025)
   */
  async createFixedAsset(assetData) {
    const data = await this.create(assetData);
    return FixedAsset.fromApi(data);
  }

  /**
   * Cập nhật tài sản theo ID
   * @param {string|number} id - ID tài sản
   * @param {Object} assetData - dữ liệu cập nhật
   * @returns {Promise<FixedAsset>} tài sản đã cập nhật
   * CreatedBy: HMTuan (29/10/2025)
   */
  async updateFixedAsset(id, assetData) {
    const data = await this.update(id, assetData);
    return FixedAsset.fromApi(data);
  }

  /**
   * Xóa một tài sản theo ID
   * @param {string|number} id - ID tài sản cần xóa
   * @returns {Promise<any>} kết quả xóa
   * CreatedBy: HMTuan (29/10/2025)
   */
  async deleteFixedAsset(id) {
    return await this.delete(id);
  }

  /**
   * Xóa nhiều tài sản theo danh sách ID
   * @param {Array<string|number>} ids - danh sách ID tài sản
   * @returns {Promise<any>} kết quả xóa
   * CreatedBy: HMTuan (29/10/2025)
   */
  async deleteFixedAssets(ids) {
    return await this.deleteBatch(ids);
  }

  /**
   * Nhân bản một tài sản
   * @param {string} id - UUID của tài sản cần nhân bản
   * @returns {Promise<FixedAsset>} tài sản mới nhân bản
   * CreatedBy: HMTuan (29/10/2025)
   */
  async duplicateFixedAsset(id) {
    try {
      const response = await this.axiosInstance.post(`/${this.resource}/${id}/duplicate`);
      return FixedAsset.fromApi(response.data);
    } catch (error) {
      throw error;
    }
  }
}

export default new FixedAssetApi();
