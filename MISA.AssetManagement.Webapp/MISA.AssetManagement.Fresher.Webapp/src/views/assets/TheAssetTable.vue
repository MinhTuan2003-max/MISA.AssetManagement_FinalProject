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
import MsDataTable from '@/components/ms-table/MsDataTable.vue'
import { computed } from 'vue'
import { fixedAssetColumns } from '@/utils/fixedAssetColumns.js'

const props = defineProps({
  candidates: { type: Array, default: () => [] },
  selectedIds: { type: Array, default: () => [] },
  isAllSelected: { type: Boolean, default: false },
  isSearching: { type: Boolean, default: false },
  pagination: { type: Object, default: null }
})

const emit = defineEmits([
  'select-all',
  'select-item',
  'add',
  'edit',
  'delete',
  'duplicate',
  'row-click',
  'page-change',
  'page-size-change'

])

const columns = fixedAssetColumns

const emptyMessage = computed(() =>
  props.isSearching ? 'Không có kết quả tìm kiếm' : 'Không có dữ liệu'
)
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
