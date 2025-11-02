import { createRouter, createWebHistory } from 'vue-router'
import IndexView from '@/views/assets/asset-index/IndexView.vue'
import NotFound from '@/views/not-found/NotFound.vue'

const routes = [
  {
    path: '/',
    redirect: '/assets'
  },
  {
    path: '/assets',
    name: 'Assets',
    component: IndexView
  },
  {
    path: '/:pathMatch(.*)*',
    name: 'NotFound',
    component: NotFound
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
