/**
 * @fileoverview Department Model
 * @description Đại diện cho đối tượng "Bộ phận" (Department)
 * - Map trực tiếp với DTO từ backend.
 * - Cung cấp các phương thức helper để chuyển sang định dạng option cho UI.
 * @createdBy HMTuan (30/10/2025)
 */

export class Department {
  /**
   * @constructor
   * @param {Object} data - Dữ liệu từ backend hoặc enum
   */
  constructor(data = {}) {
    /** @type {string|null} ID của bộ phận */
    this.department_id = data.department_id || null

    /** @type {string} Mã bộ phận */
    this.department_code = data.department_code || ''

    /** @type {string} Tên viết tắt của bộ phận */
    this.department_short_name = data.department_short_name || ''

    /** @type {string} Tên đầy đủ của bộ phận */
    this.department_name = data.department_name || ''

    /** @type {string} Mô tả chi tiết (nếu có) */
    this.description = data.description || ''
  }

  /** @returns {string|null} ID */
  get id() {
    return this.department_id
  }

  /** @returns {string} Mã bộ phận */
  get code() {
    return this.department_code
  }

  /** @returns {string} Tên viết tắt */
  get shortName() {
    return this.department_short_name
  }

  /** @returns {string} Tên đầy đủ */
  get name() {
    return this.department_name
  }

  /**
   * Chuyển sang option cho **toolbar filter**
   * (Hiển thị tên đầy đủ)
   * @returns {{ value: string, label: string, fullName: string, id: string|null }}
   */
  toToolbarOption() {
    return {
      value: this.department_code,
      label: this.department_name,
      fullName: this.department_name,
      id: this.department_id
    }
  }

  /**
   * Chuyển sang option cho **form popup**
   * (Hiển thị tên viết tắt)
   * @returns {{ value: string, label: string, fullName: string, id: string|null }}
   */
  toFormOption() {
    return {
      value: this.department_code,
      label: this.department_short_name,
      fullName: this.department_name,
      id: this.department_id
    }
  }

  /**
   * Deprecated: Giữ lại cho backward compatibility.
   * @deprecated Dùng `toToolbarOption()` hoặc `toFormOption()` thay thế.
   * @returns {object}
   */
  toOption() {
    return this.toToolbarOption()
  }

  /**
   * Alias cho `toFormOption()`
   * @returns {object}
   */
  toSelectOption() {
    return this.toFormOption()
  }

  /**
   * Tạo 1 instance từ dữ liệu API.
   * @param {object} data
   * @returns {Department}
   */
  static fromApi(data) {
    return new Department(data)
  }

  /**
   * Tạo mảng các instance từ danh sách API.
   * @param {object[]} dataArray
   * @returns {Department[]}
   */
  static fromApiArray(dataArray) {
    if (!Array.isArray(dataArray)) return []
    return dataArray.map(data => new Department(data))
  }
}
