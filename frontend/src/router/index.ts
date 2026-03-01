import { createRouter, createWebHistory } from 'vue-router'
import CardsListView from '../views/CardListsView.vue'
import NoteDetailView from '../views/NoteDetailView.vue'
import NoteCreateView from '../views/NoteCreateView.vue'
import NotFoundView from '../views/NotFoundView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: CardsListView,
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
    {
      path: '/:pathMatch(.*)*',
      name: 'not-found',
      component: NotFoundView,
    },
  ],
})

export default router
