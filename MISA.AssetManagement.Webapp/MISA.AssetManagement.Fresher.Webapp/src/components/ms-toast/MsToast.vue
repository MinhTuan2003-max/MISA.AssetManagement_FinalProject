<template>
  <transition name="toast-slide">
    <div v-if="visible" class="toast-notification" :class="`toast-${type}`">
      <div class="toast-icon">
        <img :src="iconSrc" alt="icon" />
      </div>

      <div class="toast-text">
        <span class="toast-message">{{ message }}</span>
      </div>

      <button v-if="showUndo" class="toast-action" @click="handleUndo">
        Hoàn tác
      </button>

      <button class="toast-close" @click="handleClose">
        <svg width="16" height="16" viewBox="0 0 16 16" fill="none">
          <path
            d="M12 4L4 12M4 4L12 12"
            stroke="currentColor"
            stroke-width="1.5"
            stroke-linecap="round"
            stroke-linejoin="round"
          />
        </svg>
      </button>
    </div>
  </transition>
</template>

<script setup>
/**
 * @fileoverview Toast Notification Component
 * @description Component hiển thị thông báo success / update / delete.
 * @createdBy HMTuan (31/10/2025)
 */

// #region Import
import { ref, computed, onMounted } from 'vue'
import successIcon from '@/assets/images/success-icon.png'
// #endregion

// #region Props
/**
 * Các props điều khiển nội dung và hành vi của Toast
 */
const props = defineProps({
  type: {
    type: String,
    default: 'success', // 'success' | 'update' | 'delete'
    validator: (value) => ['success', 'update', 'delete'].includes(value)
  },
  title: {
    type: String,
    default: 'Thành công!'
  },
  message: {
    type: String,
    required: true
  },
  duration: {
    type: Number,
    default: 3000 // 3 giây
  },
  showUndo: {
    type: Boolean,
    default: false
  }
})
// #endregion

// #region Emits
/**
 * Các sự kiện được phát ra ngoài:
 * - close: khi Toast đóng
 * - undo: khi người dùng bấm "Hoàn tác"
 */
const emit = defineEmits(['close', 'undo'])
// #endregion

// #region State
/**
 * visible: trạng thái hiển thị của Toast
 * timer: biến lưu setTimeout để auto đóng
 */
const visible = ref(false)
let timer = null
// #endregion

// #region Computed
/**
 * Tính toán icon hiển thị theo loại thông báo
 * @returns {String} đường dẫn icon
 */
const iconSrc = computed(() => {
  switch (props.type) {
    case 'success':
    case 'update':
    case 'delete':
      return successIcon // Dùng chung icon success
    default:
      return successIcon
  }
})
// #endregion

// #region Lifecycle
/**
 * Khi component mount:
 * - Hiển thị Toast
 * - Tự động đóng sau thời gian quy định (nếu có)
 */
onMounted(() => {
  visible.value = true

  if (props.duration > 0) {
    timer = setTimeout(() => {
      handleClose()
    }, props.duration)
  }
})
// #endregion

// #region Methods
/**
 * Đóng Toast và emit sự kiện "close"
 * @createdBy HMTuan (31/10/2025)
 */
function handleClose() {
  visible.value = false
  clearTimeout(timer)

  // Delay để đảm bảo animation leave kết thúc
  setTimeout(() => {
    emit('close')
  }, 3000)
}

/**
 * Xử lý khi người dùng bấm "Hoàn tác"
 * - Gửi emit "undo"
 * - Tự động đóng Toast
 * @createdBy HMTuan (31/10/2025)
 */
function handleUndo() {
  emit('undo')
  handleClose()
}
// #endregion
</script>


<style scoped>
/* TOAST NOTIFICATION BASE */

.toast-notification {
  display: flex;
  align-items: center;
  gap: 16px;
  min-width: 450px;
  max-width: 650px;
  padding: 16px;
  background: #ffffff;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15), 0 0 0 1px rgba(0, 0, 0, 0.05);
  font-family: 'Roboto', sans-serif;
  position: relative;
}

/* ICON */

.toast-icon {
  flex-shrink: 0;
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.toast-icon img {
  width: 100%;
  height: 100%;
  object-fit: contain;
}

/* TEXT */

.toast-text {
  flex: 1;
  display: flex;
  align-items: center;
  gap: 4px;
  font-size: 14px;
  line-height: 1.5;
  color: #1f2937;
}

.toast-title {
  font-weight: 600;
  color: #10b981; /* Màu xanh lá */
}

.toast-message {
  font-weight: 400;
  color: #1f2937;
}

/* ACTION */

.toast-action {
  flex-shrink: 0;
  background: none;
  border: none;
  padding: 0;
  font-size: 14px;
  font-weight: 500;
  color: #000000;
  text-decoration: underline;
  cursor: pointer;
  transition: color 0.2s ease;
}

.toast-action:hover {
  color: #374151;
}

/* CLOSE BUTTON */

.toast-close {
  flex-shrink: 0;
  width: 16px;
  height: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: none;
  border: none;
  padding: 0;
  cursor: pointer;
  color: #6b7280;
  transition: color 0.2s ease;
}

.toast-close:hover {
  color: #1f2937;
}

.toast-close svg {
  width: 16px;
  height: 16px;
}

/* TOAST VARIANTS */

/* Success */
.toast-success {
  border-left: 4px solid #10b981;
}

/* Update */
.toast-update {
  border-left: 4px solid #3b82f6;
}

.toast-update .toast-title {
  color: #3b82f6; /* Màu xanh dương */
}

/* Delete */
.toast-delete {
  border-left: 4px solid #ef4444;
}

.toast-delete .toast-title {
  color: #ef4444; /* Màu đỏ */
}

/* ANIMATION */

.toast-slide-enter-active {
  animation: slideIn 0.3s ease-out;
}

.toast-slide-leave-active {
  animation: slideOut 0.3s ease-in;
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateY(-20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes slideOut {
  from {
    opacity: 1;
    transform: translateY(0);
  }
  to {
    opacity: 0;
    transform: translateY(-20px);
  }
}

/* RESPONSIVE */

@media (max-width: 768px) {
  .toast-notification {
    min-width: 320px;
    max-width: calc(100vw - 32px);
    padding: 12px;
    gap: 12px;
  }

  .toast-text {
    font-size: 13px;
  }

  .toast-action {
    font-size: 13px;
  }
}
</style>
