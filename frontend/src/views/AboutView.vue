<script setup lang="ts">
import { ref } from 'vue'
import { Button } from '@/components/ui/button'
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from '@/components/ui/card'
import { Input } from '@/components/ui/input'

const name = ref('')
const message = ref('')

function handleSubmit() {
  if (!name.value.trim()) return
  alert(`Thanks, ${name.value}!${message.value ? ` Message: ${message.value}` : ''}`)
}
</script>

<template>
  <div class="about">
    <h1 class="text-3xl font-bold mb-8">This is an about page</h1>

    <Card class="w-full max-w-md">
      <CardHeader>
        <CardTitle>Get in touch</CardTitle>
        <CardDescription>
          Send us a quick message. We’ll get back to you soon.
        </CardDescription>
      </CardHeader>
      <CardContent class="space-y-4">
        <div class="space-y-2">
          <label for="name" class="text-sm font-medium text-foreground">Name</label>
          <Input
            id="name"
            v-model="name"
            placeholder="Your name"
            class="w-full"
          />
        </div>
        <div class="space-y-2">
          <label for="message" class="text-sm font-medium text-foreground">Message</label>
          <Input
            id="message"
            v-model="message"
            placeholder="Your message"
            class="w-full"
          />
        </div>
      </CardContent>
      <CardFooter class="flex gap-2">
        <Button @click="handleSubmit">Submit</Button>
        <Button variant="outline" @click="name = ''; message = ''">Clear</Button>
      </CardFooter>
    </Card>
  </div>
</template>

<style scoped>
@media (min-width: 1024px) {
  .about {
    min-height: 100vh;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
  }
}
</style>
