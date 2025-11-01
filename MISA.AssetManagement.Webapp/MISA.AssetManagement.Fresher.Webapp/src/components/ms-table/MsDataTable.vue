<template>
  <div class="table-wrapper">
    <!-- Table Container với scrollbar dọc -->
    <div
      ref="tableWrapperRef"
      class="table-container"
      tabindex="0"
      @keydown="handleKeyDown"
      @scroll="syncScrollFromTable"
    >
      <table ref="tableRef" class="data-table">
        <!-- THEAD - Sticky header -->
        <thead class="sticky-header">
          <tr>
            <!-- Checkbox column -->
            <th v-if="showCheckbox" class="checkbox-col">
              <input type="checkbox" :checked="isAllSelected" @change="emit('select-all')" />
            </th>

            <!-- Data columns -->
            <th
              v-for="column in columns"
              :key="column.key"
              :class="column.class"
              :style="{ width: columnWidths[column.key] ? `${columnWidths[column.key]}px` : (column.width || 'auto') }"
            >
              <div class="th-content" :class="{ 'th-right': column.class?.includes('text-right') }">
                <!-- Column label với sort -->
                <span
                  :class="{ sortable: column.sortable !== false }"
                  @click="column.sortable !== false && handleSort(column.key)"
                >
                  {{ column.label }}
                  <!-- Sort indicator -->
                  <span
                    v-if="sortField === column.key"
                    class="sort-icon"
                    :class="{ 'sort-desc': sortOrder === -1 }"
                  >
                  </span>
                </span>

                <!-- Resize handle -->
                <div class="resize-handle" @mousedown="handleResizeStart($event, column.key)" />
              </div>
            </th>
          </tr>
        </thead>

        <!-- TBODY -->
        <tbody>
          <!-- Empty state -->
          <tr v-if="sortedItems.length === 0">
            <td :colspan="columns.length + (showCheckbox ? 1 : 0)" class="empty-state">
              {{ emptyMessage }}
            </td>
          </tr>

          <!-- Data rows -->
          <tr
            v-for="(item, index) in sortedItems"
            :key="getItemId(item)"
            class="data-row"
            :class="{ selected: isSelected(item), focused: focusedRow === index }"
            tabindex="0"
            @focus="focusedRow = index"
            @click="(e) => handleRowSelection(e, item, index)"
            @contextmenu="handleContextMenu($event, item)"
          >
            <!-- Checkbox cell -->
            <td v-if="showCheckbox" class="checkbox-col" @click.stop>
              <input type="checkbox" :checked="isSelected(item)" @change="emit('select-item', getItemId(item))" />
            </td>

            <!-- Data cells -->
            <td v-for="column in columns" :key="column.key" :class="column.class">
              <!-- Custom slot -->
              <slot
                v-if="column.type === 'custom'"
                :name="`cell-${column.key}`"
                :item="item"
                :value="item[column.key]"
                :column="column"
              />

              <!-- Index column -->
              <template v-else-if="column.type === 'index'">
                {{ index + 1 }}
              </template>

              <!-- Formatted value -->
              <template v-else>
                {{ formatValue(item[column.key], column.type) }}
              </template>
            </td>
          </tr>
        </tbody>

        <tfoot v-if="pagination" class="sticky-footer">
          <tr class="pagination-row">
            <!-- Pagination controls cell -->
            <td :colspan="quantityColumnIndex + (showCheckbox ? 1 : 0)" class="pagination-controls-cell">
              <div class="pagination-left">
                <!-- Total records -->
                <span>Tổng số: <strong>{{ pagination.totalItems }}</strong> bản ghi</span>

                <!-- Page size selector -->
                <select class="select-input" :value="pagination.pageSize" @change="handlePageSizeChange">
                  <option value="10">10</option>
                  <option value="20">20</option>
                  <option value="50">50</option>
                  <option value="100">100</option>
                </select>

                <!-- Page navigation -->
                <div class="pagination-controls">
                  <!-- Previous button -->
                  <button class="btn-page" :disabled="pagination.currentPage === 1" @click="handlePrevPage">
                    <span class="icon prev-icon"></span>
                  </button>

                  <!-- Page numbers -->
                  <button
                    v-for="(page, idx) in pages"
                    :key="idx"
                    class="btn-page"
                    :class="{ active: page === pagination.currentPage }"
                    :disabled="page === '...'"
                    @click="handlePageClick(page)"
                  >
                    {{ page }}
                  </button>

                  <!-- Next button -->
                  <button class="btn-page" :disabled="pagination.currentPage === pagination.totalPages" @click="handleNextPage">
                    <span class="icon after-icon"></span>
                  </button>
                </div>
              </div>
            </td>

            <!-- Summary cells - align với data columns -->
            <td class="summary-cell text-right">
              {{ pagination.totalQuantity || 0 }}
            </td>

            <td class="summary-cell text-right">
              {{ (pagination.totalCost || 0).toLocaleString('vi-VN') }}
            </td>

            <td class="summary-cell text-right">
              {{ (pagination.totalDepreciation || 0).toLocaleString('vi-VN') }}
            </td>

            <td class="summary-cell text-right">
              {{ (pagination.totalResidual || 0).toLocaleString('vi-VN') }}
            </td>

            <td class="summary-cell"></td>
          </tr>
        </tfoot>
      </table>
    </div>

    <!-- Context Menu -->
    <MsContextMenu
      :visible="!!contextMenu"
      :position="contextMenu || { x: 0, y: 0 }"
      :items="contextMenuItems"
      @action="onContextMenuAction"
      @close="closeContextMenu"
    />
  </div>
