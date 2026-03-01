<script setup lang="ts">
import { ref, computed, watch, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { refDebounced } from '@vueuse/core'
import { ChevronDown } from 'lucide-vue-next'
import NoteCard from '@/components/NoteCard.vue'
import { Button } from '@/components/ui/button'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
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

type SortOption = 'createdAt-desc' | 'createdAt-asc' | 'title-asc' | 'title-desc'
const sortOption = ref<SortOption>('createdAt-desc')

const sortBy = computed(() => sortOption.value.split('-')[0] as 'title' | 'createdAt')
const sortOrder = computed(() => sortOption.value.split('-')[1] as 'asc' | 'desc')

const SORT_LABELS: Record<SortOption, string> = {
  'createdAt-desc': 'Newest first',
  'createdAt-asc': 'Oldest first',
  'title-asc': 'Title A–Z',
  'title-desc': 'Title Z–A',
}

function setSortOption(option: SortOption) {
  sortOption.value = option
}
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
  const params: { title?: string; type?: number; sortBy?: string; sortOrder?: string } = {}
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
  <div class="flex flex-col gap-4 sm:gap-6">
    <h1 class="text-2xl sm:text-3xl lg:text-4xl py-4 sm:py-6 font-extrabold mt-4 sm:mt-8">
      Note Taking App
    </h1>

    <div class="pb-4 sm:pb-6">
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
      </div>
    </div>

    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 pb-4 sm:pb-6">
      <div class="flex flex-wrap items-end gap-3 sm:gap-4 min-w-0">
        <nav
          class="flex gap-4 sm:gap-6 overflow-x-auto [scrollbar-width:none] [&::-webkit-scrollbar]:hidden min-h-[28px] items-end -mx-1 px-1"
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
        <DropdownMenu>
          <DropdownMenuTrigger
            as-child
          >
            <Button
              variant="outline"
              class="text-sm font-medium h-9 gap-1.5 px-3 min-w-0 max-w-[180px] border-border"
              aria-label="Sort by"
            >
              {{ SORT_LABELS[sortOption] }}
              <ChevronDown class="size-4 shrink-0 opacity-50" />
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="start" class="min-w-[160px]">
            <DropdownMenuItem
              v-for="opt in (['createdAt-desc', 'createdAt-asc', 'title-asc', 'title-desc'] as const)"
              :key="opt"
              @select="setSortOption(opt)"
            >
              {{ SORT_LABELS[opt] }}
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
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
