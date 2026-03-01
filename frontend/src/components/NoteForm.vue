<script setup lang="ts">
import { computed } from 'vue'
import { TYPE_LABELS, TYPE_PILL_CLASS, TITLE_MAX, CONTENT_MAX } from '@/constants/note'
import { formatDateShort } from '@/lib/date'

const props = defineProps<{
  title: string
  content: string
  type: number
  date?: string
}>()

const emit = defineEmits<{
  'update:title': [value: string]
  'update:content': [value: string]
  'update:type': [value: number]
}>()

const typeLabel = computed(() => TYPE_LABELS[props.type] ?? 'No Type')
const typePillClass = computed(() => TYPE_PILL_CLASS[props.type] ?? 'bg-[#E5E5E5] text-gray-900')
const contentCount = computed(() => props.content.length)
const dateLabel = computed(() => props.date ? formatDateShort(props.date) : new Date().toLocaleDateString('en-US', { month: 'short', day: '2-digit' }))

function cycleType() {
  const next = (props.type + 1) % 3
  emit('update:type', next)
}
</script>

<template>
  <div class="flex flex-col flex-1">
    <textarea
      :value="title"
      placeholder="Title"
      rows="1"
      :maxlength="TITLE_MAX"
      class="text-[26px] font-extrabold tracking-tight border-none bg-transparent w-full text-foreground mb-4 resize-none outline-none placeholder:text-muted-foreground leading-tight !overflow-hidden !mb-4"
      @input="emit('update:title', ($event.target as HTMLTextAreaElement).value)"
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
        {{ dateLabel }}
      </span>
    </div>

    <div class="relative flex-1 flex flex-col">
      <textarea
        :value="content"
        placeholder="Start writing…"
        :maxlength="CONTENT_MAX"
        class="flex-1 min-h-[300px] text-[15px] leading-[1.7] border-none bg-transparent w-full text-foreground resize-none outline-none placeholder:text-muted-foreground"
        @input="emit('update:content', ($event.target as HTMLTextAreaElement).value)"
      />
      <span style="bottom: -32px !important;" class="absolute right-0 text-xs text-muted-foreground">
        {{ contentCount }}/{{ CONTENT_MAX }}
      </span>
    </div>
  </div>
</template>
