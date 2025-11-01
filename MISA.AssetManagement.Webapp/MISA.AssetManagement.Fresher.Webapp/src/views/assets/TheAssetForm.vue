<template>
  <BasePopup :is-open="isOpen" :title="dialogTitle" @close="handleCancel">
    <div class="form-container">
      <!-- Row 1 -->
      <div class="form-row">
        <div class="form-group col-4">
          <label class="form-label">Mã tài sản <span class="required">*</span></label>
          <MsInput v-model="formData.FixedAssetCode" tabindex="-1" />
        </div>
        <div class="form-group col-6">
          <label class="form-label">Tên tài sản <span class="required">*</span></label>
          <MsInput
            v-model="formData.FixedAssetName"
            placeholder="Nhập tên tài sản"
            :error="errors.FixedAssetName"
            tabindex="1"
            @input="notifyFormChange"
          />
        </div>
      </div>

      <!-- Row 2 -->
      <div class="form-row">
        <div class="form-group col-4">
          <label class="form-label">Mã bộ phận sử dụng <span class="required">*</span></label>
          <MsSelect
            v-model="formData.DepartmentCode"
            :options="departmentOptions"
            placeholder="Chọn mã bộ phận sử dụng"
            :error="errors.DepartmentCode"
            tabindex="2"
            @input="notifyFormChange"
          />
        </div>
        <div class="form-group col-6 " >
          <label class="form-label">Tên bộ phận sử dụng</label>
          <MsInput :model-value="departmentName" readonly tabindex="-1" class="force-left"/>
        </div>
      </div>

      <!-- Row 3 -->
      <div class="form-row">
        <div class="form-group col-4">
          <label class="form-label">Mã loại tài sản <span class="required">*</span></label>
          <MsSelect
            v-model="formData.FixedAssetCategoryCode"
            :options="categoryOptions"
            placeholder="Chọn mã loại tài sản"
            :error="errors.FixedAssetCategoryCode"
            tabindex="3"
            @input="notifyFormChange"
          />
        </div>
        <div class="form-group col-6">
          <label class="form-label">Tên loại tài sản</label>
          <MsInput :model-value="categoryName" readonly tabindex="-1" class="force-left"/>
        </div>
      </div>

      <!-- Row 4 -->
      <div class="form-row form-row-triple">
        <div class="form-group">
          <label class="form-label">Số lượng <span class="required">*</span></label>
          <MsNumberInput
            v-model="formData.Quantity"
            :min="1"
            :error="errors.Quantity"
            tabindex="4"
            @input="notifyFormChange"
          />
        </div>
        <div class="form-group">
          <label class="form-label">Nguyên giá <span class="required">*</span></label>
          <MsNumberInput
            v-model="formData.Cost"
            :min="0"
            format="currency"
            :error="errors.Cost"
            tabindex="5"
            @input="notifyFormChange"
          />
        </div>
        <div class="form-group">
          <label class="form-label">Tỷ lệ hao mòn (%) <span class="required">*</span></label>
          <MsNumberInput
            v-model="formData.DepreciationRate"
            :min="0"
            format="currency"
            :error="errors.DepreciationRate"
            tabindex="5"
            @input="notifyFormChange"
          />
        </div>
      </div>

      <!-- Row 5 -->
      <div class="form-row form-row-bottom">
        <div class="form-group">
          <label class="form-label">Ngày mua <span class="required">*</span></label>
          <MsDatePicker
            v-model="formData.PurchaseDate"
            placeholder="dd/mm/yyyy"
            :error="errors.PurchaseDate"
            tabindex="6"
            @input="notifyFormChange"
          />
        </div>
        <div class="form-group">
          <label class="form-label">Ngày bắt đầu sử dụng <span class="required">*</span></label>
          <MsDatePicker
            v-model="formData.StartUsingDate"
            placeholder="dd/mm/yyyy"
            :error="errors.StartUsingDate"
            tabindex="7"
            @input="notifyFormChange"
          />
        </div>
        <div class="form-group">
          <label class="form-label">Năm theo dõi</label>
          <MsInput :model-value="formData.TrackedYear" placeholder="Tự động" readonly tabindex="-1" />
        </div>
      </div>

      <!-- Row 6 -->
      <div class="form-row form-row-bottom">
        <div class="form-group">
          <label class="form-label">Số năm sử dụng <span class="required">*</span></label>
          <MsNumberInput
            v-model="formData.LifeTime"
            :min="0"
            format="currency"
            :error="errors.LifeTime"
            tabindex="5"
            @input="notifyFormChange"
          />
        </div>
        <div class="form-group">
          <label class="form-label">Giá trị hao mòn năm <span class="required">*</span></label>
          <MsInput
            :model-value="formatCurrency(formData.DepreciationValue)"
            placeholder="Tự động"
            readonly
            tabindex="-1"
          />
        </div>
      </div>
    </div>

    <template #footer>
      <button class="btn btn-cancel" @click="handleCancel" tabindex="9">Hủy</button>
      <button class="btn btn-primary" @click="handleSubmit" tabindex="8">Lưu</button>
    </template>
  </BasePopup>
