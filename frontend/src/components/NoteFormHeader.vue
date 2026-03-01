<script setup lang="ts">
import { useRouter } from 'vue-router'

const router = useRouter()

function goBack() {
  router.push('/')
}
</script>

<template>
  <div class="sticky top-0 z-10 flex flex-col sm:flex-row sm:justify-between sm:items-center gap-4 py-4 sm:py-6 pb-4 bg-background overflow-hidden [scrollbar-width:none] [&::-webkit-scrollbar]:hidden">
    <button
      type="button"
      class="text-sm font-semibold text-muted-foreground hover:text-foreground transition-colors self-start shrink-0"
      @click="goBack"
    >
      ← Notes
    </button>
    <div class="flex gap-2 flex-1 sm:flex-initial w-full sm:w-auto min-w-0">
      <slot name="actions" />
    </div>
  </div>
</template>
