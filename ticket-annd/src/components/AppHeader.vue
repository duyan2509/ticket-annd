<template>
  <nav class="bg-white shadow flex items-center justify-between px-4 flex-wrap gap-2">
    <div class="flex items-center gap-4">
      <span class="font-medium text-gray-800">TicketAnnd</span>
      <router-link to="/" class="text-blue-600 hover:underline">Dashboard</router-link>
      <router-link v-if="isSuperAdmin" to="/admin" class="text-blue-600 hover:underline">Admin</router-link>
    </div>
    <div class="flex items-center gap-3">
      <AccountFrame :email="userContext?.email" :current-role="userContext?.currentRole" :company-name="userContext?.companyName" />
      <button @click="logout" class="text-gray-600 hover:text-gray-800 text-sm bg-red-500 text-white px-3 py-1 rounded">Logout</button>
    </div>
  </nav>
</template>

<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import AccountFrame from './AccountFrame.vue'
import { useAuth } from '../composables/useAuth'
import { getMe, logout as apiLogout } from '../api/auth'
import { AppRoles } from '../types/appRoles'

const { getUserContextRef, clearTokens } = useAuth()
const userContext = getUserContextRef()
const router = useRouter()

const isSuperAdmin = computed(() => (userContext.value?.currentRole ?? '') === AppRoles.SupperAdmin)

async function logout() {
  try {
    await apiLogout()
  } catch {
    // ignore
  }
  clearTokens()
  router.push('/login')
}

onMounted(async () => {
  try {
    await getMe()
  } catch {
    // ignore
  }
})
</script>
