<template>
  <div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-blue-50 to-indigo-100">
    <div class="w-full max-w-md">
      <RegisterForm :loading="loading" :error="error" :success="success" @submit="handleRegister" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import RegisterForm from '~/components/RegisterForm.vue'
import { register as registerUser } from '~/api/auth'

definePageMeta({
  layout: 'auth',
})

const router = useRouter()
const loading = ref(false)
const error = ref('')
const success = ref(false)

async function handleRegister(payload: { email: string; password: string }) {
  loading.value = true
  error.value = ''
  success.value = false
  
  try {
    await registerUser(payload.email, payload.password)
    success.value = true
    
    setTimeout(() => {
      router.push('/login')
    }, 2000)
  } catch (err: any) {
    error.value = err?.response?.data?.message || err?.message || 'Registration failed. Please try again.'
  } finally {
    loading.value = false
  }
}
</script>
