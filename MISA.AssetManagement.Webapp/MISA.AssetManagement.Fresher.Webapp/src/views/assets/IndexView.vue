<template>
  <BaseLayout>
    <!-- Toolbar -->
    <div class="toolbar">
      <div class="toolbar-left">
        <MsSearchBar
          v-model="searchQuery"
          placeholder="Tìm kiếm tài sản"
          @search="handleSearch"
        />

        <div class="filter-group">
          <MsFilterButton
            label="Loại tài sản"
            :options="categoryOptions"
            v-model="filterCategory"
            :isActive="!!filterCategory"
          />
          <MsFilterButton
            label="Bộ phận sử dụng"
            :options="departmentOptions"
            v-model="filterDepartment"
            :isActive="!!filterDepartment"
          />
        </div>
      </div>

      <div class="toolbar-right">
        <MsButton variant="asset" class="add-btn" @click="handleAddAsset">
          + Thêm tài sản
        </MsButton>
        <MsButton
          class="btn-icon toolbar-button"
          title="Xuất dữ liệu"
          @click="handleExportExcel"
          :disabled="exporting || filteredAssets.length === 0"
        >
          <i class="icon excel-icon"></i>
          <span v-if="exporting" style="margin-left: 4px; font-size: 10px;">...</span>
        </MsButton>
        <MsButton
          class="btn-icon toolbar-button"
          title="Xóa"
          @click="handleDeleteSelected"
        >
          <i class="icon delete-icon"></i>
        </MsButton>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="loading-overlay">
      <div class="spinner"></div>
      <p>Đang tải dữ liệu...</p>
    </div>

    <!-- Error State -->
    <div v-else-if="error && !loading" class="error-message">
      <i class="icon-error"></i>
      <p>{{ error }}</p>
      <button @click="loadFixedAssets">Thử lại</button>
    </div>

    <!-- Main Content -->
    <div v-else class="content-card">
      <!-- Filter/Search Loading Indicator -->
      <transition name="fade">
        <div v-if="isFiltering" key="loading" class="filter-loading">
          <div class="filter-spinner"></div>
          <span>Đang lọc dữ liệu...</span>
        </div>
      </transition>

      <div class="content-container">
        <div class="table-area">
          <transition name="fade-slide" mode="out-in">
            <TheAssetTable
              v-if="showTable"
              :key="tableKey"
              :candidates="tableData"
              :selected-ids="selectedIds"
              :is-all-selected="isAllSelected"
              :is-searching="searchQuery.trim().length > 0"
              :pagination="paginationData"
              @select-all="handleSelectAll"
              @select-item="handleSelectItem"
              @add="handleAddAsset"
              @edit="handleEdit"
              @delete="handleDelete"
              @delete-multiple="handleDeleteMultiple"
              @duplicate="handleDuplicate"
              @row-click="handleRowClick"
              @page-change="handlePageChange"
              @page-size-change="handlePageSizeChange"
            />
          </transition>
        </div>
      </div>
    </div>

    <!-- Form Popup -->
    <TheAssetForm
      :is-open="isPopupOpen"
      :mode="popupMode"
      :asset-id="selectedAssetId"
      :initial-data="editingAssetData"
      :category-options="categoryOptions"
      :department-options="departmentOptions"
      :existing-asset-codes="tableData.map(a => a.AssetCode || a.FixedAssetCode)"
      @close="handleCloseForm"
      @submit="handleSubmitAsset"
    />

    <!-- Dialog 1: Xóa tài sản -->
    <BaseDialog
      v-if="showDeleteDialog"
      :icon-class="'warning-icon'"
      :description="deleteDialogDescription"
      :buttons="deleteDialogButtons"
      @close="handleCancelDelete"
    />

    <!-- Dialog 2: Hủy khai báo tài sản -->
    <BaseDialog
      v-if="showCancelDeclarationDialog"
      icon-class="warning-icon"
      description="Bạn có muốn hủy khai báo tài sản?"
      :buttons="[
        { label: 'Không', type: 'cancel', action: handleCancelDeclarationNo },
        { label: 'Hủy bỏ', type: 'primary', action: handleCancelDeclarationYes }
      ]"
      @close="showCancelDeclarationDialog = false"
    />

    <!-- Dialog 3: Lưu thay đổi trước khi tắt -->
    <BaseDialog
      v-if="showSaveChangesDialog"
      icon-class="warning-icon"
      :description="'Thông tin thay đổi sẽ không được cập nhật nếu bạn không lưu. Bạn có muốn lưu các thay đổi này?'"
      :buttons="[
        { label: 'Hủy bỏ', type: 'cancel', action: handleSaveChangesCancel },
        { label: 'Không lưu', type: 'cancel', variant: 'primary-variant', action: handleSaveChangesNo },
        { label: 'Lưu', type: 'primary', action: handleSaveChangesYes }
      ]"
      @close="showSaveChangesDialog = false"
    />

    <!-- Dialog 4: Duplicate tài sản -->
    <BaseDialog
      v-if="showDuplicateDialog"
      :description="dialogMessage"
      icon-class="warning-icon"
      :buttons="[
        {
          label: 'Hủy',
          type: 'cancel',
          action: () => (showDuplicateDialog = false)
        },
        {
          label: 'Nhân bản',
          type: 'primary',
          action: confirmDuplicate
        }
      ]"
      @close="showDuplicateDialog = false"
    />

    <!-- Toast Notifications Container -->
    <div class="toast-container">
      <transition-group name="toast-list" tag="div">
        <MsToastNotification
          v-for="toast in toasts"
          :key="toast.id"
          :type="toast.type"
          :title="toast.title"
          :message="toast.message"
          :show-undo="toast.showUndo"
          :duration="toast.duration"
          @close="removeToast(toast.id)"
          @undo="toast.onUndo && toast.onUndo()"
        />
      </transition-group>
    </div>
  </BaseLayout>
