<template>
  <!-- Nút bấm tổng quát, có thể hiển thị icon và label -->
  <button
    :type="type"
    :form="form || undefined"
    :class="[variantClasses[variant], sizeClasses[size]]"
    :disabled="disabled"
    @click="$emit('click', $event)"
  >
    <!-- Icon bên trái -->
    <i
      v-if="icon && iconPosition === 'left'"
      :class="icon"
      class="me-2"
    ></i>

    <!-- Nội dung label -->
    <slot>{{ label }}</slot>

    <!-- Icon bên phải -->
    <i
      v-if="icon && iconPosition === 'right'"
      :class="icon"
      class="ms-2"
    ></i>
  </button>
</template>

<script setup>
/*
 * Component: MsButton
 * Component nút bấm dùng chung trong toàn hệ thống
 * CreatedBy: HMTuan (31/10/2025)
*/

// #region Props
const props = defineProps({
  /** Văn bản hiển thị trên nút */
  label: { type: String, default: '' },

  /** Kiểu nút HTML (button, submit, reset) */
  type: { type: String, default: 'button' },

  /** Kiểu hiển thị (primary, secondary, danger, v.v...) */
  variant: { type: String, default: 'none' },

  /** Kích thước nút (sm, md, lg) */
  size: { type: String, default: 'md' },

  /** Class icon fontawesome (ví dụ: 'fa fa-plus') */
  icon: { type: String, default: '' },

  /** Vị trí icon (left hoặc right) */
  iconPosition: { type: String, default: 'left' },

  /** Trạng thái disable của nút */
  disabled: { type: Boolean, default: false },

  /** Gắn với form cụ thể nếu cần submit */
  form: { type: String, default: '' }
})
// #endregion

// #region Emits
const emit = defineEmits(['click'])
// #endregion

// #region Constants
/** Kích thước nút */
const sizeClasses = {
  sm: 'px-3 py-1 text-sm',
  md: 'px-4 py-2',
  lg: 'px-5 py-3 text-lg'
}

/** Kiểu hiển thị của nút */
const variantClasses = {
  primary: 'btn btn-primary',
  secondary: 'btn btn-secondary',
  danger: 'btn btn-danger',
  light: 'btn btn-light',
  warning: 'btn btn-warning',
  success: 'btn btn-success',
  none: 'btn btn-none',
  asset: 'btn btn-asset'
}
// #endregion
</script>
