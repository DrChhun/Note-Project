<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { toast } from '@/components/ui/sonner'
import { Button } from '@/components/ui/button'
import NoteFormHeader from '@/components/NoteFormHeader.vue'
import NoteForm from '@/components/NoteForm.vue'
import { getNoteById, updateNote, deleteNote } from '@/services/api/notes'
import type { NoteApi } from '@/types/note'

const route = useRoute()
const router = useRouter()
const noteId = computed(() => route.params.id as string)

const note = ref<NoteApi | null>(null)
const title = ref('')
const content = ref('')
const type = ref<number>(0)
const isLoading = ref(false)
const loadFailed = ref(false)
const isSaving = ref(false)
const isDeleting = ref(false)

onMounted(async () => {
  isLoading.value = true
  loadFailed.value = false
  try {
    note.value = await getNoteById(noteId.value)
    title.value = note.value.title
    content.value = note.value.content
    const t = note.value.type
    type.value = t === 0 || t === 1 || t === 2 ? t : 0
  } catch (e) {
    loadFailed.value = true
    toast.error(e instanceof Error ? e.message : 'Failed to load note')
  } finally {
    isLoading.value = false
  }
})

function goHome() {
  router.push('/')
}

async function save() {
  if (!title.value.trim()) {
    toast.error('Title is required')
    return
  }
  isSaving.value = true
  try {
    await updateNote(noteId.value, {
      title: title.value.trim(),
      content: content.value.trim(),
      type: type.value,
    })
    toast.success('Note saved successfully')
    router.push('/')
  } catch (e) {
    toast.error(e instanceof Error ? e.message : 'Failed to save note')
  } finally {
    isSaving.value = false
  }
}

async function handleDelete() {
  isDeleting.value = true
  try {
    const { success } = await deleteNote(noteId.value)
    if (success) {
      toast.success('Note deleted successfully')
      router.push('/')
    } else {
      toast.error('Delete request did not succeed')
    }
  } catch (e) {
    toast.error(e instanceof Error ? e.message : 'Failed to delete note')
  } finally {
    isDeleting.value = false
  }
}
</script>

<template>
  <div class="flex flex-col min-h-[calc(100vh-8rem)]">
    <NoteFormHeader>
      <template #actions>
        <template v-if="loadFailed">
          <Button
            class="w-full sm:w-[116px] py-4 sm:py-6 cursor-pointer bg-[#DEEBF7] hover:bg-[#C5D9F0] text-gray-900 border-0"
            @click="goHome"
          >
            Back to notes
          </Button>
        </template>
        <template v-else>
          <Button
            class="flex-1 min-w-0 sm:flex-none sm:w-[116px] py-4 sm:py-6 cursor-pointer bg-[#F8D7DA] hover:bg-[#F5C6CB] text-gray-800 border-0"
            :disabled="isDeleting || isSaving"
            @click="handleDelete"
          >
            {{ isDeleting ? 'Deleting...' : 'Delete' }}
          </Button>
          <Button
            class="flex-1 min-w-0 sm:flex-none sm:w-[116px] py-4 sm:py-6 cursor-pointer bg-[#DEEBF7] hover:bg-[#C5D9F0] text-gray-900 border-0"
            :disabled="isSaving || isDeleting"
            @click="save"
          >
            {{ isSaving ? 'Saving...' : 'Save' }}
          </Button>
        </template>
      </template>
    </NoteFormHeader>

    <div v-if="isLoading" class="text-muted-foreground text-sm">
      Loading note...
    </div>

    <div
      v-else-if="loadFailed"
      class="flex flex-col items-center justify-center gap-4 py-16 text-center"
    >
      <p class="text-muted-foreground">
        Note not found. It may have been deleted or the link is invalid.
      </p>
      <Button
        class="cursor-pointer bg-[#DEEBF7] hover:bg-[#C5D9F0] text-gray-900 border-0"
        @click="goHome"
      >
        Back to notes
      </Button>
    </div>

    <NoteForm
      v-else
      v-model:title="title"
      v-model:content="content"
      v-model:type="type"
      :date="note?.createdAt"
    />
  </div>
</template>