</template>

<script setup>
import { ref, computed, onMounted, watch, nextTick } from 'vue'
import { useFixedAssets } from '@/composables/useFixedAssets'
import BaseLayout from '@/layout/BaseLayout.vue'
import MsSearchBar from '@/components/ms-search/MsSearchBar.vue'
import MsButton from '@/components/ms-button/MsButton.vue'
import TheAssetTable from '@/views/assets/TheAssetTable.vue'
import MsFilterButton from '@/components/ms-filter/MsFilterButton.vue'
import BaseDialog from '@/components/ms-dialog/MsDialog.vue'
import TheAssetForm from '@/views/assets/TheAssetForm.vue'
import MsToastNotification from '@/components/ms-toast/MsToast.vue'
import { useExcelExport } from '@/composables/useExcelExport.js'
import { FixedAsset } from '@/domains/models/FixedAsset'
import { debounce } from '@/utils/component/debounce.js'

//#region Composables
const {
  loading,
  error,
  selectedIds,
  currentPage,
  pageSize,
  searchQuery,
  filterCategory,
  filterDepartment,
  categoryOptions,
  departmentOptions,
  tableData,
  totalPages,
  isAllSelected,
  totalQuantity,
  totalCost,
  totalDepreciation,
  totalResidual,
  filteredAssets,
  loadFixedAssets,
  loadFilterOptions,
  loadShortNameSelectOptions,
  createFixedAsset,
  updateFixedAsset,
  deleteFixedAsset,
  duplicateFixedAsset,
  deleteSelectedAssets,
  handleSelectAll,
  handleSelectItem,
  handlePageChange,
  handlePageSizeChange,
  handleSearch
} = useFixedAssets()

const { exporting, exportFixedAssets } = useExcelExport()
//#endregion

//#region Local State
// Form Popup State
const isPopupOpen = ref(false)
const popupMode = ref('add')
const selectedAssetId = ref(null)
const editingAssetData = ref(null)
const formDataBeforeEdit = ref(null)

// Delete Dialog State
const showDeleteDialog = ref(false)
const deleteDialogData = ref({ type: 'single', asset: null })

// Cancel Declaration Dialog State
const showCancelDeclarationDialog = ref(false)

// Save Changes Dialog State
const showSaveChangesDialog = ref(false)
const pendingSubmitData = ref(null)
const showDuplicateDialog = ref(false)
const dialogMessage = ref('')
let assetToDuplicate = null

// Filter/Search Animation State
const isFiltering = ref(false)
const showTable = ref(true)
const tableKey = ref(0)

// Toast State
const toasts = ref([])
let filterTimeout = null
//#endregion

//#region Toast Management
function showToast({ type = 'success', title, message, showUndo = false, onUndo, duration = 3000 }) {
  const id = Date.now() + Math.random()
  toasts.value.push({ id, type, title, message, showUndo, duration, onUndo })
  return id
}

function removeToast(id) {
  const index = toasts.value.findIndex(t => t.id === id)
  if (index > -1) toasts.value.splice(index, 1)
}

