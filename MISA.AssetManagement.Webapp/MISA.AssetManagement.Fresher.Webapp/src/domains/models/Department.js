/**
 * @fileoverview Department Model
 * @description Đại diện cho đối tượng "Bộ phận" (Department)
 * - Map trực tiếp với DTO từ backend.
 * - Cung cấp các phương thức helper để chuyển sang định dạng option cho UI.
 * @createdBy HMTuan (30/10/2025)
 */

export class Department {
  constructor(data = {}) {
    this.DepartmentId = data.DepartmentId || data.departmentId || null
    this.DepartmentCode = data.DepartmentCode || data.departmentCode || ''
    this.DepartmentShortName = data.DepartmentShortName || data.department_short_name || data.departmentShortName || ''
    this.DepartmentName = data.DepartmentName || data.departmentName || ''
    this.Description = data.Description || data.description || ''
  }

  get id() {
    return this.DepartmentId
  }

  get code() {
    return this.DepartmentCode
  }

  get shortName() {
    return this.DepartmentShortName
  }

  get name() {
    return this.DepartmentName
  }

  toToolbarOption() {
    return {
      value: this.DepartmentCode,
      label: this.DepartmentName,
      fullName: this.DepartmentName,
      id: this.DepartmentId
    }
  }

  toFormOption() {
    return {
      value: this.DepartmentCode,
      label: this.DepartmentShortName,
      fullName: this.DepartmentName,
      id: this.DepartmentId
    }
  }

  toOption() {
    return this.toToolbarOption()
  }

  toSelectOption() {
    return this.toFormOption()
  }

  static fromApi(data) {
    return new Department(data)
  }

  static fromApiArray(dataArray) {
    if (!Array.isArray(dataArray)) return []
    return dataArray.map(data => new Department(data))
  }
}

