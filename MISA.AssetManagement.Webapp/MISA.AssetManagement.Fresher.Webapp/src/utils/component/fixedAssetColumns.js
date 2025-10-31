/**
 * Danh sách cột cho bảng tài sản
 * CreatedBy: HMTuan (30/10/2025)
 */
export const fixedAssetColumns = [
  { key: 'Index', label: 'STT', type: 'index', width: '60px' },
  { key: 'AssetCode', label: 'Mã tài sản', type: 'text', width: '150px' },
  { key: 'AssetName', label: 'Tên tài sản', type: 'text', width: '250px' },
  { key: 'AssetTypeName', label: 'Loại tài sản', type: 'text', width: '180px' },
  { key: 'DepartmentName', label: 'Bộ phận sử dụng', type: 'text', width: '200px' },
  { key: 'Quantity', label: 'Số lượng', type: 'number', class: 'text-right', width: '100px' },
  { key: 'Cost', label: 'Nguyên giá', type: 'currency', class: 'text-right', width: '130px' },
  { key: 'AccumulatedDepreciation', label: 'HM/KH lũy kế', type: 'currency', class: 'text-right', width: '140px' },
  { key: 'RemainingValue', label: 'Giá trị còn lại', type: 'currency', class: 'text-right', width: '140px' },
  { key: 'Actions', label: 'Chức năng', type: 'custom', class: 'text-center', sortable: false, width: '100px' }
]
