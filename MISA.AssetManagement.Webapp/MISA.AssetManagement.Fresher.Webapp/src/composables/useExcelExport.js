/**
 * @fileoverview Composable export Excel cho danh sách tài sản cố định
 * @description Cung cấp các hàm hỗ trợ export danh sách và tổng hợp sang Excel
 * @createdBy HMTuan (30/10/2025)
 */

// #region Import
import { ref } from 'vue'
import { exportToExcelWithHeaders } from '@/utils/excelExporter'
// #endregion

// #region useExcelExport
/**
 * Composable cung cấp logic export Excel
 * @returns {Object} exporting state và các hàm export
 * @createdBy HMTuan (30/10/2025)
 */
export function useExcelExport() {
  // #region State
  /**
   * Trạng thái đang export Excel
   * @type {Ref<Boolean>}
   */
  const exporting = ref(false)
  // #endregion

  // #region exportFixedAssets
  /**
   * Hàm export danh sách tài sản ra Excel
   * @param {Array} assets - Danh sách tài sản cần export
   * @param {Object} options - Tuỳ chọn (fileName, sheetName)
   * @returns {Boolean} - Trả về true nếu export thành công
   * @createdBy HMTuan (30/10/2025)
   */
  async function exportFixedAssets(assets, options = {}) {
    exporting.value = true

    try {
      const headers = [
        { key: 'fixed_asset_code', label: 'Mã tài sản' },
        { key: 'fixed_asset_name', label: 'Tên tài sản' },
        { key: 'fixed_asset_category_name', label: 'Loại tài sản' },
        { key: 'department_name', label: 'Bộ phận sử dụng' },
        { key: 'quantity', label: 'Số lượng' },
        { key: 'cost', label: 'Nguyên giá' },
        { key: 'accumulated_depreciation', label: 'HM/KH lũy kế' },
        { key: 'remaining_value', label: 'Giá trị còn lại' }
      ]

      // Chuẩn hoá dữ liệu export
      const exportData = assets.map(asset => ({
        fixed_asset_code: asset.AssetCode || asset.fixed_asset_code || '',
        fixed_asset_name: asset.AssetName || asset.fixed_asset_name || '',
        fixed_asset_category_name: asset.AssetTypeName || asset.fixed_asset_category_name || '',
        department_name: asset.DepartmentName || asset.department_name || '',
        quantity: asset.Quantity || asset.quantity || 0,
        cost: asset.Cost || asset.cost || 0,
        accumulated_depreciation: asset.AccumulatedDepreciation || asset.accumulated_depreciation || 0,
        remaining_value: asset.RemainingValue || asset.remaining_value || 0
      }))

      // Sinh tên file & sheet
      const timestamp = new Date().toISOString().slice(0, 10).replace(/-/g, '')
      const fileName = options.fileName || `DanhSachTaiSan_${timestamp}`
      const sheetName = options.sheetName || 'Danh sách tài sản'

      exportToExcelWithHeaders(exportData, headers, fileName, sheetName)
      return true
    } catch (error) {
      console.error('Export error:', error)
      throw error
    } finally {
      exporting.value = false
    }
  }
  // #endregion

  // #region exportWithSummary
  /**
   * Hàm export danh sách tài sản kèm dòng tổng hợp
   * @param {Array} assets - Danh sách tài sản cần export
   * @param {Object} summary - Dữ liệu tổng hợp (số lượng, nguyên giá,...)
   * @returns {Boolean} - Trả về true nếu export thành công
   * @createdBy HMTuan (30/10/2025)
   */
  async function exportWithSummary(assets, summary) {
    exporting.value = true

    try {
      const headers = [
        { key: 'fixed_asset_code', label: 'Mã tài sản' },
        { key: 'fixed_asset_name', label: 'Tên tài sản' },
        { key: 'fixed_asset_category_name', label: 'Loại tài sản' },
        { key: 'department_name', label: 'Bộ phận sử dụng' },
        { key: 'quantity', label: 'Số lượng' },
        { key: 'cost', label: 'Nguyên giá' },
        { key: 'accumulated_depreciation', label: 'HM/KH lũy kế' },
        { key: 'remaining_value', label: 'Giá trị còn lại' }
      ]

      // Chuẩn hoá dữ liệu export
      const exportData = assets.map(asset => ({
        fixed_asset_code: asset.AssetCode || asset.fixed_asset_code || '',
        fixed_asset_name: asset.AssetName || asset.fixed_asset_name || '',
        fixed_asset_category_name: asset.AssetTypeName || asset.fixed_asset_category_name || '',
        department_name: asset.DepartmentName || asset.department_name || '',
        quantity: asset.Quantity || asset.quantity || 0,
        cost: asset.Cost || asset.cost || 0,
        accumulated_depreciation: asset.AccumulatedDepreciation || asset.accumulated_depreciation || 0,
        remaining_value: asset.RemainingValue || asset.remaining_value || 0
      }))

      // Thêm dòng tổng hợp vào cuối
      exportData.push({
        fixed_asset_code: '',
        fixed_asset_name: '',
        fixed_asset_category_name: '',
        department_name: 'TỔNG CỘNG',
        quantity: summary.totalQuantity || 0,
        cost: summary.totalCost || 0,
        accumulated_depreciation: summary.totalDepreciation || 0,
        remaining_value: summary.totalResidual || 0
      })

      const timestamp = new Date().toISOString().slice(0, 10).replace(/-/g, '')
      const fileName = `DanhSachTaiSan_${timestamp}`

      exportToExcelWithHeaders(exportData, headers, fileName, 'Danh sách tài sản')
      return true
    } catch (error) {
      console.error('Export with summary error:', error)
      throw error
    } finally {
      exporting.value = false
    }
  }
  // #endregion

  // #region Return
  return {
    exporting,
    exportFixedAssets,
    exportWithSummary
  }
  // #endregion
}
// #endregion