function toastSuccess(message) {
  return showToast({ type: 'success', message, duration: 3000 })
}

function toastUpdate(message) {
  return showToast({ type: 'update', message, duration: 3000 })
}

function toastDelete(message) {
  return showToast({ type: 'delete', message, duration: 5000 })
}

function toastError(message) {
  return showToast({ type: 'error', message, duration: 4000 })
}
//#endregion

//#region Lifecycle
onMounted(async () => {
  await Promise.all([loadFixedAssets(), loadFilterOptions()])
})

const debouncedSearch = debounce(async (query) => {
  await handleSearch(query)
}, 1000)

watch(searchQuery, async () => {
  if (filterTimeout) clearTimeout(filterTimeout)
  showTable.value = false
  isFiltering.value = true
  await debouncedSearch(searchQuery.value)
  tableKey.value++
  await nextTick()
  isFiltering.value = false
  showTable.value = true
})

watch([filterCategory, filterDepartment], async () => {
  console.log('Filter changed:', {
    category: filterCategory.value,
    department: filterDepartment.value
  })

  // Hiển thị animation
  showTable.value = false
  isFiltering.value = true

  // Đợi composable load xong
  // (composable đã có watch riêng để gọi loadFixedAssets)
  await new Promise(resolve => {
    const checkLoading = setInterval(() => {
      if (!loading.value) {
        clearInterval(checkLoading)
        resolve()
      }
    }, 50)
  })

  tableKey.value++
  await nextTick()
  isFiltering.value = false
  showTable.value = true
})
//#endregion

//#region Computed
const paginationData = computed(() => ({
  totalItems: filteredAssets.value.length,
  currentPage: currentPage.value,
  pageSize: pageSize.value,
  totalPages: totalPages.value,
  totalQuantity: totalQuantity.value,
  totalCost: totalCost.value,
  totalDepreciation: totalDepreciation.value,
  totalResidual: totalResidual.value
}))

const deleteDialogDescription = computed(() => {
  if (!deleteDialogData.value) return ''
  if (deleteDialogData.value.type === 'single') {
    const asset = deleteDialogData.value.asset
    if (!asset) return 'Bạn có muốn xóa tài sản này không?'
    const code = asset.AssetCode || asset.FixedAssetCode || 'N/A'
    const name = asset.AssetName || asset.FixedAssetName || 'N/A'
    return `Bạn có muốn xóa tài sản ${code} - ${name}?`
  }
  const count = selectedIds.value.length
  return `${count} tài sản đã được chọn. Bạn có muốn xóa các tài sản này ra khỏi danh sách?`
})

const deleteDialogButtons = computed(() => [
  { label: 'Không', type: 'cancel', action: handleCancelDelete },
  { label: 'Xóa', type: 'primary', action: confirmDelete }
])
//#endregion

//#region CRUD Actions - Add
async function handleAddAsset() {
  popupMode.value = 'add'
  selectedAssetId.value = null
  editingAssetData.value = null
  formDataBeforeEdit.value = null
  await loadShortNameSelectOptions()
  isPopupOpen.value = true
}
//#endregion

//#region CRUD Actions - Edit
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

//#region CRUD Actions - Submit
/**
 * Submit asset từ form
 */
