<script setup lang="ts">
import type { HTMLAttributes } from 'vue'
import { badgeVariants } from './index'
import { cn } from '@/lib/utils'

const props = withDefaults(
  defineProps<{
    variant?: 'default' | 'secondary' | 'destructive' | 'outline'
    class?: HTMLAttributes['class']
  }>(),
  { variant: 'default' },
)
</script>

<template>
  <span
    :class="cn(badgeVariants({ variant }), props.class)"
  >
    <slot />
  </span>
</template>
