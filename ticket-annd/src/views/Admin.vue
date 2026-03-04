<template>
  <div class="min-h-screen bg-gray-100">
    <AppHeader />
    <main class="max-w-4xl mx-auto p-6">
      <h1 class="text-2xl font-bold text-gray-800 mb-4">Super admin area</h1>
      <p class="text-gray-600">Admin site placeholder.</p>
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '../composables/useAuth'
import { getMe } from '../api/auth'
import AppHeader from '../components/AppHeader.vue'
import { AppRoles } from '../types/appRoles'
import type { MeResponse } from '../api/auth'

const { clearTokens } = useAuth()
const router = useRouter()
const meResult = ref<MeResponse | null>(null)

onMounted(async () => {
  try {
    await getMe()
  } catch {
    // ignore
  }
})

// logout handled by AppHeader
</script>
