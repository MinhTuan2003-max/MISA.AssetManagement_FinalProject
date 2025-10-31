<template>
  <div class="number-input-wrapper">
    <div class="number-input-container" :class="{ 'has-error': hasError, 'readonly': readonly }">
      <input
        :value="displayValue"
        type="text"
        :placeholder="placeholder"
        :readonly="readonly"
        :disabled="disabled"
        class="number-input-field"
        @input="handleInput"
        @focus="handleFocus"
        @blur="handleBlur"
      />

      <div v-if="!readonly && !disabled" class="spinner-buttons">
        <button
          type="button"
          class="spinner-btn spinner-up"
          @click="increment"
          :disabled="max !== null && numericValue >= max"
        />
        <button
          type="button"
          class="spinner-btn spinner-down"
          @click="decrement"
          :disabled="min !== null && numericValue <= min"
        />
      </div>
    </div>

    <span v-if="hasError" class="error-message">{{ error }}</span>
  </div>
</template>

<script setup>
import { computed, ref, watch } from 'vue'

const props = defineProps({
  modelValue: { type: [Number, String], default: 0 },
  min: { type: Number, default: null },
  max: { type: Number, default: null },
  step: { type: Number, default: 1 },
  format: { type: String, default: 'number', validator: (value) => ['number', 'currency'].includes(value) },
  placeholder: { type: String, default: '0' },
  error: { type: String, default: '' },
  readonly: { type: Boolean, default: false },
  disabled: { type: Boolean, default: false }
})

const emit = defineEmits(['update:modelValue'])

const isFocused = ref(false)
const displayValue = ref('')

const hasError = computed(() => !!props.error)

const numericValue = computed(() => {
  const value = typeof props.modelValue === 'string'
    ? parseFloat(props.modelValue.replace(/[^\d.-]/g, ''))
    : props.modelValue
  return isNaN(value) ? 0 : value
})

function formatNumber(value) {
  if (value === null || value === undefined || value === '') return ''
  const num = parseFloat(value)
  if (isNaN(num)) return ''
  return new Intl.NumberFormat('vi-VN').format(num)
}

function parseNumber(str) {
  if (!str) return 0
  const cleaned = String(str).replace(/\./g, '').replace(/[^\d-]/g, '')
  const num = parseFloat(cleaned)
  return isNaN(num) ? 0 : num
}

function validateValue(value) {
  let num = parseNumber(value)
  if (props.min !== null && num < props.min) num = props.min
  if (props.max !== null && num > props.max) num = props.max
  return num
}

function handleInput(e) {
  let inputValue = e.target.value

  // Chặn nhập ký tự không hợp lệ (chỉ cho số, dấu chấm, dấu trừ)
  inputValue = inputValue.replace(/[^0-9.-]/g, '')
  e.target.value = inputValue

  const num = parseNumber(inputValue)

  if (inputValue === '' || inputValue === '-') {
    displayValue.value = inputValue
    emit('update:modelValue', 0)
    return
  }

  const formatted = formatNumber(num)
  displayValue.value = formatted
  emit('update:modelValue', num)
}

function handleFocus() {
  isFocused.value = true
  displayValue.value = formatNumber(numericValue.value)
}

function handleBlur() {
  isFocused.value = false
  const validated = validateValue(displayValue.value)
  emit('update:modelValue', validated)
  displayValue.value = formatNumber(validated)
}

function increment() {
  const newValue = numericValue.value + props.step
  const validated = validateValue(newValue)
  emit('update:modelValue', validated)
}

function decrement() {
  const newValue = numericValue.value - props.step
  const validated = validateValue(newValue)
  emit('update:modelValue', validated)
}

watch(() => props.modelValue, (newVal) => {
  if (!isFocused.value) {
    displayValue.value = formatNumber(newVal)
  }
}, { immediate: true })
</script>

<style scoped>
/* CSS giữ nguyên */
.number-input-wrapper {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.number-input-container {
  position: relative;
  display: flex;
  align-items: center;
  height: 36px;
  border: 1px solid #d1d5db;
  border-radius: 3px;
  background: #ffffff;
  transition: all 0.2s ease;
}

.number-input-container:focus-within {
  border-color: #2563eb;
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
}

.number-input-container.has-error {
  border-color: #dc2626;
}

.number-input-container.readonly {
  background: #f9fafb;
}

.number-input-field {
  flex: 1;
  height: 100%;
  padding: 0 32px 0 12px;
  border: none;
  background: transparent;
  font-family: 'Roboto', sans-serif;
  font-size: 13px;
  color: #1f2937;
  text-align: right;
}

.number-input-field:focus {
  outline: none;
}

.number-input-field:disabled,
.number-input-field:read-only {
  color: #6b7280;
  cursor: not-allowed;
}

.number-input-field::placeholder {
  color: #9ca3af;
}

.spinner-buttons {
  display: flex;
  flex-direction: column;
  justify-content: center;
  position: absolute;
  right: 1px;
  top: 1px;
  bottom: 1px;
  width: 20px;
  gap: 1px;
}

.spinner-btn {
  width: 7px;
  height: 5px;
  border: none;
  background-color: transparent;
  cursor: pointer;
  background-repeat: no-repeat;
  background-size: contain;
  padding: 4px;
}

.spinner-btn:hover:not(:disabled) {
  opacity: 0.8;
}

.spinner-btn:active:not(:disabled) {
  opacity: 0.6;
}

.spinner-btn:disabled {
  opacity: 0.3;
  cursor: not-allowed;
}

.spinner-btn.spinner-up {
  background: url('../../assets/icons/qlts-icon.svg') no-repeat -28px -338px;
}

.spinner-btn.spinner-down {
  background: url('../../assets/icons/qlts-icon.svg') no-repeat -72px -338px;
}

.error-message {
  font-size: 12px;
  color: #dc2626;
  margin-top: 2px;
}
</style>
