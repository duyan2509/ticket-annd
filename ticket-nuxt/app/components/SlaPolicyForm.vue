<template>
  <div v-if="show" class="mb-3 p-3 bg-gray-50 rounded">
    <div class="flex gap-2">
      <input v-model="name" placeholder="Policy name" class="px-2 py-1 border rounded w-full" />
      <button @click.prevent="onSubmit" class="px-3 py-1 bg-blue-600 text-white rounded">Create</button>
      <button @click.prevent="onCancel" class="px-3 py-1 bg-gray-200 rounded">Cancel</button>
    </div>
    <p v-if="error" class="text-sm text-red-500 mt-1">{{ error }}</p>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useSlaPolicyValidation } from '../composables/useSlaPolicyValidation'

const props = defineProps<{ show?: boolean; error?: string }>()
const emit = defineEmits<{ (e: 'submit', name: string): void; (e: 'cancel'): void }>()

const show = props.show ?? true
const error = props.error ?? ''
const name = ref('')
const { v$ } = useSlaPolicyValidation({ name })

function onSubmit() {
  v$.value.$touch()
  if (!v$.value.$invalid) emit('submit', name.value)
}

function onCancel() {
  emit('cancel')
}
</script>
