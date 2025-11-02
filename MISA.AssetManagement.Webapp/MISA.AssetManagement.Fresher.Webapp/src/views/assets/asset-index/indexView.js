import { ref } from 'vue'
import { FixedAsset } from '@/domains/models/FixedAsset.js'

/**
 * @fileoverview Cung cấp toàn bộ logic handler cho các thao tác CRUD & UI trên danh sách tài sản.
 * @module useAssetHandlers
 * @author HMTuan (30/10/2025)
 * @updated 03/11/2025
 */

/**
 * useAssetHandlers
 * @description
 * Cung cấp các phương thức quản lý thao tác CRUD (thêm, sửa, xóa, nhân bản)
 * và xử lý các dialog xác nhận, popup, toast... của module Tài sản.
 *
 * @param {Object} composable - useFixedAssets composable (bao gồm repository & state)
 * @param {Object} toastFn - Nhóm hàm toast notification (toastSuccess, toastError, ...)
 * @returns {Object} Các state & handler được export để dùng trong component
 */
export function useAssetHandlers(composable, toastFn) {
  //#region Destructure Composable
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
  //#endregion

  //#region Popup & Dialog State
  const isPopupOpen = ref(false)
  const popupMode = ref('add') // 'add' | 'edit' | 'duplicate'
  const selectedAssetId = ref(null)
  const editingAssetData = ref(null)
  const formDataBeforeEdit = ref(null)

  // Dialog
  const showDeleteDialog = ref(false)
  const deleteDialogData = ref({ type: 'single', asset: null })
  const showCancelDeclarationDialog = ref(false)
  const showSaveChangesDialog = ref(false)
  const pendingSubmitData = ref(null)
  //#endregion

  //#region CRUD - Add
  /**
   * Mở popup để thêm mới tài sản
   * CreatedBy: HMT (03/11/2025)
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
   * Mở popup sửa tài sản
   * @param {Object} item - Dòng dữ liệu được chọn
   */
  async function handleEdit(item) {
    try {
      const assetId = item.FixedAssetId || item.CandidateID || item.id
      if (!assetId) return toastError('Không thể xác định ID tài sản')

      popupMode.value = 'edit'
      selectedAssetId.value = assetId

      // Convert model sang dữ liệu form
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
   * Nhân bản tài sản → lấy dữ liệu từ server + mở form thêm mới
   * @param {Object} item - Dòng tài sản cần nhân bản
   */
  async function handleDuplicate(item) {
    try {
      const assetId = item.FixedAssetId || item.CandidateID || item.id
      if (!assetId) return toastError('Không thể xác định ID tài sản')

      const duplicateData = await getDuplicateData(assetId)

      popupMode.value = 'duplicate'
      selectedAssetId.value = null
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
   * Xử lý submit form tài sản (tạo mới / nhân bản / cập nhật)
   * @param {Object} assetData - Dữ liệu form đã được validate
   */
  async function handleSubmitAsset(assetData) {
    try {
      // ADD hoặc DUPLICATE
      if (popupMode.value === 'add' || popupMode.value === 'duplicate') {
        await createFixedAsset(assetData)
        const msg =
          popupMode.value === 'duplicate'
            ? 'Nhân bản tài sản thành công!'
            : 'Thêm tài sản thành công!'
        toastSuccess(msg)
      }
      // EDIT
      else {
        await updateFixedAsset(selectedAssetId.value, assetData)
        toastUpdate('Cập nhật tài sản thành công!')
      }

      await loadFixedAssets()
      closePopup()
    } catch (err) {
      const serverErr = err.response?.data?.errors
      if (serverErr) toastError(`Lỗi validation: ${JSON.stringify(serverErr, null, 2)}`)
      else toastError(`Lỗi: ${err.message}`)
    }
  }

  /**
   * Kiểm tra form có bị thay đổi so với trước khi sửa hay không
   * @param {Object} currentData - Dữ liệu hiện tại của form
   * @returns {boolean}
   */
  function hasFormChanged(currentData) {
    if (!formDataBeforeEdit.value) return false
    return JSON.stringify(formDataBeforeEdit.value) !== JSON.stringify(currentData)
  }

  /**
   * Xử lý khi người dùng đóng form → kiểm tra thay đổi & hiển thị dialog phù hợp
   * @param {Object} currentFormData - Dữ liệu form hiện tại
   */
  function handleCloseForm(currentFormData) {
    // Add hoặc Duplicate → hỏi xác nhận hủy
    if (popupMode.value === 'add' || popupMode.value === 'duplicate') {
      showCancelDeclarationDialog.value = true
      return
    }

    // Edit → kiểm tra thay đổi
    if (hasFormChanged(currentFormData)) {
      pendingSubmitData.value = currentFormData
      showSaveChangesDialog.value = true
    } else {
      closePopup()
    }
  }

  /**
   * Reset toàn bộ state liên quan popup và dialog
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

  //#region Dialog - Hủy khai báo / Lưu thay đổi

    /**
     * @description Xử lý khi người dùng chọn **Không** trong dialog “Hủy khai báo”.
     * → Đóng dialog, giữ nguyên form đang mở.
     */
    function handleCancelDeclarationNo() {
      showCancelDeclarationDialog.value = false
    }

    /**
     * @description Xử lý khi người dùng chọn **Có** trong dialog “Hủy khai báo”.
     * → Đóng dialog và đóng luôn popup form tài sản.
     */
    function handleCancelDeclarationYes() {
      showCancelDeclarationDialog.value = false
      closePopup()
    }

    /**
     * @description Xử lý khi người dùng nhấn **Hủy bỏ** trong dialog “Lưu thay đổi”.
     * → Đóng dialog, không làm gì thêm (vẫn giữ popup đang mở và dữ liệu form hiện tại).
     */
    function handleSaveChangesCancel() {
      showSaveChangesDialog.value = false
      pendingSubmitData.value = null
    }

    /**
     * @description Xử lý khi người dùng chọn **Không lưu** trong dialog “Lưu thay đổi”.
     * → Đóng dialog và đóng popup mà không lưu lại dữ liệu vừa chỉnh sửa.
     */
    function handleSaveChangesNo() {
      showSaveChangesDialog.value = false
      closePopup()
    }

    /**
     * @description Xử lý khi người dùng chọn **Lưu** trong dialog “Lưu thay đổi”.
     * → Tiến hành gọi hàm `handleSubmitAsset()` để lưu lại dữ liệu đang chờ.
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

  //#region CRUD - Delete
  /**
   * Xử lý xóa 1 tài sản
   * @param {Object} item - Dữ liệu tài sản được chọn
   */
  function handleDelete(item) {
    if (!item) return toastError('Không thể xác định tài sản cần xóa')

    deleteDialogData.value = { type: 'single', asset: item }
    showDeleteDialog.value = true
  }

  /**
   * Xử lý xóa nhiều tài sản đã chọn (checkbox)
   */
  function handleDeleteSelected() {
    if (selectedIds.value.length === 0) return toastError('Vui lòng chọn ít nhất một tài sản')

    if (selectedIds.value.length === 1) {
      const asset = tableData.value.find(a => {
        const id = a.CandidateID || a.FixedAssetId || a.id
        return id === selectedIds.value[0]
      })
      if (!asset) return toastError('Không tìm thấy tài sản')
      deleteDialogData.value = { type: 'single', asset }
    } else {
      deleteDialogData.value = { type: 'multiple', asset: null }
    }

    showDeleteDialog.value = true
  }

  /**
   * Xử lý xóa nhiều tài sản (khi truyền danh sách trực tiếp)
   */
  function handleDeleteMultiple(items) {
    if (!items || items.length === 0) return toastError('Không có tài sản nào được chọn')

    deleteDialogData.value =
      items.length === 1
        ? { type: 'single', asset: items[0] }
        : { type: 'multiple', asset: null }

    showDeleteDialog.value = true
  }

  /**
   * Xác nhận thực hiện xóa tài sản
   */
  async function confirmDelete() {
    try {
      if (deleteDialogData.value.type === 'single') {
        const asset = deleteDialogData.value.asset
        if (!asset) return toastError('Không có tài sản để xóa')

        const assetId = asset.CandidateID || asset.FixedAssetId || asset.id
        if (!assetId) return toastError('Không thể xác định ID tài sản')

        await deleteFixedAsset(assetId)
        const code = asset.AssetCode || asset.FixedAssetCode || 'N/A'
        const name = asset.AssetName || asset.FixedAssetName || 'N/A'
        toastDelete(`Tài sản ${code} - ${name} đã bị xóa.`)
      } else {
        await deleteSelectedAssets()
        toastDelete('Các tài sản đã bị xóa.')
      }

      showDeleteDialog.value = false
      await loadFixedAssets()
    } catch (err) {
      toastError(`Lỗi: ${err.message}`)
    }
  }

  /**
   * Hủy hành động xóa tài sản
   */
  function handleCancelDelete() {
    showDeleteDialog.value = false
    deleteDialogData.value = { type: 'single', asset: null }
  }
  //#endregion

  return {
    //#region State
    isPopupOpen,
    popupMode,
    selectedAssetId,
    editingAssetData,
    showDeleteDialog,
    deleteDialogData,
    showCancelDeclarationDialog,
    showSaveChangesDialog,
    //#endregion

    //#region CRUD & Dialog Handlers
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
    //#endregion
  }
}
