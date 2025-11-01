/**
 * @fileoverview Composable export Excel cho danh sách tài sản cố định
 * @description Cung cấp các hàm hỗ trợ export danh sách và tổng hợp sang Excel
 * @createdBy HMTuan (30/10/2025)
 */

// #region Import
import { ref } from 'vue'
import { exportToExcelWithHeaders } from '@/utils/excel/excelExporter.js'
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
        { key: 'FixedAssetCode', label: 'Mã tài sản' },
        { key: 'FixedAssetName', label: 'Tên tài sản' },
        { key: 'FixedAssetCategoryName', label: 'Loại tài sản' },
        { key: 'DepartmentName', label: 'Bộ phận sử dụng' },
        { key: 'Quantity', label: 'Số lượng' },
        { key: 'Cost', label: 'Nguyên giá' },
        { key: 'AccumulatedDepreciation', label: 'HM/KH lũy kế' },
        { key: 'RemainingValue', label: 'Giá trị còn lại' }
      ]

      // Chuẩn hoá dữ liệu export
      const exportData = assets.map(asset => ({
        FixedAssetCode: asset.AssetCode || asset.FixedAssetCode || '',
        FixedAssetName: asset.AssetName || asset.FixedAssetName || '',
        FixedAssetCategoryName: asset.AssetTypeName || asset.FixedAssetCategoryName || '',
        DepartmentName: asset.DepartmentName || asset.DepartmentName || '',
        Quantity: asset.Quantity || asset.Quantity || 0,
        Cost: asset.Cost || asset.Cost || 0,
        AccumulatedDepreciation: asset.AccumulatedDepreciation || asset.AccumulatedDepreciation || 0,
        RemainingValue: asset.RemainingValue || asset.RemainingValue || 0
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
        { key: 'FixedAssetCode', label: 'Mã tài sản' },
        { key: 'FixedAssetName', label: 'Tên tài sản' },
        { key: 'FixedAssetCategoryName', label: 'Loại tài sản' },
        { key: 'DepartmentName', label: 'Bộ phận sử dụng' },
        { key: 'Quantity', label: 'Số lượng' },
        { key: 'Cost', label: 'Nguyên giá' },
        { key: 'AccumulatedDepreciation', label: 'HM/KH lũy kế' },
        { key: 'RemainingValue', label: 'Giá trị còn lại' }
      ]

      // Chuẩn hoá dữ liệu export
      const exportData = assets.map(asset => ({
        FixedAssetCode: asset.AssetCode || asset.FixedAssetCode || '',
        FixedAssetName: asset.AssetName || asset.FixedAssetName || '',
        FixedAssetCategoryName: asset.AssetTypeName || asset.FixedAssetCategoryName || '',
        DepartmentName: asset.DepartmentName || asset.DepartmentName || '',
        Quantity: asset.Quantity || asset.Quantity || 0,
        Cost: asset.Cost || asset.Cost || 0,
        AccumulatedDepreciation: asset.AccumulatedDepreciation || asset.AccumulatedDepreciation || 0,
        RemainingValue: asset.RemainingValue || asset.RemainingValue || 0
      }))

      // Thêm dòng tổng hợp vào cuối
      exportData.push({
        FixedAssetCode: '',
        FixedAssetName: '',
        FixedAssetCategoryName: '',
        DepartmentName: 'TỔNG CỘNG',
        Quantity: summary.totalQuantity || 0,
        Cost: summary.totalCost || 0,
        AccumulatedDepreciation: summary.totalDepreciation || 0,
        RemainingValue: summary.totalResidual || 0
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
