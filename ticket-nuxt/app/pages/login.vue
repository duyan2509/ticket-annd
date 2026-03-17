<template>
  <div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-blue-50 to-indigo-100">
    <div class="w-full max-w-md ">
      <LoginForm :loading="loading" :error="error" @submit="handleLogin" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import LoginForm from '~/components/LoginForm.vue'
import { useAuth } from '~/composables/useAuth'
import { getMe, login } from '~/api/auth'

definePageMeta({
  layout: 'auth',
})

const router = useRouter()
const { setTokens } = useAuth()
const loading = ref(false)
const error = ref('')

async function handleLogin(payload: { email: string; password: string }) {
  loading.value = true
  error.value = ''
  
  try {
    const result = await login(payload.email, payload.password)
    setTokens(result.accessToken)
    await getMe()
    await router.push('/dashboard')
  } catch (err: any) {
    error.value = err?.response?.data?.message || err?.message || 'Login failed. Please try again.'
  } finally {
    loading.value = false
  }
}
</script>
