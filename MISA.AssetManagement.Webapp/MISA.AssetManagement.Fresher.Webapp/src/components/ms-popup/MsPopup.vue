<template>
  <div
    v-if="isOpen"
    class="popup-overlay active"
    @click="handleOverlayClick"
  >
    <div class="popup-content" :style="{ width: width, maxWidth: '95vw' }">
      <div class="popup-header">
        <h3 class="popup-title">{{ title }}</h3>
        <button class="btn-icon btn-sm" @click="emit('close')">
          <i class="icon cancel-icon"></i>
        </button>
      </div>

      <div class="popup-body">
        <slot></slot>
      </div>

      <div class="popup-footer">
        <slot name="footer"></slot>
      </div>
    </div>
  </div>
</template>

<script setup>
/**
 * Component MsPopup
 * Mô tả: Component popup hiển thị nội dung, có header, body, footer và overlay nền
 * Created by: HMTuan - 28/10/2025
 */

defineProps({
  /**
   * Trạng thái mở/đóng popup
   */
  isOpen: {
    type: Boolean,
    default: false
  },

  /**
   * Tiêu đề của popup
   */
  title: {
    type: String,
    default: ''
  },

  /**
   * Chiều rộng của popup (mặc định: 900px)
   */
  width: {
    type: String,
    default: '900px'
  }
})

/**
 * Emit ra ngoài khi đóng popup
 */
const emit = defineEmits(['close'])

/**
 * Hàm xử lý khi click vào overlay (bên ngoài popup)
 * -> Đóng popup nếu click vào vùng nền ngoài nội dung
 * @param {MouseEvent} e
 * createdBy: HMTuan - 28/10/2025
 */
function handleOverlayClick(e) {
  if (e.target === e.currentTarget) {
    emit('close')
  }
}
</script>

<style scoped>
/* Overlay nền mờ */
.popup-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: none;
  align-items: center;
  justify-content: center;
  z-index: 9999;
  padding: 20px;
}

/* Khi popup active thì hiển thị */
.popup-overlay.active {
  display: flex;
}

/* Khung popup chính */
.popup-content {
  background: #ffffff;
  border-radius: 4px;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
  max-height: 90vh;
  display: flex;
  flex-direction: column;
  animation: popupFadeIn 0.2s ease;
}

/* Hiệu ứng mở popup */
@keyframes popupFadeIn {
  from {
    opacity: 0;
    transform: translateY(-20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Header chứa tiêu đề và nút đóng */
.popup-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 16px;
  flex-shrink: 0;
}

/* Tiêu đề popup */
.popup-title {
  font-family: 'Roboto', sans-serif;
  font-size: 16px;
  font-weight: 600;
  color: #1f2937;
  margin: 0;
}

/* Nút icon đóng popup */
.btn-icon {
  width: 32px;
  height: 32px;
  border: none;
  background: transparent;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 4px;
  transition: all 0.2s ease;
  color: #6b7280;
}

/* Hover nút đóng */
.btn-icon:hover {
  background: #f3f4f6;
  color: #1f2937;
}

/* Phần body chứa nội dung popup */
.popup-body {
  flex: 1;
  overflow-y: auto;
  overflow-x: hidden;
}

/* Custom scrollbar cho nội dung */
.popup-body::-webkit-scrollbar {
  width: 8px;
}

.popup-body::-webkit-scrollbar-thumb {
  background-color: rgba(0, 0, 0, 0.2);
  border-radius: 4px;
}

.popup-body::-webkit-scrollbar-track {
  background-color: #f1f3f5;
}

/* Footer popup: chứa nút hành động */
.popup-footer {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  padding: 16px;
  border-top: 1px solid #e5e7eb;
  flex-shrink: 0;
}
</style>
