<template>
  <aside class="app-sidebar" :class="{ collapsed }">
    <!-- Logo khu vực trên cùng -->
    <div class="sidebar-logo">
      <span class="logo-icon"></span>
      <span class="logo-text" v-if="!collapsed">MISA QLTS</span>
    </div>

    <!-- Navigation menu - cho phép scroll -->
    <nav class="sidebar-nav">
      <RouterLink
        v-for="(item, index) in navItems"
        :key="index"
        :to="item.to || '#'"
        class="nav-item"
        :class="{ active: item.active }"
        :data-tooltip="item.tooltip"
      >
        <span :class="item.icon"></span>
        <span class="nav-item-text">{{ item.text }}</span>
      </RouterLink>
    </nav>

    <!-- Footer cố định đáy sidebar -->
    <div class="sidebar-footer">
      <div class="footer-border"></div>
      <button class="btn-collapse" @click="emit('toggle')">
        <span :class="collapsed ? 'next-icon' : 'back-icon'"></span>
      </button>
    </div>
  </aside>
</template>

<script setup>
/**
 *   @fileoverview Sidebar Component (AppSidebar)
 *   @description Thanh điều hướng chính của ứng dụng (Navigation Sidebar)
 *   - Hiển thị logo, menu chức năng, và nút thu gọn (collapse).
 *   - Hỗ trợ chế độ thu gọn để tiết kiệm không gian hiển thị.
 *   - Các mục menu có thể scroll dọc khi danh sách dài.
 *   @createdBy: HMTuan (31/10/2025)
 */

/**
 * Props
 * @prop {Boolean} collapsed - Trạng thái thu gọn sidebar
 */
defineProps({
  collapsed: {
    type: Boolean,
    default: false
  }
})

/**
 * Emits
 * @event toggle - Bắn ra khi người dùng click nút thu gọn/mở rộng
 */
const emit = defineEmits(['toggle'])

/**
 * Danh sách các mục điều hướng (menu items)
 * @type {Array<{ icon: string, text: string, tooltip: string, active?: boolean, to?: string }>}
 */
const navItems = [
  { icon: 'news-icon', text: 'Tổng quan', tooltip: 'Tổng quan' },
  { icon: 'users-icon', text: 'Tài sản', tooltip: 'Tài sản', active: true, to: '/assets' },
  { icon: 'calendar-icon', text: 'Tài sản HT-ĐB', tooltip: 'Tài sản HT-ĐB' },
  { icon: 'depot-icon', text: 'Công cụ dụng cụ', tooltip: 'Công cụ dụng cụ' },
  { icon: 'letter-icon', text: 'Danh mục', tooltip: 'Danh mục' },
  { icon: 'message-icon', text: 'Tra cứu', tooltip: 'Tra cứu' },
  { icon: 'report-icon', text: 'Báo cáo', tooltip: 'Báo cáo' }
]
</script>

<style scoped>
/* LOGO */
.sidebar-logo {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 0 2px 24px;
  flex-shrink: 0;
}

.sidebar-logo .logo-icon {
  width: 36px;
  height: 36px;
  background: url('../../assets/icons/qlts-icon.svg') no-repeat -20px -680px;
  flex-shrink: 0;
}

.sidebar-logo .logo-text {
  font-size: 25px;
  font-weight: 500;
  color: #fff;
  letter-spacing: 0.5px;
}

.app-sidebar.collapsed .sidebar-logo {
  justify-content: center;
  padding: 0 0 24px;
}

.app-sidebar.collapsed .sidebar-logo .logo-text {
  display: none;
}

/* SIDEBAR CONTAINER */
.app-sidebar {
  width: 240px;
  height: 100vh;
  background-color: #1c3048;
  color: #ffffff;
  display: flex;
  flex-direction: column;
  padding: 12px;
  transition: width 0.3s ease;
  flex-shrink: 0;
  overflow: hidden;
  box-shadow: 0 3px 6px rgba(0, 0, 0, .16);
  position: relative;
}

/* Hiệu ứng nền gradient trên phần đầu sidebar */
.app-sidebar::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 30%;
  background: linear-gradient(180deg, rgba(255, 255, 255, 0.06) 0%, transparent 100%);
  pointer-events: none;
}

/* Sidebar thu gọn */
.app-sidebar.collapsed {
  width: 66px;
}

