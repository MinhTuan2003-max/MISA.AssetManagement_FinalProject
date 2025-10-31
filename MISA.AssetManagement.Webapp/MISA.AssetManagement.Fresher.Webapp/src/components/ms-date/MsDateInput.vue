<template>
  <div class="date-wrapper">
    <div class="date-input-container" :class="{ 'has-error': error, 'is-disabled': disabled }">
      <input
        ref="inputRef"
        type="text"
        :placeholder="placeholder"
        :disabled="disabled"
        :readonly="readonly"
        class="date-input"
        autocomplete="off"
      />
      <span class="icon calendar-icon"></span>
    </div>

    <span v-if="error" class="error-text">{{ error }}</span>
  </div>
</template>

<script setup>
/*
 * Component: MsDateInput
 * Ô nhập ngày có sử dụng thư viện Flatpickr,
 * hỗ trợ hiển thị lịch và đồng bộ 2 chiều với modelValue
 * CreatedBy: HMTuan - 28/10/2025 */

import { ref, watch, onMounted, onBeforeUnmount, nextTick } from 'vue'
import flatpickr from 'flatpickr'
import 'flatpickr/dist/flatpickr.min.css'
import 'flatpickr/dist/themes/airbnb.css'

// #region Props
/**
 * @property {String} modelValue - Giá trị ngày theo format YYYY-MM-DD
 * @property {String} placeholder - Text hiển thị khi chưa có giá trị
 * @property {String} error - Thông báo lỗi (nếu có)
 * @property {Boolean} disabled - Trạng thái khóa input
 * @property {Boolean} readonly - Trạng thái chỉ đọc
 */
const props = defineProps({
  modelValue: { type: String, default: '' },
  placeholder: { type: String, default: 'dd/mm/yyyy' },
  error: { type: String, default: '' },
  disabled: { type: Boolean, default: false },
  readonly: { type: Boolean, default: false }
})
// #endregion

// #region Emits
/**
 * @emits update:modelValue - Phát ra khi người dùng chọn hoặc nhập ngày
 * @emits change - Phát ra khi giá trị thay đổi
 */
const emit = defineEmits(['update:modelValue', 'change'])
// #endregion

// #region Refs & Variables
/** Tham chiếu tới thẻ input DOM */
const inputRef = ref(null)

/** Instance của Flatpickr */
let flatpickrInstance = null

/** Biến cờ kiểm soát việc cập nhật để tránh vòng lặp watcher */
let isUpdating = false
// #endregion

// #region Methods
/**
 * @description Chuyển đổi chuỗi ngày (YYYY-MM-DD) sang đối tượng Date
 * @param {String} dateStr - Chuỗi ngày dạng YYYY-MM-DD
 * @returns {Date|null} - Trả về đối tượng Date hợp lệ hoặc null nếu lỗi
 * @createdBy: HMTuan - 28/10/2025
 */
function parseYYYYMMDD(dateStr) {
  if (!dateStr) return null
  const parts = dateStr.split('-')
  if (parts.length !== 3) return null

  const year = parseInt(parts[0], 10)
  const month = parseInt(parts[1], 10) - 1
  const day = parseInt(parts[2], 10)
  return new Date(year, month, day)
}
// #endregion

// #region Lifecycle: onMounted
/**
 * @description Khởi tạo Flatpickr sau khi component được mount
 * @createdBy: HMTuan - 28/10/2025
 */
onMounted(async () => {
  if (!inputRef.value) return
  await nextTick()

  flatpickrInstance = flatpickr(inputRef.value, {
    dateFormat: 'd/m/Y',
    allowInput: true,
    disableMobile: true,
    locale: {
      firstDayOfWeek: 1,
      weekdays: {
        shorthand: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'],
        longhand: ['Chủ nhật', 'Thứ hai', 'Thứ ba', 'Thứ tư', 'Thứ năm', 'Thứ sáu', 'Thứ bảy']
      },
      months: {
        shorthand: ['1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12'],
        longhand: [
          'Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6',
          'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'
        ]
      }
    },
    /**
     * Sự kiện khi chọn ngày trong Flatpickr
     */
    onChange: (selectedDates, dateStr) => {
      if (isUpdating) return
      const date = selectedDates[0]

      if (date) {
        const year = date.getFullYear()
        const month = String(date.getMonth() + 1).padStart(2, '0')
        const day = String(date.getDate()).padStart(2, '0')
        const formattedDate = `${year}-${month}-${day}`

        emit('update:modelValue', formattedDate)
        emit('change', formattedDate)
      } else {
        emit('update:modelValue', '')
        emit('change', '')
      }
    },

    /** Tùy chỉnh giao diện lịch khi render */
    onReady: (selectedDates, dateStr, instance) => {
      instance.calendarContainer.classList.add('flatpickr-custom')
    }
  })

  // Nếu có giá trị modelValue ban đầu thì set vào lịch
  if (props.modelValue) {
    const dateObj = parseYYYYMMDD(props.modelValue)
    if (dateObj) {
      isUpdating = true
      flatpickrInstance.setDate(dateObj, true)
      isUpdating = false
    }
  }
})
// #endregion