</template>

<script setup>
/**
 * Component: MsTable
 * Mô tả: Component bảng dữ liệu có sorting, chọn dòng, resize cột và phân trang.
 * CreatedBy: HMTuan - 29/10/2025
 */

// #region Import
import { ref, computed, onMounted, onUnmounted, watch } from 'vue'
import MsContextMenu from '@/components/ms-context-menu/MsContextMenu.vue'
import { contexMenuItem } from '@/utils/component/contexMenuItem.js'
// #endregion

// #region Props
/**
 * Các props điều khiển dữ liệu, cấu hình cột và trạng thái bảng
 */
const props = defineProps({
  columns: {
    type: Array,
    required: true,
    validator: (value) => value.every(col => col.key && col.label)
  },
  items: {
    type: Array,
    default: () => []
  },
  showCheckbox: {
    type: Boolean,
    default: true
  },
  selectedIds: {
    type: Array,
    default: () => []
  },
  isAllSelected: {
    type: Boolean,
    default: false
  },
  emptyMessage: {
    type: String,
    default: 'Không có dữ liệu'
  },
  pagination: {
    type: Object,
    default: null
  },
  defaultSort: {
    type: Object,
    default: () => ({ field: 'AssetCode', order: 1 })
  }
})
// #endregion

// #region Emits
/**
 * Các event phát ra ngoài để cha xử lý
 */
const emit = defineEmits([
  'select-all',
  'select-item',
  'row-click',
  'add',
  'edit',
  'delete',
  'duplicate',
  'page-change',
  'page-size-change'
])
// #endregion

// #region State
const sortField = ref(props.defaultSort?.field || null)
const sortOrder = ref(props.defaultSort?.order || 1)
const columnWidths = ref({})
const resizing = ref(null)
const contextMenu = ref(null)
const selectedRows = ref([]) // Mảng index các rows đang chọn (keyboard)
const focusedRow = ref(0) // Row đang focus
const lastSelectedRow = ref(null) // Row cuối cùng được chọn (để Shift select range)
// #endregion

// #region Refs
const tableRef = ref(null)
const scrollbarRef = ref(null)
const tableWrapperRef = ref(null)
// #endregion

