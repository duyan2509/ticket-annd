<template>
  <ForgotForm :loading="loading" :error="error" :success="success" @submit="submit" />
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { forgotPassword } from '../api/auth'
import type { AxiosError } from 'axios'
import ForgotForm from '../components/ForgotForm.vue'

const error = ref('')
const success = ref(false)
const loading = ref(false)

async function submit(payload: { email: string }) {
  error.value = ''
  success.value = false
  loading.value = true
  try {
    await forgotPassword(payload.email)
    success.value = true
  } catch (e) {
    const err = e as AxiosError<{ message?: string }>
    error.value = err.response?.data?.message ?? (e as Error).message ?? 'Request failed'
  } finally {
    loading.value = false
  }
}
</script>
