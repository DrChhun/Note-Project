<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import NoteCard from '@/components/NoteCard.vue'
import { Button } from '@/components/ui/button'
import { getNotes } from '@/services/api/notes'
import type { NoteApi } from '@/types/note'

const router = useRouter()

type Filter = 'all' | 'work' | 'learn' | 'personal'

const FILTER_TO_TYPE: Record<Exclude<Filter, 'all'>, number> = {
  work: 1,
  learn: 2,
  personal: 0,
}

const searchQuery = ref('')
const activeFilter = ref<Filter>('all')
const notes = ref<NoteApi[]>([])
const isLoading = ref(false)
const error = ref<string | null>(null)

async function fetchNotes() {
  isLoading.value = true
  error.value = null
  try {
    const params: { title?: string; type?: number } = {}
    if (searchQuery.value.trim()) params.title = searchQuery.value.trim()
    if (activeFilter.value !== 'all') params.type = FILTER_TO_TYPE[activeFilter.value]
    notes.value = await getNotes(params)
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'Failed to load notes'
    notes.value = []
  } finally {
    isLoading.value = false
  }
}

onMounted(() => {
  fetchNotes()
})

watch([searchQuery, activeFilter], fetchNotes)

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
  "after:block after:content-[''] after:absolute after:-bottom-1 after:left-0 after:w-full after:h-0.5 after:bg-primary"

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
    <h1 class="text-4xl font-extrabold mt-8">
      Note Taking App
    </h1>

    <div class="pb-6">
      <div
        class="w-full flex items-center gap-2.5 bg-background border border-border rounded-[14px] px-3.5 py-3 text-muted-foreground cursor-text shadow-sm focus-within:outline focus-within:outline-2 focus-within:outline-ring focus-within:outline-offset-2"
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
          placeholder="Search notes, tags, dates..."
          aria-label="Search notes"
          class="flex-1 min-w-0 border-none bg-transparent text-[15px] text-foreground outline-none placeholder:text-muted-foreground"
        >
        <span class="text-xs font-bold text-muted-foreground border border-border rounded-lg px-1.5 py-0.5 shrink-0">/</span>
      </div>
    </div>

    <div class="flex items-center justify-between gap-4 pb-6">
      <nav
        class="flex gap-6 overflow-x-auto [scrollbar-width:none] [&::-webkit-scrollbar]:hidden"
      >
        <button
          v-for="filter in (['all', 'work', 'learn', 'personal'] as const)"
          :key="filter"
          type="button"
          class="text-sm font-semibold pb-0.5 whitespace-nowrap cursor-pointer bg-transparent border-none relative hover:text-foreground transition-colors"
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

    <div v-if="error" class="text-destructive text-sm">
      {{ error }}
    </div>

    <div v-else-if="isLoading" class="text-muted-foreground text-sm">
      Loading notes...
    </div>

    <div v-else-if="notes.length === 0" class="text-muted-foreground text-center py-12">
      There are no data
    </div>

    <div v-else class="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
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
  </div>
</template>
