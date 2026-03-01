<script setup lang="ts">
import { Input } from '@/components/ui/input'
import NoteCard from '@/components/NoteCard.vue'

const notes = [
  { title: 'Todo', description: 'Buy milk.', date: 'Mar 1, 2026' },
  { title: 'Standup', description: 'Shipped auth. Blocked on API rate limits. Need design for settings page by Friday.', date: 'Feb 28, 2026' },
  { title: 'The beginning of screenless design', description: 'Ul jobs to be taken over by Solution Architect. Voice and ambient UI will change how we build products in the next decade.', date: 'May 21, 2020' },
  { title: 'Recipe', description: 'Pasta: boil 10 min, sauté garlic, add cream and parmesan. Salt to taste.', date: 'Feb 25, 2026' },
  { title: 'Book idea', description: "A developer moves to a small town and fixes the local library's website. Slow burn, lots of coffee.", date: 'Feb 24, 2026' },
  { title: 'Call Mom', description: 'Sunday 3pm.', date: 'Feb 20, 2026' },
  { title: 'Sprint retro', description: 'What went well: deployment pipeline. What didn\'t: scope creep on the dashboard. Action: stricter DoD and a dedicated QA pass before release.', date: 'Feb 18, 2026' },
  { title: 'Passwords', description: 'Netflix: updated. Bank: need to reset. Work SSO: ok.', date: 'Feb 15, 2026' },
  { title: 'Trip', description: 'Flight 9am. Hotel check-in 3pm. Dinner at 7. Don\'t forget charger and adapter.', date: 'Feb 10, 2026' },
  { title: 'Quote', description: 'The best code is the code you don\'t have to write.', date: 'Feb 5, 2026' },
]
</script>

<template>
  <div class="flex flex-col gap-6">
    <Input placeholder="Add a note..." class="w-full" />
    <h1 class="text-4xl !font-extrabold">
      Note Taking App
    </h1>
    <div class="grid gap-8 sm:grid-cols-2 lg:grid-cols-4">
      <NoteCard
        v-for="(note, index) in notes"
        :key="index"
        :title="note.title"
        :description="note.description"
        :date="note.date"
      />
    </div>
  </div>
</template>
