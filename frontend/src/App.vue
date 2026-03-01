<script setup lang="ts">
import { RouterView } from 'vue-router'
import { Toaster } from '@/components/ui/sonner'
</script>

<template>
  <div class="app-container">
    <RouterView />
    <Toaster position="top-right" />
  </div>
</template>
