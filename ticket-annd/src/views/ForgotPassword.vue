<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-100">
    <form class="bg-white p-8 rounded-lg shadow-md w-full max-w-md" @submit.prevent="submit">
      <h1 class="text-2xl font-bold mb-6 text-gray-800">Forgot password</h1>
      <div v-if="error" class="mb-4 p-3 bg-red-100 text-red-700 rounded">{{ error }}</div>
      <div v-if="success" class="mb-4 p-3 bg-green-100 text-green-700 rounded">Check your email for the reset link.</div>
      <div class="mb-6">
        <label class="block text-gray-700 mb-2">Email</label>
        <input v-model="email" type="email" required
          class="w-full px-3 py-2 border border-gray-300 rounded focus:ring focus:ring-blue-500" />
      </div>
      <button type="submit" :disabled="loading"
        class="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700 disabled:opacity-50">
        {{ loading ? 'Sending...' : 'Send reset link' }}
      </button>
      <div class="mt-4 text-sm">
        <router-link to="/login" class="text-blue-600 hover:underline">Back to login</router-link>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { forgotPassword } from '../api/auth'
import type { AxiosError } from 'axios'

const email = ref('')
const error = ref('')
const success = ref(false)
const loading = ref(false)

async function submit() {
  error.value = ''
  success.value = false
  loading.value = true
  try {
    await forgotPassword(email.value)
    success.value = true
  } catch (e) {
    const err = e as AxiosError<{ message?: string }>
    error.value = err.response?.data?.message ?? (e as Error).message ?? 'Request failed'
  } finally {
    loading.value = false
  }
}
</script>