/* NAVIGATION MENU */
.sidebar-nav {
  display: flex;
  flex-direction: column;
  gap: 4px;
  flex: 1;
  overflow-y: auto;
  overflow-x: hidden;
  padding-bottom: 12px;
}

/* Scrollbar tuỳ chỉnh cho menu */
.sidebar-nav::-webkit-scrollbar {
  width: 4px;
}
.sidebar-nav::-webkit-scrollbar-thumb {
  background-color: rgba(255, 255, 255, 0.2);
  border-radius: 2px;
}
.sidebar-nav::-webkit-scrollbar-track {
  background-color: transparent;
}

/* NAV ITEM */
.nav-item {
  display: flex;
  align-items: center;
  gap: 12px;
  height: 44px;
  border-radius: 4px;
  text-decoration: none;
  color: rgb(159,130,162);
  transition: all 0.2s ease;
  white-space: nowrap;
  position: relative;
  flex-shrink: 0;
}

/* Trạng thái hover / active */
.nav-item:hover,
.nav-item.active {
  color: #ffffff;
  background-color: #1aa4c8;
}

/* Icon mặc định & khi hover */
.nav-item .icon,
.nav-item [class*='-icon'] {
  width: 24px;
  height: 24px;
  flex-shrink: 0;
  filter: invert(50%) sepia(8%) saturate(820%) hue-rotate(266deg) brightness(91%) contrast(88%);
  transition: color 0.2s ease;
}
.nav-item:hover .icon,
.nav-item:hover [class*='-icon'],
.nav-item.active .icon,
.nav-item.active [class*='-icon'] {
  filter: brightness(0) invert(1);
}
.nav-item-text {
  font-size: 14px;
  font-weight: 500;
}

/* Ẩn text khi thu gọn */
.app-sidebar.collapsed .nav-item-text {
  display: none;
}

/* Tooltip hiển thị khi sidebar thu gọn */
.app-sidebar.collapsed .nav-item::after {
  content: attr(data-tooltip);
  position: absolute;
  left: 100%;
  margin-left: 10px;
  top: 50%;
  transform: translateY(-50%);
  background-color: rgba(0, 0, 0, 0.9);
  color: #fff;
  padding: 6px 12px;
  border-radius: 4px;
  font-size: 12px;
  white-space: nowrap;
  opacity: 0;
  pointer-events: none;
  transition: opacity 0.2s;
  z-index: 1000;
}
.app-sidebar.collapsed .nav-item:hover::after {
  opacity: 1;
}

/* FOOTER SIDEBAR */
.footer-border {
  width: calc(100% + 24px);
  height: 1px;
  background-color: rgba(255, 255, 255, 0.2);
  margin: 0 -12px 12px -12px;
}

/* Nút thu gọn/mở rộng sidebar */
.btn-collapse {
  width: 30px;
  height: 30px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 12px 0 auto;
  padding: 0;
  background-color: rgb(28,48,72);
  border: 1px solid #ffffff;
  border-radius: 6px;
  color: #ffffff;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
}
.btn-collapse:hover {
  background-color: rgba(255, 255, 255, 0.1);
  border-color: rgba(255, 255, 255, 0.5);
}

.app-sidebar.collapsed .btn-collapse {
  margin: 0 auto;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 10px;
}
.app-sidebar.collapsed .btn-collapse .nav-item-text {
  display: none;
}

@media (max-width: 991.98px) {
  .app-sidebar {
    width: 66px;
  }

  .sidebar-logo .logo-text {
    display: none;
  }

  .nav-item-text {
    display: none;
  }

  .nav-item::after {
    content: attr(data-tooltip);
    position: absolute;
    left: 100%;
    margin-left: 8px;
    top: 50%;
    transform: translateY(-50%);
    background-color: rgba(0, 0, 0, 0.9);
    color: #fff;
    padding: 6px 10px;
    border-radius: 4px;
    font-size: 12px;
    white-space: nowrap;
    opacity: 0;
    pointer-events: none;
    transition: opacity 0.2s;
  }

  .nav-item:hover::after {
    opacity: 1;
  }
}

@media (min-width: 992px) {
  .app-sidebar {
    width: 240px;
  }

  .sidebar-logo .logo-text {
    display: block;
  }

  .nav-item-text {
    display: inline;
  }

  .nav-item::after {
    display: none;
  }
}
</style>
