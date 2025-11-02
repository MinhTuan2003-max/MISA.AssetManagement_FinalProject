import axios from "axios";

// Đọc API Base URL từ biến môi trường
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

/**
 * Cấu hình Axios instance chung cho tất cả API
 * CreatedBy: HMTuan (28/10/2025)
 */
const http = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

// Interceptor request
http.interceptors.request.use(
  (config) => {
    // Có thể thêm token hoặc log request ở đây
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Interceptor response
http.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    return Promise.reject(error);
  }
);

/**
 * Lớp BaseApi dùng làm lớp cha cho tất cả API
 * Cung cấp các phương thức CRUD cơ bản
 * CreatedBy: HMTuan (28/10/2025)
 */
export class BaseApi {
  /**
   * Constructor
   * @param {string} resource - tên resource/endpoint của API
   */
  constructor(resource) {
    this.resource = resource.toLowerCase();
    this.axiosInstance = http;
  }

  /**
   * Lấy tất cả dữ liệu từ API
   * @param {Object} params - tham số lọc/query (tùy chọn)
   * @returns {Promise<any>} dữ liệu trả về từ API
   * CreatedBy: HMTuan (28/10/2025)
   */
  async getAll(params = {}) {
    // Loại bỏ các giá trị null/undefined/empty string
    const cleanParams = Object.entries(params).reduce((acc, [key, value]) => {
      if (value !== null && value !== undefined && value !== '') {
        acc[key] = value;
      }
      return acc;
    }, {});

    const res = await http.get(`/${this.resource}`, { params: cleanParams });
    return res.data;
  }

  /**
   * Lấy dữ liệu chi tiết theo ID
   * @param {string|number} id - ID bản ghi
   * @returns {Promise<any>} dữ liệu chi tiết
   * CreatedBy: HMTuan (28/10/2025)
   */
  async getById(id) {
    const res = await http.get(`/${this.resource}/${id}`);
    return res.data;
  }

  /**
   * Tạo mới một bản ghi
   * @param {Object} entity - dữ liệu bản ghi cần tạo
   * @returns {Promise<any>} dữ liệu bản ghi vừa tạo
   * CreatedBy: HMTuan (28/10/2025)
   */
  async create(entity) {
    const res = await http.post(`/${this.resource}/create`, entity);
    return res.data;
  }

  /**
   * Cập nhật bản ghi theo ID
   * @param {string|number} id - ID bản ghi cần cập nhật
   * @param {Object} entity - dữ liệu bản ghi cập nhật
   * @returns {Promise<any>} dữ liệu bản ghi sau khi cập nhật
   * CreatedBy: HMTuan (28/10/2025)
   */
  async update(id, entity) {
    const res = await http.put(`/${this.resource}/${id}/update`, entity);
    return res.data;
  }

  /**
   * Xóa một bản ghi theo ID
   * @param {string|number} id - ID bản ghi cần xóa
   * @returns {Promise<any>} kết quả xóa từ API
   * CreatedBy: HMTuan (28/10/2025)
   */
  async delete(id) {
    const res = await http.delete(`/${this.resource}/${id}`);
    return res.data;
  }

  /**
   * Xóa nhiều bản ghi theo danh sách ID
   * @param {Array<string|number>} ids - danh sách ID cần xóa
   * @returns {Promise<any>} kết quả xóa từ API
   * CreatedBy: HMTuan (28/10/2025)
   */
  async deleteBatch(ids) {
    const res = await http.delete(`/${this.resource}/batch`, { data: ids });
    return res.data;
  }
}
