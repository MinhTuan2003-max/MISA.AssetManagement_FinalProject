<template>
  <div class="input-wrapper">
    <!-- Ô input chính -->
    <input
      :value="modelValue"
    :type="type"
    :placeholder="placeholder"
    :readonly="readonly"
    :disabled="disabled"
    :class="{ 'has-error': hasError }"
    class="input-field"
    @input="handleInput"
    />
    <!-- Hiển thị thông báo lỗi nếu có -->
    <span v-if="hasError" class="error-message">{{ error }}</span>
  </div>
</template>

<script setup>
/**
 * Component: MsInput
 * Ô nhập liệu cơ bản
 * CreatedBy: HMTuan - 29/10/2025
 */
import { computed } from 'vue'

/**
 * Định nghĩa props nhận từ component cha
 */
const props = defineProps({
  modelValue: {
    type: [String, Number],
    default: ''
  },
  placeholder: {
    type: String,
    default: ''
  },
  type: {
    type: String,
    default: 'text'
  },
  error: {
    type: String,
    default: ''
  },
  readonly: {
    type: Boolean,
    default: false
  },
  disabled: {
    type: Boolean,
    default: false
  }
})

/**
 * Khai báo sự kiện emit ra ngoài
 */
const emit = defineEmits(['update:modelValue'])

/**
 * Tính toán trạng thái lỗi (có error message hay không)
 */
const hasError = computed(() => !!props.error)

/**
 * Hàm xử lý khi người dùng nhập liệu
 * -> Emit giá trị mới ra ngoài để đồng bộ với v-model
 */
function handleInput(e) {
  emit('update:modelValue', e.target.value)
}
</script>

<style scoped>
/* Bọc input và message trong cùng 1 khối dọc */
.input-wrapper {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

/* Style cơ bản cho input */
.input-field {
  height: 36px;
  padding: 0 12px;
  border: 1px solid #d1d5db;
  border-radius: 3px;
  font-family: 'Roboto', sans-serif;
  font-size: 14px;
  color: #1f2937;
  background: #ffffff;
  transition: all 0.2s ease;
}

.input-field::placeholder {
  font-style: italic;
  color: #9ca3af; /* đổi màu placeholder nhẹ hơn */
}

/* Hiệu ứng focus: đổi màu viền và thêm shadow nhẹ */
.input-field:focus {
  outline: none;
  border-color: #2563eb;
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
}

/* Hiệu ứng focus: đổi màu viền và thêm shadow nhẹ */
.input-field:hover {
  outline: none;
  border-color: #2563eb;
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
}

/* Trạng thái readonly hoặc disabled */
.input-field:disabled,
.input-field:read-only {
  background: #f9fafb;
  color: #6b7280;
  text-align: right;
  cursor: not-allowed;
}

/* Khi có lỗi: viền đỏ */
.input-field.has-error {
  border-color: #dc2626;
}

/* Style cho thông báo lỗi */
.error-message {
  font-size: 12px;
  color: #dc2626;
  margin-top: 2px;
}
</style>