// #region Computed
/**
 * Sắp xếp dữ liệu theo cột được chọn
 * @returns {Array} danh sách items đã sắp xếp
 * createdBy: HMTuan - 29/10/2025
 */
const sortedItems = computed(() => {
  if (!sortField.value) return props.items

  return [...props.items].sort((a, b) => {
    const aVal = a[sortField.value]
    const bVal = b[sortField.value]

    if (aVal === null || aVal === undefined) return 1
    if (bVal === null || bVal === undefined) return -1

    if (typeof aVal === 'number' && typeof bVal === 'number') {
      return (aVal - bVal) * sortOrder.value
    }

    const aStr = String(aVal).toLowerCase()
    const bStr = String(bVal).toLowerCase()
    return aStr.localeCompare(bStr, 'vi') * sortOrder.value
  })
})

/**
 * Tính danh sách số trang hiển thị
 */
const pages = computed(() => {
  if (!props.pagination) return []

  const { currentPage, totalPages } = props.pagination
  const result = []

  if (totalPages <= 5) {
    for (let i = 1; i <= totalPages; i++) result.push(i)
  } else {
    if (currentPage <= 3) {
      result.push(1, 2, 3, '...', totalPages)
    } else if (currentPage >= totalPages - 2) {
      result.push(1, '...', totalPages - 2, totalPages - 1, totalPages)
    } else {
      result.push(1, '...', currentPage - 1, currentPage, currentPage + 1, '...', totalPages)
    }
  }
  return result
})

/**
 * Tìm vị trí cột "Quantity" trong bảng để căn summary đúng cột
 */
const quantityColumnIndex = computed(() => {
  const idx = props.columns.findIndex(c => c.key === 'Quantity')
  return idx >= 0 ? idx : props.columns.length - 4
})

const contextMenuItems = contexMenuItem
// #endregion

// #region Methods
/**
 * Xử lý chọn hành động trong context menu
 * @param {Object} item - item trong context menu
 */
function onContextMenuAction(item) {
  emit(item.action, contextMenu.value?.item)
  closeContextMenu()
}

/**
 * Xử lý sắp xếp cột
 */
function handleSort(columnKey) {
  if (sortField.value === columnKey) {
    sortOrder.value = sortOrder.value * -1
  } else {
    sortField.value = columnKey
    sortOrder.value = 1
  }
}

/**
 * Định dạng giá trị hiển thị trong ô
 */
function formatValue(value, type) {
  if (value === null || value === undefined) return '--'

  switch (type) {
    case 'number':
    case 'currency':
      return new Intl.NumberFormat('vi-VN').format(value)
    case 'date':
      return new Date(value).toLocaleDateString('vi-VN')
    default:
      return value
  }
}

/**
 * Lấy ID của một bản ghi (ưu tiên các field phổ biến)
 */
function getItemId(item) {
  return item.CandidateID || item.fixed_asset_id || item.id || item.Id
}

/**
 * Kiểm tra một bản ghi có được chọn hay không
 */
function isSelected(item) {
  const id = getItemId(item)
  return props.selectedIds.includes(id)
}

/**
 * Xử lý phím điều hướng trong bảng - GỬI SELECT-ITEM CHO PARENT
 */
