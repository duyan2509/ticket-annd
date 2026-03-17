<template>
  <div class="min-h-screen bg-gray-100">
    <main class="max-w-4xl mx-auto p-6">
      <div class="flex items-center justify-between mb-4">
        <h1 class="text-2xl font-semibold mb-4">Tickets</h1>
        <div class="flex items-center gap-2">
          <button
            @click="viewMode = 'list'"
            :class="viewMode === 'list' ? 'px-3 py-1 rounded bg-blue-600 text-white' : 'px-3 py-1 rounded bg-gray-200'"
          >
            Ticket List
          </button>
          <button
            @click="viewMode = 'create'"
            :class="viewMode === 'create' ? 'px-3 py-1 rounded bg-blue-600 text-white' : 'px-3 py-1 rounded bg-gray-200'"
          >
            Raise Ticket
          </button>
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

            <select v-if="userContext?.currentRole === AppRoles.CompanyAdmin" v-model="form.teamId" class="px-2 py-2 border rounded">
              <option value="">Assign to team (optional)</option>
              <option v-for="t in teams" :key="t.teamId" :value="t.teamId">{{ t.name }}</option>
            </select>

            <input v-model="form.subject" placeholder="Subject" class="px-2 py-2 border rounded" />
            <textarea v-model="form.body" placeholder="Body (optional)" class="px-2 py-2 border rounded h-28"></textarea>

            <div class="flex items-center gap-2">
              <button @click="onCreate" class="px-3 py-2 bg-blue-600 text-white rounded">Create</button>
              <div v-if="createState.loading" class="text-sm text-gray-600">Creating...</div>
              <div v-if="createState.success" class="text-sm text-green-600">Created successfully.</div>
              <div v-if="createState.error" class="text-sm text-red-600">{{ createState.error }}</div>
            </div>
          </div>
        </div>
      </div>

      <div v-if="viewMode === 'list'" class="mt-6 bg-white rounded shadow p-4 md:col-span-2">
        <div class="flex items-center justify-between mb-4">
          <h2 class="text-lg font-medium">Tickets list</h2>
          <div class="flex items-center gap-2">
            <input v-model="filters.subject" placeholder="Search subject" class="px-2 py-1 border rounded" />
            <select v-model="filters.teamId" class="px-2 py-1 border rounded">
              <option value="">All teams</option>
              <option value="__unassigned">Unassigned</option>
              <option v-for="t in teams" :key="t.teamId" :value="t.teamId">{{ t.name }}</option>
            </select>
            <select v-model="filters.status" class="px-2 py-1 border rounded">
              <option value="">All status</option>
              <option value="Open">Open</option>
              <option value="Resolved">Resolved</option>
              <option value="InProgress">In Progress</option>
              <option value="WaitingCustomer">Waiting Customer</option>
              <option value="WaitingThirdParty">Waiting Third Party</option>
            </select>
            <button @click="onSearch" class="px-3 py-1 bg-blue-600 text-white rounded">Search</button>
            <button @click="loadTickets" class="px-3 py-1 bg-gray-200 rounded">Refresh</button>
          </div>
        </div>

        <div v-if="listState.loading" class="text-sm text-gray-600">Loading tickets...</div>
        <div v-if="listState.error" class="text-sm text-red-600">{{ listState.error }}</div>

        <table v-if="!listState.loading && listState.tickets.length" class="w-full table-auto border-collapse">
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
            <tr v-for="t in listState.tickets" :key="t.id || t.ticketCode" class="border-t">
              <td class="py-2">{{ t.ticketCode }}</td>
              <td class="py-2 text-lg">{{ t.subject }}</td>
              <td class="py-2">
                <span class="px-2 py-1 bg-green-500 text-white rounded">{{ t.status }}</span>
              </td>
              <td class="py-2">{{ t.raiserName }}</td>
              <td class="py-2">{{ t.categoryName }}</td>
              <td v-if="t.teamName" class="py-2">{{ t.teamName }}</td>
              <td v-else class="py-2 text-gray-500">
                <button @click="openAssignModal(t.id, t.teamId)" class="px-2 py-1 bg-yellow-400 text-white rounded text-sm">
                  Assign to team
                </button>
              </td>
              <td class="py-2">
                <button @click="onViewTicket(t.ticketCode)" class="px-3 py-1 bg-blue-600 text-white rounded">View</button>
              </td>
            </tr>
          </tbody>
        </table>

        <div v-if="!listState.loading && !listState.tickets.length" class="text-sm text-gray-600">No tickets found.</div>

        <div class="mt-4 flex items-center justify-between">
          <div class="text-sm text-gray-600">Total: {{ pagination.total ?? listState.tickets.length }}</div>
          <div class="flex items-center gap-2">
            <button @click="prevPage" class="px-3 py-1 bg-gray-200 rounded">Prev</button>
            <div>Page {{ pagination.page }}</div>
            <button @click="nextPage" class="px-3 py-1 bg-gray-200 rounded">Next</button>
          </div>
        </div>
      </div>
    </main>

    <template v-if="assignModal.show">
      <div class="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50">
        <div class="bg-white rounded p-4 w-full max-w-md">
          <h3 class="text-lg font-medium mb-2">Assign team</h3>
          <select v-model="assignModal.teamId" class="w-full px-2 py-2 border rounded mb-3">
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
  </div>
</template>

