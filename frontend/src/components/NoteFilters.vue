<script setup lang="ts">
import { ChevronDown } from 'lucide-vue-next'
import { Button } from '@/components/ui/button'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
import type { Filter, SortOption } from '@/types/note-filters'

defineProps<{
  activeFilter: Filter
  sortOption: SortOption
}>()

const emit = defineEmits<{
  'update:activeFilter': [value: Filter]
  'update:sortOption': [value: SortOption]
}>()

const FILTER_LABELS: Record<Filter, string> = {
  all: 'All Notes',
  work: 'Work',
  learn: 'Learn',
  personal: 'Personal',
}

const SORT_LABELS: Record<SortOption, string> = {
  'createdAt-desc': 'Newest first',
  'createdAt-asc': 'Oldest first',
  'title-asc': 'Title A–Z',
  'title-desc': 'Title Z–A',
}

const activeFilterUnderline =
  "after:block after:content-[''] after:absolute after:bottom-0 after:left-0 after:w-full after:h-0.5 after:bg-primary"

function setFilter(filter: Filter) {
  emit('update:activeFilter', filter)
}

function setSortOption(option: SortOption) {
  emit('update:sortOption', option)
}
</script>

<template>
  <div class="flex flex-wrap items-end gap-3 sm:gap-4 min-w-0">
    <nav
      class="flex gap-4 sm:gap-6 overflow-x-auto [scrollbar-width:none] [&::-webkit-scrollbar]:hidden min-h-[28px] items-end -mx-1 px-1"
    >
      <button
        v-for="filter in (['all', 'work', 'learn', 'personal'] as const)"
        :key="filter"
        type="button"
        class="text-sm font-semibold py-1 pb-1.5 whitespace-nowrap cursor-pointer bg-transparent border-none relative hover:text-foreground transition-colors min-w-0"
        :class="[
          activeFilter === filter ? 'text-foreground' : 'text-muted-foreground',
          activeFilter === filter && activeFilterUnderline
        ]"
        @click="setFilter(filter)"
      >
        {{ FILTER_LABELS[filter] }}
      </button>
    </nav>
    <DropdownMenu>
      <DropdownMenuTrigger as-child>
        <Button
          variant="outline"
          class="text-sm font-medium h-9 gap-1.5 px-3 min-w-0 max-w-[180px] border-border"
          aria-label="Sort by"
        >
          {{ SORT_LABELS[sortOption] }}
          <ChevronDown class="size-4 shrink-0 opacity-50" />
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="start" class="min-w-[160px]">
        <DropdownMenuItem
          v-for="opt in (['createdAt-desc', 'createdAt-asc', 'title-asc', 'title-desc'] as const)"
          :key="opt"
          @select="setSortOption(opt)"
        >
          {{ SORT_LABELS[opt] }}
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  </div>
</template>
