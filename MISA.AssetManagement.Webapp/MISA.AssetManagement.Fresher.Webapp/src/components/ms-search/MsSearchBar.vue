<template>
  <div class="search-wrapper">
    <i class="icon search-icon"></i>
    <input
      v-model="localValue"
      type="text"
      class="search-input"
      :placeholder="placeholder"
    >
  </div>
</template>

<script setup>
/**
 * Component MsSearchBar
 * Mô tả: Ô tìm kiếm có debounce để tránh gọi API liên tục khi người dùng nhập
 * Created by: HMTuan - 28/10/2025
 */

import { ref, watch } from 'vue'

const props = defineProps({
  /**
   * Giá trị chuỗi tìm kiếm hiện tại (two-way binding)
   */
  modelValue: {
    type: String,
    default: ''
  },

  /**
   * Placeholder hiển thị trong ô tìm kiếm
   */
  placeholder: {
    type: String,
    default: 'Tìm kiếm...'
  }
})

/**
 * @emits update:modelValue - Cập nhật giá trị khi người dùng nhập
 * @emits search - Phát sự kiện tìm kiếm (debounced)
 */
const emit = defineEmits(['update:modelValue', 'search'])

const localValue = ref(props.modelValue) // Giá trị local để debounce

/**
 * Theo dõi props.modelValue để đồng bộ dữ liệu bên ngoài vào input
 * createdBy: HMTuan - 28/10/2025
 */
watch(() => props.modelValue, (newValue) => {
  localValue.value = newValue
})

/**
 * Theo dõi localValue để:
 * - Cập nhật giá trị ra ngoài (emit update:modelValue)
 * createdBy: HMTuan - 28/10/2025
 */
watch(localValue, (newValue) => {
  emit('update:modelValue', newValue)
})
</script>

<style scoped>
  .search-input:hover {
    outline: none;
    border-color: #2563eb;
    box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
  }

  .search-input::placeholder {
    font-style: italic;
  }

  @media (max-width: 1144px) {
    .search-input {
      padding-right: 52px;
    }
  }
</style>
