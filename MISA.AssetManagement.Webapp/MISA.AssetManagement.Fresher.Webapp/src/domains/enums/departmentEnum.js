/**
 * Map code số (backend) sang code chữ (frontend)
 * CreatedBy: HMTuan (29/10/2025)
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
 * CreatedBy: HMTuan (29/10/2025)
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
 * CreatedBy: HMTuan (29/10/2025)
 */
export function getDepartmentOptions() {
  return [
    {
      department_code: '01',
      department_name: 'Ban Giám hiệu',
      mapped_code: 'BGH'
    },
    {
      department_code: '02',
      department_name: 'Phòng Hành chính - Quản trị',
      mapped_code: 'HCQT'
    },
    {
      department_code: '03',
      department_name: 'Phòng Tài vụ',
      mapped_code: 'TV'
    },
    {
      department_code: '04',
      department_name: 'Phòng Đào tạo',
      mapped_code: 'DT'
    },
    {
      department_code: '05',
      department_name: 'Tổ chuyên môn (Giáo viên bộ môn)',
      mapped_code: 'CM'
    }
  ];
}

/**
 * Hàm chuyển code chữ sang code số
 * @param {string} mappedCode - Code chữ (BGH, HCQT...)
 * @returns {string} - Code số (01, 02...)
 * CreatedBy: HMTuan (29/10/2025)
 */
export function getOriginalDepartmentCode(mappedCode) {
  return ReverseDepartmentCodeMap[mappedCode] || mappedCode;
}

/**
 * Hàm chuyển code số sang code chữ
 * @param {string} originalCode - Code số (01, 02...)
 * @returns {string} - Code chữ (BGH, HCQT...)
 * CreatedBy: HMTuan (29/10/2025)
 */
export function getMappedDepartmentCode(originalCode) {
  return DepartmentCodeMap[originalCode] || originalCode;
}
