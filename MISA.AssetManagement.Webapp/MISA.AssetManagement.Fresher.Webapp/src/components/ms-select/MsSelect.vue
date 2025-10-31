<template>
  <div class="select-wrapper" :class="{ 'has-error': hasError }">
    <div class="select-field" @click="toggleDropdown">
      <span
        class="select-value"
        :class="{ 'placeholder-text': !props.modelValue }"
      >{{ selectedLabel }}</span>
      <i class="select-arrow" :class="{ 'open': isOpen }">
        <span class="icon down-icon"></span>
      </i>
    </div>

    <div v-if="isOpen" class="select-dropdown">
      <div
        v-for="option in options"
        :key="option.value"
        class="select-option"
        :class="{ 'selected': option.value === modelValue }"
        @click="selectOption(option.value)"
      >
        {{ option.label }}
      </div>
    </div>

    <span v-if="hasError" class="error-message">{{ error }}</span>
  </div>
</template>

<script setup>
/**
 * Component: MsSelect
 * Mô tả: Component select tùy chỉnh hiển thị danh sách lựa chọn dạng dropdown
 * CreatedBy: HMTuan - 28/10/2025
 */

// #region Import
import { ref, computed } from 'vue'
// #endregion

// #region Props
const props = defineProps({
  /**
   * Giá trị đang được chọn trong select
   */
  modelValue: {
    type: [String, Number],
    default: null
  },

  /**
   * Danh sách các tùy chọn hiển thị trong dropdown
   * Mỗi phần tử có dạng: { label: String, value: any }
   */
  options: {
    type: Array,
    default: () => []
  },

  /**
   * Chuỗi placeholder hiển thị khi chưa chọn giá trị
   */
  placeholder: {
    type: String,
    default: 'Chọn'
  },

  /**
   * Thông báo lỗi hiển thị dưới select (nếu có)
   */
  error: {
    type: String,
    default: ''
  }
})
// #endregion

// #region Emits
/**
 * @emits update:modelValue - Cập nhật giá trị được chọn ra ngoài (two-way binding)
 */
const emit = defineEmits(['update:modelValue'])
// #endregion

// #region Reactive State
const isOpen = ref(false) // Trạng thái mở/đóng dropdown
const hasError = computed(() => !!props.error) // Kiểm tra có lỗi hay không
// #endregion

// #region Computed
/**
 * Tính toán nhãn hiển thị tương ứng với giá trị đã chọn
 * @returns {string} - Nhãn hiển thị hoặc placeholder nếu chưa chọn
 * createdBy: HMTuan - 28/10/2025
 */
const selectedLabel = computed(() => {
  const selected = props.options.find(opt => opt.value === props.modelValue)
  return selected ? selected.label : props.placeholder
})
// #endregion

// #region Methods
/**
 * Hàm bật/tắt dropdown
 * createdBy: HMTuan - 28/10/2025
 */
function toggleDropdown() {
  isOpen.value = !isOpen.value
}

/**
 * Hàm chọn một giá trị trong dropdown
 * @param {*} value - Giá trị được chọn
 * createdBy: HMTuan - 28/10/2025
 */
function selectOption(value) {
  emit('update:modelValue', value)
  isOpen.value = false
}
// #endregion
</script>

<style scoped>
.select-wrapper {
  position: relative;
}

.select-field {
  height: 36px;
  padding: 0 32px 0 12px;
  border: 1px solid #d1d5db;
  border-radius: 3px;
  background: #ffffff;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: space-between;
  font-family: 'Roboto', sans-serif;
  font-size: 13px;
  transition: all 0.2s ease;
}

.select-field:hover {
  outline: none;
  border-color: #2563eb;
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
}

/* Placeholder nghiêng và nhạt màu */
.placeholder-text {
  font-style: italic;
  color: #9ca3af;
}

.has-error .select-field {
  border-color: #dc2626;
}

.select-value {
  flex: 1;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  color: #1f2937;
}

.select-arrow {
  position: absolute;
  right: 12px;
  font-size: 10px;
  color: #6b7280;
  transition: transform 0.2s ease;
  pointer-events: none;
}

.select-arrow.open {
  transform: rotate(180deg);
}

.icon {
  width: 8px;
  height: 8px;
}

.select-dropdown {
  position: absolute;
  top: 100%;
  left: 0;
  right: 0;
  margin-top: 4px;
  background: #ffffff;
  border: 1px solid #d1d5db;
  border-radius: 3px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  max-height: 200px;
  overflow-y: auto;
  z-index: 1000;
}

.select-option {
  padding: 8px 12px;
  cursor: pointer;
  font-size: 13px;
  color: #1f2937;
  transition: background-color 0.15s ease;
}

.select-option:hover {
  background-color: rgb(199,224,245);
}

.select-option.selected {
  background-color: #eff6ff;
  color: #2563eb;
  font-weight: 500;
}

.error-message {
  font-size: 12px;
  color: #dc2626;
  margin-top: 2px;
  display: block;
}
</style>