function handleKeyDown(e) {
  if (!sortedItems.value.length) return

  const isCtrl = e.ctrlKey || e.metaKey
  const isShift = e.shiftKey

  switch (e.key) {
    // ARROW DOWN
    case 'ArrowDown':
      e.preventDefault()
      focusedRow.value = Math.min(focusedRow.value + 1, sortedItems.value.length - 1)

      if (isShift) {
        updateSelectionWithShift()
      }

      scrollToFocusedRow()
      break

    // ARROW UP
    case 'ArrowUp':
      e.preventDefault()
      focusedRow.value = Math.max(focusedRow.value - 1, 0)

      if (isShift) {
        updateSelectionWithShift()
      }

      scrollToFocusedRow()
      break

    // SPACE: Toggle chọn/bỏ chọn row focus
    case ' ':
      e.preventDefault()

      if (isCtrl) {
        // Ctrl+Space: toggle row + emit select-item
        toggleRowSelection(focusedRow.value)
      } else if (isShift) {
        // Shift+Space: chọn dải từ lastSelectedRow → focusedRow
        updateSelectionWithShift()
      } else {
        // Space thường: chọn CHỈ row focus
        selectedRows.value = [focusedRow.value]
        lastSelectedRow.value = focusedRow.value
        const itemId = getItemId(sortedItems.value[focusedRow.value])
        emit('select-item', itemId)
      }

      scrollToFocusedRow()
      break

    // ENTER: Mở/xem chi tiết row focus
    case 'Enter':
      e.preventDefault()
      emit('row-click', sortedItems.value[focusedRow.value])
      break

    // DELETE: Xóa các row đã chọn
    case 'Delete':
      e.preventDefault()

      if (selectedRows.value.length === 0) {
        // Nếu không có gì được chọn, xóa row focus
        const itemId = getItemId(sortedItems.value[focusedRow.value])
        emit('delete', [itemId])
      } else {
        // Xóa tất cả rows đã chọn - GỬI ID, KHÔNG PHẢI ITEM
        const itemIdsToDelete = selectedRows.value.map(i => getItemId(sortedItems.value[i]))
        emit('delete', itemIdsToDelete)
        console.log(itemIdsToDelete)
      }
      break

    // Ctrl+A: Chọn tất cả
    case 'a':
    case 'A':
      if (isCtrl) {
        e.preventDefault()
        selectedRows.value = sortedItems.value.map((_, index) => index)
        lastSelectedRow.value = sortedItems.value.length - 1

        // Emit select-item cho tất cả
        sortedItems.value.forEach(item => {
          emit('select-item', getItemId(item))
        })
      }
      break

    // Escape: Bỏ chọn tất cả
    case 'Escape':
      e.preventDefault()
      selectedRows.value = []
      lastSelectedRow.value = null
      break
  }
}

/**
 * ✅ Cập nhật selection khi Shift được giữ - GỬI SELECT-ITEM
 */
function updateSelectionWithShift() {
  if (lastSelectedRow.value === null) {
    lastSelectedRow.value = 0
  }

  const start = Math.min(lastSelectedRow.value, focusedRow.value)
  const end = Math.max(lastSelectedRow.value, focusedRow.value)

  selectedRows.value = []
  for (let i = start; i <= end; i++) {
    selectedRows.value.push(i)
  }

  // ✅ Emit select-item cho tất cả items trong range
  for (let i = start; i <= end; i++) {
    const itemId = getItemId(sortedItems.value[i])
    emit('select-item', itemId)
  }
}

/**
 * ✅ Toggle chọn/bỏ chọn 1 row - GỬI SELECT-ITEM
 */
function toggleRowSelection(rowIndex) {
  const index = selectedRows.value.indexOf(rowIndex)

  if (index > -1) {
    selectedRows.value.splice(index, 1)
  } else {
    selectedRows.value.push(rowIndex)
  }

  lastSelectedRow.value = rowIndex

  // ✅ Emit select-item để parent xử lý toggle
  const itemId = getItemId(sortedItems.value[rowIndex])
  emit('select-item', itemId)
}

/**
 * Cuộn đến dòng đang được focus trong bảng
 */
function scrollToFocusedRow() {
  const tableBody = document.querySelector('.table-body')
  if (!tableBody) return

  const rows = tableBody.querySelectorAll('tr')
  const focusedRowEl = rows[focusedRow.value]

  if (focusedRowEl) {
    focusedRowEl.scrollIntoView({
      behavior: 'smooth',
      block: 'nearest'
    })
  }
}

/**
 * Mở menu chuột phải tại vị trí con trỏ
 */
