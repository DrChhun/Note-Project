<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { toast } from '@/components/ui/sonner'
import { Button } from '@/components/ui/button'
import NoteFormHeader from '@/components/NoteFormHeader.vue'
import NoteForm from '@/components/NoteForm.vue'
import { createNote } from '@/services/api/notes'

const router = useRouter()
const title = ref('')
const content = ref('')
const type = ref(0)
const isCreating = ref(false)

async function create() {
  if (!title.value.trim()) {
    toast.error('Title is required')
    return
  }
  isCreating.value = true
  try {
    const note = await createNote({
      title: title.value.trim(),
      content: content.value.trim(),
      type: type.value,
    })
    if (note?.id != null) {
      toast.success('Note created successfully')
      router.push('/')
    } else {
      toast.error('Create request did not return a valid note')
    }
  } catch (e) {
    toast.error(e instanceof Error ? e.message : 'Failed to create note')
  } finally {
    isCreating.value = false
  }
}
</script>

<template>
  <div class="flex flex-col min-h-[calc(100vh-8rem)]">
    <NoteFormHeader>
      <template #actions>
        <Button
          class="w-full sm:w-[116px] py-4 sm:py-6 cursor-pointer bg-[#DEEBF7] hover:bg-[#C5D9F0] text-gray-900 border-0"
          :disabled="isCreating"
          @click="create"
        >
          {{ isCreating ? 'Creating...' : 'Save' }}
        </Button>
      </template>
    </NoteFormHeader>

    <NoteForm
      v-model:title="title"
      v-model:content="content"
      v-model:type="type"
    />
  </div>
</template>
