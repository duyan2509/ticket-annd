<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-100">
    <form class="bg-white p-8 rounded-lg shadow-md w-full max-w-md" @submit.prevent="submit">
      <h1 class="text-2xl font-bold mb-6 text-gray-800">Login</h1>
      <div v-if="error" class="mb-4 p-3 bg-red-100 text-red-700 rounded">{{ error }}</div>
      <div class="mb-4">
        <label class="block text-gray-700 mb-2">Email</label>
        <input v-model="email" type="email" required
          class="w-full px-3 py-2 border border-gray-300 rounded focus:ring focus:ring-blue-500" />
      </div>
      <div class="mb-6">
        <label class="block text-gray-700 mb-2">Password</label>
        <input v-model="password" type="password" required
          class="w-full px-3 py-2 border border-gray-300 rounded focus:ring focus:ring-blue-500" />
      </div>
      <button type="submit" :disabled="loading"
        class="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700 disabled:opacity-50">
        {{ loading ? 'Signing in...' : 'Sign in' }}
      </button>
      <div class="mt-4 text-sm text-gray-600">
        <router-link to="/register" class="text-blue-600 hover:underline">Register</router-link>
        ·
        <router-link to="/forgot-password" class="text-blue-600 hover:underline">Forgot password</router-link>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '../composables/useAuth'
import { login } from '../api/auth'
import type { AxiosError } from 'axios'

const router = useRouter()
const { setTokens } = useAuth()
const email = ref('')
const password = ref('')
const error = ref('')
const loading = ref(false)

async function submit() {
  error.value = ''
  loading.value = true
  try {
    const data = await login(email.value, password.value)
    setTokens(data.accessToken)
    router.push('/')
  } catch (e) {
    const err = e as AxiosError<{ message?: string; errors?: { propertyName: string; errorMessage: string }[] }>
    const data = err.response?.data
    if (data?.errors?.length) {
      error.value = data.errors.map((x) => x.errorMessage).join(' ')
    } else {
      error.value = data?.message ?? (e as Error).message ?? 'Invalid email or password.'
    }
  } finally {
    loading.value = false
  }
}
</script>
