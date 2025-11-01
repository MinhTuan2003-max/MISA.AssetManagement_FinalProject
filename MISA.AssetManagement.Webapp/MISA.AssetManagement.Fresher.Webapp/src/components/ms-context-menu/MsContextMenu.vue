<template>
  <div
    v-if="visible"
    class="context-menu"
    ref="menuRef"
    :style="{ top: `${computedTop}px`, left: `${computedLeft}px` }"
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
import { defineProps, defineEmits, ref, nextTick } from 'vue'

// #region Props
const { visible, items } = defineProps({
  visible: { type: Boolean, default: false },
  items: { type: Array, default: () => [] }
})
// #endregion

// #region Emits
const emit = defineEmits(['action', 'close'])
// #endregion

// #region Refs & Reactive
const menuRef = ref(null)
const computedTop = ref(0)
const computedLeft = ref(0)
// #endregion

// #region Methods
function handleClick(item) {
  emit('action', item)
  emit('close')
}

/**
 * Cập nhật vị trí menu dựa trên sự kiện contextmenu
 */
function updatePosition(event) {
  event.preventDefault()
  const mouseX = event.clientX
  const mouseY = event.clientY

  nextTick(() => {
    if (menuRef.value) {
      const menuHeight = menuRef.value.offsetHeight
      const menuWidth = menuRef.value.offsetWidth
      const spaceBelow = window.innerHeight - mouseY
      const spaceRight = window.innerWidth - mouseX

      // Xác định top: trên hoặc dưới chuột
      computedTop.value = spaceBelow < menuHeight ? mouseY - menuHeight : mouseY

      // Xác định left: tránh tràn ra ngoài màn hình
      computedLeft.value = spaceRight < menuWidth ? mouseX - menuWidth : mouseX
    }
  })
}
// #endregion

// #region Mounted
// Lắng nghe sự kiện click chuột phải toàn cục
window.addEventListener('contextmenu', (e) => {
  updatePosition(e)
})
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

/* ========== RESPONSIVE DESIGN ========== */

/* Breakpoint Mobile: < 576px */
@media (max-width: 575.98px) {
  .context-menu {
    min-width: 140px;
    max-width: calc(100vw - 16px);
    border-radius: 8px;
    box-shadow: 0 8px 24px rgba(0, 0, 0, 0.2);
  }

  .menu-item {
    padding: 12px 14px;
    font-size: 14px;
    gap: 10px;
    min-height: 44px;
    display: flex;
    align-items: center;
  }
}

/* Breakpoint Tablet: 576px - 768px */
@media (min-width: 576px) and (max-width: 767.98px) {
  .context-menu {
    min-width: 150px;
    max-width: calc(100vw - 24px);
  }

  .menu-item {
    padding: 10px 15px;
    font-size: 13px;
    min-height: 40px;
  }
}

/* Breakpoint Desktop: >= 768px */
@media (min-width: 768px) {
  .context-menu {
    min-width: 160px;
  }

  .menu-item {
    padding: 8px 16px;
    font-size: 13px;
  }
}

/* Prevent menu overflow */
.context-menu {
  max-height: 80vh;
  overflow-y: auto;
}

/* Icon responsive sizing */
@media (max-width: 575.98px) {
  .context-menu .icon {
    font-size: 1.1em;
  }
}

@media (min-width: 768px) {
  .context-menu .icon {
    font-size: 1em;
  }
}

/* Smooth animation on mobile */
@media (max-width: 575.98px) {
  .context-menu {
    animation: slideUp 0.15s ease-out;
  }

  @keyframes slideUp {
    from {
      opacity: 0;
      transform: translateY(4px);
    }
    to {
      opacity: 1;
      transform: translateY(0);
    }
  }
}
</style>
