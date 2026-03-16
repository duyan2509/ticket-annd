<template>
    <div class="min-h-screen bg-gray-100">
        <AppHeader />
        <main class="max-w-4xl mx-auto p-6">
            <div class="flex items-center justify-between mb-4">
                <h1 class="text-2xl font-semibold mb-4">Tickets</h1>
                <div class="flex items-center gap-2">
                    <button @click="viewMode = 'list'" :class="viewMode === 'list' ? 'px-3 py-1 rounded bg-blue-600 text-white' : 'px-3 py-1 rounded bg-gray-200'">Ticket List</button>
                    <button @click="viewMode = 'create'" :class="viewMode === 'create' ? 'px-3 py-1 rounded bg-blue-600 text-white' : 'px-3 py-1 rounded bg-gray-200'">Raise Ticket</button>
                    <button @click="goGeneral" class="px-3 py-1 bg-gray-200 rounded">Back to Dashboard</button>
                </div>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
                <div v-if="viewMode === 'create'" class="bg-white rounded shadow p-4 md:col-span-3">
                    <h2 class="text-lg font-medium mb-3">Create ticket</h2>
                    <div class="grid grid-cols-1 gap-3">
                        <select v-model="form.categoryId" class="px-2 py-2 border rounded">
                            <option value="">Select category</option>
                            <option v-for="c in categories" :key="c.id" :value="c.id">{{ c.name }}</option>
                        </select>


                        <select v-model="form.slaRuleId" class="px-2 py-2 border rounded">
                            <option value="">Select SLA rule</option>
                            <option v-for="r in rules" :key="r.id" :value="r.id">{{ r.name }}</option>
                        </select>

                        <select v-if="userContext?.currentRole == AppRoles.CompanyAdmin" v-model="form.teamId"
                            class="px-2 py-2 border rounded">
                            <option value="">Assign to team (optional)</option>
                            <option v-for="t in teams" :key="t.teamId" :value="t.teamId">{{ t.name }}</option>
                        </select>

                        <input v-model="form.subject" placeholder="Subject" class="px-2 py-2 border rounded" />
                        <textarea v-model="form.body" placeholder="Body (optional)"
                            class="px-2 py-2 border rounded h-28"></textarea>

                        <div class="flex items-center gap-2">
                            <button @click="onCreate" class="px-3 py-2 bg-blue-600 text-white rounded">Create</button>
                            <div v-if="loading" class="text-sm text-gray-600">Creating...</div>
                            <div v-if="success" class="text-sm text-green-600">Created successfully.</div>
                            <div v-if="error" class="text-sm text-red-600">{{ error }}</div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Tickets list -->
            <div v-if="viewMode === 'list'" class="mt-6 bg-white rounded shadow p-4 md:col-span-2">
                <div class="flex items-center justify-between mb-4">
                    <h2 class="text-lg font-medium">Tickets list</h2>
                    <div class="flex items-center gap-2">
                        <input v-model="subjectFilter" placeholder="Search subject" class="px-2 py-1 border rounded" />
                        <select v-model="teamFilter" class="px-2 py-1 border rounded">
                            <option value="">All teams</option>
                            <option value="__unassigned">Unassigned</option>
                            <option v-for="t in teams" :key="t.teamId" :value="t.teamId">{{ t.name }}</option>
                        </select>
                        <select v-model="statusFilter" class="px-2 py-1 border rounded">
                            <option value="">All status</option>
                            <option value="Open">Open</option>
                            <option value="Resolved">Resolved</option>
                            <option value="InProgress">In Progress</option>
                            <option value="WaitingCustomer">Waiting Customer</option>
                            <option value="WaitingThirdParty">Waiting Third Party</option>
                        </select>
                        <button @click="() => { page = 1; loadTickets() }"
                            class="px-3 py-1 bg-blue-600 text-white rounded">Search</button>
                        <button @click="loadTickets" class="px-3 py-1 bg-gray-200 rounded">Refresh</button>
                    </div>
                </div>

                <div v-if="listLoading" class="text-sm text-gray-600">Loading tickets...</div>
                <div v-if="listError" class="text-sm text-red-600">{{ listError }}</div>

                <table v-if="!listLoading && tickets.length" class="w-full table-auto border-collapse">
                    <thead>
                        <tr class="text-left">
                            <th class="py-2">Code</th>
                            <th class="py-2">Subject</th>
                            <th class="py-2">Status</th>
                            <th class="py-2">Raiser</th>
                            <th class="py-2">Category</th>
                            <th class="py-2">Team</th>
                            <th class="py-2">Actions</th>

                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="t in tickets" :key="t.id || t.ticketCode" class="border-t">
                            <td class="py-2">{{ t.ticketCode }}</td>
                            <td class="py-2 text-lg">{{ t.subject }}</td>
                            <td class="py-2">
                                <span class="px-2 py-1 bg-green-500 text-white rounded">{{ t.status }}</span>
                            </td>
                            <td class="py-2">{{ t.raiserName }}</td>
                            <td class="py-2">{{ t.categoryName }}</td>
                            <td v-if="t.teamName" class="py-2">{{ t.teamName }}</td>
                            <td v-else class="py-2 text-gray-500">
                                <button @click="openAssignModal(t.id, t.teamId)"
                                    class="px-2 py-1 bg-yellow-400 text-white rounded text-sm">Assign to team</button>
                            </td>
                            <td class="py-2">
                                <button @click="() => router.push(`/${companySlug}/tickets/${t.ticketCode}`)"
                                    class="px-3 py-1 bg-blue-600 text-white rounded">View</button>
                            </td>
                        </tr>
                    </tbody>
                </table>

                <div v-if="!listLoading && !tickets.length" class="text-sm text-gray-600">No tickets found.</div>

                <div class="mt-4 flex items-center justify-between">
                    <div class="text-sm text-gray-600">Total: {{ total ?? tickets.length }}</div>
                    <div class="flex items-center gap-2">
                        <button @click="() => { if (page > 1) { page--; loadTickets() } }"
                            class="px-3 py-1 bg-gray-200 rounded">Prev</button>
                        <div>Page {{ page }}</div>
                        <button @click="() => { page++; loadTickets() }"
                            class="px-3 py-1 bg-gray-200 rounded">Next</button>
                    </div>
                </div>
            </div>
        </main>
    </div>

    <template v-if="showAssignModal">
        <div class="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50">
            <div class="bg-white rounded p-4 w-full max-w-md">
                <h3 class="text-lg font-medium mb-2">Assign team</h3>
                <select v-model="selectedTeamId" class="w-full px-2 py-2 border rounded mb-3">
                    <option value="">Select team</option>
                    <option v-for="t in teams" :key="t.teamId" :value="t.teamId">{{ t.name }}</option>
                </select>
                <div class="flex justify-end gap-2">
                    <button @click="closeAssignModal" class="px-3 py-1 bg-gray-200 rounded">Cancel</button>
                    <button @click="assignFromModal" class="px-3 py-1 bg-blue-600 text-white rounded">Assign</button>
                </div>
            </div>
        </div>
    </template>
