<template>
  <div class="mt-2 p-3 bg-gray-50 rounded">
    <div class="flex gap-2 items-center">
      <input v-model="name" placeholder="Rule name" class="px-2 py-1 border rounded w-48" />
      <input type="number" v-model.number="firstResponseMinutes" placeholder="First response (min)" class="px-2 py-1 border rounded w-36" />
      <input type="number" v-model.number="resolutionMinutes" placeholder="Resolution (min)" class="px-2 py-1 border rounded w-36" />
      <button @click.prevent="onSubmit" class="px-3 py-1 bg-blue-600 text-white rounded">Add Rule</button>
    </div>
    <p v-if="error" class="text-sm text-red-500 mt-1">{{ error }}</p>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useSlaRuleValidation } from '../composables/useSlaRuleValidation'

const props = defineProps<{ error?: string }>()
const emit = defineEmits<{ (e: 'submit', payload: { name: string; firstResponseMinutes: number | null; resolutionMinutes: number | null }): void }>()

const name = ref('')
const firstResponseMinutes = ref<number | null>(null)
const resolutionMinutes = ref<number | null>(null)
const error = props.error ?? ''
const { v$ } = useSlaRuleValidation({ name, firstResponseMinutes, resolutionMinutes })

function onSubmit() {
  v$.value.$touch()
  if (!v$.value.$invalid) emit('submit', { name: name.value, firstResponseMinutes: firstResponseMinutes.value, resolutionMinutes: resolutionMinutes.value })
}
</script>
