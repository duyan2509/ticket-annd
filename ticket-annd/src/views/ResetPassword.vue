<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-100">
    <form class="bg-white p-8 rounded-lg shadow-md w-full max-w-md" @submit.prevent="submit">
      <h1 class="text-2xl font-bold mb-6 text-gray-800">Reset password</h1>
      <div v-if="error" class="mb-4 p-3 bg-red-100 text-red-700 rounded">{{ error }}</div>
      <div v-if="success" class="mb-4 p-3 bg-green-100 text-green-700 rounded">Password reset. Redirecting to login...</div>
      <div class="mb-4">
        <label class="block text-gray-700 mb-2">Email</label>
        <input v-model="email" type="email" required
          class="w-full px-3 py-2 border border-gray-300 rounded focus:ring focus:ring-blue-500" />
      </div>
      <div class="mb-4">
        <label class="block text-gray-700 mb-2">New password</label>
        <input v-model="newPassword" type="password" required minlength="6"
          class="w-full px-3 py-2 border border-gray-300 rounded focus:ring focus:ring-blue-500" />
      </div>
      <div class="mb-6">
        <label class="block text-gray-700 mb-2">Token (from email)</label>
        <input v-model="token" type="text" required
          class="w-full px-3 py-2 border border-gray-300 rounded focus:ring focus:ring-blue-500" />
      </div>
      <button type="submit" :disabled="loading"
        class="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700 disabled:opacity-50">
        {{ loading ? 'Resetting...' : 'Reset password' }}
      </button>
      <div class="mt-4 text-sm">
        <router-link to="/login" class="text-blue-600 hover:underline">Back to login</router-link>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { resetPassword } from '../api/auth'
import type { AxiosError } from 'axios'

const route = useRoute()
const router = useRouter()
const email = ref(typeof route.query.email === 'string' ? route.query.email : '')
const token = ref(typeof route.query.token === 'string' ? route.query.token : '')
const newPassword = ref('')
const error = ref('')
const success = ref(false)
const loading = ref(false)

onMounted(() => {
  if (route.query.email) email.value = String(route.query.email)
  if (route.query.token) token.value = String(route.query.token)
})

async function submit() {
  error.value = ''
  loading.value = true
  try {
    await resetPassword(email.value, token.value, newPassword.value)
    success.value = true
    setTimeout(() => router.push('/login'), 1500)
  } catch (e) {
    const err = e as AxiosError<{ message?: string; errors?: Record<string, string[]> }>
    const d = err.response?.data
    error.value = d?.errors ? Object.values(d.errors).flat().join(' ') : (d?.message ?? (e as Error).message ?? 'Reset failed')
  } finally {
    loading.value = false
  }
}
</script>
