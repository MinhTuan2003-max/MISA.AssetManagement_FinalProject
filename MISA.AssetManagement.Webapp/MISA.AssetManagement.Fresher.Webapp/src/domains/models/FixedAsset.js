/**
 * Lớp mô hình đại diện cho Tài sản cố định (FixedAsset)
 * Dùng để ánh xạ dữ liệu từ API và chuẩn hóa dữ liệu hiển thị trong UI.
 * CreatedBy: HMTuan (30/10/2025)
 * UpdatedBy: HMTuan (01/11/2025) - Convert to PascalCase
 */
export class FixedAsset {
  /**
   * @constructor
   * @param {Object} [data={}] - Dữ liệu khởi tạo tài sản (nhận từ API hoặc form)
   */
  constructor(data = {}) {
// #region Thuộc tính cơ bản
    this.FixedAssetId = data.FixedAssetId || data.fixedAssetId || null
    this.FixedAssetCode = data.FixedAssetCode || data.fixedAssetCode || ''
    this.FixedAssetName = data.FixedAssetName || data.fixedAssetName || ''
    // #endregion

    // #region Thông tin bộ phận sử dụng
    this.DepartmentId = data.DepartmentId || data.departmentId || null
    this.DepartmentCode = data.DepartmentCode || data.departmentCode || ''
    this.DepartmentName = data.DepartmentName || data.departmentName || ''
    // #endregion

    // #region Thông tin loại tài sản
    this.FixedAssetCategoryId = data.FixedAssetCategoryId || data.fixedAssetCategoryId || null
    this.FixedAssetCategoryCode = data.FixedAssetCategoryCode || data.fixedAssetCategoryCode || ''
    this.FixedAssetCategoryName = data.FixedAssetCategoryName || data.fixedAssetCategoryName || ''
    // #endregion

    // #region Thông tin giá trị, hao mòn
    this.Quantity = data.Quantity || data.quantity || 0
    this.Cost = data.Cost || data.cost || 0
    this.LifeTime = data.LifeTime || data.lifeTime || 0
    this.DepreciationRate = data.DepreciationRate || data.depreciationRate || 0
    this.DepreciationValue = data.DepreciationValue || data.depreciationValue || 0
    this.AccumulatedDepreciation = data.DepreciationValue || data.depreciationValue || 0
    this.RemainingValue = data.Cost - data.DepreciationValue || data.cost - data.depreciationValue || 0
    // #endregion

    // #region Thông tin thời gian
    this.PurchaseDate = data.PurchaseDate || data.purchaseDate || ''
    this.StartUsingDate = data.StartUsingDate || data.startUsingDate || ''
    this.ProductionYear = data.ProductionYear || data.productionYear || 0
    this.TrackedYear = data.TrackedYear || data.trackedYear || 0
    // #endregion

    // #region Mô tả
    this.Description = data.Description || data.description || ''
    this.IsActive = data.IsActive || data.isActive || true
    this.CreatedDate = data.CreatedDate || data.createdDate || ''
    this.CreatedBy = data.CreatedBy || data.createdBy || null
    this.ModifiedDate = data.ModifiedDate || data.modifiedDate || ''
    this.ModifiedBy = data.ModifiedBy || data.modifiedBy || null
    // #endregion
  }

  // #region Getters

  /** @returns {string|null} ID tài sản */
  get id() {
    return this.FixedAssetId
  }

  /** @returns {string} Mã tài sản */
  get assetCode() {
    return this.FixedAssetCode
  }

  /** @returns {string} Tên tài sản */
  get assetName() {
    return this.FixedAssetName
  }

  /** @returns {string} Tên loại tài sản */
  get categoryName() {
    return this.FixedAssetCategoryName
  }

  /** @returns {string} Tên bộ phận sử dụng */
  get departmentName() {
    return this.DepartmentName
  }

  /** @returns {number} Giá trị hao mòn năm */
  get depreciationValue() {
    return this.DepreciationValue
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
      FixedAssetId: this.FixedAssetId,
      FixedAssetCode: this.FixedAssetCode,
      FixedAssetName: this.FixedAssetName,

      DepartmentCode: this.DepartmentCode,
      DepartmentName: this.DepartmentName,

      FixedAssetCategoryCode: this.FixedAssetCategoryCode,
      FixedAssetCategoryName: this.FixedAssetCategoryName,

      Quantity: this.Quantity,
      Cost: this.Cost,

      LifeTime: this.LifeTime,
      DepreciationRate: this.DepreciationRate,
      DepreciationValue: this.DepreciationValue,

      PurchaseDate: this.formatDateString(this.PurchaseDate),
      ProductionYear: this.ProductionYear,
      TrackedYear: this.TrackedYear,

      Description: this.Description
    }
  }

  /**
   * Chuyển đối tượng FixedAsset sang định dạng dùng cho bảng (table)
   * @returns {Object} Dữ liệu định dạng phục vụ table
   */
  toTableFormat() {
    return {
      FixedAssetId: this.FixedAssetId,
      CandidateID: this.FixedAssetId,

      FixedAssetCode: this.FixedAssetCode,
      AssetCode: this.FixedAssetCode,

      FixedAssetName: this.FixedAssetName,
      AssetName: this.FixedAssetName,

      DepartmentCode: this.DepartmentCode,
      DepartmentName: this.DepartmentName,

      FixedAssetCategoryCode: this.FixedAssetCategoryCode,
      FixedAssetCategoryName: this.FixedAssetCategoryName,
      AssetTypeName: this.FixedAssetCategoryName,

      Quantity: this.Quantity,
      Cost: this.Cost,

      AccumulatedDepreciation: this.AccumulatedDepreciation,
      RemainingValue: this.RemainingValue,
      DepreciationValue: this.DepreciationValue,

      PurchaseDate: this.PurchaseDate,
      ProductionYear: this.ProductionYear,

      TrackedYear: this.TrackedYear,
      LifeTime: this.LifeTime,
      DepreciationRate: this.DepreciationRate,
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
