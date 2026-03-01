<script setup lang="ts">
import { ref, watch, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { refDebounced } from '@vueuse/core'
import NoteCard from '@/components/NoteCard.vue'
import { Button } from '@/components/ui/button'
import { getNotesPaginated } from '@/services/api/notes'
import type { NoteApi } from '@/types/note'

const router = useRouter()

type Filter = 'all' | 'work' | 'learn' | 'personal'

const FILTER_TO_TYPE: Record<Exclude<Filter, 'all'>, number> = {
  work: 1,
  learn: 2,
  personal: 0,
}

const PAGE_SIZE = 15

const searchQuery = ref('')
const debouncedSearchQuery = refDebounced(searchQuery, 400)
const activeFilter = ref<Filter>('all')
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
  const params: { title?: string; type?: number } = {}
  if (debouncedSearchQuery.value.trim()) params.title = debouncedSearchQuery.value.trim()
  if (activeFilter.value !== 'all') params.type = FILTER_TO_TYPE[activeFilter.value]
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

watch([debouncedSearchQuery, activeFilter], () => fetchNotes(true))

function setFilter(filter: Filter) {
  activeFilter.value = filter
}

const filterLabels: Record<Filter, string> = {
  all: 'All Notes',
  work: 'Work',
  learn: 'Learn',
  personal: 'Personal',
}

const activeFilterUnderline =
  "after:block after:content-[''] after:absolute after:bottom-0 after:left-0 after:w-full after:h-0.5 after:bg-primary"

function openNote(id: string | number) {
  router.push(`/notes/${id}`)
}

function goToCreate() {
  router.push('/notes/new')
}

function formatDate(iso?: string): string {
  if (!iso) return ''
  const d = new Date(iso)
  if (Number.isNaN(d.getTime())) return ''
  return d.toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric',
  })
}
</script>

<template>
  <div class="flex flex-col gap-6">
    <h1 class="text-4xl py-6 font-extrabold mt-8">
      Note Taking App
    </h1>

    <div class="pb-6">
      <div
        class="w-full flex items-center gap-2.5 bg-background border border-border rounded-[14px] px-3.5 py-3 text-muted-foreground cursor-text"
      >
        <span class="inline-flex items-center text-muted-foreground shrink-0" aria-hidden="true">
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <circle cx="11" cy="11" r="7" />
            <line x1="16.65" y1="16.65" x2="21" y2="21" />
          </svg>
        </span>
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Search by title..."
          aria-label="Search notes"
          class="flex-1 min-w-0 border-none bg-transparent text-[15px] text-foreground outline-none placeholder:text-muted-foreground"
        >
        <span class="text-xs font-bold text-muted-foreground border border-border rounded-lg px-1.5 py-0.5 shrink-0">/</span>
      </div>
    </div>

    <div class="flex items-center justify-between gap-4 pb-6">
      <nav
        class="flex gap-6 overflow-x-auto [scrollbar-width:none] [&::-webkit-scrollbar]:hidden min-h-[28px] items-end"
      >
        <button
          v-for="filter in (['all', 'work', 'learn', 'personal'] as const)"
          :key="filter"
          type="button"
          class="text-sm font-semibold py-1 pb-1.5 whitespace-nowrap cursor-pointer bg-transparent border-none relative hover:text-foreground transition-colors min-w-0"
          :class="[
            activeFilter === filter ? 'text-foreground' : 'text-muted-foreground',
            activeFilter === filter && activeFilterUnderline
          ]"
          @click="setFilter(filter)"
        >
          {{ filterLabels[filter] }}
        </button>
      </nav>
      <Button
        class="shrink-0 py-6 w-[116px] cursor-pointer bg-[#DEEBF7] hover:bg-[#C5D9F0] text-gray-900 border-0"
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
          class="grid gap-4 sm:grid-cols-2 lg:grid-cols-3 transition-opacity duration-150"
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

      <!-- Infinite scroll sentinel + loading indicator -->
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
