// File: @/views/assets/handlers/assetFormLogic.js
import { ref, computed, watch, nextTick } from 'vue'
import { validateAssetForm } from '@/utils/validate/validateAssetForm.js'

/**
 * Logic xử lý form tài sản
 */
export function useAssetFormLogic(props, emit) {
  //#region Helper Functions
  function getTodayString() {
    const today = new Date()
    const year = today.getFullYear()
    const month = String(today.getMonth() + 1).padStart(2, '0')
    const day = String(today.getDate()).padStart(2, '0')
    return `${year}-${month}-${day}`
  }

  function generateAssetCode() {
    if (props.existingAssetCodes.length === 0) return 'TS000001'
    const numbers = props.existingAssetCodes
      .map(code => {
        const match = code.match(/\d+$/)
        return match ? parseInt(match[0], 10) : 0
      })
      .filter(n => n > 0)
    const maxNumber = Math.max(...numbers, 0)
    return `TS${String(maxNumber + 1).padStart(6, '0')}`
  }

  function formatCurrency(value) {
    if (value == null || value === '') return ''
    return new Intl.NumberFormat('vi-VN').format(value)
  }

  /**
   * Convert ISO datetime string → yyyy-MM-dd format
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
  const dialogTitle = computed(() => {
    if (props.mode === 'add') return 'Thêm tài sản'
    if (props.mode === 'duplicate') return 'Nhân bản tài sản'
    return 'Sửa tài sản'
  })

  const departmentName = computed(() => {
    if (!formData.value.DepartmentCode) return ''
    const dept = props.departmentOptions.find(d => d.value === formData.value.DepartmentCode)
    return dept ? dept.fullName : ''
  })

  const categoryName = computed(() => {
    if (!formData.value.FixedAssetCategoryCode) return ''
    const cat = props.categoryOptions.find(c => c.value === formData.value.FixedAssetCategoryCode)
    return cat ? cat.fullName : ''
  })
  //#endregion

  //#region Watchers
  // Watch isOpen - load data khi mở form
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

  // Watch category để set LifeTime + DepreciationRate
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

  // Watch PurchaseDate để set ProductionYear + TrackedYear
  watch(() => formData.value.PurchaseDate, (newDate) => {
    if (!newDate) return
    const date = new Date(newDate)
    if (isNaN(date.getTime())) return
    const year = date.getFullYear()
    formData.value.ProductionYear = year
    formData.value.TrackedYear = year
  })

  // Watch Cost + DepreciationRate để tính DepreciationValue
  watch([() => formData.value.Cost, () => formData.value.DepreciationRate], ([cost, rate]) => {
    if (cost && rate) {
      formData.value.DepreciationValue = (cost * rate) / 100
    } else {
      formData.value.DepreciationValue = 0
    }
  })
  //#endregion

  //#region Methods
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

  function handleSubmit() {
    const { errors: validationErrors, isValid } = validateAssetForm(
      formData.value,
      props.existingAssetCodes
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

  function notifyFormChange() {
    // Local state update - placeholder
  }

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
