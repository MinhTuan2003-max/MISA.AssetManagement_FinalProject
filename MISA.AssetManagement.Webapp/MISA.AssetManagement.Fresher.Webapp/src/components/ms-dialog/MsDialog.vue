<template>
  <div class="dialog-overlay" @click.self="closeOnOverlay && emit('close')">
    <div class="dialog">
      <!-- #region Header -->
      <div v-if="description || iconClass" class="dialog-header">
        <span v-if="iconClass" :class="['icon', iconClass]"></span>
        <div v-if="description" class="dialog-content">
          <p class="dialog-description" v-html="description"></p>
        </div>
      </div>
      <!-- #endregion -->

      <!-- #region Footer -->
      <div class="dialog-footer">
        <button
          v-for="(btn, i) in buttons"
          :key="i"
          class="dialog-btn"
          :class="[btn.type, btn.variant]"
          @click="btn.action?.()"
        >
          {{ btn.label }}
        </button>
      </div>
      <!-- #endregion -->

      <!-- Nút đóng -->
      <button class="dialog-close" @click="emit('close')">
        <span class="icon close-icon"></span>
      </button>
    </div>
  </div>
</template>

<script setup>
/**
 * Component: MsDialog
 * Mô tả: Hiển thị hộp thoại xác nhận hoặc thông báo (Confirm / Alert Dialog)
 * CreatedBy: HMTuan - 28/10/2025
 */

// #region Props
const props = defineProps({
  /**
   * Nội dung mô tả hiển thị trong dialog
   * @type {String}
   */
  description: { type: String, default: '' },

  /**
   * Class icon hiển thị bên trái nội dung
   * @type {String}
   */
  iconClass: { type: String, default: '' },

  /**
   * Danh sách các nút hiển thị ở footer
   * @type {Array<{label: String, type: String, variant?: String, action?: Function}>}
   * @example [{ label: 'Đóng', type: 'primary', action: () => {} }]
   */
  buttons: {
    type: Array,
    default: () => [
      { label: 'Đóng', type: 'primary', action: () => {} }
    ]
  },

  /**
   * Cho phép đóng dialog khi click ra ngoài overlay
   * @type {Boolean}
   */
  closeOnOverlay: { type: Boolean, default: true }
})
// #endregion

// #region Emits
/**
 * Các sự kiện mà component phát ra
 * - close: khi dialog bị đóng
 */
const emit = defineEmits(['close'])
// #endregion
</script>

<style scoped>
/* =================================== CSS: MsDialog =================================== */

.dialog-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.4);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
  animation: fadeIn 0.3s ease-out;
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

.dialog {
  background: #ffffff;
  border-radius: 8px;
  width: 460px;
  padding: 40px;
  position: relative;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.12);
  animation: slideUp 0.3s ease-out;
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.dialog-header {
  display: flex;
  align-items: flex-start;
  gap: 16px;
  margin-bottom: 24px;
}

.dialog-header .icon {
  flex-shrink: 0;
  width: 40px;
  height: 30px;
  display: inline-block;
}

.dialog-content {
  flex: 1;
  display: flex;
  align-items: center;
}

.dialog-description {
  font-size: 14px;
  color: #374151;
  line-height: 1.6;
  margin: 0;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}

/* ===================== BUTTON ===================== */
.dialog-btn {
  min-width: 100px;
  height: 36px;
  border-radius: 4px;
  border: 1px solid transparent;
  padding: 0 16px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 500;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s ease;
  font-family: 'Roboto', sans-serif;
}

/* Primary */
.dialog-btn.primary {
  background-color: #1aa4c8;
  color: #ffffff;
  border-color: #1aa4c8;
}

.dialog-btn.primary:hover {
  background-color: #1892ae;
  box-shadow: 0 2px 8px rgba(26, 164, 200, 0.3);
}

.dialog-btn.primary:active {
  background-color: #167a94;
}

/* Secondary / Cancel */
.dialog-btn.secondary,
.dialog-btn.cancel {
  background-color: #ffffff;
  color: #374151;
  border-color: #d1d5db;
}

.dialog-btn.secondary:hover,
.dialog-btn.cancel:hover {
  background-color: #f9fafb;
  border-color: #9ca3af;
}

.dialog-btn.secondary:active,
.dialog-btn.cancel:active {
  background-color: #f3f4f6;
}

/* Danger */
.dialog-btn.danger {
  background-color: #ef4444;
  color: #ffffff;
  border-color: #ef4444;
}

.dialog-btn.danger:hover {
  background-color: #dc2626;
  box-shadow: 0 2px 8px rgba(239, 68, 68, 0.3);
}

.dialog-btn.danger:active {
  background-color: #b91c1c;
}

/* Cancel - Primary variant (nút "Không lưu") */
.dialog-btn.cancel.primary-variant {
  background-color: #ffffff;
  color: #1aa4c8;
  border-color: #1aa4c8;
}

.dialog-btn.cancel.primary-variant:hover {
  background-color: #f0f9fb;
  border-color: #1892ae;
  color: #1892ae;
}

.dialog-btn.cancel.primary-variant:active {
  background-color: #e0f2f7;
  border-color: #167a94;
  color: #167a94;
}

/* Close button (icon X) */
.dialog-close {
  position: absolute;
  top: 16px;
  right: 16px;
  width: 24px;
  height: 24px;
  background: none;
  border: none;
  cursor: pointer;
  padding: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: opacity 0.2s ease;
}

.dialog-close .icon {
  width: 16px;
  height: 16px;
}

.dialog-close:hover .icon {
  opacity: 0.7;
}

/* ===================== RESPONSIVE ===================== */
@media (max-width: 480px) {
  .dialog {
    width: calc(100vw - 32px);
    padding: 20px;
  }

  .dialog-header {
    gap: 12px;
    margin-bottom: 20px;
  }

  .dialog-footer {
    flex-direction: column;
  }

  .dialog-btn {
    width: 100%;
  }
}
</style>
