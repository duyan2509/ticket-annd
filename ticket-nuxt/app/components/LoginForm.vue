<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-100">
    <form class="bg-white p-8 rounded-lg shadow-md w-full max-w-md" @submit.prevent="onSubmit">
      <h1 class="text-2xl font-bold mb-6 text-gray-800">Login</h1>

      <div v-if="error" class="mb-4 p-3 bg-red-100 text-red-700 rounded">{{ error }}</div>

      <div class="mb-4">
        <label class="block text-gray-700 mb-2">Email</label>
        <input v-model="email" type="email"
          class="w-full px-3 py-2 border border-gray-300 rounded focus:ring focus:ring-blue-500" />
        <p v-if="v$.email.$dirty && v$.email.required.$invalid" class="text-sm text-red-500 mt-1">Email is required.</p>
        <p v-else-if="v$.email.$dirty && v$.email.email.$invalid" class="text-sm text-red-500 mt-1">Invalid email.</p>
      </div>

      <div class="mb-6">
        <label class="block text-gray-700 mb-2">Password</label>
        <input v-model="password" type="password"
          class="w-full px-3 py-2 border border-gray-300 rounded focus:ring focus:ring-blue-500" />
        <p v-if="v$.password.$dirty && v$.password.required.$invalid" class="text-sm text-red-500 mt-1">Password is required.</p>
        <p v-else-if="v$.password.$dirty && v$.password.minLength.$invalid" class="text-sm text-red-500 mt-1">Password must be at least 6 characters.</p>
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
import { ref, toRefs } from 'vue'
import { useLoginValidation } from '../composables/useLoginValidation'

const props = defineProps<{ loading?: boolean; error?: string }>()
const emit = defineEmits<{ (e: 'submit', payload: { email: string; password: string }): void }>()

const loading = props.loading ?? false
const error = props.error ?? ''

const email = ref('')
const password = ref('')

const { v$ } = useLoginValidation({ email, password })

function onSubmit() {
  v$.value.$touch()
  if (!v$.value.$invalid) {
    emit('submit', { email: email.value, password: password.value })
  }
}

const { value: v$value } = toRefs(v$)
const v = v$


</script>
