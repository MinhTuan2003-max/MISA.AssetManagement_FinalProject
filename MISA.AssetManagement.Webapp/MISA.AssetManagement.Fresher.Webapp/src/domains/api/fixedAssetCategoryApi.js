/**
 * FixedAssetCategoryApi
 * Service quản lý API cho "Loại tài sản" (FixedAssetCategory)
 * - Kế thừa từ lớp BaseApi
 * - Có cơ chế fallback sang enum nếu API gặp lỗi
 * @createdBy HMTuan (29/10/2025)
 */

import { BaseApi } from '@/domains/api/baseApi.js'
import { FixedAssetCategory } from '@/domains/models/FixedAssetCategory.js'
import { getFixedAssetCategoryOptions } from '@/domains/enums/fixedAssetCategoryEnum.js'

/**
 * @class FixedAssetCategoryApi
 * @extends BaseApi
 * @classdesc Quản lý các thao tác gọi API liên quan đến "Loại tài sản".
 */
class FixedAssetCategoryApi extends BaseApi {
  /**
   * Khởi tạo endpoint `/api/fixed-asset-categories`
   */
  constructor() {
    super('fixed-asset-categories')
  }

  /**
   * Lấy danh sách loại tài sản.
   * Nếu backend lỗi hoặc không có API, fallback sang enum tĩnh.
   *
   * @async
   * @function getFixedAssetCategories
   * @returns {Promise<FixedAssetCategory[]>} Danh sách đối tượng FixedAssetCategory
   * @throws {Error} Nếu API và fallback đều lỗi
   */
  async getFixedAssetCategories() {
    try {
      // Gọi API chính
      const data = await this.getAll()

      // Chuyển dữ liệu API sang model chuẩn
      return FixedAssetCategory.fromApiArray(data)
    } catch (error) {
      console.warn('Lỗi khi gọi API fixed asset category, dùng enum fallback:', error)

      // Fallback sang enum tĩnh, map về model tương tự
      const fallbackData = getFixedAssetCategoryOptions().map(opt => ({
        fixed_asset_category_id: opt.value,
        fixed_asset_category_code: opt.value,
        fixed_asset_category_name: opt.label
      }))

      return FixedAssetCategory.fromApiArray(fallbackData)
    }
  }
}

export default new FixedAssetCategoryApi()
