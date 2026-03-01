<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{
  title: string
  description: string
  date: string
  type?: number
}>()

const NOTE_TYPE_COLORS: Record<number, string> = {
  0: '#DEEBF7',
  1: '#E2EFDA',
  2: '#FFF2CC',
}

const backgroundColor = computed(() =>
  props.type !== undefined && NOTE_TYPE_COLORS[props.type] !== undefined
    ? NOTE_TYPE_COLORS[props.type]
    : '#E5E5E5',
)
</script>

<template>
  <div
    class="h-[220px] rounded-2xl p-5 text-left shadow-sm flex flex-col hover:cursor-pointer overflow-hidden"
    :style="{ backgroundColor }"
  >
    <h2 class="text-xl font-semibold text-gray-900 leading-snug shrink-0 overflow-hidden text-ellipsis whitespace-nowrap">
      {{ title }}
    </h2>
    <div class="flex-1 min-h-0 mt-1 flex flex-col">
      <p class="text-sm text-gray-700 leading-snug line-clamp-4 break-words">
        {{ description }}
      </p>
    </div>
    <p class="text-sm text-gray-600 mt-4 shrink-0">
      {{ date }}
    </p>
  </div>
</template>
