<template>
  <div class="bg-white rounded-lg shadow p-6">
    <h2 class="text-lg font-semibold text-gray-800 mb-4">Create company</h2>
    <form @submit.prevent="submit">
      <div class="mb-4">
        <label class="block text-gray-700 mb-2">Company name</label>
        <input v-model="name" type="text" required
          class="w-full px-3 py-2 border border-gray-300 rounded focus:ring focus:ring-blue-500" />
      </div>
      <div v-if="error" class="mb-4 p-2 bg-red-100 text-red-700 rounded text-sm">{{ error }}</div>
      <button type="submit" :disabled="loading"
        class="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 disabled:opacity-50">
        {{ loading ? 'Creating...' : 'Create' }}
      </button>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { createCompany } from '../api/companies'
import type { AxiosError } from 'axios'

const emit = defineEmits<{ (e: 'created'): void }>()
const name = ref('')
const error = ref('')
const loading = ref(false)

async function submit() {
  error.value = ''
  loading.value = true
  try {
    await createCompany(name.value)
    name.value = ''
    emit('created')
  } catch (e) {
    const err = e as AxiosError<{ message?: string }>
    error.value = err.response?.data?.message ?? (e as Error).message ?? 'Failed to create company'
  } finally {
    loading.value = false
  }
}
</script>
