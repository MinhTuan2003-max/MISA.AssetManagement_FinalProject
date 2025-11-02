<template>
  <MsDataTable
    :columns="columns"
    :items="candidates"
    :selected-ids="selectedIds"
    :is-all-selected="isAllSelected"
    :empty-message="emptyMessage"
    :pagination="pagination"
    @select-all="emit('select-all')"
    @select-item="emit('select-item', $event)"
    @row-click="emit('row-click', $event)"
    @add="emit('add', $event)"
    @edit="emit('edit', $event)"
    @delete="emit('delete', $event)"
    @duplicate="emit('duplicate', $event)"
    @page-change="emit('page-change', $event)"
    @page-size-change="emit('page-size-change', $event)"
  >
    <template #cell-Actions="{ item }">
      <div class="action-buttons">
        <button class="btn-action" title="Sửa" @click.stop="emit('edit', item)">
          <i class="icon icon-edit"></i>
        </button>
        <button class="btn-action" title="Nhân bản" @click.stop="emit('duplicate', item)">
          <i class="icon duplicate-icon"></i>
        </button>
      </div>
    </template>
  </MsDataTable>
</template>

<script setup>
/**
 * @fileoverview Component hiển thị bảng dữ liệu tài sản (TheAssetTable)
 * - Gói gọn logic hiển thị danh sách tài sản, selection, và phân trang.
 * - Sử dụng MsDataTable làm component bảng nền.
 * CreatedBy: HMTuan (30/10/2025)
 */

import { computed } from 'vue'
import MsDataTable from '@/components/ms-table/MsDataTable.vue'
import { fixedAssetColumns } from '@/utils/component/fixedAssetColumns.js'

//#region Props

/**
 * Các props nhận từ component cha
 */
const props = defineProps({
  /**
   * Danh sách tài sản hiển thị trong bảng
   * @type {Array<Object>}
   */
  candidates: { type: Array, default: () => [] },

  /**
   * Danh sách ID tài sản được chọn (checkbox)
   * @type {Array<string>}
   */
  selectedIds: { type: Array, default: () => [] },

  /**
   * Cờ kiểm tra có chọn tất cả hay không
   * @type {boolean}
   */
  isAllSelected: { type: Boolean, default: false },

  /**
   * Cờ hiển thị khi đang ở trạng thái tìm kiếm
   * @type {boolean}
   */
  isSearching: { type: Boolean, default: false },

  /**
   * Dữ liệu phân trang của bảng (page, pageSize, total, ...)
   * @type {Object|null}
   */
  pagination: { type: Object, default: null }
})
//#endregion

//#region Emits

/**
 * Các sự kiện phát ra để component cha xử lý
 */
const emit = defineEmits([
  /**
   * Khi người dùng chọn / bỏ chọn tất cả hàng
   */
  'select-all',

  /**
   * Khi người dùng chọn / bỏ chọn 1 hàng cụ thể
   */
  'select-item',

  /**
   * Khi người dùng nhấn nút thêm tài sản mới
   */
  'add',

  /**
   * Khi người dùng chỉnh sửa tài sản
   */
  'edit',

  /**
   * Khi người dùng xóa tài sản
   */
  'delete',

  /**
   * Khi người dùng nhân bản tài sản
   */
  'duplicate',

  /**
   * Khi click vào một dòng trong bảng
   */
  'row-click',

  /**
   * Khi thay đổi trang (pagination)
   */
  'page-change',

  /**
   * Khi thay đổi kích thước trang (page size)
   */
  'page-size-change'
])
//#endregion

//#region Table Config

/**
 * Cấu hình các cột hiển thị trong bảng tài sản
 * Import từ file fixedAssetColumns.js
 * @type {Array<Object>}
 */
const columns = fixedAssetColumns

/**
 * Thông báo hiển thị khi bảng không có dữ liệu
 * @type {ComputedRef<string>}
 */
const emptyMessage = computed(() =>
  props.isSearching ? 'Không có kết quả tìm kiếm' : 'Không có dữ liệu'
)
//#endregion
</script>

<style scoped>
.action-buttons {
  display: flex;
  gap: 8px;
  justify-content: center;
  align-items: center;
  opacity: 0;
  visibility: hidden;
  transition: opacity 0.15s ease, visibility 0.15s ease;
}

.data-row:hover .action-buttons,
.data-row.focused .action-buttons {
  opacity: 1;
  visibility: visible;
}

.btn-action {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 28px;
  height: 28px;
  padding: 0;
  background: transparent;
  border-radius: 4px;
  cursor: pointer;
  color: #6b7280;
  transition: all 0.2s ease;
  border: 1px solid transparent;
}

.btn-action:hover {
  background: #f3f4f6;
  border-color: #2563eb;
  color: #2563eb;
}

.btn-action:focus {
  outline: none;
  border-color: #2563eb;
  color: #2563eb;
}
</style>
