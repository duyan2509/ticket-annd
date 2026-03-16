<template>
  <ResetForm :loading="loading" :error="error" :success="success" :enable="enable" :email="email" @submit="submit" />
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { resetPassword } from '../api/auth'
import type { AxiosError } from 'axios'
import ResetForm from '../components/ResetForm.vue'

const route = useRoute()
const router = useRouter()
const email = ref(typeof route.query.email === 'string' ? route.query.email : '')
const token = ref(typeof route.query.token === 'string' ? route.query.token : '')
const error = ref('')
const success = ref(false)
const loading = ref(false)
const enable = ref(false)

onMounted(() => {
  if (route.query.email) email.value = String(route.query.email)
  if (route.query.token) token.value = String(route.query.token)
  if (route.query.token && route.query.email) enable.value = true
})

async function submit(payload: { newPassword: string }) {
  error.value = ''
  loading.value = true
  try {
    await resetPassword(email.value, token.value, payload.newPassword)
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
