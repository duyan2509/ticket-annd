<template>
  <div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-blue-50 to-indigo-100">
    <div class="w-full max-w-md">
      <ForgotForm :loading="loading" :error="error" :success="success" @submit="handleForgotPassword" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import ForgotForm from '~/components/ForgotForm.vue'
import { forgotPassword } from '~/api/auth'

definePageMeta({
  layout: 'auth',
})

const loading = ref(false)
const error = ref('')
const success = ref(false)

async function handleForgotPassword(payload: { email: string }) {
  loading.value = true
  error.value = ''
  success.value = false
  
  try {
    await forgotPassword(payload.email)
    success.value = true
  } catch (err: any) {
    error.value = err?.response?.data?.message || err?.message || 'Failed to send reset email. Please try again.'
  } finally {
    loading.value = false
  }
}
</script>
