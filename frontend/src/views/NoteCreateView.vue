<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { Button } from '@/components/ui/button'
import { createNote } from '@/services/api/notes'

const router = useRouter()
const title = ref('')
const content = ref('')
const type = ref(0)
const isCreating = ref(false)
const error = ref<string | null>(null)

const TYPE_LABELS: Record<number, string> = {
  0: 'Personal',
  1: 'Work',
  2: 'Learn',
}

const TYPE_PILL_CLASS: Record<number, string> = {
  0: 'bg-[#ffc1de] text-foreground',
  1: 'bg-[#4a1733] text-white',
  2: 'bg-[#ffd8ea] text-foreground',
}

function cycleType() {
  type.value = (type.value + 1) % 3
}

const todayShort = computed(() =>
  new Date().toLocaleDateString('en-US', { month: 'short', day: '2-digit' }),
)

async function create() {
  isCreating.value = true
  error.value = null
  try {
    await createNote({
      title: title.value || 'Untitled',
      content: content.value,
      type: type.value,
    })
    router.push('/')
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'Failed to create note'
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
    <div class="sticky top-0 z-10 flex justify-between items-center py-6 pb-4 bg-background overflow-hidden [scrollbar-width:none] [&::-webkit-scrollbar]:hidden">
      <button
        type="button"
        class="text-sm font-semibold text-muted-foreground hover:text-foreground transition-colors"
        @click="goBack"
      >
        ← Notes
      </button>
      <Button :disabled="isCreating" @click="create">
        {{ isCreating ? 'Creating...' : 'Save' }}
      </Button>
    </div>

    <div v-if="error" class="text-destructive text-sm mb-4">
      {{ error }}
    </div>

    <div class="flex flex-col flex-1">
      <textarea
        v-model="title"
        placeholder="Title"
        rows="1"
        class="text-[26px] font-extrabold tracking-tight border-none bg-transparent w-full text-foreground mb-4 resize-none outline-none placeholder:text-muted-foreground leading-tight !overflow-hidden !mb-4"
      />

      <div class="flex gap-4 items-center !mb-8 pb-4 border-b border-border overflow-hidden">
        <button
          type="button"
          :class="[
            'text-[11px] font-bold uppercase tracking-wider px-2.5 py-1 cursor-pointer border-none rounded transition-colors shrink-0 overflow-hidden whitespace-nowrap',
            TYPE_PILL_CLASS[type] ?? 'bg-muted text-muted-foreground',
          ]"
          @click="cycleType"
        >
          {{ TYPE_LABELS[type] ?? 'No Type' }}
        </button>
        <span class="text-[11px] font-semibold uppercase tracking-wider text-muted-foreground ml-auto">
          {{ todayShort }}
        </span>
      </div>

      <textarea
        v-model="content"
        placeholder="Start writing…"
        class="flex-1 min-h-[300px] text-[15px] leading-[1.7] border-none bg-transparent w-full text-foreground resize-none outline-none placeholder:text-muted-foreground"
      />
    </div>
  </div>
</template>
