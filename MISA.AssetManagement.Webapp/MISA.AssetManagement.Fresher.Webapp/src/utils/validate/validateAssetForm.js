/**
 * Validate Fixed Asset form data
 * @param {Object} formData - Form data sử dụng để validate
 * @returns {Object} - { errors: {}, isValid: boolean }
 */
export function validateAssetForm(formData) {
  const errors = {}

  // Tên tài sản
  if (!formData.fixed_asset_name?.trim()) {
    errors.fixed_asset_name = 'Tên tài sản không được để trống'
  }

  // Mã bộ phận sử dụng
  if (!formData.department_code) {
    errors.department_code = 'Mã bộ phận sử dụng không được để trống'
  }

  // Mã loại tài sản
  if (!formData.fixed_asset_category_code) {
    errors.fixed_asset_category_code = 'Mã loại tài sản không được để trống'
  }

  // Số lượng
  if (!formData.quantity || formData.quantity < 1) {
    errors.quantity = 'Số lượng phải lớn hơn 0'
  }

  // Nguyên giá
  if (formData.cost === null || formData.cost === undefined || formData.cost < 0) {
    errors.cost = 'Nguyên giá không được âm'
  }

  // Ngày mua
  if (!formData.purchase_date) {
    errors.purchase_date = 'Ngày mua không được để trống'
  }

  // Ngày bắt đầu sử dụng
  if (!formData.start_using_date) {
    errors.start_using_date = 'Ngày bắt đầu sử dụng không được để trống'
  }

  const today = new Date()
  const purchaseDate = formData.purchase_date ? new Date(formData.purchase_date) : null
  const startUsingDate = formData.start_using_date ? new Date(formData.start_using_date) : null

  // Ngày mua không được lớn hơn ngày hiện tại
  if (purchaseDate && purchaseDate > today) {
    errors.purchase_date = 'Ngày mua không được lớn hơn ngày hiện tại'
  }

  // Ngày bắt đầu sử dụng không được lớn hơn ngày mua
  if (purchaseDate && startUsingDate && startUsingDate > purchaseDate) {
    errors.start_using_date = 'Ngày bắt đầu sử dụng không được lớn hơn ngày mua'
  }

  // Số năm sử dụng
  if (formData.life_time === null || formData.life_time === undefined || formData.life_time <= 0) {
    errors.life_time = 'Số năm sử dụng phải lớn hơn 0'
  }

  // Giá trị hao mòn năm
  if (formData.depreciation_value === null || formData.depreciation_value === undefined || formData.depreciation_value < 0) {
    errors.depreciation_value = 'Giá trị hao mòn năm không được âm'
  }

  if (formData.life_time > 0) {
    // Tỷ lệ hao mòn phải bằng 1 / Số năm sử dụng
    if (formData.depreciation_rate > 1) {
      formData.depreciation_rate /= 100;
      const expectedRate = Math.round((1 / formData.life_time) * 100) / 100
      const actualRate = formData.depreciation_rate

      // So sánh sai số cho phép nhỏ (tránh lỗi số thực)
      const EPSILON = 0.9999999
      if (actualRate !== undefined && Math.abs(actualRate - expectedRate) > EPSILON) {
        errors.depreciation_rate = 'Tỷ lệ hao mòn phải bằng 1/Số năm sử dụng'
      }
    } else  {
      const expectedRate = Math.round((1 / formData.life_time) * 100) / 100
      const actualRate = formData.depreciation_rate

      // So sánh sai số cho phép nhỏ (tránh lỗi số thực)
      const EPSILON = 0.000001
      if (actualRate !== undefined && Math.abs(actualRate - expectedRate) > EPSILON) {
        errors.depreciation_rate = 'Tỷ lệ hao mòn phải bằng 1/Số năm sử dụng'
      }
    }
  }

  // Hao mòn năm không được lớn hơn nguyên giá
  if (formData.depreciation_value > formData.cost) {
    errors.depreciation_value = 'Hao mòn năm phải nhỏ hơn hoặc bằng nguyên giá'
  }

  return {
    errors,
    isValid: Object.keys(errors).length === 0
  }
}
