<template>
  <LoginForm :loading="loading" :error="error" @submit="submit" />
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '../composables/useAuth'
import { login, getMe } from '../api/auth'
import { getCurrentCompany } from '../api/companies'
import { AppRoles } from '../types/appRoles'
import type { AxiosError } from 'axios'
import LoginForm from '../components/LoginForm.vue'

const router = useRouter()
const { setTokens } = useAuth()
const error = ref('')
const loading = ref(false)

async function submit(payload: { email: string; password: string }) {
  error.value = ''
  loading.value = true
  try {
    const data = await login(payload.email, payload.password)
    setTokens(data.accessToken)

    // fetch current user context
    const me = await getMe()

    if (me.currentRole === AppRoles.EmptyUser) {
      router.push('/')
      return
    }

    if (me.currentRole === AppRoles.SupperAdmin) {
      router.push('/admin')
      return
    }

    try {
      const company = await getCurrentCompany()
      router.push(`/${company.slug}`)
    } catch {
      router.push('/')
    }
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
