<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-100">
    <form class="bg-white p-8 rounded-lg shadow-md w-full max-w-md" @submit.prevent="onSubmit">
      <h1 class="text-2xl font-bold mb-6 text-gray-800">Reset password</h1>
      <div v-if="error" class="mb-4 p-3 bg-red-100 text-red-700 rounded">{{ error }}</div>
      <div v-if="success" class="mb-4 p-3 bg-green-100 text-green-700 rounded">Password reset. Redirecting to login...</div>
      <div v-if="emailProp" class="text-blue-700 text-lg">{{ emailProp }}</div>

      <div class="mb-4">
        <label class="block text-gray-700 mb-2">New password</label>
        <input v-model="newPassword" type="password" class="w-full px-3 py-2 border border-gray-300 rounded" />
        <p v-if="v$.newPassword.$dirty && v$.newPassword.required.$invalid" class="text-sm text-red-500 mt-1">Password is required.</p>
        <p v-else-if="v$.newPassword.$dirty && v$.newPassword.minLength.$invalid" class="text-sm text-red-500 mt-1">Password must be at least 6 characters.</p>
      </div>

      <button type="submit" :disabled="loading || !enable" class="w-full bg-blue-600 text-white py-2 rounded">
        {{ loading ? 'Resetting...' : 'Reset password' }}
      </button>

      <div class="mt-4 text-sm">
        <router-link to="/login" class="text-blue-600 hover:underline">Back to login</router-link>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useResetValidation } from '../composables/useResetValidation'

const props = defineProps<{ loading?: boolean; error?: string; success?: boolean; enable?: boolean; email?: string }>()
const emit = defineEmits<{ (e: 'submit', payload: { newPassword: string }): void }>()

const loading = props.loading ?? false
const error = props.error ?? ''
const success = props.success ?? false
const enable = props.enable ?? false
const emailProp = props.email ?? ''

const newPassword = ref('')
const { v$ } = useResetValidation({ newPassword })

function onSubmit() {
  v$.value.$touch()
  if (!v$.value.$invalid) emit('submit', { newPassword: newPassword.value })
}
</script>
