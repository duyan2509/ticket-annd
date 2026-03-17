<template>
  <div>
    <label class="block text-sm text-gray-700">Add category</label>
    <div class="flex gap-2 mt-2">
      <input v-model="name" placeholder="Category name" class="px-3 py-2 border rounded w-full" />
      <button @click.prevent="onSubmit" class="px-3 py-2 bg-blue-600 text-white rounded">Add</button>
    </div>
    <p v-if="error" class="text-sm text-red-500 mt-1">{{ error }}</p>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useCategoryValidation } from '../composables/useCategoryValidation'

const props = defineProps<{ error?: string }>()
const emit = defineEmits<{ (e: 'submit', name: string): void }>()

const name = ref('')
const error = props.error ?? ''
const { v$ } = useCategoryValidation({ name })

function onSubmit() {
  v$.value.$touch()
  if (!v$.value.$invalid) emit('submit', name.value)
}
</script>
