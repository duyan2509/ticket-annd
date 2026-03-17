<template>
  <div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-blue-50 to-indigo-100">
    <div class="w-full max-w-md">
      <ResetForm 
        :loading="loading" 
        :error="error" 
        :success="success"
        :enable="canReset"
        :email="email"
        @submit="handleResetPassword" 
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import ResetForm from '~/components/ResetForm.vue'
import { resetPassword } from '~/api/auth'

definePageMeta({
  layout: 'auth',
})

const router = useRouter()
const route = useRoute()
const loading = ref(false)
const error = ref('')
const success = ref(false)
const email = ref('')
const token = ref('')
const canReset = computed(() => !!email.value && !!token.value)

onMounted(() => {
  const emailParam = route.query.email as string
  const tokenParam = route.query.token as string
  if (emailParam) {
    email.value = emailParam
  }
  if (tokenParam) {
    token.value = tokenParam
  }
})

async function handleResetPassword(payload: { newPassword: string }) {
  loading.value = true
  error.value = ''
  success.value = false
  
  try {
    if (!canReset.value) {
      throw new Error('Reset link is invalid or incomplete.')
    }

    await resetPassword(email.value, token.value, payload.newPassword)
    
    success.value = true
    
    setTimeout(() => {
      router.push('/login')
    }, 2000)
  } catch (err: any) {
    error.value = err?.response?.data?.message || err?.message || 'Failed to reset password. Please try again.'
  } finally {
    loading.value = false
  }
}
</script>
