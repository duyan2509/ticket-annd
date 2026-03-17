<template>
  <nav class="bg-white shadow flex items-center justify-between px-4 flex-wrap gap-2 sticky top-0 z-50">
    <div class="flex items-center gap-4">
      <span class="font-medium text-gray-800">TicketAnnd</span>
      <NuxtLink to="/dashboard" class="text-blue-600 hover:underline">Dashboard</NuxtLink>
      <NuxtLink v-if="isSuperAdmin" to="/admin" class="text-blue-600 hover:underline">Admin</NuxtLink>
    </div>
    <div class="flex items-center gap-3">
      <AccountFrame :email="userContext?.email" :current-role="userContext?.currentRole" :company-name="userContext?.companyName" />
      <button @click="logout" class="text-gray-600 hover:text-gray-800 text-sm bg-red-500 px-3 py-1 rounded">Logout</button>
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
import { getCurrentCompany } from '~/api/companies'

const { getUserContextRef, clearTokens } = useAuth()
const userContext = getUserContextRef() // Ref<MeCache | null>
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
    await getCurrentCompany()

  } catch {
    // ignore
  }
})
</script>
