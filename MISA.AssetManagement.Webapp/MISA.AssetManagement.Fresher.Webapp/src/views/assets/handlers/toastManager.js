import { ref } from 'vue'

/**
 * Quản lý toast notifications
 */
export function useToastManager() {
  const toasts = ref([])

  /**
   * Hiển thị toast
   */
  function showToast({
                       type = 'success',
                       title,
                       message,
                       showUndo = false,
                       onUndo,
                       duration = 3000
                     }) {
    const id = Date.now() + Math.random()
    toasts.value.push({
      id,
      type,
      title,
      message,
      showUndo,
      duration,
      onUndo
    })
    return id
  }

  /**
   * Xóa toast
   */
  function removeToast(id) {
    const index = toasts.value.findIndex(t => t.id === id)
    if (index > -1) toasts.value.splice(index, 1)
  }

  /**
   * Toast thành công
   */
  function toastSuccess(message) {
    return showToast({
      type: 'success',
      message,
      duration: 3000
    })
  }

  /**
   * Toast cập nhật
   */
  function toastUpdate(message) {
    return showToast({
      type: 'update',
      message,
      duration: 3000
    })
  }

  /**
   * Toast xóa
   */
  function toastDelete(message) {
    return showToast({
      type: 'delete',
      message,
      duration: 5000
    })
  }

  /**
   * Toast lỗi
   */
  function toastError(message) {
    return showToast({
      type: 'error',
      message,
      duration: 4000
    })
  }

  return {
    toasts,
    showToast,
    removeToast,
    toastSuccess,
    toastUpdate,
    toastDelete,
    toastError
  }
}
