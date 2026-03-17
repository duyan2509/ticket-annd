<template>
    <div class="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 p-8">
        <div class="max-w-6xl mx-auto">
            <!-- Welcome Section -->
            <div class="bg-white rounded-lg shadow-lg p-8 mb-8">
                <h1 class="text-4xl font-bold text-gray-800 mb-2">Dashboard</h1>
                <p class="text-gray-600 text-lg">Manage your tickets and team efficiently.</p>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-8 mb-8">
                <div class="bg-white rounded-lg shadow-lg p-6">
                    <h2 class="text-2xl font-bold text-gray-800 mb-6">Your Companies</h2>
                    <CreateCompanyForm @created="onCreated" />
                    <CompanyList :companies="companies"
                        :can-switch="(meResult?.currentRole ?? userContext?.role) !== AppRoles.SupperAdmin"
                        @switch="onSwitch" />

                    <div v-if="total > size" class="flex items-center gap-3 mt-4">
                        <button @click="prevPage" :disabled="page <= 1"
                            class="px-3 py-1 bg-gray-200 rounded hover:bg-gray-300 disabled:opacity-50">Prev</button>
                        <span class="text-sm text-gray-600">Page {{ page }} / {{ Math.max(1, Math.ceil(total / size))
                        }}</span>
                        <button @click="nextPage" :disabled="page >= Math.ceil(total / size)"
                            class="px-3 py-1 bg-gray-200 rounded hover:bg-gray-300 disabled:opacity-50">Next</button>
                    </div>
                </div>

                <div class="bg-white rounded-lg shadow-lg p-6">
                    <h2 class="text-2xl font-bold text-gray-800 mb-6">Invitations</h2>
                    <InvitationList :invitations="invitations" @accept="onAccept" @reject="onReject" />
                </div>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { computed, ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuth } from '~/composables/useAuth'
import { useAuthStore } from '~/store/authStore'
import { getMe, switchRole } from '~/api/auth'
import { getCompanies, getCurrentCompany } from '~/api/companies'
import { getInvitations, acceptInvitation, rejectInvitation } from '~/api/invitations'
import CreateCompanyForm from '~/components/CreateCompanyForm.vue'
import CompanyList from '~/components/CompanyList.vue'
import InvitationList from '~/components/InvitationList.vue'
import { AppRoles } from '~/types/appRoles'
import type { CompanyOption, InvitationItem } from '~/types/auth'
import type { MeResponse } from '~/api/auth'

definePageMeta({
    middleware: 'auth',
})

type PaginationResult = { items: any[]; total: number; page: number; size: number }

const router = useRouter()
const { getUserContext, setTokens, isLoggedIn } = useAuth()
const authStore = useAuthStore()

const userContext = ref<any>(null)
const meResult = ref<MeResponse | null>(null)
const companies = ref<CompanyOption[]>([])
const invitations = ref<InvitationItem[]>([])
const total = ref(0)
const page = ref(1)
const size = ref(10)

const isSuperAdmin = computed(() => (meResult.value?.currentRole ?? userContext.value?.role) === AppRoles.SupperAdmin)

async function load() {
    if (!isLoggedIn()) {
        try {
            meResult.value = await getMe()
        } catch {
            meResult.value = null
        }
    }
    userContext.value = getUserContext()
    if (!userContext.value) return
    try {
        const paged = await getCompanies(userContext.value, page.value, size.value)
        companies.value = paged.items
        total.value = paged.total
    } catch {
        companies.value = []
        total.value = 0
    }
    try {
        invitations.value = await getInvitations(userContext.value)
    } catch {
        invitations.value = []
    }
}

async function onSwitch(companyId: string) {
    try {
        const data = await switchRole(companyId)
        setTokens(data.accessToken)
        try {
            const company = await getCurrentCompany()
            router.push(`/${encodeURIComponent(company.slug)}`)
        } catch {
            router.push('/dashboard')
        }
    } catch {
        await load()
    }
}

async function onCreated(paged?: PaginationResult) {
    page.value = 1
    if (paged) {
        companies.value = paged.items
        total.value = paged.total
        return
    }
    await load()
}

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

async function onAccept(id: string) {
    try {
        await acceptInvitation(id)
        invitations.value = await getInvitations(userContext.value)
    } catch {
        // ignore
    }
}

async function onReject(id: string) {
    try {
        await rejectInvitation(id)
        invitations.value = await getInvitations(userContext.value)
    } catch {
        // ignore
    }
}

onMounted(load)

function navigateTo(path: string) {
    router.push(path)
}
</script>
