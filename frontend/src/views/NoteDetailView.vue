<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { Button } from '@/components/ui/button'
import { getNoteById, updateNote, deleteNote } from '@/services/api/notes'
import type { NoteApi } from '@/types/note'

const route = useRoute()
const router = useRouter()
const noteId = computed(() => route.params.id as string)

const note = ref<NoteApi | null>(null)
const title = ref('')
const content = ref('')
const type = ref<number | null>(null)
const isLoading = ref(false)
const isSaving = ref(false)
const isDeleting = ref(false)
const error = ref<string | null>(null)

const TYPE_LABELS: Record<number, string> = {
  0: 'Personal',
  1: 'Work',
  2: 'Learn',
}

const TYPE_PILL_CLASS: Record<number, string> = {
  0: 'bg-[#DEEBF7] text-gray-900',
  1: 'bg-[#E2EFDA] text-gray-900',
  2: 'bg-[#FFF2CC] text-gray-900',
}

const typeLabel = computed(() => {
  if (type.value === null || type.value === undefined || !(type.value in TYPE_LABELS)) return 'No Type'
  return TYPE_LABELS[type.value]
})

const typePillClass = computed(() => {
  if (type.value === null || type.value === undefined || !(type.value in TYPE_PILL_CLASS)) return 'bg-[#E5E5E5] text-gray-900'
  return TYPE_PILL_CLASS[type.value]
})

function cycleType() {
  if (type.value === null || type.value === undefined || !(type.value in TYPE_LABELS)) {
    type.value = 0
    return
  }
  type.value = (type.value + 1) % 3
}

const TITLE_MAX = 50
const CONTENT_MAX = 500

const contentCount = computed(() => content.value.length)

const displayDate = computed(() => {
  const iso = note.value?.createdAt
  if (!iso) return ''
  const d = new Date(iso)
  if (Number.isNaN(d.getTime())) return ''
  return d.toLocaleDateString('en-US', { month: 'short', day: '2-digit' })
})

onMounted(async () => {
  isLoading.value = true
  error.value = null
  try {
    note.value = await getNoteById(noteId.value)
    title.value = note.value.title
    content.value = note.value.content
    const t = note.value.type
    type.value = t === 0 || t === 1 || t === 2 ? t : null
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'Failed to load note'
  } finally {
    isLoading.value = false
  }
})

async function save() {
  isSaving.value = true
  error.value = null
  try {
    await updateNote(noteId.value, {
      title: title.value,
      content: content.value,
      type: type.value ?? 0,
    })
    router.push('/')
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'Failed to save note'
  } finally {
    isSaving.value = false
  }
}

async function handleDelete() {
  isDeleting.value = true
  error.value = null
  try {
    await deleteNote(noteId.value)
    router.push('/')
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'Failed to delete note'
  } finally {
    isDeleting.value = false
  }
}

function goBack() {
  router.push('/')
}
</script>

<template>
  <div class="flex flex-col min-h-[calc(100vh-8rem)]">
    <div class="sticky top-0 z-10 flex justify-between items-center py-6 pb-4 bg-background overflow-hidden [scrollbar-width:none] [&::-webkit-scrollbar]:hidden">
      <button
        type="button"
        class="text-sm font-semibold text-muted-foreground hover:text-foreground transition-colors"
        @click="goBack"
      >
        ← Notes
      </button>
      <div class="flex gap-2">
        <Button
          class="py-6 w-[116px] cursor-pointer bg-[#F8D7DA] hover:bg-[#F5C6CB] text-gray-800 border-0"
          :disabled="isDeleting || isSaving"
          @click="handleDelete"
        >
          {{ isDeleting ? 'Deleting...' : 'Delete' }}
        </Button>
        <Button
          class="py-6 w-[116px] cursor-pointer bg-[#DEEBF7] hover:bg-[#C5D9F0] text-gray-900 border-0"
          :disabled="isSaving || isDeleting"
          @click="save"
        >
          {{ isSaving ? 'Saving...' : 'Save' }}
        </Button>
      </div>
    </div>

    <div v-if="error" class="text-destructive text-sm mb-4">
      {{ error }}
    </div>

    <div v-else-if="isLoading" class="text-muted-foreground text-sm">
      Loading note...
    </div>

    <div v-else class="flex flex-col flex-1">
      <textarea
        v-model="title"
        placeholder="Title"
        rows="1"
        :maxlength="TITLE_MAX"
        class="text-[26px] font-extrabold tracking-tight border-none bg-transparent w-full text-foreground mb-4 resize-none outline-none placeholder:text-muted-foreground leading-tight !overflow-hidden !mb-4"
      />

      <div class="flex gap-4 items-center !mb-8 pb-4 border-b border-border overflow-hidden">
        <button
          type="button"
          :class="[
            'text-[11px] font-bold uppercase tracking-wider px-2.5 py-1 cursor-pointer border-none rounded transition-colors shrink-0 overflow-hidden whitespace-nowrap',
            typePillClass,
          ]"
          @click="cycleType"
        >
          {{ typeLabel }}
        </button>
        <span class="text-[11px] font-semibold uppercase tracking-wider text-muted-foreground ml-auto">
          {{ displayDate }}
        </span>
      </div>

      <div class="relative flex-1 flex flex-col">
        <textarea
          v-model="content"
          placeholder="Start writing…"
          :maxlength="CONTENT_MAX"
          class="flex-1 min-h-[300px] text-[15px] leading-[1.7] border-none bg-transparent w-full text-foreground resize-none outline-none placeholder:text-muted-foreground"
        />
        <span class="absolute bottom-2 right-0 text-xs text-muted-foreground">
          {{ contentCount }}/{{ CONTENT_MAX }}
        </span>
      </div>
    </div>
  </div>
</template>
