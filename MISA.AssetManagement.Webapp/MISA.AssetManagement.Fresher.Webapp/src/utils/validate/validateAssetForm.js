/**
 * Validate Fixed Asset form data
 * Chấp nhận cả PascalCase (frontend) và snake_case (fallback)
 * @param {Object} formData - Form data sử dụng để validate
 * @returns {Object} - { errors: {}, isValid: boolean }
 * UpdatedBy: HMTuan (01/11/2025)
 */
export function validateAssetForm(formData) {
  const errors = {}

  // Helper function: Lấy giá trị từ cả hai format
  const getValue = (pascalKey, snakeKey) => {
    return formData[pascalKey] !== undefined ? formData[pascalKey] : formData[snakeKey]
  }

  // Helper function: Lấy key lỗi
  const getErrorKey = (pascalKey, snakeKey) => {
    return formData[pascalKey] !== undefined ? pascalKey : snakeKey
  }

  // Tên tài sản
  const fixedAssetName = getValue('FixedAssetName', 'fixed_asset_name')
  const fixedAssetNameKey = getErrorKey('FixedAssetName', 'fixed_asset_name')
  if (!fixedAssetName?.toString().trim()) {
    errors[fixedAssetNameKey] = 'Tên tài sản không được để trống'
  }

  // Mã bộ phận sử dụng
  const departmentCode = getValue('DepartmentCode', 'department_code')
  const departmentCodeKey = getErrorKey('DepartmentCode', 'department_code')
  if (!departmentCode) {
    errors[departmentCodeKey] = 'Mã bộ phận sử dụng không được để trống'
  }

  // Mã loại tài sản
  const categoryCode = getValue('FixedAssetCategoryCode', 'fixed_asset_category_code')
  const categoryCodeKey = getErrorKey('FixedAssetCategoryCode', 'fixed_asset_category_code')
  if (!categoryCode) {
    errors[categoryCodeKey] = 'Mã loại tài sản không được để trống'
  }

  // Số lượng
  const quantity = getValue('Quantity', 'quantity')
  const quantityKey = getErrorKey('Quantity', 'quantity')
  if (!quantity || parseInt(quantity, 10) < 1) {
    errors[quantityKey] = 'Số lượng phải lớn hơn 0'
  }

  // Nguyên giá
  const cost = getValue('Cost', 'cost')
  const costKey = getErrorKey('Cost', 'cost')
  if (cost === null || cost === undefined || parseFloat(cost) < 0) {
    errors[costKey] = 'Nguyên giá không được âm'
  }

  // Ngày mua
  const purchaseDate = getValue('PurchaseDate', 'purchase_date')
  const purchaseDateKey = getErrorKey('PurchaseDate', 'purchase_date')
  if (!purchaseDate) {
    errors[purchaseDateKey] = 'Ngày mua không được để trống'
  }

  // Ngày bắt đầu sử dụng
  const startUsingDate = getValue('StartUsingDate', 'start_using_date')
  const startUsingDateKey = getErrorKey('StartUsingDate', 'start_using_date')
  if (!startUsingDate) {
    errors[startUsingDateKey] = 'Ngày bắt đầu sử dụng không được để trống'
  }

  // Kiểm tra ngày mua không được lớn hơn ngày hiện tại
  const today = new Date()
  today.setHours(0, 0, 0, 0)

  const purchaseDateObj = purchaseDate ? new Date(purchaseDate) : null
  if (purchaseDateObj) {
    purchaseDateObj.setHours(0, 0, 0, 0)
    if (purchaseDateObj > today) {
      errors[purchaseDateKey] = 'Ngày mua không được lớn hơn ngày hiện tại'
    }
  }

  // Kiểm tra ngày bắt đầu sử dụng không được lớn hơn ngày mua
  const startUsingDateObj = startUsingDate ? new Date(startUsingDate) : null
  if (purchaseDateObj && startUsingDateObj) {
    startUsingDateObj.setHours(0, 0, 0, 0)
    if (startUsingDateObj > purchaseDateObj) {
      errors[startUsingDateKey] = 'Ngày bắt đầu sử dụng không được lớn hơn ngày mua'
    }
  }

  // Số năm sử dụng
  const lifeTime = getValue('LifeTime', 'life_time')
  const lifeTimeKey = getErrorKey('LifeTime', 'life_time')
  if (lifeTime === null || lifeTime === undefined || parseInt(lifeTime, 10) <= 0) {
    errors[lifeTimeKey] = 'Số năm sử dụng phải lớn hơn 0'
  }

  // Tỷ lệ hao mòn
  let depreciationRate = getValue('DepreciationRate', 'depreciation_rate')
  const depreciationRateKey = getErrorKey('DepreciationRate', 'depreciation_rate')

  // Convert % sang decimal nếu cần (>1 = %, <1 = decimal)
  if (depreciationRate > 1) {
    depreciationRate = depreciationRate / 100
  }

  // Kiểm tra tỷ lệ hao mòn = 1 / Số năm sử dụng
  if (lifeTime && parseInt(lifeTime, 10) > 0) {
    const expectedRate = Math.round((1 / parseInt(lifeTime, 10)) * 10000) / 10000
    const actualRate = Math.round(parseFloat(depreciationRate) * 10000) / 10000
    const EPSILON = 0.0001

    if (depreciationRate !== undefined && Math.abs(actualRate - expectedRate) > EPSILON) {
      errors[depreciationRateKey] = `Tỷ lệ hao mòn phải bằng 1/${parseInt(lifeTime, 10)} ≈ ${(expectedRate * 100).toFixed(2)}%`
    }
  }

  // Giá trị hao mòn năm
  const depreciationValue = getValue('DepreciationValue', 'depreciation_value')
  const depreciationValueKey = getErrorKey('DepreciationValue', 'depreciation_value')
  if (depreciationValue === null || depreciationValue === undefined || parseFloat(depreciationValue) < 0) {
    errors[depreciationValueKey] = 'Giá trị hao mòn năm không được âm'
  }

  // Hao mòn năm không được lớn hơn nguyên giá
  if (cost !== null && cost !== undefined && depreciationValue !== null && depreciationValue !== undefined) {
    if (parseFloat(depreciationValue) > parseFloat(cost)) {
      errors[depreciationValueKey] = 'Hao mòn năm phải nhỏ hơn hoặc bằng nguyên giá'
    }
  }

  return {
    errors,
    isValid: Object.keys(errors).length === 0
  }
}