// #region Watchers
/**
 * Theo dõi thay đổi của modelValue để đồng bộ với Flatpickr
 */
watch(() => props.modelValue, (newVal, oldVal) => {
  if (!flatpickrInstance) return
  if (newVal !== oldVal) {
    if (newVal) {
      const dateObj = parseYYYYMMDD(newVal)
      if (dateObj) {
        isUpdating = true
        flatpickrInstance.setDate(dateObj, true)
        isUpdating = false
      }
    } else {
      flatpickrInstance.clear()
    }
  }
})

/**
 * Theo dõi thuộc tính disabled để bật/tắt input
 */
watch(() => props.disabled, (newVal) => {
  if (flatpickrInstance) {
    flatpickrInstance._input.disabled = newVal
  }
})
// #endregion

// #region Lifecycle: onBeforeUnmount
/**
 * @description Hủy instance Flatpickr khi component bị unmount
 * @createdBy: HMTuan - 28/10/2025
 */
onBeforeUnmount(() => {
  if (flatpickrInstance) {
    flatpickrInstance.destroy()
    flatpickrInstance = null
  }
})
// #endregion
</script>

<style scoped>
.date-wrapper {
  display: flex;
  flex-direction: column;
  gap: 4px;
  width: 100%;
}

.date-input-container {
  position: relative;
  display: flex;
  align-items: center;
}

.date-input {
  width: 100%;
  height: 36px;
  padding: 0 36px 0 12px;
  border: 1px solid #d1d5db;
  border-radius: 3px;
  font-family: 'Roboto', sans-serif;
  font-size: 13px;
  color: #1f2937;
  background: #ffffff;
  transition: all 0.2s ease;
}

.date-input:hover:not(:disabled) {
  border-color: #9ca3af;
}

.date-input:focus {
  outline: none;
  border-color: #2563eb;
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
}

.date-input:hover {
  outline: none;
  border-color: #2563eb;
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
}

.date-input::placeholder {
  color: #9ca3af;
}

.date-input:disabled {
  background: #f9fafb;
  color: #9ca3af;
  cursor: not-allowed;
  border-color: #d1d5db;
}

.date-input-container.has-error .date-input {
  border-color: #dc2626;
}

.date-input-container.has-error .date-input:focus {
  border-color: #dc2626;
  box-shadow: 0 0 0 3px rgba(220, 38, 38, 0.1);
}

.calendar-icon {
  background: url('../../assets/icons/qlts-icon.svg') no-repeat -287px -67px;
  position: absolute;
  right: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.error-text {
  font-size: 12px;
  color: #dc2626;
  padding-left: 4px;
}
</style>

<style>
/* ========== FLATPICKR CUSTOM THEME ========== */

.flatpickr-custom.flatpickr-calendar {
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.12), 0 0 0 1px rgba(0, 0, 0, 0.08) !important;
  border-radius: 12px !important;
  padding: 16px !important;
  background: #ffffff !important;
  font-family: 'Roboto', sans-serif !important;
}

/* ========== HEADER ========== */
.flatpickr-custom .flatpickr-months {
  padding: 0 0 16px 0 !important;
  margin-bottom: 0 !important;
  background: #ffffff !important;
  border-radius: 8px 8px 0 0 !important;
}

.flatpickr-custom .flatpickr-month {
  height: 40px !important;
  background: #ffffff !important;
  color: #1f2937 !important;
}

.flatpickr-custom .flatpickr-current-month {
  height: 40px !important;
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
  gap: 8px !important;
  padding: 0 !important;
  font-size: 16px !important;
  font-weight: 600 !important;
  left: 50% !important;
  transform: translateX(-50%) !important;
  color: #1f2937 !important;
}

/* Month Dropdown */
.flatpickr-custom .flatpickr-monthDropdown-months {
  appearance: none !important;
  background: #ffffff !important;
  border: 1px solid #e5e7eb !important;
  color: #1f2937 !important;
  font-family: 'Roboto', sans-serif !important;
  font-weight: 600 !important;
  font-size: 16px !important;
  padding: 6px 32px 6px 12px !important;
  border-radius: 8px !important;
  cursor: pointer !important;
  transition: all 0.2s ease !important;
  margin: 0 !important;
}

.flatpickr-custom .flatpickr-monthDropdown-months:hover {
  background: #f9fafb !important;
  border-color: #d1d5db !important;
}

.flatpickr-custom .flatpickr-monthDropdown-months:focus {
  outline: none !important;
  background: #ffffff !important;
  border-color: #2563eb !important;
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1) !important;
}

/* Year Input */
.flatpickr-custom .numInputWrapper {
  width: 80px !important;
  background: #ffffff !important;
  border: 1px solid #e5e7eb !important;
  border-radius: 8px !important;
  padding: 6px 12px !important;
}

