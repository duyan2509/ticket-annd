<template>
  <div class="min-h-screen bg-gray-100">
    <nav class="bg-white shadow flex items-center justify-between px-4 py-3">
      <div class="flex items-center gap-4">
        <router-link to="/" class="text-blue-600 hover:underline">Back to Dashboard</router-link>
      </div>
      <div class="flex items-center gap-3">
        <AccountFrame
          :email="meResult?.email"
          :current-role="meResult?.currentRole ?? AppRoles.SupperAdmin"
        />
        <button @click="logout" class="text-gray-600 hover:text-gray-800">Logout</button>
      </div>
    </nav>
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
import { getMe, logout as apiLogout } from '../api/auth'
import AccountFrame from '../components/AccountFrame.vue'
import { AppRoles } from '../types/appRoles'
import type { MeResponse } from '../api/auth'

const { clearTokens } = useAuth()
const router = useRouter()
const meResult = ref<MeResponse | null>(null)

onMounted(async () => {
  try {
    meResult.value = await getMe()
  } catch {
    meResult.value = null
  }
})

async function logout() {
  try {
    await apiLogout()
  } catch {
    // ignore
  }
  clearTokens()
  router.push('/login')
}
</script>