async function handleSubmitAsset(assetData) {
  try {
    if (popupMode.value === 'add') {
      await createFixedAsset(assetData)
      toastSuccess('Thêm tài sản thành công!')
    } else {
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
 * Kiểm tra thay đổi form
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
  // Add mode - hỏi hủy khai báo
  if (popupMode.value === 'add') {
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
  editingAssetData.value = null
  formDataBeforeEdit.value = null
  pendingSubmitData.value = null
}
//#endregion

//#region Dialog Handlers

// Dialog 2: Hủy khai báo
function handleCancelDeclarationNo() {
  showCancelDeclarationDialog.value = false
}

function handleCancelDeclarationYes() {
  showCancelDeclarationDialog.value = false
  closePopup()
}

/**
 * Dialog 3: Lưu thay đổi - Bấm "Hủy bỏ" (tắt dialog, giữ form)
 */
function handleSaveChangesCancel() {
  showSaveChangesDialog.value = false
  pendingSubmitData.value = null
  // Chỉ tắt dialog 3, giữ lại form edit
}

/**
 * Dialog 3: Lưu thay đổi - Bấm "Không lưu" (tắt form + dialog)
 */
function handleSaveChangesNo() {
  showSaveChangesDialog.value = false
  closePopup() // Đóng form + dialog
}

/**
 * Dialog 3: Lưu thay đổi - Bấm "Lưu" (lưu API + tắt form + dialog)
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
function handleDelete(item) {
  if (!item) {
    toastError('Không thể xác định tài sản cần xóa')
    return
  }
  deleteDialogData.value = { type: 'single', asset: item }
  showDeleteDialog.value = true
}

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

function handleCancelDelete() {
  showDeleteDialog.value = false
  deleteDialogData.value = { type: 'single', asset: null }
}
//#endregion

//#region Other Actions
function handleRowClick(item) {
  console.log('Row clicked:', item)
}

/**
 * Hàm được gọi khi người dùng chọn "Nhân bản" từ context menu
 */
function handleDuplicate(item) {
  if (!item) {
    toastError('Không thể xác định tài sản cần nhân bản')
    return
  }

  const assetId = item.CandidateID || item.FixedAssetId || item.id
  if (!assetId) {
    toastError('Không thể xác định ID tài sản')
    return
  }

  assetToDuplicate = item
  dialogMessage.value = `Bạn có muốn nhân bản tài sản <strong>${item.AssetName || item.FixedAssetName}</strong> không?`
  showDuplicateDialog.value = true
}

/**
 * Hàm thực hiện khi người dùng xác nhận trong dialog
 */
async function confirmDuplicate() {
  showDuplicateDialog.value = false
  const assetId = assetToDuplicate.CandidateID || assetToDuplicate.FixedAssetId || assetToDuplicate.id
  await duplicateFixedAsset(assetId)

  try {
    toastSuccess('Nhân bản tài sản thành công!')
    await loadFixedAssets() // reload danh sách
  } catch (err) {
    toastError(`Lỗi khi nhân bản tài sản: ${err.message}`)
  }
}

async function handleExportExcel() {
  try {
    if (filteredAssets.value.length === 0) {
      toastError('Không có dữ liệu để xuất')
      return
    }
    exportFixedAssets(filteredAssets.value)
    toastSuccess('Xuất Excel thành công!')
  } catch (err) {
    console.error('Export error:', err)
    toastError(`Lỗi xuất Excel: ${err.message}`)
  }
}
//#endregion
</script>

<style scoped>
.content-card {
  flex: 1;
  background-color: #ffffff;
  border: 1px solid rgb(175, 175, 175);
  display: flex;
  flex-direction: column;
  overflow: hidden;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.06);
  min-height: 0;
  position: relative;
}

.toolbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  flex-wrap: wrap;
  flex-shrink: 0;
  background-color: #f4f7ff;
}

.toolbar-left,
.toolbar-right {
  display: flex;
  align-items: center;
  gap: 8px;
}

.toolbar-left {
  flex: 1;
  min-width: 0;
}

.filter-group {
  display: flex;
  gap: 8px;
}

.toolbar-right {
  flex-shrink: 0;
}

.add-btn {
  background-color: #1aa4c8;
  border-radius: 3px;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.16);
  width: 110px;
  height: 36px;
  font-size: 13px;
  font-weight: normal;
  overflow: hidden;
}

.toolbar-button {
  background-color: #ffffff;
  border-radius: 3px;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.16);
  width: 36px;
  height: 36px;
  padding: 0;
  display: flex;
  align-items: center;
  justify-content: center;
}

.toolbar-button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.content-container {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  min-height: 0;
}

.table-area {
  flex: 1;
  display: flex;
  flex-direction: column;
  background: #fff;
  min-height: 200px;
  overflow: hidden;
  position: relative;
}

.loading-overlay {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 400px;
  background: white;
  border-radius: 8px;
  padding: 40px;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #f3f4f6;
  border-top-color: #2563eb;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.loading-overlay p {
  margin-top: 16px;
  color: #6b7280;
  font-size: 14px;
  font-weight: 500;
}

.error-message {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 400px;
  background: white;
  border-radius: 8px;
  padding: 40px;
  text-align: center;
}

.error-message .icon-error {
  width: 48px;
  height: 48px;
  background: #fee2e2;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 16px;
}

.error-message .icon-error::before {
  content: '⚠';
  font-size: 24px;
  color: #dc2626;
}

.error-message p {
  color: #dc2626;
  font-size: 14px;
  margin-bottom: 16px;
  max-width: 400px;
}

.error-message button {
  padding: 8px 16px;
  background: #2563eb;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 500;
  transition: background-color 0.2s;
}

