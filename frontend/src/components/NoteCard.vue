<script setup lang="ts">
defineProps<{
  title: string
  description: string
  date: string
}>()
</script>

<template>
  <div
    class="rounded-2xl p-5 text-left shadow-sm min-h-[180px] flex flex-col hover:cursor-pointer"
    style="background-color: #FDF2CE"
  >
    <div class="flex-1 space-y-1">
      <h2 class="text-2xl font-semibold text-gray-900 leading-snug">
        {{ title }}
      </h2>
      <p class="text-base text-gray-800 leading-snug">
        {{ description }}
      </p>
    </div>
    <p class="text-sm text-gray-600 mt-4">
      {{ date }}
    </p>
  </div>
</template>
