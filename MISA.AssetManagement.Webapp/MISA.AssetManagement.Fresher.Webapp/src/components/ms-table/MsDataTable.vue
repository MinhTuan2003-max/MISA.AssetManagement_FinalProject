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
 * Component: MsDataTable
 * Mô tả: Component bảng dữ liệu có sorting, chọn dòng, resize cột và phân trang.
 * CreatedBy: HMTuan - 29/10/2025
 */

import MsContextMenu from '@/components/ms-context-menu/MsContextMenu.vue'
import { contexMenuItem } from '@/utils/component/contexMenuItem.js'
import { useMsDataTable } from './msDataTable.js'

// #region Props
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
    default: () => ({
      field: 'AssetCode',
      order: -1
    })
  }
})
// #endregion

// #region Emits
const emit = defineEmits([
  'select-all',
  'select-item',
  'row-click',
  'add',
  'edit',
  'delete',
  'delete-multiple',
  'duplicate',
  'page-change',
  'page-size-change'
])
// #endregion

// #region Setup
const {
  sortField,
  sortOrder,
  columnWidths,
  contextMenu,
  focusedRow,
  tableRef,
  tableWrapperRef,
  sortedItems,
  pages,
  quantityColumnIndex,
  onContextMenuAction,
  handleSort,
  formatValue,
  getItemId,
  isSelected,
  handleKeyDown,
  handleContextMenu,
  closeContextMenu,
  handleRowSelection,
  handleResizeStart,
  syncScrollFromTable,
  handlePrevPage,
  handleNextPage,
  handlePageClick,
  handlePageSizeChange
} = useMsDataTable(props, emit)

const contextMenuItems = contexMenuItem
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

.data-row {
  transition: background-color 0.15s ease-in-out;
}

/* Hover chuột */
.data-row:hover {
  background-color: rgba(8, 66, 114, 0.2);
}

/* Focus bàn phím */
.keyboard-nav .data-row.focused {
  background-color: rgba(8, 66, 114, 0.2);
}

/* Dòng được chọn */
.data-row.selected {
  background-color: rgba(26, 164, 200, 0.2);
}

/* Ưu tiên hover (chuột) hơn focus/selected */
.data-row.selected:hover,
.keyboard-nav .data-row.focused:hover {
  background-color: rgba(8, 66, 114, 0.2);
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
  outline: 2px solid rgba(26, 164, 200, .2);;
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
  color: #333333;
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
