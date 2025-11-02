/**
 * File: msDataTable.js
 * Mô tả: Logic xử lý cho component MsDataTable
 * CreatedBy: HMTuan - 29/10/2025
 */

import { ref, computed, onMounted, onUnmounted, watch } from 'vue'

export function useMsDataTable(props, emit) {
  // #region State
  const sortField = ref(props.defaultSort?.field || null)
  const sortOrder = ref(props.defaultSort?.order || -1)
  const columnWidths = ref({})
  const resizing = ref(null)
  const contextMenu = ref(null)
  const selectedRows = ref([]) // Mảng index các rows đang chọn (keyboard)
  const focusedRow = ref(null) // Row đang focus
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
    const id = item.CandidateID || item.fixed_asset_id || item.id || item.Id
    return id
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
    const isArrow = ['ArrowUp', 'ArrowDown'].includes(e.key)

    if (isArrow) {
      e.preventDefault()
      tableWrapperRef.value.classList.add('keyboard-nav') // đánh dấu chế độ bàn phím
    }

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
          const item = sortedItems.value[focusedRow.value]
          emit('delete', item)
        } else {
          // Xóa tất cả rows đã chọn - GỬI ARRAY ITEMS
          const itemsToDelete = selectedRows.value.map(i => sortedItems.value[i])
          emit('delete-multiple', itemsToDelete)
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
    }
  }

  /**
   * Cập nhật selection khi Shift được giữ - GỬI SELECT-ITEM
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

    // Emit select-item cho tất cả items trong range
    for (let i = start; i <= end; i++) {
      const itemId = getItemId(sortedItems.value[i])
      emit('select-item', itemId)
    }
  }

  /**
   * Toggle chọn/bỏ chọn 1 row - GỬI SELECT-ITEM
   */
  function toggleRowSelection(rowIndex) {
    const index = selectedRows.value.indexOf(rowIndex)

    if (index > -1) {
      selectedRows.value.splice(index, 1)
    } else {
      selectedRows.value.push(rowIndex)
    }

    lastSelectedRow.value = rowIndex

    // Emit select-item để parent xử lý toggle
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

  return {
    // State
    sortField,
    sortOrder,
    columnWidths,
    resizing,
    contextMenu,
    selectedRows,
    focusedRow,
    lastSelectedRow,

    // Refs
    tableRef,
    scrollbarRef,
    tableWrapperRef,

    // Computed
    sortedItems,
    pages,
    quantityColumnIndex,

    // Methods
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
  }
}
