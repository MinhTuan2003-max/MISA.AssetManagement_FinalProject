import { ref, computed, watch, nextTick } from 'vue'
import { validateAssetForm } from '@/utils/validate/validateAssetForm.js'

/**
 * Logic xử lý form tài sản
 * CreatedBy: HMTuan (29/10/2025)
 */
export function useAssetFormLogic(props, emit) {
  //#region Helper Functions

  /**
   * Hàm trả về chuỗi ngày hiện tại theo định dạng yyyy-MM-dd
   * CreatedBy: HMTuan (29/10/2025)
   * @return {string} Ngày hiện tại (vd: "2025-11-03")
   */
  function getTodayString() {
    const today = new Date()
    const year = today.getFullYear()
    const month = String(today.getMonth() + 1).padStart(2, '0')
    const day = String(today.getDate()).padStart(2, '0')
    return `${year}-${month}-${day}`
  }

  /**
   * Hàm sinh mã tài sản mới dựa trên danh sách mã đã có
   * CreatedBy: HMTuan (29/10/2025)
   * @return {string} Mã tài sản mới (vd: "TS00001")
   */
  function generateAssetCode() {
    if (props.existingAssetCodes.length === 0) return 'TS000001'

    const numbers = props.existingAssetCodes
      .map(code => {
        const match = code.match(/\d+$/)
        return match ? parseInt(match[0], 10) : 0
      })
      .filter(n => n > 0)

    const maxNumber = Math.max(...numbers, 0)

    // Format với 5 chữ số
    return `TS${String(maxNumber + 1).padStart(6, '0')}`
  }

  /**
   * Hàm định dạng giá trị tiền tệ theo chuẩn 'vi-VN'
   * CreatedBy: HMTuan (29/10/2025)
   * @param {number|string} value - Giá trị cần định dạng
   * @return {string} Chuỗi tiền tệ đã format
   */
  function formatCurrency(value) {
    if (value == null || value === '') return ''
    return new Intl.NumberFormat('vi-VN').format(value)
  }

  /**
   * Hàm chuyển đổi chuỗi ngày ISO sang định dạng yyyy-MM-dd
   * CreatedBy: HMTuan (29/10/2025)
   * @param {string} dateStr - Chuỗi ngày cần chuẩn hóa
   * @return {string} Ngày hợp lệ dạng yyyy-MM-dd
   */
  function normalizeDateString(dateStr) {
    if (!dateStr) return getTodayString()

    const date = new Date(dateStr)
    if (isNaN(date.getTime())) return getTodayString()

    const year = date.getFullYear()
    const month = String(date.getMonth() + 1).padStart(2, '0')
    const day = String(date.getDate()).padStart(2, '0')
    return `${year}-${month}-${day}`
  }
  //#endregion

  //#region Form State
  const formData = ref({
    FixedAssetCode: '',
    FixedAssetName: '',
    DepartmentCode: null,
    FixedAssetCategoryCode: null,
    Quantity: 1,
    Cost: 0,
    PurchaseDate: getTodayString(),
    StartUsingDate: getTodayString(),
    ProductionYear: new Date().getFullYear(),
    TrackedYear: new Date().getFullYear(),
    LifeTime: 0,
    DepreciationRate: 0,
    DepreciationValue: 0,
    Description: ''
  })

  const errors = ref({})
  //#endregion

  //#region Computed

  /**
   * Computed: Tiêu đề dialog dựa theo chế độ của form (add, duplicate, edit)
   * CreatedBy: HMTuan (29/10/2025)
   * @return {string} Tiêu đề của form
   */
  const dialogTitle = computed(() => {
    if (props.mode === 'add') return 'Thêm tài sản'
    if (props.mode === 'duplicate') return 'Nhân bản tài sản'
    return 'Sửa tài sản'
  })

  /**
   * Computed: Lấy tên phòng ban dựa trên DepartmentCode được chọn
   * CreatedBy: HMTuan (29/10/2025)
   * @return {string} Tên phòng ban tương ứng
   */
  const departmentName = computed(() => {
    if (!formData.value.DepartmentCode) return ''
    const dept = props.departmentOptions.find(d => d.value === formData.value.DepartmentCode)
    return dept ? dept.fullName : ''
  })

  /**
   * Computed: Lấy tên loại tài sản dựa trên FixedAssetCategoryCode được chọn
   * CreatedBy: HMTuan (29/10/2025)
   * @return {string} Tên loại tài sản tương ứng
   */
  const categoryName = computed(() => {
    if (!formData.value.FixedAssetCategoryCode) return ''
    const cat = props.categoryOptions.find(c => c.value === formData.value.FixedAssetCategoryCode)
    return cat ? cat.fullName : ''
  })
  //#endregion

  //#region Watchers

  /**
   * Watcher: Theo dõi props.isOpen để load hoặc reset dữ liệu khi form mở/đóng
   * CreatedBy: HMTuan (29/10/2025)
   */
  watch(() => props.isOpen, async (newVal) => {
    if (newVal) {
      await nextTick()
      if (props.mode === 'add') {
        resetForm()
      } else if ((props.mode === 'edit' || props.mode === 'duplicate') && props.initialData) {
        loadInitialData(props.initialData)
      }
    } else {
      setTimeout(() => {
        resetForm()
      }, 300)
    }
  })

  /**
   * Watcher: Theo dõi FixedAssetCategoryCode để cập nhật LifeTime và DepreciationRate
   * CreatedBy: HMTuan (29/10/2025)
   */
  watch(() => formData.value.FixedAssetCategoryCode, (newCode) => {
    if (!newCode) {
      formData.value.LifeTime = 0
      formData.value.DepreciationRate = 0
      return
    }
    const category = props.categoryOptions.find(c => c.value === newCode)
    if (category) {
      formData.value.LifeTime = category.lifeTime || 0
      formData.value.DepreciationRate = category.depreciationRate || 0
    }
  })

  /**
   * Watcher: Theo dõi PurchaseDate để cập nhật ProductionYear và TrackedYear
   * CreatedBy: HMTuan (29/10/2025)
   */
  watch(() => formData.value.PurchaseDate, (newDate) => {
    if (!newDate) return
    const date = new Date(newDate)
    if (isNaN(date.getTime())) return
    const year = date.getFullYear()
    formData.value.ProductionYear = year
    formData.value.TrackedYear = year
  })

  /**
   * Watcher: Theo dõi Cost và DepreciationRate để tính DepreciationValue
   * CreatedBy: HMTuan (29/10/2025)
   */
  watch([() => formData.value.Cost, () => formData.value.DepreciationRate], ([cost, rate]) => {
    if (cost && rate) {
      formData.value.DepreciationValue = (cost * rate) / 100
    } else {
      formData.value.DepreciationValue = 0
    }
  })
  //#endregion

  //#region Methods

  /**
   * Hàm reset toàn bộ dữ liệu form về giá trị mặc định
   * CreatedBy: HMTuan (29/10/2025)
   */
  function resetForm() {
    const today = getTodayString()
    const currentYear = new Date().getFullYear()
    const newCode = props.mode === 'add' ? generateAssetCode() : ''

    formData.value = {
      FixedAssetCode: newCode,
      FixedAssetName: '',
      DepartmentCode: null,
      FixedAssetCategoryCode: null,
      Quantity: 1,
      Cost: 0,
      PurchaseDate: today,
      StartUsingDate: today,
      ProductionYear: currentYear,
      TrackedYear: currentYear,
      LifeTime: 0,
      DepreciationRate: 0,
      DepreciationValue: 0,
      Description: ''
    }
    errors.value = {}
  }

  /**
   * Hàm load dữ liệu ban đầu vào form khi sửa hoặc nhân bản
   * CreatedBy: HMTuan (29/10/2025)
   * @param {Object} data - Dữ liệu tài sản ban đầu
   */
  function loadInitialData(data) {
    if (!data) return

    const normalized = {
      FixedAssetCode: data.fixedAssetCode || data.FixedAssetCode || '',
      FixedAssetName: data.fixedAssetName || data.FixedAssetName || '',
      DepartmentCode: data.departmentCode || data.DepartmentCode || null,
      FixedAssetCategoryCode: data.fixedAssetCategoryCode || data.FixedAssetCategoryCode || null,
      Quantity: data.quantity || data.Quantity || 1,
      Cost: data.cost || data.Cost || 0,
      PurchaseDate: normalizeDateString(data.purchaseDate || data.PurchaseDate),
      StartUsingDate: normalizeDateString(data.startUsingDate || data.StartUsingDate || data.purchaseDate || data.PurchaseDate),
      ProductionYear: data.productionYear || data.ProductionYear || new Date().getFullYear(),
      TrackedYear: data.trackedYear || data.TrackedYear || new Date().getFullYear(),
      LifeTime: data.lifeTime || data.LifeTime || 0,
      DepreciationRate: data.depreciationRate || data.DepreciationRate || 0,
      DepreciationValue: data.depreciationValue || data.DepreciationValue || 0,
      Description: data.description || data.Description || ''
    }

    formData.value = normalized
  }

  /**
   * Hàm xử lý khi người dùng nhấn nút lưu form
   * Thực hiện validate dữ liệu và emit sự kiện submit
   * CreatedBy: HMTuan (29/10/2025)
   */
  function handleSubmit() {
    const shouldCheckDuplicate = props.mode === 'add' || props.mode === 'duplicate'

    const { errors: validationErrors, isValid } = validateAssetForm(
      formData.value,
      shouldCheckDuplicate ? props.existingAssetCodes : []
    )

    if (!isValid) {
      errors.value = validationErrors
      return
    }

    const submitData = {
      FixedAssetCode: formData.value.FixedAssetCode.trim(),
      FixedAssetName: formData.value.FixedAssetName.trim(),
      DepartmentCode: formData.value.DepartmentCode,
      FixedAssetCategoryCode: formData.value.FixedAssetCategoryCode,
      Quantity: parseInt(formData.value.Quantity, 10),
      Cost: parseFloat(formData.value.Cost),
      PurchaseDate: formData.value.PurchaseDate,
      StartUsingDate: formData.value.StartUsingDate,
      Description: formData.value.Description?.trim() || ''
    }

    emit('submit', submitData)
  }

  /**
   * Hàm thông báo khi form có sự thay đổi dữ liệu (placeholder)
   * CreatedBy: HMTuan (29/10/2025)
   */
  function notifyFormChange() {
    // Local state update - placeholder
  }

  /**
   * Hàm xử lý khi người dùng hủy hoặc đóng form
   * Emit sự kiện 'close' và truyền dữ liệu hiện tại của form
   * CreatedBy: HMTuan (29/10/2025)
   */
  function handleCancel() {
    emit('close', JSON.parse(JSON.stringify(formData.value)))
  }
  //#endregion

  return {
    // State
    formData,
    errors,

    // Computed
    dialogTitle,
    departmentName,
    categoryName,

    // Methods
    formatCurrency,
    handleSubmit,
    handleCancel,
    notifyFormChange
  }
}
