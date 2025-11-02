<template>
  <BasePopup :is-open="isOpen" :title="dialogTitle" @close="handleCancel">
    <div class="form-container">
      <!-- Row 1 -->
      <div class="form-row">
        <div class="form-group col-4">
          <label class="form-label">Mã tài sản <span class="required">*</span></label>
          <MsInput
            v-model="formData.FixedAssetCode"
            :error="errors.FixedAssetCode"
            tabindex="-1" />
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
        <div class="form-group col-6">
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
import BasePopup from '@/components/ms-popup/MsPopup.vue'
import MsInput from '@/components/ms-input/MsInput.vue'
import MsSelect from '@/components/ms-select/MsSelect.vue'
import MsDatePicker from '@/components/ms-date/MsDateInput.vue'
import MsNumberInput from '@/components/ms-number-input/MsNumberInput.vue'
import { useAssetFormLogic } from './handlers/assetFormLogic'

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

const {
  formData,
  errors,
  dialogTitle,
  departmentName,
  categoryName,
  formatCurrency,
  handleSubmit,
  handleCancel,
  notifyFormChange
} = useAssetFormLogic(props, emit)
</script>

<style scoped>
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
