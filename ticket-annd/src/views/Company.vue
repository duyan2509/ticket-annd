<template>
  <div class="min-h-screen bg-gray-100">
    <AppHeader />

    <main class="max-w-4xl mx-auto p-6">
      <h1 class="text-2xl font-semibold text-gray-800">{{ company?.name ?? 'Company' }}</h1>
      <p class="text-sm text-gray-600 mt-2">Slug: {{ company?.slug }}</p>
      <div class="mt-6 bg-white rounded shadow p-4">
        <p class="text-sm text-gray-700">Role: {{ company?.role ?? '-' }}</p>
      </div>

      <div class="mt-6">
        <button @click="goDashboard" class="px-3 py-1 bg-gray-200 rounded">Back to Dashboard</button>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuth } from '../composables/useAuth'
import { getMe } from '../api/auth'
import { getCurrentCompany } from '../api/companies'
import AppHeader from '../components/AppHeader.vue'

const route = useRoute()
const router = useRouter()
const { getUserContextRef } = useAuth()
const company = ref<{ id: string; name: string; slug: string; role?: string } | null>(null)
const userContext = getUserContextRef()

async function load() {
  try {
    const c = await getCurrentCompany()
    if (route.params.slug && String(route.params.slug) !== c.slug) {
      router.replace('/')
      return
    }
    company.value = c
    try {
      await getMe()
    } catch {
      // ignore
    }
  } catch {
    router.replace('/')
  }
}
function goDashboard() {
  router.push('/')
}

onMounted(load)
</script>

