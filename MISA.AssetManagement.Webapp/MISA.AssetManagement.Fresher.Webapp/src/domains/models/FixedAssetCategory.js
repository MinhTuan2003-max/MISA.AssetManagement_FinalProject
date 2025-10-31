/**
 * @fileoverview FixedAssetCategory Model
 * @description Đại diện cho đối tượng "Loại tài sản" (Fixed Asset Category)
 * - Map trực tiếp với DTO từ backend.
 * - Cung cấp phương thức helper để hiển thị trong UI (filter, form...).
 * @createdBy HMTuan (30/10/2025)
 */

export class FixedAssetCategory {
  /**
   * @constructor
   * @param {Object} data - Dữ liệu từ backend hoặc enum
   */
  constructor(data = {}) {
    /** @type {string|null} ID loại tài sản */
    this.fixed_asset_category_id = data.fixed_asset_category_id || null

    /** @type {string} Mã loại tài sản */
    this.fixed_asset_category_code = data.fixed_asset_category_code || ''

    /** @type {string} Tên viết tắt của loại tài sản */
    this.fixed_asset_category_short_name = data.fixed_asset_category_short_name || ''

    /** @type {string} Tên đầy đủ của loại tài sản */
    this.fixed_asset_category_name = data.fixed_asset_category_name || ''

    /** @type {number} Thời gian sử dụng (năm) */
    this.life_time = data.life_time || 0

    /** @type {number} Tỷ lệ hao mòn (%) */
    this.depreciation_rate = data.depreciation_rate || 0
  }

  /** @returns {string|null} ID loại tài sản */
  get id() {
    return this.fixed_asset_category_id
  }

  /** @returns {string} Mã loại tài sản */
  get code() {
    return this.fixed_asset_category_code
  }

  /** @returns {string} Tên viết tắt */
  get shortName() {
    return this.fixed_asset_category_short_name
  }

  /** @returns {string} Tên đầy đủ */
  get name() {
    return this.fixed_asset_category_name
  }

  /** @returns {number} Thời gian sử dụng (năm) */
  get lifeTime() {
    return this.life_time
  }

  /** @returns {number} Tỷ lệ hao mòn (%) */
  get depreciationRate() {
    return this.depreciation_rate
  }

  /**
   * Chuyển sang option cho **toolbar filter**
   * (Hiển thị tên đầy đủ)
   * @returns {{ value: string, label: string, fullName: string, id: string|null, life_time: number, depreciation_rate: number }}
   */
  toToolbarOption() {
    return {
      value: this.fixed_asset_category_code,
      label: this.fixed_asset_category_name,
      fullName: this.fixed_asset_category_name,
      id: this.fixed_asset_category_id,
      life_time: this.life_time,
      depreciation_rate: this.depreciation_rate
    }
  }

  /**
   * Chuyển sang option cho **form popup**
   * (Hiển thị tên viết tắt)
   * @returns {{ value: string, label: string, fullName: string, id: string|null, life_time: number, depreciation_rate: number }}
   */
  toFormOption() {
    return {
      value: this.fixed_asset_category_code,
      label: this.fixed_asset_category_short_name,
      fullName: this.fixed_asset_category_name,
      id: this.fixed_asset_category_id,
      life_time: this.life_time,
      depreciation_rate: this.depreciation_rate
    }
  }

  /**
   * Deprecated: Giữ lại cho backward compatibility.
   * @deprecated Dùng `toToolbarOption()` hoặc `toFormOption()` thay thế.
   * @returns {object}
   */
  toOption() {
    return this.toToolbarOption()
  }

  /**
   * Alias cho `toFormOption()`
   * @returns {object}
   */
  toSelectOption() {
    return this.toFormOption()
  }

  /**
   * Tạo instance từ dữ liệu API.
   * @param {object} data
   * @returns {FixedAssetCategory}
   */
  static fromApi(data) {
    return new FixedAssetCategory(data)
  }

  /**
   * Tạo mảng các instance từ danh sách API.
   * @param {object[]} dataArray
   * @returns {FixedAssetCategory[]}
   */
  static fromApiArray(dataArray) {
    if (!Array.isArray(dataArray)) return []
    return dataArray.map(data => new FixedAssetCategory(data))
  }
}
