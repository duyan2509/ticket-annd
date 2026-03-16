<template>
  <RegisterForm :loading="loading" :error="error" :success="success" @submit="submit" />
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { register } from '../api/auth'
import type { AxiosError } from 'axios'
import RegisterForm from '../components/RegisterForm.vue'

const router = useRouter()
const error = ref('')
const success = ref(false)
const loading = ref(false)

async function submit(payload: { email: string; password: string }) {
  error.value = ''
  loading.value = true
  try {
    await register(payload.email, payload.password)
    success.value = true
    setTimeout(() => router.push('/login'), 1500)
  } catch (e) {
    const err = e as AxiosError<{ message?: string; errors?: Record<string, string[]> }>
    const d = err.response?.data
    console.log(d)
    error.value = d?.message ?? 'Register fail'
  } finally {
    loading.value = false
  }
}
</script>
