import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import NoteDetailView from '../views/NoteDetailView.vue'
import NoteCreateView from '../views/NoteCreateView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView,
    },
    {
      path: '/notes/new',
      name: 'note-create',
      component: NoteCreateView,
    },
    {
      path: '/notes/:id',
      name: 'note-detail',
      component: NoteDetailView,
    },
  ],
})

export default router
