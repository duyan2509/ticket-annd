<template>
  <div class="min-h-screen bg-gray-100">
    <nav class="bg-white shadow flex items-center justify-between px-4 py-3 flex-wrap gap-2">
      <div class="flex items-center gap-4">
        <span class="font-medium text-gray-800">TicketAnnd</span>
        <router-link to="/" class="text-blue-600 hover:underline">Dashboard</router-link>
        <router-link v-if="isSuperAdmin" to="/admin" class="text-blue-600 hover:underline">Admin</router-link>
      </div>
      <div class="flex items-center gap-3">
        <AccountFrame
          :email="meResult?.email"
          :current-role="meResult?.currentRole ?? userContext?.role"
        />
        <button @click="logout" class="text-gray-600 hover:text-gray-800 text-sm">Logout</button>
      </div>
    </nav>
    <main class="max-w-4xl mx-auto p-6 space-y-6">
      <h1 class="text-xl font-bold text-gray-800">Dashboard</h1>
      <CreateCompanyForm v-if="showCreateCompany" @created="load" />
      <CompanyList
        v-if="userContext && (meResult?.currentRole ?? userContext.role) !== AppRoles.EmptyUser"
        :companies="companies"
        :can-switch="(meResult?.currentRole ?? userContext?.role) !== AppRoles.SupperAdmin"
        @switch="onSwitch"
      />
      <InvitationList :invitations="invitations" />
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '../composables/useAuth'
import { getMe, logout as apiLogout, switchRole } from '../api/auth'
import { getCompanies } from '../api/companies'
import { getInvitations } from '../api/invitations'
import AccountFrame from '../components/AccountFrame.vue'
import CreateCompanyForm from '../components/CreateCompanyForm.vue'
import CompanyList from '../components/CompanyList.vue'
import InvitationList from '../components/InvitationList.vue'
import type { UserContext, CompanyOption, InvitationItem } from '../types/auth'
import { AppRoles } from '../types/appRoles'
import type { MeResponse } from '../api/auth'

const router = useRouter()
const { getUserContext, clearTokens, setTokens, isLoggedIn } = useAuth()
const userContext = ref<UserContext | null>(null)
const meResult = ref<MeResponse | null>(null)
const companies = ref<CompanyOption[]>([])
const invitations = ref<InvitationItem[]>([])

const isSuperAdmin = computed(() => (meResult.value?.currentRole ?? userContext.value?.role) === AppRoles.SupperAdmin)
const showCreateCompany = computed(() => !companies.value.some((c) => c.role === AppRoles.CompanyAdmin))

async function load() {
  if (!isLoggedIn()) return
  try {
    meResult.value = await getMe()
  } catch {
    meResult.value = null
  }
  userContext.value = getUserContext()
  if (!userContext.value) return
  companies.value = await getCompanies(userContext.value)
  invitations.value = await getInvitations(userContext.value)
}

async function onSwitch(companyId: string) {
  try {
    const data = await switchRole(companyId)
    setTokens(data.accessToken)
    await load()
  } catch {
    await load()
  }
}

async function logout() {
  try {
    await apiLogout()
  } catch {
    // ignore
  }
  clearTokens()
  router.push('/login')
}

onMounted(load)
</script>
