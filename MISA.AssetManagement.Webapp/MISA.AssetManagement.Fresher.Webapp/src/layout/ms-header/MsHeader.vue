<template>
  <header class="app-header">
    <!-- BÊN TRÁI: Tiêu đề màn hình -->
    <div class="header-left">
      <h1 class="app-title">Danh sách tài sản</h1>
    </div>

    <!-- BÊN PHẢI: Thông tin và hành động -->
    <div class="header-right">
      <!-- Nhóm chứa tên đơn vị + input năm theo dõi -->
      <div class="header-info-group">
        <h1 class="app-content">Sở tài chính</h1>

        <!-- Ô nhập năm theo dõi -->
        <div class="year-input">
          <span class="year-prefix">Năm</span>
          <input
            type="number"
            v-model="trackedYear"
            min="2000"
            max="2100"
            @keydown.stop
          />
          <!-- Nút tăng giảm năm -->
          <div class="year-controls">
            <button type="button" class="year-btn up" @click="increaseYear"></button>
            <button type="button" class="year-btn down" @click="decreaseYear"></button>
          </div>
        </div>
      </div>

      <!-- Nhóm các icon thao tác góc phải -->
      <div class="icons-group">
        <div class="icon-action">
          <span class="icon bell-icon"></span>
        </div>
        <div class="icon-action">
          <span class="icon show-list-icon"></span>
        </div>
        <div class="icon-action">
          <span class="icon question-icon"></span>
        </div>

        <!-- Nhóm avatar + icon dropdown -->
        <div class="icon-action avatar-group">
          <span class="icon avatar-icon"></span>
          <span class="icon down-icon"></span>
        </div>
      </div>
    </div>
  </header>
</template>

<script setup>
/**
 * MsHeader
 * Hiển thị tên màn hình hiện tại, năm theo dõi, và các icon thao tác (thông báo, hỗ trợ, tài khoản).
 * CreatedBy: HMTuan (28/10/2025)
 */

import { ref } from 'vue'

//#region STATE
/**
 * Năm theo dõi hiện tại
 * Mặc định: năm hiện tại của hệ thống
 */
const trackedYear = ref(new Date().getFullYear())
//#endregion

//#region METHODS
/**
 * Tăng năm theo dõi lên 1 (tối đa 2100)
 */
function increaseYear() {
  if (trackedYear.value < 2100) trackedYear.value++
}

/**
 * Giảm năm theo dõi đi 1 (tối thiểu 2000)
 */
function decreaseYear() {
  if (trackedYear.value > 2000) trackedYear.value--
}
//#endregion
</script>

<style scoped>
.app-header {
  height: 44px;
  background-color: #fff;
  padding: 0 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
}

/* LEFT SIDE */
.header-left {
  display: flex;
  align-items: center;
}

.app-title {
  font-size: 18px;
  font-weight: 600;
  color: #111;
  margin: 0;
}

/* RIGHT SIDE */
.header-right {
  display: flex;
  align-items: center;
  gap: 20px;
}

/* THÔNG TIN NĂM THEO DÕI */
.header-info-group {
  display: flex;
  align-items: center;
  gap: 20px;
}

.app-content {
  font-size: 16px;
  font-weight: 500;
  color: #111;
  margin: 0;
  white-space: nowrap;
}

/* INPUT NĂM THEO DÕI */
.year-input {
  display: flex;
  align-items: center;
  gap: 6px;
  background-color: rgba(26, 164, 200, 0.2);
  border-radius: 3px;
  width: 112px;
  height: 30px;
  padding: 0 4px 0 8px;
  position: relative;
  font-family: inherit;
}

.year-prefix {
  font-size: 13px;
  color: #111;
  white-space: nowrap;
}

.year-input input {
  border: none;
  background: transparent;
  text-align: right;
  font-size: 13px;
  font-weight: 700;
  color: #111;
  outline: none;
  flex: none;
  padding: 0;
  width: 50px;
  height: 16px;
}

/* Ẩn nút mặc định của input[type=number] */
.year-input input::-webkit-inner-spin-button,
.year-input input::-webkit-outer-spin-button {
  -webkit-appearance: none;
  margin: 0;
}

.year-input input:focus {
  color: #007b9c;
  font-weight: 700;
}

.year-input:hover {
  background-color: rgba(26, 164, 200, 0.3);
}

/* NÚT TĂNG / GIẢM NĂM */
.year-controls {
  display: flex;
  flex-direction: column;
  justify-content: center;
  gap: 2px;
}

.year-btn {
  width: 7px;
  height: 5px;
  border: none;
  background-color: transparent;
  cursor: pointer;
  background-repeat: no-repeat;
  background-size: contain;
  padding: 4px;
}

.year-btn.up {
  background: url('../../assets/icons/qlts-icon.svg') no-repeat -28px -338px;
}

.year-btn.down {
  background: url('../../assets/icons/qlts-icon.svg') no-repeat -72px -338px;
}

/* ICONS NHÓM HÀNH ĐỘNG */
.icons-group {
  display: flex;
  align-items: center;
  gap: 20px;
}

.icon-action {
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  width: 24px;
  height: 24px;
}

.icon-action.avatar-group {
  display: flex;
  align-items: center;
  gap: 4px;
  width: auto;
}

.icon-action.avatar-group .avatar-icon {
  width: 24px;
  height: 24px;
}

.icon-action.avatar-group .down-icon {
  width: 8px;
  height: 8px;
}

/* ICON ĐỊNH DẠNG CHUNG */
.icon {
  display: inline-block;
}

.bell-icon,
.show-list-icon,
.question-icon,
.avatar-icon {
  width: 24px;
  height: 24px;
}

.down-icon {
  width: 8px;
  height: 8px;
}
</style>
