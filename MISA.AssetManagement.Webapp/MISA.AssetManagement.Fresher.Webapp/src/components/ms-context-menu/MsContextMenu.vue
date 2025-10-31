<template>
  <div
    v-if="visible"
    class="context-menu"
    :style="{ top: `${position.y}px`, left: `${position.x}px` }"
    @click.stop
  >
    <div
      v-for="(item, index) in items"
      :key="index"
      class="menu-item"
      :class="{ danger: item.type === 'danger' }"
      @click="handleClick(item)"
    >
      <i v-if="item.icon" :class="['icon', item.icon]"></i>
      {{ item.label }}
    </div>
  </div>
</template>

<script setup>
/*
 * Component: MsContextMenu
 * Mục đích: Hiển thị menu chuột phải (context menu)
 * CreatedBy: HMTuan (29/10/2025)
 */

import { defineProps, defineEmits } from 'vue'

// #region Props
const props = defineProps({
  /** Trạng thái hiển thị của menu */
  visible: { type: Boolean, default: false },

  /** Vị trí hiển thị (tọa độ x, y) */
  position: {
    type: Object,
    default: () => ({ x: 0, y: 0 })
  },

  /** Danh sách các item hiển thị trong menu */
  items: {
    type: Array,
    default: () => []
  }
})
// #endregion

// #region Emits
const emit = defineEmits(['action', 'close'])
// #endregion

// #region Methods
/**
 * Xử lý khi click vào một item trong menu
 * @param {*} item Đối tượng item được click
 * @createdBy: HMTuan - 29/10/2025
 */
function handleClick(item) {
  emit('action', item)
  emit('close')
}
// #endregion
</script>

<style scoped>
/* CSS: MsContextMenu */

.context-menu {
  position: fixed;
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 6px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  z-index: 1000;
  min-width: 160px;
  padding: 4px 0;
}

.menu-item {
  padding: 8px 16px;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 13px;
  color: #333;
  transition: background-color 0.15s ease;
}

.menu-item:hover {
  background-color: #f3f4f6;
}

.menu-item.danger {
  color: #dc2626;
}

.menu-item.danger:hover {
  background-color: #fee2e2;
}
</style>
