<script setup lang="ts">
defineProps<{
  modelValue: string
}>()

const emit = defineEmits<{
  'update:modelValue': [value: string]
}>()
</script>

<template>
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
      :value="modelValue"
      type="text"
      placeholder="Search by title..."
      aria-label="Search notes"
      class="flex-1 min-w-0 border-none bg-transparent text-[15px] text-foreground outline-none placeholder:text-muted-foreground"
      @input="emit('update:modelValue', ($event.target as HTMLInputElement).value)"
    >
  </div>
</template>
