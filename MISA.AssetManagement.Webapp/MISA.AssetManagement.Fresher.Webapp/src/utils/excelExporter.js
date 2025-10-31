import * as XLSX from 'xlsx'

/**
 * Hàm xuất dữ liệu ra file Excel (có tiêu đề tùy chỉnh)
 * @param {Array<Object>} data - Danh sách dữ liệu cần export (mỗi phần tử là 1 object)
 * @param {Array<{ key: string, label: string }>} headers - Cấu hình cột hiển thị
 *        key: tên thuộc tính trong data
 *        label: tên hiển thị trong file Excel
 * @param {String} [fileName='export'] - Tên file Excel (không bao gồm phần mở rộng)
 * @param {String} [sheetName='Sheet1'] - Tên sheet trong file Excel
 * @returns {Boolean} - Trả về true nếu export thành công, ngược lại throw lỗi
 * CreatedBy: HMTuan (29/10/2025)
 */
export function exportToExcelWithHeaders(data, headers, fileName = 'export', sheetName = 'Sheet1') {
  try {
    // Chuyển đổi dữ liệu sang định dạng có header hiển thị rõ ràng
    const transformedData = data.map(row => {
      const newRow = {}
      headers.forEach(header => {
        newRow[header.label] = row[header.key]
      })
      return newRow
    })

    // Tạo worksheet từ dữ liệu JSON
    const ws = XLSX.utils.json_to_sheet(transformedData)

    // Tính toán và tự động set độ rộng cột (auto-size)
    const colWidths = headers.map(header => {
      const maxWidth = Math.max(
        header.label.length,
        ...data.map(row => {
          const value = row[header.key]
          return value ? String(value).length : 0
        })
      )
      return { wch: Math.min(maxWidth + 2, 50) } // Giới hạn độ rộng tối đa
    })
    ws['!cols'] = colWidths

    // Tạo workbook và gắn sheet vào
    const wb = XLSX.utils.book_new()
    XLSX.utils.book_append_sheet(wb, ws, sheetName)

    // Ghi file Excel xuống máy người dùng
    XLSX.writeFile(wb, `${fileName}.xlsx`)

    return true
  } catch (error) {
    // Xử lý lỗi khi export
    console.error('Lỗi khi xuất Excel (exportToExcelWithHeaders):', error)
    throw error
  }
}
