import { ref, computed } from 'vue'

/**
 * Composable quản lý trạng thái Popup (thêm / sửa)
 * Dùng cho các form thêm, sửa
 * CreatedBy: HMTuan (15/10/2025)
 */
export function usePopup() {
  //#region Reactive state
  /** Trạng thái mở/đóng popup */
  const isOpen = ref(false)

  /** Chế độ popup: 'add' hoặc 'edit' */
  const popupMode = ref('add')

  /** ID của bản ghi đang chỉnh sửa (nếu có) */
  const editingId = ref(null)

  /** Tên bản ghi đang chỉnh sửa (để hiển thị tiêu đề, nếu cần) */
  const editingName = ref('')
  //#endregion

  //#region Popup actions
  /**
   * Mở popup ở chế độ thêm mới
   * - Đặt mode = 'add'
   * - Reset ID và tên
   * - Mở popup
   * CreatedBy: HMTuan (15/10/2025)
   */
  function openAddPopup() {
    popupMode.value = 'add'
    editingId.value = null
    editingName.value = ''
    isOpen.value = true
  }

  /**
   * Mở popup ở chế độ chỉnh sửa
   * @param {string|number} id - ID bản ghi cần chỉnh sửa
   * @param {string} [name=''] - Tên bản ghi (hiển thị trên giao diện)
   * CreatedBy: HMTuan (15/10/2025)
   */
  function openEditPopup(id, name = '') {
    popupMode.value = 'edit'
    editingId.value = id
    editingName.value = name
    isOpen.value = true
  }

  /**
   * Đóng popup
   * - Đặt isOpen = false
   * - Sau 300ms reset dữ liệu (để chờ animation hoàn tất)
   * CreatedBy: HMTuan (15/10/2025)
   */
  function closePopup() {
    isOpen.value = false
    setTimeout(() => {
      popupMode.value = 'add'
      editingId.value = null
      editingName.value = ''
    }, 300)
  }
  //#endregion

  //#region Computed helpers
  /** Kiểm tra popup có đang ở chế độ thêm mới không */
  const isAddMode = computed(() => popupMode.value === 'add')

  /** Kiểm tra popup có đang ở chế độ chỉnh sửa không */
  const isEditMode = computed(() => popupMode.value === 'edit')
  //#endregion

  //#region Return API
  return {
    isOpen,
    popupMode,
    editingId,
    editingName,
    isAddMode,
    isEditMode,
    openAddPopup,
    openEditPopup,
    closePopup
  }
  //#endregion
}
