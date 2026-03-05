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

      <div class="mt-6">
        <h2 class="text-lg font-semibold">Invitations</h2>
        <div class="mt-3 grid grid-cols-1 gap-4">
          <div>
            <label class="block text-sm text-gray-700">Invite by email</label>
            <div class="flex gap-2 mt-2">
              <input v-model="inviteEmail" placeholder="email@example.com" class="px-3 py-2 border rounded w-full" />
              <select v-model="inviteRole" class="px-3 py-2 border rounded">
                <option value="Customer">Customer</option>
                <option value="Agent">Agent</option>
              </select>
              <button @click="onInvite" class="px-3 py-2 bg-blue-600 text-white rounded">Invite</button>
            </div>
            <p v-if="inviteError" class="text-sm text-red-500 mt-1">{{ inviteError }}</p>
          </div>

          <InvitationList :invitations="invitations" :showActions="false" />

          <div v-if="companyTotal > companySize" class="flex items-center gap-3">
            <button @click="prevCompanyPage" :disabled="companyPage <= 1"
              class="px-3 py-1 bg-gray-200 rounded">Prev</button>
            <span class="text-sm text-gray-600">Page {{ companyPage }} / {{ Math.max(1, Math.ceil(companyTotal /
              companySize)) }}</span>
            <button @click="nextCompanyPage" :disabled="companyPage >= Math.ceil(companyTotal / companySize)"
              class="px-3 py-1 bg-gray-200 rounded">Next</button>
          </div>
        </div>
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
import { getCompanyInvitations, createInvitation } from '../api/invitations'
import type { CompanyInvitationItem } from '../types/auth'
import AppHeader from '../components/AppHeader.vue'
import InvitationList from '../components/InvitationList.vue'

const route = useRoute()
const router = useRouter()
const { getUserContextRef } = useAuth()
const company = ref<{ id: string; name: string; slug: string; role?: string } | null>(null)
const invitations = ref<CompanyInvitationItem[]>([])
const inviteEmail = ref('')
const inviteRole = ref('Customer')
const inviteError = ref('')
const companyTotal = ref(0)
const companyPage = ref(1)
const companySize = ref(10)
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
    try {
      const paged = await getCompanyInvitations(companyPage.value, companySize.value)
      invitations.value = paged.items
      companyTotal.value = paged.total
    } catch {
      invitations.value = []
    }
  } catch {
    router.replace('/')
  }
}
function goDashboard() {
  router.push('/')
}

async function onInvite() {
  inviteError.value = ''
  if (!inviteEmail.value || !/.+@.+\..+/.test(inviteEmail.value)) {
    inviteError.value = 'Invalid email'
    return
  }
  try {
    await createInvitation(inviteEmail.value, inviteRole.value)
    inviteEmail.value = ''
    // refresh
    const paged = await getCompanyInvitations(companyPage.value, companySize.value)
    invitations.value = paged.items
    companyTotal.value = paged.total
  } catch (err: unknown) {
    let message = 'Failed to send invitation'
    const axiosErr = err as any
    if (axiosErr?.response) {
      const status = axiosErr.response.status
      const data = axiosErr.response.data
      if (status === 403) {
        message = data?.message ?? 'You do not have permission to invite users.'
      } else if (status === 400) {
        if (data?.errors && Array.isArray(data.errors) && data.errors.length > 0) {
          message = data.errors
            .map((e: any) => e.ErrorMessage ?? e.errorMessage ?? `${e.PropertyName ?? e.propertyName}: ${e.ErrorMessage ?? e.errorMessage}`)
            .join('; ')
        } else {
          message = data?.message ?? 'Bad request'
        }
      } else {
        message = data?.message ?? axiosErr.message ?? message
      }
    } else if (err instanceof Error) {
      message = err.message
    }
    inviteError.value = message
  }
}

async function prevCompanyPage() {
  if (companyPage.value <= 1) return
  companyPage.value -= 1
  const paged = await getCompanyInvitations(companyPage.value, companySize.value)
  invitations.value = paged.items
  companyTotal.value = paged.total
}

async function nextCompanyPage() {
  const totalPages = Math.max(1, Math.ceil(companyTotal.value / companySize.value))
  if (companyPage.value >= totalPages) return
  companyPage.value += 1
  const paged = await getCompanyInvitations(companyPage.value, companySize.value)
  invitations.value = paged.items
  companyTotal.value = paged.total
}

onMounted(load)
</script>