<script setup lang="ts">
import { onMounted, reactive, ref, watch } from 'vue'
import { getCurrentCompany } from '~/api/companies'
import { getCategories } from '~/api/categories'
import { getActiveSlaRules } from '~/api/sla'
import { getTeamsByCompany } from '~/api/teams'
import { assignTeam, createTicket, getTickets } from '~/api/tickets'
import { useAuthStore } from '~/store/authStore'
import { AppRoles } from '~/types/appRoles'
import type { CategoryItem, CurrentCompany, TicketPageItem } from '~/types/auth'
import type { SlaRuleItem } from '~/api/sla'
import type { TeamItem } from '~/api/teams'

definePageMeta({
  middleware: 'auth',
})

const route = useRoute()
const companySlug = String(route.params.slug || '')

const company = ref<CurrentCompany | null>(null)
const companyId = ref<string | null>(null)
const categories = ref<CategoryItem[]>([])
const rules = ref<SlaRuleItem[]>([])
const teams = ref<TeamItem[]>([])
const viewMode = ref<'list' | 'create'>('list')
const userContext = useAuthStore().me

const form = reactive({
  categoryId: '',
  slaRuleId: '',
  teamId: '',
  subject: '',
  body: '',
})

const createState = reactive({
  loading: false,
  success: false,
  error: '',
})

const listState = reactive({
  loading: false,
  error: '',
  tickets: [] as TicketPageItem[],
})

const filters = reactive({
  subject: '',
  status: '',
  teamId: '',
})

const pagination = reactive({
  total: null as number | null,
  page: 1,
  pageSize: 10,
})

const assignModal = reactive({
  show: false,
  ticketId: null as string | null,
  teamId: '',
})

function goGeneral() {
  navigateTo(`/${companySlug}`)
}

function onViewTicket(ticketCode: string) {
  navigateTo(`/${companySlug}/tickets/${ticketCode}`)
}

async function loadLists() {
  try {
    let currentCompany: CurrentCompany | null = useAuthStore().getCurrentCompanyRef().value
    if (!currentCompany) {
      currentCompany = await getCurrentCompany()
    }

    company.value = currentCompany
    companyId.value = currentCompany?.id ?? null

    categories.value = await getCategories()
    rules.value = await getActiveSlaRules()
    teams.value = companyId.value ? await getTeamsByCompany(companyId.value) : []
  } catch {
    // ignore load errors, show empty state
  }
}

async function onCreate() {
  createState.error = ''
  createState.success = false
  if (!form.categoryId) {
    createState.error = 'Category required'
    return
  }
  if (!form.slaRuleId) {
    createState.error = 'SLA rule required'
    return
  }
  if (!form.subject || form.subject.trim() === '') {
    createState.error = 'Subject required'
    return
  }

  createState.loading = true
  try {
    await createTicket(form.categoryId, form.slaRuleId, form.subject.trim(), form.body || null, form.teamId || null)
    createState.success = true
    form.categoryId = ''
    form.slaRuleId = ''
    form.teamId = ''
    form.subject = ''
    form.body = ''
  } catch (ex: any) {
    createState.error = ex?.response?.data?.message ?? (ex?.message ?? 'Failed to create ticket')
  } finally {
    createState.loading = false
  }
}

async function assignToTeam(ticketId: string, teamId?: string) {
  listState.error = ''
  listState.loading = true
  try {
    const idToAssign = teamId || form.teamId
    if (!idToAssign) {
      listState.error = 'No team selected to assign'
      return
    }
    await assignTeam(ticketId, idToAssign)
    await loadTickets()
  } catch (ex: any) {
    listState.error = ex?.response?.data?.message ?? ex?.message ?? 'Failed to assign team'
  } finally {
    listState.loading = false
  }
}

function openAssignModal(ticketId: string, teamId?: string) {
  assignModal.ticketId = ticketId
  assignModal.teamId = teamId || ''
  assignModal.show = true
}

function closeAssignModal() {
  assignModal.show = false
  assignModal.ticketId = null
  assignModal.teamId = ''
}

async function assignFromModal() {
  if (!assignModal.ticketId) return
  await assignToTeam(assignModal.ticketId, assignModal.teamId || undefined)
  closeAssignModal()
}

function onSearch() {
  pagination.page = 1
  loadTickets()
}

function prevPage() {
  if (pagination.page > 1) {
    pagination.page -= 1
    loadTickets()
  }
}

function nextPage() {
  pagination.page += 1
  loadTickets()
}

async function loadTickets() {
  listState.error = ''
  listState.loading = true
  try {
    const params: Record<string, any> = { page: pagination.page, pageSize: pagination.pageSize }
    if (companyId.value) params.companyId = companyId.value
    if (filters.subject) params.subject = filters.subject
    if (filters.status) params.status = filters.status
    if (filters.teamId === '__unassigned') params.unassigned = true
    else if (filters.teamId) params.teamId = filters.teamId
    if (form.categoryId) params.categoryId = form.categoryId

    const data = await getTickets(params)
    if (data && Array.isArray(data.items)) {
      listState.tickets = data.items as TicketPageItem[]
      pagination.total = data.total ?? null
    } else {
      listState.tickets = []
      pagination.total = null
    }
  } catch (ex: any) {
    listState.error = ex?.response?.data?.message ?? ex?.message ?? 'Failed to load tickets'
  } finally {
    listState.loading = false
  }
}

onMounted(async () => {
  await loadLists()
  await loadTickets()
})

watch(
  () => pagination.pageSize,
  () => loadTickets()
)
</script>