.error-message button:hover {
  background: #1d4ed8;
}

.filter-loading {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px 24px;
  background: rgba(255, 255, 255, 0.95);
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  z-index: 10;
  backdrop-filter: blur(4px);
}

.filter-spinner {
  width: 20px;
  height: 20px;
  border: 3px solid #e5e7eb;
  border-top-color: #1aa4c8;
  border-radius: 50%;
  animation: spin 0.6s linear infinite;
}

.filter-loading span {
  color: #374151;
  font-size: 13px;
  font-weight: 500;
  white-space: nowrap;
}

.fade-enter-active, .fade-leave-active {
  transition: opacity 0.2s ease;
}

.fade-enter-from, .fade-leave-to {
  opacity: 0;
}

.fade-slide-enter-active {
  transition: all 0.4s ease-out;
}

.fade-slide-leave-active {
  transition: all 0.3s ease-in;
}

.fade-slide-enter-from {
  opacity: 0;
  transform: translateY(-15px);
}

.fade-slide-leave-to {
  opacity: 0;
  transform: translateY(15px);
}

.toast-container {
  position: fixed;
  top: 20px;
  right: 20px;
  z-index: 9999;
  pointer-events: none;
}

.toast-container > * {
  pointer-events: auto;
  margin-bottom: 12px;
}

.toast-list-enter-active,
.toast-list-leave-active {
  transition: all 0.3s ease;
}

.toast-list-enter-from {
  opacity: 0;
  transform: translateX(30px);
}

.toast-list-leave-to {
  opacity: 0;
  transform: translateX(30px);
}

@media (max-width: 1366px) {
  .toolbar-left, .toolbar-right { gap: 6px; }
  .filter-group { gap: 6px; }
}

@media (max-width: 768px) {
  .toolbar { flex-direction: column; align-items: stretch; }
  .toolbar-left, .toolbar-right { width: 100%; justify-content: space-between; }
  .filter-group { flex-wrap: wrap; }
  .toast-container { top: 10px; right: 10px; left: 10px; }
}

/* Breakpoint Mobile: < 576px */
@media (max-width: 575.98px) {
  .toolbar {
    flex-direction: column;
    align-items: stretch;
    gap: 8px;
    padding: 8px;
  }

  .toolbar-left {
    flex: 1;
    flex-direction: column;
    gap: 8px;
  }

  .toolbar-left > div:first-child {
    width: 100%;
  }

  .filter-group {
    width: 100%;
    flex-wrap: wrap;
    gap: 6px;
    margin-left: 0;
  }

  .toolbar-right {
    width: 100%;
    justify-content: space-between;
    gap: 6px;
  }

  .add-btn {
    flex: 1;
    min-width: 100px;
    height: 40px;
    font-size: 12px;
  }

  .toolbar-button {
    width: 40px;
    height: 40px;
    flex-shrink: 0;
  }

  .content-card {
    border: none;
    box-shadow: none;
  }

  .toast-container {
    top: 10px;
    right: 10px;
    left: 10px;
  }

  .filter-loading {
    padding: 10px 16px;
    font-size: 12px;
  }

  .filter-spinner {
    width: 16px;
    height: 16px;
    border-width: 2px;
  }
}

/* Breakpoint Tablet: 576px - 992px */
@media (min-width: 576px) and (max-width: 991.98px) {
  .toolbar {
    gap: 10px;
    padding: 10px;
  }

  .toolbar-left {
    gap: 8px;
  }

  .filter-group {
    gap: 6px;
  }

  .toolbar-right {
    gap: 6px;
  }

  .add-btn {
    width: 100px;
    height: 36px;
    font-size: 12px;
  }

  .toolbar-button {
    width: 36px;
    height: 36px;
  }

  .toast-container {
    top: 12px;
    right: 12px;
  }
}

/* Breakpoint Desktop: >= 992px */
@media (min-width: 992px) {
  .toolbar {
    flex-direction: row;
    gap: 12px;
  }

  .toolbar-left {
    flex-direction: row;
    flex: 1;
    gap: 8px;
  }

  .filter-group {
    gap: 8px;
  }

  .toolbar-right {
    flex-direction: row;
    gap: 8px;
  }

  .add-btn {
    width: 110px;
    height: 36px;
    font-size: 13px;
  }

  .toolbar-button {
    width: 36px;
    height: 36px;
  }

  .toast-container {
    top: 20px;
    right: 20px;
  }
}
</style>
