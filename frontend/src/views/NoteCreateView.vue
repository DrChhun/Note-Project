<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { toast } from '@/components/ui/sonner'
import { Button } from '@/components/ui/button'
import { createNote } from '@/services/api/notes'

const router = useRouter()
const title = ref('')
const content = ref('')
const type = ref(0)
const isCreating = ref(false)

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

function cycleType() {
  type.value = (type.value + 1) % 3
}

const TITLE_MAX = 50
const CONTENT_MAX = 500

const contentCount = computed(() => content.value.length)

const todayShort = computed(() =>
  new Date().toLocaleDateString('en-US', { month: 'short', day: '2-digit' }),
)

async function create() {
  if (!title.value.trim()) {
    toast.error('Title is required')
    return
  }
  isCreating.value = true
  try {
    const note = await createNote({
      title: title.value.trim(),
      content: content.value.trim(),
      type: type.value,
    })
    if (note?.id != null) {
      toast.success('Note created successfully')
      router.push('/')
    } else {
      toast.error('Create request did not return a valid note')
    }
  } catch (e) {
    toast.error(e instanceof Error ? e.message : 'Failed to create note')
  } finally {
    isCreating.value = false
  }
}

function goBack() {
  router.push('/')
}
</script>

<template>
  <div class="flex flex-col min-h-[calc(100vh-8rem)]">
    <div class="sticky top-0 z-10 flex flex-col sm:flex-row sm:justify-between sm:items-center gap-4 py-4 sm:py-6 pb-4 bg-background overflow-hidden [scrollbar-width:none] [&::-webkit-scrollbar]:hidden">
      <button
        type="button"
        class="text-sm font-semibold text-muted-foreground hover:text-foreground transition-colors self-start shrink-0"
        @click="goBack"
      >
        ← Notes
      </button>
      <Button
        class="w-full sm:w-[116px] py-4 sm:py-6 cursor-pointer bg-[#DEEBF7] hover:bg-[#C5D9F0] text-gray-900 border-0"
        :disabled="isCreating"
        @click="create"
      >
        {{ isCreating ? 'Creating...' : 'Save' }}
      </Button>
    </div>

    <div class="flex flex-col flex-1">
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
            TYPE_PILL_CLASS[type] ?? 'bg-[#E5E5E5] text-gray-900',
          ]"
          @click="cycleType"
        >
          {{ TYPE_LABELS[type] ?? 'No Type' }}
        </button>
        <span class="text-[11px] font-semibold uppercase tracking-wider text-muted-foreground ml-auto">
          {{ todayShort }}
        </span>
      </div>

      <div class="relative flex-1 flex flex-col">
        <textarea
          v-model="content"
          placeholder="Start writing…"
          :maxlength="CONTENT_MAX"
          class="flex-1 min-h-[300px] text-[15px] leading-[1.7] border-none bg-transparent w-full text-foreground resize-none outline-none placeholder:text-muted-foreground"
        />
        <span style="bottom: -32px !important;" class="absolute right-0 text-xs text-muted-foreground">
          {{ contentCount }}/{{ CONTENT_MAX }}
        </span>
      </div>
    </div>
  </div>
</template>