function handleContextMenu(e, item) {
  e.preventDefault()
  contextMenu.value = { x: e.clientX, y: e.clientY, item }
}

/**
 * Đóng context menu
 */
function closeContextMenu() {
  contextMenu.value = null
}

/**
 * Xử lý chọn dòng trong bảng (hỗ trợ Ctrl, Shift)
 */
function handleRowSelection(e, item, index) {
  const id = getItemId(item)

  if (e.ctrlKey || e.metaKey) {
    emit('select-item', id)
    focusedRow.value = index
    return
  }

  if (e.shiftKey) {
    focusedRow.value = index
    return
  }

  emit('select-item', id)
  focusedRow.value = index
}

/**
 * Bắt đầu resize cột
 */
function handleResizeStart(e, columnKey) {
  e.preventDefault()
  const currentWidth = columnWidths.value[columnKey] || 150
  resizing.value = { columnKey, startX: e.clientX, startWidth: currentWidth }
}

/**
 * Resize cột khi kéo chuột
 */
function handleMouseMove(e) {
  if (!resizing.value) return

  const diff = e.clientX - resizing.value.startX
  const newWidth = Math.max(80, resizing.value.startWidth + diff)

  columnWidths.value = {
    ...columnWidths.value,
    [resizing.value.columnKey]: newWidth
  }
}

/**
 * Kết thúc resize cột
 */
function handleMouseUp() {
  resizing.value = null
}

/**
 * Đồng bộ scroll giữa bảng và scrollbar phụ
 */
function syncScrollFromTable(e) {
  if (scrollbarRef.value) scrollbarRef.value.scrollLeft = e.target.scrollLeft
}

/**
 * Cập nhật chiều rộng scrollbar để khớp với bảng
 */
function updateScrollbarWidth() {
  if (tableRef.value && scrollbarRef.value) {
    const scrollbarContent = scrollbarRef.value.querySelector('.scrollbar-content')
    if (scrollbarContent) scrollbarContent.style.width = `${tableRef.value.scrollWidth}px`
  }
}

/**
 * Phân trang: sang trang trước
 */
function handlePrevPage() {
  if (props.pagination && props.pagination.currentPage > 1) {
    emit('page-change', props.pagination.currentPage - 1)
  }
}

/**
 * Phân trang: sang trang kế tiếp
 */
function handleNextPage() {
  if (props.pagination && props.pagination.currentPage < props.pagination.totalPages) {
    emit('page-change', props.pagination.currentPage + 1)
  }
}

/**
 * Phân trang: chọn trang cụ thể
 */
function handlePageClick(page) {
  if (page !== '...') emit('page-change', page)
}

/**
 * Thay đổi số bản ghi/trang
 */
function handlePageSizeChange(e) {
  emit('page-size-change', parseInt(e.target.value))
}
// #endregion

// #region Lifecycle
onMounted(() => {
  document.addEventListener('mousemove', handleMouseMove)
  document.addEventListener('mouseup', handleMouseUp)
  document.addEventListener('click', closeContextMenu)
  setTimeout(() => updateScrollbarWidth(), 100)
  window.addEventListener('resize', updateScrollbarWidth)
})

onUnmounted(() => {
  document.removeEventListener('mousemove', handleMouseMove)
  document.removeEventListener('mouseup', handleMouseUp)
  document.removeEventListener('click', closeContextMenu)
  window.removeEventListener('resize', updateScrollbarWidth)
})

watch(() => props.items, () => {
  setTimeout(() => updateScrollbarWidth(), 50)
}, { deep: true })
// #endregion
</script>

<style scoped>
.table-wrapper {
  position: relative;
  display: flex;
  flex-direction: column;
  height: 100%;
  border: 1px solid #e9ecef;
  border-radius: 6px;
  overflow: hidden;
}

.table-container {
  flex: 1;
  overflow-x: auto;
  overflow-y: auto;
  outline: none;
  position: relative;
}

