/**
 * Lớp mô hình đại diện cho Tài sản cố định (FixedAsset)
 * Dùng để ánh xạ dữ liệu từ API và chuẩn hóa dữ liệu hiển thị trong UI.
 * CreatedBy: HMTuan (30/10/2025)
 */
export class FixedAsset {
  /**
   * @constructor
   * @param {Object} [data={}] - Dữ liệu khởi tạo tài sản (nhận từ API hoặc form)
   */
  constructor(data = {}) {
    // #region Thuộc tính cơ bản
    this.fixed_asset_id = data.fixed_asset_id || null
    this.fixed_asset_code = data.fixed_asset_code || ''
    this.fixed_asset_name = data.fixed_asset_name || ''
    // #endregion

    // #region Thông tin bộ phận sử dụng
    this.department_id = data.department_id || null
    this.department_code = data.department_code || ''
    this.department_name = data.department_name || ''
    // #endregion

    // #region Thông tin loại tài sản
    this.fixed_asset_category_id = data.fixed_asset_category_id || null
    this.fixed_asset_category_code = data.fixed_asset_category_code || ''
    this.fixed_asset_category_name = data.fixed_asset_category_name || ''
    // #endregion

    // #region Thông tin giá trị, hao mòn
    this.quantity = data.quantity || 0
    this.cost = data.cost || 0
    this.life_time = data.life_time || 0
    this.depreciation_rate = data.depreciation_rate || 0
    this.depreciation_value = data.depreciation_value || 0
    this.accumulated_depreciation = data.accumulated_depreciation || 0
    this.remaining_value = data.remaining_value || 0
    // #endregion

    // #region Thông tin thời gian
    this.purchase_date = data.purchase_date || ''
    this.start_using_date = data.start_using_date || ''
    this.production_year = data.production_year || 0
    this.tracked_year = data.tracked_year || 0
    // #endregion

    // #region Mô tả
    this.description = data.description || ''
    // #endregion
  }

  // #region Getters

  /** @returns {string|null} ID tài sản */
  get id() {
    return this.fixed_asset_id
  }

  /** @returns {string} Mã tài sản */
  get assetCode() {
    return this.fixed_asset_code
  }

  /** @returns {string} Tên tài sản */
  get assetName() {
    return this.fixed_asset_name
  }

  /** @returns {string} Tên loại tài sản */
  get categoryName() {
    return this.fixed_asset_category_name
  }

  /** @returns {string} Tên bộ phận sử dụng */
  get departmentName() {
    return this.department_name
  }

  /** @returns {number} Giá trị hao mòn năm */
  get depreciationValue() {
    return this.depreciation_value
  }
  // #endregion

  // #region Hàm tiện ích
  /**
   * Chuẩn hóa chuỗi ngày theo định dạng yyyy-MM-dd
   * @param {string|Date} dateStr - Chuỗi ngày hoặc đối tượng Date
   * @returns {string} Ngày đã format
   */
  formatDateString(dateStr) {
    if (!dateStr) return ''
    if (typeof dateStr === 'string') {
      return dateStr.includes('T') ? dateStr.split('T')[0] : dateStr
    }
    const date = new Date(dateStr)
    return date.toISOString().split('T')[0]
  }
  // #endregion

  // #region Chuyển đổi dữ liệu

  /**
   * Chuyển đối tượng FixedAsset sang định dạng dùng cho form chỉnh sửa
   * @returns {Object} Dữ liệu định dạng phục vụ edit popup
   */
  toEditFormat() {
    return {
      fixed_asset_id: this.fixed_asset_id,
      fixed_asset_code: this.fixed_asset_code,
      fixed_asset_name: this.fixed_asset_name,

      department_code: this.department_code,
      department_name: this.department_name,

      fixed_asset_category_code: this.fixed_asset_category_code,
      fixed_asset_category_name: this.fixed_asset_category_name,

      quantity: this.quantity,
      cost: this.cost,

      life_time: this.life_time,
      depreciation_rate: this.depreciation_rate,
      depreciation_value: this.depreciation_value,

      purchase_date: this.formatDateString(this.purchase_date),
      production_year: this.production_year,
      tracked_year: this.tracked_year,

      description: this.description
    }
  }

  /**
   * Chuyển đối tượng FixedAsset sang định dạng dùng cho bảng (table)
   * @returns {Object} Dữ liệu định dạng phục vụ table
   */
  toTableFormat() {
    return {
      fixed_asset_id: this.fixed_asset_id,
      CandidateID: this.fixed_asset_id,

      fixed_asset_code: this.fixed_asset_code,
      AssetCode: this.fixed_asset_code,

      fixed_asset_name: this.fixed_asset_name,
      AssetName: this.fixed_asset_name,

      department_code: this.department_code,
      DepartmentCode: this.department_code,
      department_name: this.department_name,
      DepartmentName: this.department_name,

      fixed_asset_category_code: this.fixed_asset_category_code,
      FixedAssetCategoryCode: this.fixed_asset_category_code,
      fixed_asset_category_name: this.fixed_asset_category_name,
      AssetTypeName: this.fixed_asset_category_name,

      quantity: this.quantity,
      Quantity: this.quantity,

      cost: this.cost,
      Cost: this.cost,

      accumulated_depreciation: this.accumulated_depreciation,
      AccumulatedDepreciation: this.accumulated_depreciation,

      remaining_value: this.remaining_value,
      RemainingValue: this.remaining_value,

      depreciation_value: this.depreciation_value,
      DepreciationValue: this.depreciation_value,

      purchase_date: this.purchase_date,
      PurchaseDate: this.purchase_date,

      production_year: this.production_year,
      ProductionYear: this.production_year,

      tracked_year: this.tracked_year,
      TrackedYear: this.tracked_year,

      life_time: this.life_time,
      LifeTime: this.life_time,

      depreciation_rate: this.depreciation_rate,
      DepreciationRate: this.depreciation_rate
    }
  }
  // #endregion

  // #region Static methods
  /**
   * Tạo đối tượng FixedAsset từ dữ liệu API
   * @param {Object} data - Dữ liệu trả về từ API
   * @returns {FixedAsset}
   */
  static fromApi(data) {
    return new FixedAsset(data)
  }

  /**
   * Tạo danh sách FixedAsset từ mảng dữ liệu API
   * @param {Array<Object>} dataArray - Danh sách dữ liệu tài sản
   * @returns {FixedAsset[]}
   */
  static fromApiArray(dataArray) {
    return dataArray.map(data => new FixedAsset(data))
  }
  // #endregion
}
