<template>
  <div class="min-h-screen bg-gray-100">
    <AppHeader />
    <main class="max-w-4xl mx-auto p-6 space-y-6">
      <h1 class="text-xl font-bold text-gray-800">Dashboard</h1>
      <CreateCompanyForm @created="load" />
      <CompanyList v-if="userContext && (meResult?.currentRole ?? userContext.role) !== AppRoles.EmptyUser"
        :companies="companies" :can-switch="(meResult?.currentRole ?? userContext?.role) !== AppRoles.SupperAdmin"
        @switch="onSwitch" />

      <div v-if="total > size" class="flex items-center gap-3">
        <button @click="prevPage" :disabled="page <= 1" class="px-3 py-1 bg-gray-200 rounded">Prev</button>
        <span class="text-sm text-gray-600">Page {{ page }} / {{ Math.max(1, Math.ceil(total / size)) }}</span>
        <button @click="nextPage" :disabled="page >= Math.ceil(total / size)"
          class="px-3 py-1 bg-gray-200 rounded">Next</button>
      </div>

      <InvitationList :invitations="invitations" />
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '../composables/useAuth'
import { getMe, logout as apiLogout, switchRole } from '../api/auth'
import { getCompanies, getCurrentCompany } from '../api/companies'

import { getInvitations } from '../api/invitations'
import AppHeader from '../components/AppHeader.vue'
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
const total = ref(0)
const page = ref(1)
const size = ref(10)
const isSuperAdmin = computed(() => (meResult.value?.currentRole ?? userContext.value?.role) === AppRoles.SupperAdmin)

async function load() {
  if (!isLoggedIn()) return
  try {
    meResult.value = await getMe()
  } catch {
    meResult.value = null
  }
  userContext.value = getUserContext()
  if (!userContext.value) return
  const paged = await getCompanies(userContext.value, page.value, size.value)
  companies.value = paged.items
  total.value = paged.total
  invitations.value = await getInvitations(userContext.value)

}

async function onSwitch(companyId: string) {
  try {
    const data = await switchRole(companyId)
    setTokens(data.accessToken)
    try {
      const company = await getCurrentCompany()
      router.push(`/${company.slug}`)
    } catch {
      router.push('/')
    }
  } catch {
    await load()
  }
}

// logout handled by AppHeader

onMounted(load)

function prevPage() {
  if (page.value <= 1) return
  page.value -= 1
  load()
}

function nextPage() {
  const totalPages = Math.max(1, Math.ceil(total.value / size.value))
  if (page.value >= totalPages) return
  page.value += 1
  load()
}
</script>
