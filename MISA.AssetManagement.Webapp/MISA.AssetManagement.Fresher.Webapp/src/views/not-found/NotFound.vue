<template>
  <div class="notfound-page">
    <div class="notfound-card">
      <div class="notfound-icon">⚠️</div>
      <h1 class="notfound-title">404 - Không tìm thấy trang</h1>
      <p class="notfound-message">
        Có vẻ như bạn đã nhập sai địa chỉ hoặc trang này không tồn tại trong hệ thống MISA.
      </p>
      <button class="btn-primary" @click="goBack">
        Quay lại trang chủ
      </button>
    </div>
  </div>
</template>

<script setup>
/**
 * Component NotFoundPage.vue
 * Hiển thị giao diện thông báo lỗi 404 (không tìm thấy trang)
 * CreatedBy: HMTuan (29/10/2025)
 */

import { useRouter } from 'vue-router'

// Khởi tạo router để điều hướng
const router = useRouter()

/**
 * Hàm điều hướng người dùng quay lại trang chủ
 */
const goBack = () => {
  router.push('/')
}
</script>

<style scoped>
/* Toàn trang */
.notfound-page {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100vh;
  background-color: #f6f8fb;
  font-family: 'Roboto', sans-serif;
}

/* Thẻ card chứa nội dung thông báo */
.notfound-card {
  text-align: center;
  background-color: #fff;
  padding: 40px 60px;
  border-radius: 16px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.08);
  max-width: 480px;
}

/* Icon cảnh báo */
.notfound-icon {
  font-size: 56px;
  margin-bottom: 12px;
}

/* Tiêu đề chính (404 - Không tìm thấy trang) */
.notfound-title {
  font-size: 24px;
  font-weight: 600;
  color: #2d3748;
  margin-bottom: 8px;
}

/* Thông báo mô tả */
.notfound-message {
  font-size: 15px;
  color: #4a5568;
  margin-bottom: 24px;
}

/* Nút quay lại trang chủ */
.btn-primary {
  background-color: #0078d4;
  border: none;
  color: #fff;
  font-size: 15px;
  font-weight: 500;
  padding: 10px 24px;
  border-radius: 8px;
  cursor: pointer;
  transition: background 0.2s ease;
}

/* Hiệu ứng hover */
.btn-primary:hover {
  background-color: #005ea1;
}
</style>