.data-table {
  width: 100%;
  min-width: max-content;
  border-collapse: collapse;
  font-family: 'Roboto', sans-serif;
  font-size: 13px;
  color: #333;
  background-color: #ffffff;
}

.sticky-header {
  position: sticky;
  top: 0;
  z-index: 100;
  background-color: #f8f9fa;
}

th {
  height: 38px;
  padding: 12px 16px;
  font-weight: 600;
  font-size: 13px;
  color: #495057;
  border-bottom: 2px solid #dee2e6;
  text-align: left;
  position: relative;
  user-select: none;
  white-space: nowrap;
}

.th-content {
  display: flex;
  align-items: center;
  justify-content: space-between;
  height: 100%;
}

.th-content.th-right {
  justify-content: flex-end;
}

.sortable {
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 4px;
}

.sortable:hover {
  color: #2563eb;
}

.sort-icon {
  font-size: 10px;
  transition: transform 0.2s;
  display: inline-block;
  color: #2563eb;
}

.sort-icon.sort-desc {
  transform: rotate(180deg);
}

.resize-handle {
  position: absolute;
  right: 0;
  top: 0;
  bottom: 0;
  width: 8px;
  cursor: col-resize;
  background: transparent;
}

.resize-handle:hover {
  background: #2563eb;
  opacity: 0.3;
}

.checkbox-col {
  width: 40px;
  text-align: center;
  padding-left: 16px !important;
}

tbody {
  display: table-row-group;
}

td {
  padding: 12px 16px;
  font-size: 13px;
  color: #333333;
  border-bottom: 1px solid #f1f3f5;
  white-space: nowrap;
}

.data-row {
  cursor: pointer;
  transition: background-color 0.15s;
  display: table-row;
  height: 39px;
}

.data-row:hover {
  background-color: #f8f9fa;
}

.data-row.focused {
  background-color: rgba(26, 164, 200, 0.2);
}

.data-row.selected {
  background-color: #eff6ff;
}

.empty-state {
  display: table-cell;
  text-align: center;
  padding: 230px 24px;
  color: #6b7280;
  font-style: italic;
  min-height: 300px;
  vertical-align: middle;
}

.sticky-footer {
  position: sticky;
  bottom: 0;
  background: #ffffff;
  z-index: 90;
  border-top: 2px solid #dee2e6;
}

.pagination-row {
  display: table-row;
  height: 39px;
  background: #ffffff;
}

.pagination-controls-cell {
  display: table-cell;
  padding: 0 16px;
  vertical-align: middle;
  border-bottom: none;
  background: #f8f9fa;
}

.pagination-left {
  display: flex;
  align-items: center;
  gap: 12px;
  font-size: 13px;
  height: 100%;
}

.select-input {
  width: 59px;
  height: 26px;
  background-color: #ffffff;
  border-radius: 2.625px;
  border: 1px solid #afafaf;
  font-size: 13px;
  cursor: pointer;
  font-family: 'Roboto', sans-serif;
}

.select-input:focus {
  outline: 2px solid #2563eb;
  outline-offset: 0;
}

.pagination-controls {
  display: flex;
  gap: 4px;
  align-items: center;
}

.btn-page {
  min-width: 20px;
  height: 20px;
  padding: 0 8px;
  background: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 11px;
  font-family: 'Roboto', sans-serif;
  font-weight: 400;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.btn-page:hover:not(:disabled) {
  background-color: #f3f4f6;
}

.btn-page.active {
  background: rgb(229, 229, 229);
  color: #333;
  font-weight: 700;
}

.btn-page:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.summary-cell {
  display: table-cell;
  height: 39px;
  font-weight: 600;
  font-size: 13px;
  color: #1f2937;
  background: #f8f9fa;
  padding: 0 16px;
  border-bottom: none;
  vertical-align: middle;
  text-align: right;
}

.text-right {
  text-align: right;
}

@media (max-width: 1366px) {
  th,
  td {
    padding: 0 12px;
  }
}

</style>
