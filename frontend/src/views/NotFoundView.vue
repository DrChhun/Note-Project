<script setup lang="ts">
import { useRouter } from 'vue-router'
import { Button } from '@/components/ui/button'

const router = useRouter()

function goHome() {
  router.push('/')
}
</script>

<template>
  <div class="flex flex-col items-center justify-center min-h-[60vh] gap-6 px-4">
    <p class="text-muted-foreground text-center text-lg">
      There is no page available
    </p>
    <Button
      class="py-4 sm:py-6 px-6 cursor-pointer bg-[#DEEBF7] hover:bg-[#C5D9F0] text-gray-900 border-0"
      @click="goHome"
    >
      Back to Notes
    </Button>
  </div>
</template>
