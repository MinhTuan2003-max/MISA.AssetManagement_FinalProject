/**
 * Map code số (backend) sang code chữ (frontend)
 * UpdatedBy: HMTuan (01/11/2025) - Convert to PascalCase
 */
export const DepartmentCodeMap = Object.freeze({
  '01': 'BGH',
  '02': 'HCQT',
  '03': 'TV',
  '04': 'DT',
  '05': 'CM'
});

/**
 * Map code chữ sang code số (reverse)
 * UpdatedBy: HMTuan (01/11/2025) - Convert to PascalCase
 */
export const ReverseDepartmentCodeMap = Object.freeze({
  'BGH': '01',
  'HCQT': '02',
  'TV': '03',
  'DT': '04',
  'CM': '05'
});

/**
 * Danh sách bộ phận sử dụng (fallback khi backend chưa có)
 * UpdatedBy: HMTuan (01/11/2025) - Convert to PascalCase
 */
export function getDepartmentOptions() {
  return [
    {
      DepartmentCode: '01',
      DepartmentName: 'Ban Giám hiệu',
      MappedCode: 'BGH'
    },
    {
      DepartmentCode: '02',
      DepartmentName: 'Phòng Hành chính - Quản trị',
      MappedCode: 'HCQT'
    },
    {
      DepartmentCode: '03',
      DepartmentName: 'Phòng Tài vụ',
      MappedCode: 'TV'
    },
    {
      DepartmentCode: '04',
      DepartmentName: 'Phòng Đào tạo',
      MappedCode: 'DT'
    },
    {
      DepartmentCode: '05',
      DepartmentName: 'Tổ chuyên môn (Giáo viên bộ môn)',
      MappedCode: 'CM'
    }
  ];
}

/**
 * Hàm chuyển code chữ sang code số
 * @param {string} mappedCode - Code chữ (BGH, HCQT...)
 * @returns {string} - Code số (01, 02...)
 * UpdatedBy: HMTuan (01/11/2025)
 */
export function getOriginalDepartmentCode(mappedCode) {
  return ReverseDepartmentCodeMap[mappedCode] || mappedCode;
}

/**
 * Hàm chuyển code số sang code chữ
 * @param {string} originalCode - Code số (01, 02...)
 * @returns {string} - Code chữ (BGH, HCQT...)
 * UpdatedBy: HMTuan (01/11/2025)
 */
export function getMappedDepartmentCode(originalCode) {
  return DepartmentCodeMap[originalCode] || originalCode;
}
