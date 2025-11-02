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
/* ========== BASE STYLES (Default cho 1920px) ========== */
.app-header {
  height: 44px;
  background-color: #fff;
  padding: 0 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
  transition: all 0.3s ease;
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
  transition: font-size 0.3s ease;
}

/* RIGHT SIDE */
.header-right {
  display: flex;
  align-items: center;
  gap: 20px;
  transition: gap 0.3s ease;
}

/* THÔNG TIN NĂM THEO DÕI */
.header-info-group {
  display: flex;
  align-items: center;
  gap: 20px;
  transition: gap 0.3s ease;
}

.app-content {
  font-size: 16px;
  font-weight: 500;
  color: #111;
  margin: 0;
  white-space: nowrap;
  transition: font-size 0.3s ease;
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
  transition: all 0.3s ease;
}

.year-prefix {
  font-size: 13px;
  color: #111;
  white-space: nowrap;
  transition: font-size 0.3s ease;
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
  transition: font-size 0.3s ease;
}

/* Ẩn nút mặc định của input[type=number] */
.year-input input::-webkit-inner-spin-button,
.year-input input::-webkit-outer-spin-button {
  -webkit-appearance: none;
  margin: 0;
}

.year-input input[type="number"] {
  -moz-appearance: textfield;
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
  transition: opacity 0.2s ease;
}

.year-btn:hover {
  opacity: 0.7;
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
  transition: gap 0.3s ease;
}

.icon-action {
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  width: 24px;
  height: 24px;
  transition: all 0.2s ease;
}

.icon-action:hover {
  opacity: 0.7;
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
  transition: all 0.3s ease;
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

@media (max-width: 1368px) {
  .app-header {
    height: 42px;
    padding: 0 16px;
  }

  .app-title {
    font-size: 17px;
  }

  .header-right {
    gap: 16px;
  }

  .header-info-group {
    gap: 16px;
  }

  .app-content {
    font-size: 15px;
  }

  .year-input {
    width: 106px;
    height: 28px;
    padding: 0 4px 0 7px;
    gap: 5px;
  }

  .year-prefix {
    font-size: 12px;
  }

  .year-input input {
    font-size: 12px;
    width: 48px;
    height: 15px;
  }

  .icons-group {
    gap: 16px;
  }

  .icon-action {
    width: 22px;
    height: 22px;
  }

  .bell-icon,
  .show-list-icon,
  .question-icon,
  .avatar-icon {
    width: 22px;
    height: 22px;
  }

  .down-icon {
    width: 7px;
    height: 7px;
  }

  .year-btn {
    width: 6px;
    height: 4px;
    padding: 3px;
  }
}

@media (max-width: 1200px) {
  .app-header {
    height: 40px;
    padding: 0 14px;
  }

  .app-title {
    font-size: 16px;
  }

  .header-right {
    gap: 14px;
  }

  .header-info-group {
    gap: 14px;
  }

  .app-content {
    font-size: 14px;
  }

  .year-input {
    width: 100px;
    height: 26px;
    padding: 0 3px 0 6px;
    gap: 4px;
  }

  .year-prefix {
    font-size: 11px;
  }

  .year-input input {
    font-size: 11px;
    width: 45px;
    height: 14px;
  }

  .icons-group {
    gap: 14px;
  }

  .icon-action {
    width: 20px;
    height: 20px;
  }

  .bell-icon,
  .show-list-icon,
  .question-icon,
  .avatar-icon {
    width: 20px;
    height: 20px;
  }

  .down-icon {
    width: 6px;
    height: 6px;
  }
}

@media (max-width: 767.98px) {
  .app-header {
    height: auto;
    min-height: 40px;
    padding: 8px 12px;
    flex-wrap: wrap;
    gap: 10px;
  }

  .header-left {
    width: 100%;
    order: 1;
  }

  .app-title {
    font-size: 15px;
  }

  .header-right {
    width: 100%;
    order: 2;
    gap: 12px;
    justify-content: space-between;
  }

  .header-info-group {
    gap: 10px;
    flex: 1;
    min-width: 0;
  }

  .app-content {
    font-size: 13px;
    overflow: hidden;
    text-overflow: ellipsis;
    max-width: 100px;
  }

  .year-input {
    width: 95px;
    height: 26px;
    padding: 0 3px 0 6px;
    gap: 4px;
  }

  .year-prefix {
    font-size: 11px;
  }

  .year-input input {
    font-size: 11px;
    width: 42px;
    height: 14px;
  }

  .icons-group {
    gap: 12px;
    flex-shrink: 0;
  }

  .icon-action {
    width: 20px;
    height: 20px;
  }

  .bell-icon,
  .show-list-icon,
  .question-icon,
  .avatar-icon {
    width: 20px;
    height: 20px;
  }

  .down-icon {
    width: 6px;
    height: 6px;
  }

  .year-btn {
    width: 5px;
    height: 3px;
    padding: 3px;
  }
}

@media (max-width: 479.98px) {
  .app-header {
    padding: 6px 10px;
  }

  .app-title {
    font-size: 14px;
  }

  .app-content {
    font-size: 12px;
    max-width: 80px;
  }

  .year-input {
    width: 90px;
    height: 24px;
  }

  .year-prefix {
    font-size: 10px;
  }

  .year-input input {
    font-size: 10px;
    width: 40px;
  }

  .icons-group {
    gap: 10px;
  }

  .icon-action {
    width: 18px;
    height: 18px;
  }

  .bell-icon,
  .show-list-icon,
  .question-icon,
  .avatar-icon {
    width: 18px;
    height: 18px;
  }

  .down-icon {
    width: 5px;
    height: 5px;
  }
}

* {
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}
</style>
