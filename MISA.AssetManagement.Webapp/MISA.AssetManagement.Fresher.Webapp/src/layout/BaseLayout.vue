<script setup>
import { ref } from 'vue'
import MsHeader from './ms-header/MsHeader.vue'
import MsSidebar from './ms-sidebar/MsSidebar.vue'

const isSidebarCollapsed = ref(false)

function toggleSidebar() {
  isSidebarCollapsed.value = !isSidebarCollapsed.value
}
</script>

<template>
  <div class="app">
    <!-- SIDEBAR: Chiếm toàn bộ chiều cao bên trái -->
    <MsSidebar
      :collapsed="isSidebarCollapsed"
      @toggle="toggleSidebar"
    />

    <!-- MAIN: Header + Content bên phải -->
    <main class="main">
      <!-- HEADER: Nằm trong phần main, không tràn sang sidebar -->
      <MsHeader />

      <!-- CONTENT: Phần nội dung chính -->
      <div class="content">
        <slot />
      </div>
    </main>
  </div>
</template>

<style scoped>
/* Container chính - Flexbox layout ngang */
.app {
  display: flex;
  height: 100vh;
  overflow: hidden;
  background-color: #f7f9ff;
}

/* Main area: chiếm phần còn lại bên phải */
.main {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  min-height: 0; /* Important cho flexbox */
}

/* Content area: KHÔNG scroll ở đây nữa */
.content {
  flex: 1;
  padding: 20px;
  background-color: #f7f9ff;
  overflow: hidden; /* Changed from overflow-y: auto */
  display: flex;
  flex-direction: column;
  min-height: 0; /* Important cho flexbox */
}
</style>

