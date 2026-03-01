<script setup lang="ts">
import { ref, computed, watch, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { refDebounced } from '@vueuse/core'
import NoteCard from '@/components/NoteCard.vue'
import NoteSearchBar from '@/components/NoteSearchBar.vue'
import NoteFilters from '@/components/NoteFilters.vue'
import { Button } from '@/components/ui/button'
import { getNotesPaginated } from '@/services/api/notes'
import { formatDate } from '@/lib/date'
import type { NoteApi } from '@/types/note'
import type { Filter, SortOption } from '@/types/note-filters'

const router = useRouter()

const FILTER_TO_TYPE: Record<Exclude<Filter, 'all'>, number> = {
  work: 1,
  learn: 2,
  personal: 0,
}

const PAGE_SIZE = 15

const searchQuery = ref('')
const debouncedSearchQuery = refDebounced(searchQuery, 400)
const activeFilter = ref<Filter>('all')
const sortOption = ref<SortOption>('createdAt-desc')

const sortBy = computed(() => sortOption.value.split('-')[0] as 'title' | 'createdAt')
const sortOrder = computed(() => sortOption.value.split('-')[1] as 'asc' | 'desc')

const notes = ref<NoteApi[]>([])
const currentPage = ref(1)
const totalPages = ref(0)
const hasMore = ref(true)
const isLoading = ref(false)
const isLoadingMore = ref(false)
const error = ref<string | null>(null)
const loadMoreSentinel = ref<HTMLElement | null>(null)

let scrollObserver: IntersectionObserver | null = null

function buildParams() {
  const params: { title?: string; type?: number; sortBy?: 'title' | 'createdAt'; sortOrder?: 'asc' | 'desc' } = {}
  if (debouncedSearchQuery.value.trim()) params.title = debouncedSearchQuery.value.trim()
  if (activeFilter.value !== 'all') params.type = FILTER_TO_TYPE[activeFilter.value]
  params.sortBy = sortBy.value
  params.sortOrder = sortOrder.value
  return params
}

async function fetchNotes(reset = true) {
  if (reset) {
    isLoading.value = true
    currentPage.value = 1
    hasMore.value = true
  } else {
    isLoadingMore.value = true
  }
  error.value = null
  try {
    const res = await getNotesPaginated({
      page: currentPage.value,
      pageSize: PAGE_SIZE,
      ...buildParams(),
    })
    if (reset) {
      notes.value = res.payload
    } else {
      notes.value = [...notes.value, ...res.payload]
    }
    totalPages.value = res.totalPages
    hasMore.value = currentPage.value < res.totalPages
    currentPage.value += 1
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'Failed to load notes'
    if (reset) notes.value = []
  } finally {
    isLoading.value = false
    isLoadingMore.value = false
  }
}

async function loadMore() {
  if (isLoadingMore.value || isLoading.value || !hasMore.value) return
  await fetchNotes(false)
}

scrollObserver = new IntersectionObserver(
  (entries) => {
    const [entry] = entries
    if (entry?.isIntersecting && hasMore.value && !isLoading.value && !isLoadingMore.value) {
      loadMore()
    }
  },
  { rootMargin: '100px', threshold: 0 },
)

watch(loadMoreSentinel, (el, _, onCleanup) => {
  if (!scrollObserver) return
  let observed: Element | null = null
  if (el) {
    scrollObserver.observe(el)
    observed = el
  }
  onCleanup(() => {
    if (observed) scrollObserver?.unobserve(observed)
  })
}, { immediate: true })

onMounted(() => {
  fetchNotes(true)
})

onUnmounted(() => {
  scrollObserver?.disconnect()
})

watch([debouncedSearchQuery, activeFilter, sortOption], () => fetchNotes(true))

function openNote(id: string | number) {
  router.push(`/notes/${id}`)
}

function goToCreate() {
  router.push('/notes/new')
}
</script>

<template>
  <div class="flex flex-col gap-4 sm:gap-6">
    <h1 class="text-2xl sm:text-3xl lg:text-4xl py-4 sm:py-6 font-extrabold mt-4 sm:mt-8">
      Note Taking App
    </h1>

    <div class="pb-4 sm:pb-6">
      <NoteSearchBar v-model="searchQuery" />
    </div>

    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 pb-4 sm:pb-6">
      <NoteFilters
        v-model:active-filter="activeFilter"
        v-model:sort-option="sortOption"
      />
      <Button
        class="shrink-0 py-4 sm:py-6 w-full sm:w-[116px] cursor-pointer bg-[#DEEBF7] hover:bg-[#C5D9F0] text-gray-900 border-0"
        @click="goToCreate"
      >
        Create note
      </Button>
    </div>

    <div v-if="error" class="text-destructive text-sm min-h-[240px]">
      {{ error }}
    </div>

    <div v-else-if="isLoading && notes.length === 0" class="text-muted-foreground text-sm min-h-[240px] flex items-start">
      Loading notes...
    </div>

    <div v-else-if="notes.length === 0" class="text-muted-foreground text-center py-12 min-h-[240px] flex items-center justify-center">
      There are no data
    </div>

    <template v-else>
      <div class="relative">
        <div
          class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3 sm:gap-4 transition-opacity duration-150"
          :class="{ 'opacity-60 pointer-events-none': isLoading }"
        >
          <div
            v-for="note in notes"
            :key="note.id"
            class="cursor-pointer"
            @click="openNote(note.id)"
          >
            <NoteCard
              :title="note.title"
              :description="note.content"
              :date="formatDate(note.createdAt)"
              :type="note.type"
            />
          </div>
        </div>
        <div
          v-if="isLoading"
          class="absolute inset-0 flex items-center justify-center pointer-events-none"
        >
          <span class="text-sm text-muted-foreground">Loading...</span>
        </div>
      </div>

      <div
        v-if="notes.length > 0"
        ref="loadMoreSentinel"
        class="flex justify-center items-center py-8 min-h-[80px]"
      >
        <p v-if="isLoadingMore" class="text-sm text-muted-foreground">
          Loading more notes...
        </p>
        <p v-else-if="!hasMore" class="text-sm text-muted-foreground">
          You've seen all notes
        </p>
        <span v-else class="inline-block w-px h-px" aria-hidden="true" />
      </div>
    </template>
  </div>
</template>
