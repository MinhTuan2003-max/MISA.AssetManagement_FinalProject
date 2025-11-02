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
          <span v-if="exporting">...</span>
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
      :existing-asset-codes="tableData.map(a => a.FixedAssetCode || a.AssetCode)"
      @close="handleCloseForm"
      @submit="handleSubmitAsset"
    />

    <!-- Dialog: Xóa tài sản -->
    <BaseDialog
      v-if="showDeleteDialog"
      :icon-class="'warning-icon'"
      :description="deleteDialogDescription"
      :buttons="deleteDialogButtons"
      @close="handleCancelDelete"
    />

    <!-- Dialog: Hủy khai báo tài sản -->
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

    <!-- Dialog: Lưu thay đổi trước khi tắt -->
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
import { useFixedAssets } from '@/composables/useFixedAssets.js'
import { useAssetHandlers } from './indexView.js'
import { useToastManager } from '@/utils/component/toastManager.js'
import { useExcelExport } from '@/composables/useExcelExport.js'
import { debounce } from '@/utils/component/debounce.js'

// Components
import BaseLayout from '@/layout/BaseLayout.vue'
import MsSearchBar from '@/components/ms-search/MsSearchBar.vue'
import MsButton from '@/components/ms-button/MsButton.vue'
import TheAssetTable from '@/views/assets/asset-table/TheAssetTable.vue'
import MsFilterButton from '@/components/ms-filter/MsFilterButton.vue'
import BaseDialog from '@/components/ms-dialog/MsDialog.vue'
import TheAssetForm from '@/views/assets/asset-form/TheAssetForm.vue'
import MsToastNotification from '@/components/ms-toast/MsToast.vue'

//#region Composables
const fixedAssetsComposable = useFixedAssets()
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
  handleSelectAll,
  handleSelectItem,
  handlePageChange,
  handlePageSizeChange,
  handleSearch
} = fixedAssetsComposable

const { exporting, exportFixedAssets } = useExcelExport()

// Toast Manager
const toastManager = useToastManager()
const { toasts, removeToast, toastSuccess, toastError } = toastManager

// Asset Handlers
const handlers = useAssetHandlers(fixedAssetsComposable, toastManager)
const {
  isPopupOpen,
  popupMode,
  selectedAssetId,
  editingAssetData,
  showDeleteDialog,
  deleteDialogData,
  showCancelDeclarationDialog,
  showSaveChangesDialog,
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
} = handlers
//#endregion

//#region Local State
const isFiltering = ref(false)
const showTable = ref(true)
const tableKey = ref(0)
//#endregion

//#region Lifecycle
onMounted(async () => {
  await Promise.all([loadFixedAssets(), loadFilterOptions()])
})

const debouncedSearch = debounce(async (query) => {
  await handleSearch(query)
}, 1000)

watch(searchQuery, async () => {
  showTable.value = false
  isFiltering.value = true
  await debouncedSearch(searchQuery.value)
  tableKey.value++
  await nextTick()
  isFiltering.value = false
  showTable.value = true
})

watch([filterCategory, filterDepartment], async () => {
  showTable.value = false
  isFiltering.value = true

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

//#region Other Actions
function handleRowClick(item) {
  console.log('Row clicked:', item)
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
