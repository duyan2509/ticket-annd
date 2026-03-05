<template>
  <div class="bg-white rounded-lg shadow p-6">
    <h2 class="text-lg font-semibold text-gray-800 mb-4">Create company</h2>
    <form @submit.prevent="submit" class="flex flex-col">
      <div class="mb-4 ">
        <label class="block text-gray-700 mb-2">Company name</label>
        <input v-model="name" type="text" required
          class="w-full px-3 py-2 border border-gray-300 rounded focus:ring focus:ring-blue-500" />
      </div>
      <button type="submit" :disabled="loading"
        class="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 disabled:opacity-50">
        {{ loading ? 'Creating...' : 'Create' }}
      </button>
      <div v-if="error" class="mb-4 p-2 bg-red-100 text-red-700 rounded text-sm">{{ error }}</div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { createCompany, getCompanies } from '../api/companies'
import type { CompanyPagedResult } from '../types/auth'
import { getMe } from '../api/auth'
import { useAuth } from '../composables/useAuth'
import { AppRoles } from '../types/appRoles'
import type { AxiosError } from 'axios'

const emit = defineEmits<{ (e: 'created', paged?: CompanyPagedResult): void }>()
const name = ref('')
const error = ref('')
const loading = ref(false)

async function submit() {
  error.value = ''
  loading.value = true
  try {
    await createCompany(name.value)
    name.value = ''

    const { getUserContext } = useAuth()
    let ctx = getUserContext()
    if (!ctx || ctx.role === AppRoles.EmptyUser) {
      try {
        await getMe()
      } catch {
        // ignore
      }
      ctx = getUserContext()
    }

    if (ctx) {
      try {
        const paged = await getCompanies(ctx, 1, 10)
        emit('created', paged)
        return
      } catch {
        // ignore and fall through
      }
    }

    emit('created')
  } catch (e) {
    const err = e as AxiosError<{ message?: string }>
    error.value = err.response?.data?.message ?? (e as Error).message ?? 'Failed to create company'
  } finally {
    loading.value = false
  }
}
</script>