</template>

<script setup>
import { ref, computed, watch, nextTick } from 'vue'
import BasePopup from '@/components/ms-popup/MsPopup.vue'
import MsInput from '@/components/ms-input/MsInput.vue'
import MsSelect from '@/components/ms-select/MsSelect.vue'
import MsDatePicker from '@/components/ms-date/MsDateInput.vue'
import MsNumberInput from '@/components/ms-number-input/MsNumberInput.vue'
import { validateAssetForm } from '@/utils/validate/validateAssetForm.js'

const props = defineProps({
  isOpen: { type: Boolean, default: false },
  mode: { type: String, default: 'add' },
  assetId: { type: String, default: null },
  initialData: { type: Object, default: null },
  categoryOptions: { type: Array, default: () => [] },
  departmentOptions: { type: Array, default: () => [] },
  existingAssetCodes: { type: Array, default: () => [] }
})

const emit = defineEmits(['close', 'submit'])

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

const dialogTitle = computed(() => (props.mode === 'add' ? 'Thêm tài sản' : 'Sửa tài sản'))

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

watch(() => formData.value.PurchaseDate, (newDate) => {
  if (!newDate) return
  const date = new Date(newDate)
  if (isNaN(date.getTime())) return
  const year = date.getFullYear()
  formData.value.ProductionYear = year
  formData.value.TrackedYear = year
})

watch([() => formData.value.Cost, () => formData.value.DepreciationRate], ([cost, rate]) => {
  if (cost && rate) {
    formData.value.DepreciationValue = (cost * rate) / 100
  } else {
    formData.value.DepreciationValue = 0
  }
})

watch(() => props.isOpen, async (newVal) => {
  if (newVal) {
    await nextTick()
    if (props.mode === 'edit' && props.initialData) {
      loadInitialData(props.initialData)
    } else {
      resetForm()
    }
  } else {
    setTimeout(() => {
      resetForm()
    }, 300)
  }
})

watch(() => props.initialData, (newData) => {
  if (newData && props.mode === 'edit' && props.isOpen) {
    loadInitialData(newData)
  }
}, { deep: true })

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
  formData.value = {
    FixedAssetCode: data.FixedAssetCode || '',
    FixedAssetName: data.FixedAssetName || '',
    DepartmentCode: data.DepartmentCode || null,
    FixedAssetCategoryCode: data.FixedAssetCategoryCode || null,
    Quantity: data.Quantity || 1,
    Cost: data.Cost || 0,
    PurchaseDate: data.PurchaseDate || getTodayString(),
    StartUsingDate: data.StartUsingDate || getTodayString(),
    ProductionYear: data.ProductionYear || new Date().getFullYear(),
    TrackedYear: data.TrackedYear || new Date().getFullYear(),
    LifeTime: data.LifeTime || 0,
    DepreciationRate: data.DepreciationRate || 0,
    DepreciationValue: data.DepreciationValue || 0,
    Description: data.Description || ''
  }
}

function handleSubmit() {
  const { errors: validationErrors, isValid } = validateAssetForm(formData.value)

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
  // Local state update
}

function handleCancel() {
  emit('close', JSON.parse(JSON.stringify(formData.value)))
}

function formatCurrency(value) {
  if (value == null || value === '') return ''
  return new Intl.NumberFormat('vi-VN').format(value)
}

</script>


<style scoped>
/* CSS giữ nguyên từ trước */
.form-container {
  padding: 0 16px;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-row {
  display: grid;
  grid-template-columns: 32% calc(68% - 16px);
  gap: 16px;
}

.col-4 { grid-column: span 3.5; }
.col-6 { grid-column: span 6.8; }
.form-row-triple { grid-template-columns: repeat(3, 1fr); gap: 16px; }
.form-row-bottom { grid-template-columns: repeat(3, 1fr); gap: 16px; }

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
  min-width: 0;
}

.form-label {
  font-family: 'Roboto', sans-serif;
  font-size: 13px;
  font-weight: 400;
  color: #1f2937;
  line-height: 18px;
}

.required { color: #dc2626; margin-left: 2px; }

.btn {
  height: 36px;
  padding: 0 16px;
  border-radius: 3px;
  font-family: 'Roboto', sans-serif;
  font-size: 13px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  border: none;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  min-width: 100px;
}

.btn-cancel {
  background: #ffffff;
  color: #1f2937;
  border: 1px solid #d1d5db;
}

.btn-cancel:hover {
  background: #f9fafb;
  border-color: #9ca3af;
}

.btn-primary {
  background: rgb(26,164,200);
  color: #ffffff;
}

.btn-primary:hover {
  background: rgb(42, 156, 185);
}

.force-left ::v-deep(.input-field) {
  text-align: left !important;
}

@media (max-width: 1366px) {
  .form-row, .form-row-triple, .form-row-bottom { gap: 12px; }
}

@media (max-width: 768px) {
  .form-row { grid-template-columns: 1fr; }
  .col-4, .col-6 { grid-column: span 1; }
  .form-row-triple, .form-row-bottom { grid-template-columns: 1fr; }
}
</style>
