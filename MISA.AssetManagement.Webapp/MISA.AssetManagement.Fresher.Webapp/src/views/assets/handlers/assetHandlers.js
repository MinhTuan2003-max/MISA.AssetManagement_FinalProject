import { ref } from 'vue'
import { FixedAsset } from '@/domains/models/FixedAsset'

/**
 * Tạo handlers cho các thao tác CRUD và UI của tài sản
 * @param {Object} composable - useFixedAssets composable
 * @param {Function} toastFn - Toast notification functions
 */
export function useAssetHandlers(composable, toastFn) {
  const {
    selectedIds,
    tableData,
    loadFixedAssets,
    loadShortNameSelectOptions,
    createFixedAsset,
    updateFixedAsset,
    deleteFixedAsset,
    deleteSelectedAssets,
    getDuplicateData
  } = composable

  const { toastSuccess, toastUpdate, toastDelete, toastError } = toastFn

  //#region Form State
  const isPopupOpen = ref(false)
  const popupMode = ref('add') // 'add' | 'edit' | 'duplicate'
  const selectedAssetId = ref(null)
  const editingAssetData = ref(null)
  const formDataBeforeEdit = ref(null)
  //#endregion

  //#region Dialog State
  const showDeleteDialog = ref(false)
  const deleteDialogData = ref({ type: 'single', asset: null })
  const showCancelDeclarationDialog = ref(false)
  const showSaveChangesDialog = ref(false)
  const pendingSubmitData = ref(null)
  //#endregion

  //#region CRUD - Add
  /**
   * Mở form thêm mới tài sản
   */
  async function handleAddAsset() {
    popupMode.value = 'add'
    selectedAssetId.value = null
    editingAssetData.value = null
    formDataBeforeEdit.value = null
    await loadShortNameSelectOptions()
    isPopupOpen.value = true
  }
  //#endregion

  //#region CRUD - Edit
  /**
   * Mở form sửa tài sản
   */
  async function handleEdit(item) {
    try {
      const assetId = item.FixedAssetId || item.CandidateID || item.id
      if (!assetId) {
        toastError('Không thể xác định ID tài sản')
        return
      }

      popupMode.value = 'edit'
      selectedAssetId.value = assetId
      const asset = FixedAsset.fromApi(item)
      const editFormat = asset.toEditFormat()

      editingAssetData.value = editFormat
      formDataBeforeEdit.value = JSON.parse(JSON.stringify(editFormat))

      await loadShortNameSelectOptions()
      isPopupOpen.value = true
    } catch (err) {
      toastError(`Lỗi khi load tài sản: ${err.message}`)
    }
  }
  //#endregion

  //#region CRUD - Duplicate
  /**
   * Xử lý nhân bản tài sản - lấy dữ liệu và mở form
   */
  async function handleDuplicate(item) {
    try {
      const assetId = item.FixedAssetId || item.CandidateID || item.id
      if (!assetId) {
        toastError('Không thể xác định ID tài sản')
        return
      }

      // Lấy dữ liệu nhân bản từ API (có mã mới)
      const duplicateData = await getDuplicateData(assetId)

      // Mở form với mode duplicate
      popupMode.value = 'duplicate'
      selectedAssetId.value = null // Không cần ID vì tạo mới
      editingAssetData.value = duplicateData
      formDataBeforeEdit.value = null

      await loadShortNameSelectOptions()
      isPopupOpen.value = true

    } catch (err) {
      toastError(`Lỗi khi lấy dữ liệu nhân bản: ${err.message}`)
    }
  }
  //#endregion

  //#region CRUD - Submit
  /**
   * Submit form (tạo mới hoặc cập nhật)
   */
  async function handleSubmitAsset(assetData) {
    try {
      // Mode 'add' hoặc 'duplicate' → tạo mới
      if (popupMode.value === 'add' || popupMode.value === 'duplicate') {
        await createFixedAsset(assetData)
        const message = popupMode.value === 'duplicate'
          ? 'Nhân bản tài sản thành công!'
          : 'Thêm tài sản thành công!'
        toastSuccess(message)
      }
      // Mode 'edit' → cập nhật
      else {
        await updateFixedAsset(selectedAssetId.value, assetData)
        toastUpdate('Cập nhật tài sản thành công!')
      }

      await loadFixedAssets()
      closePopup()
    } catch (err) {
      if (err.response?.data?.errors) {
        toastError(`Lỗi validation: ${JSON.stringify(err.response.data.errors, null, 2)}`)
      } else {
        toastError(`Lỗi: ${err.message}`)
      }
    }
  }

  /**
   * Kiểm tra form có thay đổi không
   */
  function hasFormChanged(currentData) {
    if (!formDataBeforeEdit.value) return false
    const before = JSON.stringify(formDataBeforeEdit.value)
    const current = JSON.stringify(currentData)
    return before !== current
  }

  /**
   * Đóng form - kiểm tra thay đổi
   */
  function handleCloseForm(currentFormData) {
    // Add/Duplicate mode - hỏi hủy khai báo
    if (popupMode.value === 'add' || popupMode.value === 'duplicate') {
      showCancelDeclarationDialog.value = true
      return
    }

    // Edit mode - kiểm tra thay đổi
    if (hasFormChanged(currentFormData)) {
      pendingSubmitData.value = currentFormData
      showSaveChangesDialog.value = true
    } else {
      closePopup()
    }
  }

  /**
   * Đóng popup + tất cả dialogs
   */
  function closePopup() {
    isPopupOpen.value = false
    showSaveChangesDialog.value = false
    showCancelDeclarationDialog.value = false
    showDeleteDialog.value = false
    popupMode.value = 'add'
    selectedAssetId.value = null
    editingAssetData.value = null
    formDataBeforeEdit.value = null
    pendingSubmitData.value = null
  }
  //#endregion

  //#region Dialog Handlers
  /**
   * Dialog: Hủy khai báo - Không
   */
  function handleCancelDeclarationNo() {
    showCancelDeclarationDialog.value = false
  }

  /**
   * Dialog: Hủy khai báo - Có
   */
  function handleCancelDeclarationYes() {
    showCancelDeclarationDialog.value = false
    closePopup()
  }

  /**
   * Dialog: Lưu thay đổi - Hủy bỏ
   */
  function handleSaveChangesCancel() {
    showSaveChangesDialog.value = false
    pendingSubmitData.value = null
  }

  /**
   * Dialog: Lưu thay đổi - Không lưu
   */
  function handleSaveChangesNo() {
    showSaveChangesDialog.value = false
    closePopup()
  }

  /**
   * Dialog: Lưu thay đổi - Lưu
   */
  async function handleSaveChangesYes() {
    showSaveChangesDialog.value = false

    if (pendingSubmitData.value) {
      await handleSubmitAsset(pendingSubmitData.value)
    } else {
      toastError('Không có dữ liệu để lưu')
    }
  }
  //#endregion

  //#region Delete Actions
  /**
   * Xóa một tài sản
   */
  function handleDelete(item) {
    if (!item) {
      toastError('Không thể xác định tài sản cần xóa')
      return
    }
    deleteDialogData.value = { type: 'single', asset: item }
    showDeleteDialog.value = true
  }

  /**
   * Xóa các tài sản đã chọn
   */
  function handleDeleteSelected() {
    if (selectedIds.value.length === 0) {
      toastError('Vui lòng chọn ít nhất một tài sản để xóa')
      return
    }

    if (selectedIds.value.length === 1) {
      const asset = tableData.value.find(a => {
        const id = a.CandidateID || a.FixedAssetId || a.id
        return id === selectedIds.value[0]
      })
      if (!asset) {
        toastError('Không tìm thấy tài sản')
        return
      }
      deleteDialogData.value = { type: 'single', asset }
    } else {
      deleteDialogData.value = { type: 'multiple', asset: null }
    }

    showDeleteDialog.value = true
  }

  /**
   * Xóa nhiều tài sản
   */
  function handleDeleteMultiple(items) {
    if (!items || items.length === 0) {
      toastError('Không có tài sản nào được chọn')
      return
    }

    if (items.length === 1) {
      deleteDialogData.value = { type: 'single', asset: items[0] }
    } else {
      deleteDialogData.value = { type: 'multiple', asset: null }
    }

    showDeleteDialog.value = true
  }

  /**
   * Xác nhận xóa
   */
  async function confirmDelete() {
    try {
      if (deleteDialogData.value.type === 'single') {
        const asset = deleteDialogData.value.asset
        if (!asset) {
          toastError('Không có tài sản để xóa')
          return
        }

        const assetId = asset.CandidateID || asset.FixedAssetId || asset.id
        if (!assetId) {
          toastError('Không thể xác định ID tài sản')
          return
        }

        await deleteFixedAsset(assetId)
        const code = asset.AssetCode || asset.FixedAssetCode || 'N/A'
        const name = asset.AssetName || asset.FixedAssetName || 'N/A'
        toastDelete(`Tài sản ${code} - ${name} đã bị xóa.`)
      } else {
        await deleteSelectedAssets()
        toastDelete(`Các tài sản đã bị xóa.`)
      }

      showDeleteDialog.value = false
      await loadFixedAssets()
    } catch (err) {
      toastError(`Lỗi: ${err.message}`)
    }
  }

  /**
   * Hủy xóa
   */
  function handleCancelDelete() {
    showDeleteDialog.value = false
    deleteDialogData.value = { type: 'single', asset: null }
  }
  //#endregion

  return {
    // State
    isPopupOpen,
    popupMode,
    selectedAssetId,
    editingAssetData,
    showDeleteDialog,
    deleteDialogData,
    showCancelDeclarationDialog,
    showSaveChangesDialog,

    // Handlers
    handleAddAsset,
    handleEdit,
    handleDuplicate,
    handleSubmitAsset,
    handleCloseForm,
    handleDelete,
    handleDeleteSelected,
    handleDeleteMultiple,
    confirmDelete,
    handleCancelDelete,
    handleCancelDeclarationNo,
    handleCancelDeclarationYes,
    handleSaveChangesCancel,
    handleSaveChangesNo,
    handleSaveChangesYes
  }
}