</template>


<script setup lang="ts">
import AppHeader from '../components/AppHeader.vue'
import { ref, onMounted, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { getCurrentCompany } from '../api/companies'
import { getCategories } from '../api/categories'
import { getActiveSlaRules } from '../api/sla'
import { getTeamsByCompany } from '../api/teams'
import api from '../api/axios'
import { createTicket, getTickets, assignTeam } from '../api/tickets'
import { useAuthStore } from '../store/authStore'
import { AppRoles } from '../types/appRoles'
import { TicketPageItem } from '../types/auth'

const router = useRouter()
const route = useRoute()
const companySlug = String(route.params.slug || '')

const companyId = ref<string | null>(null)
const categories = ref<any[]>([])
const rules = ref<any[]>([])
const teams = ref<{ teamId: string; name: string }[]>([])

const tickets = ref<TicketPageItem[]>([])
const total = ref<number | null>(null)
const page = ref(1)
const pageSize = ref(10)
const listLoading = ref(false)
const listError = ref('')
const subjectFilter = ref('')
const statusFilter = ref('')
const teamFilter = ref('')

const form = ref({ categoryId: '', slaRuleId: '', teamId: '', subject: '', body: '' })
const viewMode = ref<'list' | 'create'>('list')
const loading = ref(false)
const success = ref(false)
const error = ref('')

// Assign modal state
const showAssignModal = ref(false)
const assignTicketId = ref<string | null>(null)
const selectedTeamId = ref<string>('')

const userContext = useAuthStore().me

async function goGeneral() {
    router.push(`/${companySlug}`)
}

async function loadLists() {
    try {
        const c = await getCurrentCompany()
        companyId.value = c?.id ?? null
        categories.value = await getCategories()
        rules.value = await getActiveSlaRules()

        if (companyId.value) {
            teams.value = await getTeamsByCompany(companyId.value)
        } else {
            const { data } = await api.get(`/teams`)
            teams.value = data
        }
    } catch (e) {
        // ignore
    }
}

async function onCreate() {
    error.value = ''
    success.value = false
    if (!form.value.categoryId) { error.value = 'Category required'; return }
    if (!form.value.slaRuleId) { error.value = 'SLA rule required'; return }
    if (!form.value.subject || form.value.subject.trim() === '') { error.value = 'Subject required'; return }

    loading.value = true
    try {
        const res = await createTicket(form.value.categoryId, form.value.slaRuleId, form.value.subject.trim(), form.value.body || null, form.value.teamId || null)
        success.value = true
        form.value = { categoryId: '', slaRuleId: '', teamId: '', subject: '', body: '' }
    } catch (ex: any) {
        error.value = ex?.response?.data?.message ?? (ex?.message ?? 'Failed to create ticket')
    } finally {
        loading.value = false
    }
}

async function assignToTeam(ticketId: string, teamId?: string) {
    listError.value = ''
    listLoading.value = true
    console.log('Assigning ticket', ticketId, 'to team', teamId)
    try {
        const idToAssign = teamId || form.value.teamId || (teams.value.length ? teams.value[0].teamId : '')
        if (!idToAssign) { listError.value = 'No team selected to assign'; return }
        await assignTeam(ticketId, idToAssign)
        await loadTickets()
    } catch (ex: any) {
        listError.value = ex?.response?.data?.message ?? ex?.message ?? 'Failed to assign team'
    } finally {
        listLoading.value = false
    }
}

onMounted(loadLists)

function openAssignModal(ticketId: string, teamId?: string) {
    assignTicketId.value = ticketId
    selectedTeamId.value = teamId || ''
    showAssignModal.value = true
}

function closeAssignModal() {
    showAssignModal.value = false
    assignTicketId.value = null
    selectedTeamId.value = ''
}

async function assignFromModal() {
    console.log('Assigning from modal', assignTicketId.value, selectedTeamId.value)
    if (!assignTicketId.value) return
    await assignToTeam(assignTicketId.value, selectedTeamId.value || undefined)
    closeAssignModal()
}

async function loadTickets() {
    listError.value = ''
    listLoading.value = true
    try {
        const params: any = { page: page.value, pageSize: pageSize.value }
        if (companyId.value) params.companyId = companyId.value
        if (subjectFilter.value) params.subject = subjectFilter.value
        if (statusFilter.value) params.status = statusFilter.value
        if (teamFilter.value === '__unassigned') params.unassigned = true
        else if (teamFilter.value) params.teamId = teamFilter.value
        if (form.value.categoryId) params.categoryId = form.value.categoryId

        const data = await getTickets(params)
        if (Array.isArray(data)) {
            tickets.value = data.items
            total.value = null
        } else if (data && data.items) {
            tickets.value = data.items
            total.value = data.total ?? null
        } else {
            tickets.value = []
            total.value = null
        }
    } catch (ex: any) {
        listError.value = ex?.response?.data?.message ?? ex?.message ?? 'Failed to load tickets'
    } finally {
        listLoading.value = false
    }
}

onMounted(() => {
    loadTickets()
})

watch([page, pageSize], () => loadTickets())

</script>
