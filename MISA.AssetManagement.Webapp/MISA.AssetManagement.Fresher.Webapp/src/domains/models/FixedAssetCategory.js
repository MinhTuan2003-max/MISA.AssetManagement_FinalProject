/**
 * @fileoverview FixedAssetCategory Model
 * @description Đại diện cho đối tượng "Loại tài sản" (Fixed Asset Category)
 * - Map trực tiếp với DTO từ backend.
 * - Cung cấp phương thức helper để hiển thị trong UI (filter, form...).
 * @createdBy HMTuan (30/10/2025)
 */
export class FixedAssetCategory {
  constructor(data = {}) {
    this.FixedAssetCategoryId = data.FixedAssetCategoryId || data.fixedAssetCategoryId || null
    this.FixedAssetCategoryCode = data.FixedAssetCategoryCode || data.fixedAssetCategoryCode || ''
    this.FixedAssetCategoryShortName = data.FixedAssetCategoryShortName || data.fixed_asset_category_short_name || data.fixedAssetCategoryShortName || ''
    this.FixedAssetCategoryName = data.FixedAssetCategoryName || data.fixedAssetCategoryName || ''
    this.LifeTime = data.LifeTime || data.lifeTime || 0
    this.DepreciationRate = data.DepreciationRate || data.depreciationRate || 0
  }

  get id() {
    return this.FixedAssetCategoryId
  }

  get code() {
    return this.FixedAssetCategoryCode
  }

  get shortName() {
    return this.FixedAssetCategoryShortName
  }

  get name() {
    return this.FixedAssetCategoryName
  }

  get lifeTime() {
    return this.LifeTime
  }

  get depreciationRate() {
    return this.DepreciationRate
  }

  toToolbarOption() {
    return {
      value: this.FixedAssetCategoryCode,
      label: this.FixedAssetCategoryName,
      fullName: this.FixedAssetCategoryName,
      id: this.FixedAssetCategoryId,
      lifeTime: this.LifeTime,
      depreciationRate: this.DepreciationRate
    }
  }

  toFormOption() {
    return {
      value: this.FixedAssetCategoryCode,
      label: this.FixedAssetCategoryShortName,
      fullName: this.FixedAssetCategoryName,
      id: this.FixedAssetCategoryId,
      lifeTime: this.LifeTime,
      depreciationRate: this.DepreciationRate
    }
  }

  toOption() {
    return this.toToolbarOption()
  }

  toSelectOption() {
    return this.toFormOption()
  }

  static fromApi(data) {
    return new FixedAssetCategory(data)
  }

  static fromApiArray(dataArray) {
    if (!Array.isArray(dataArray)) return []
    return dataArray.map(data => new FixedAssetCategory(data))
  }
}
