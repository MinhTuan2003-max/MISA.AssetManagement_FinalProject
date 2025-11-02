<template>
  <div class="ms-filter-select" ref="filterRef">
    <!-- Nút chính để mở dropdown -->
    <button
      class="base-filter-btn"
      :class="{ active: isActive }"
      @click="toggleDropdown"
    >
      <span class="icon filter-icon"></span>
      <span>{{ displayLabel }}</span>
      <span class="icon arrow-icon" :class="{ open: isOpen }"></span>
    </button>

    <!-- Menu lựa chọn -->
    <div v-if="isOpen" class="dropdown-menu">
      <div
        class="dropdown-item"
        :class="{ selected: !modelValue }"
        @click="selectOption(null)"
      >
        Tất cả
      </div>
      <div
        v-for="option in options"
        :key="option.value"
        class="dropdown-item"
        :class="{ selected: option.value === modelValue }"
        @click="selectOption(option)"
      >
        {{ option.label }}
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'

/**
 * Component: MsFilterButton
 * Mô tả: Hiển thị nút bộ lọc (dropdown filter button)
 * CreatedBy: HMTuan - 29/10/2025
 */

// #region Props
const props = defineProps({
  /** Nhãn hiển thị mặc định trên nút */
  label: { type: String, required: true },

  /** Danh sách tùy chọn trong dropdown */
  options: {
    type: Array,
    default: () => []
  },

  /** Giá trị được chọn hiện tại */
  modelValue: {
    type: [String, Number],
    default: null
  },

  /** Trạng thái hoạt động (nút đang được filter) */
  isActive: {
    type: Boolean,
    default: false
  }
})
// #endregion

// #region Emits
/**
 * Các sự kiện được phát ra:
 * - update:modelValue: khi chọn giá trị mới
 * - change: khi có thay đổi filter
 */
const emit = defineEmits(['update:modelValue', 'change'])
// #endregion

// #region State
const isOpen = ref(false)
const filterRef = ref(null)
// #endregion

// #region Computed
/**
 * Tính toán nhãn hiển thị dựa trên giá trị được chọn
 * @returns {String}
 */
const displayLabel = computed(() => {
  if (props.modelValue) {
    const selected = props.options.find(opt => opt.value === props.modelValue)
    return selected ? selected.label : props.label
  }
  return props.label
})
// #endregion

// #region Methods
/**
 * Mở hoặc đóng dropdown
 * @createdBy HMTuan - 29/10/2025
 */
const toggleDropdown = () => {
  isOpen.value = !isOpen.value
}

/**
 * Xử lý chọn 1 tùy chọn trong danh sách
 * @param {Object|null} option - Giá trị được chọn, null = Tất cả
 * @createdBy HMTuan - 29/10/2025
 */
const selectOption = (option) => {
  const value = option ? option.value : null
  emit('update:modelValue', value)
  emit('change', value)
  isOpen.value = false
}

/**
 * Đóng dropdown khi click ra ngoài
 * @param {MouseEvent} event
 * @createdBy HMTuan - 29/10/2025
 */
const handleClickOutside = (event) => {
  if (filterRef.value && !filterRef.value.contains(event.target)) {
    isOpen.value = false
  }
}
// #endregion

// #region Lifecycle
onMounted(() => {
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})
// #endregion
</script>

<style scoped>
.ms-filter-select {
  position: relative;
  width: 219px;
}

.base-filter-btn {
  padding: 7px 16px;
  background-color: #ffffff;
  border-radius: 2.5px;
  border: 1px solid #afafaf;
  width: 100%;
  height: 35px;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 13px;
  color: #333;
  transition: all 0.2s ease;
  overflow: hidden;
}

.base-filter-btn span {
  display: inline-block;
  max-width: 140px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.arrow-icon {
  margin-left: auto;
  flex-shrink: 0;
  width: 0;
  height: 0;
  border-left: 4px solid transparent;
  border-right: 4px solid transparent;
  border-top: 5px solid #666;
  transition: transform 0.2s ease;
}

.filter-icon {
  height: 15px;
}

.base-filter-btn:hover {
  outline: none;
  border-color: #2563eb;
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
}

.base-filter-btn:focus {
  outline: none;
  border-color: #2563eb;
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
}

.base-filter-btn.active {
  border-color: #007aff;
  background-color: #f0f7ff;
}

.arrow-icon.open {
  transform: rotate(180deg);
}

.dropdown-menu {
  position: absolute;
  top: calc(100% + 4px);
  left: 0;
  width: 100%;
  max-height: 250px;
  overflow-y: auto;
  background-color: #ffffff;
  border: 1px solid #afafaf;
  border-radius: 2.5px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  z-index: 1000;
}

.dropdown-item {
  padding: 8px 16px;
  font-size: 14px;
  color: #333;
  cursor: pointer;
  transition: background-color 0.15s ease;
}

.dropdown-item:hover {
  background-color: rgb(199,224,245);
}

.dropdown-item.selected {
  background-color: #b1d5f3;
  color: #333333;
  font-weight: 500;
}

.dropdown-menu::-webkit-scrollbar {
  width: 6px;
}

.dropdown-menu::-webkit-scrollbar-thumb {
  background-color: rgba(0, 0, 0, 0.2);
  border-radius: 3px;
}

.dropdown-menu::-webkit-scrollbar-track {
  background-color: #f1f3f5;
}

@media (max-width: 575.98px) {
  .base-filter-btn {
    width: 100%;
  }
}

@media (max-width: 546px) {
  .base-filter-btn {
    width: 113%;
  }
}
</style>