.flatpickr-custom .cur-year {
  color: #1f2937 !important;
  font-family: 'Roboto', sans-serif !important;
  font-weight: 600 !important;
  font-size: 16px !important;
  padding: 0 !important;
  border: none !important;
  background: transparent !important;
  width: 100% !important;
  text-align: center !important;
}

.flatpickr-custom .numInputWrapper:hover {
  background: #f9fafb !important;
  border-color: #d1d5db !important;
}

.flatpickr-custom .numInputWrapper:focus-within {
  border-color: #2563eb !important;
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1) !important;
}

.flatpickr-custom .numInputWrapper span {
  display: none !important;
}

/* Navigation Arrows */
.flatpickr-custom .flatpickr-prev-month,
.flatpickr-custom .flatpickr-next-month {
  width: 36px !important;
  height: 36px !important;
  padding: 0 !important;
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
  border-radius: 8px !important;
  transition: all 0.2s ease !important;
  position: absolute !important;
  top: 2px !important;
  background: transparent !important;
}

.flatpickr-custom .flatpickr-prev-month {
  left: 0 !important;
}

.flatpickr-custom .flatpickr-next-month {
  right: 0 !important;
}

.flatpickr-custom .flatpickr-prev-month:hover,
.flatpickr-custom .flatpickr-next-month:hover {
  background: #f3f4f6 !important;
}

.flatpickr-custom .flatpickr-prev-month svg,
.flatpickr-custom .flatpickr-next-month svg {
  fill: #6b7280 !important;
  width: 16px !important;
  height: 16px !important;
}

/* ========== WEEKDAYS ========== */
.flatpickr-custom .flatpickr-weekdays {
  height: auto !important;
  margin-top: 8px !important;
  margin-bottom: 8px !important;
  background: transparent !important;
}

.flatpickr-custom .flatpickr-weekday {
  color: #6b7280 !important;
  font-family: 'Roboto', sans-serif !important;
  font-weight: 600 !important;
  font-size: 13px !important;
  text-transform: uppercase !important;
  letter-spacing: 0.5px !important;
}

/* ========== DAYS ========== */
.flatpickr-custom .flatpickr-days {
  width: 100% !important;
}

.flatpickr-custom .dayContainer {
  width: 100% !important;
  min-width: 100% !important;
  max-width: 100% !important;
}

.flatpickr-custom .flatpickr-day {
  color: #1f2937 !important;
  font-family: 'Roboto', sans-serif !important;
  font-weight: 400 !important;
  font-size: 14px !important;
  height: 40px !important;
  line-height: 40px !important;
  border-radius: 10px !important;
  border: none !important;
  transition: all 0.2s ease !important;
  margin: 2px !important;
}

.flatpickr-custom .flatpickr-day:hover {
  background: rgba(37, 99, 235, 0.08) !important;
  border-color: transparent !important;
  color: #1f2937 !important;
}

/* Today */
.flatpickr-custom .flatpickr-day.today {
  border: 2px solid #2563eb !important;
  background: transparent !important;
  color: #2563eb !important;
  font-weight: 600 !important;
}

.flatpickr-custom .flatpickr-day.today:hover {
  background: rgba(37, 99, 235, 0.08) !important;
  border-color: #2563eb !important;
}

/* Selected */
.flatpickr-custom .flatpickr-day.selected,
.flatpickr-custom .flatpickr-day.startRange,
.flatpickr-custom .flatpickr-day.endRange {
  background: #2563eb !important;
  border-color: #2563eb !important;
  color: #ffffff !important;
  font-weight: 600 !important;
}

.flatpickr-custom .flatpickr-day.selected:hover {
  background: #1d4ed8 !important;
  border-color: #1d4ed8 !important;
}

/* In Range */
.flatpickr-custom .flatpickr-day.inRange {
  background: rgba(37, 99, 235, 0.12) !important;
  border-color: transparent !important;
  box-shadow: none !important;
  color: #2563eb !important;
}

/* Other Month Days */
.flatpickr-custom .flatpickr-day.prevMonthDay,
.flatpickr-custom .flatpickr-day.nextMonthDay {
  color: #d1d5db !important;
}

.flatpickr-custom .flatpickr-day.prevMonthDay:hover,
.flatpickr-custom .flatpickr-day.nextMonthDay:hover {
  color: #9ca3af !important;
}

/* Disabled */
.flatpickr-custom .flatpickr-day.flatpickr-disabled,
.flatpickr-custom .flatpickr-day.flatpickr-disabled:hover {
  color: #e5e7eb !important;
  cursor: not-allowed !important;
  background: transparent !important;
}

/* Focus State */
.flatpickr-custom .flatpickr-day:focus {
  outline: none !important;
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.2) !important;
}

/* ========== ANIMATION ========== */
.flatpickr-custom.open {
  animation: fadeInDown 0.2s ease-out;
}

@keyframes fadeInDown {
  from {
    opacity: 0;
    transform: translateY(-8px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
