<template>
  <div class="min-h-screen bg-gray-100">
    <main class="max-w-6xl mx-auto p-6">
      <div class="mb-4">
        <button @click="goBack" class="px-3 py-1 bg-gray-200 rounded">Back to Tickets</button>
      </div>

      <div v-if="state.loading" class="text-center py-12">Loading ticket…</div>

      <div v-else-if="state.ticket" class="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div class="md:col-span-2 space-y-4">
          <div class="bg-white rounded shadow p-5">
            <div class="flex items-start justify-between gap-4">
              <div>
                <h1 class="text-2xl font-semibold">{{ state.ticket.subject }}</h1>
                <div class="mt-2 text-sm text-gray-600">
                  #{{ state.ticket.ticketCode }} · Raised by <strong>{{ state.ticket.raiserName }}</strong>
                </div>
                <div class="mt-3 flex flex-wrap gap-2 text-sm">
                  <span class="px-2 py-1 bg-gray-100 rounded">Status: <strong class="ml-1">{{ state.ticket.status }}</strong></span>
                  <span class="px-2 py-1 bg-gray-100 rounded">Category: {{ state.ticket.categoryName }}</span>
                  <span class="px-2 py-1 bg-gray-100 rounded">Team: {{ state.ticket.teamName ?? 'Unassigned' }}</span>
                  <span class="px-2 py-1 bg-gray-100 rounded">SLA: {{ state.ticket.slaRuleName ?? '-' }}</span>
                </div>
              </div>

              <div class="flex flex-col items-end gap-2">
                <div class="text-sm text-gray-500">Created: {{ state.ticket.createdAt ?? '-' }}</div>
                <div class="flex flex-wrap justify-end gap-2">
                  <button
                    v-if="isAssignee && !state.ticket.firstResponseAt"
                    @click="startWorking"
                    :disabled="actions.starting"
                    class="px-3 py-1 bg-purple-600 text-white rounded"
                  >
                    Start Work
                  </button>
                  <button @click="openAssign" class="px-3 py-1 bg-blue-600 text-white rounded">Assign Team</button>
                  <button @click="openAssignMember" :disabled="!state.ticket.teamId" class="px-3 py-1 bg-indigo-600 text-white rounded disabled:opacity-50">
                    Assign Member
                  </button>
                  <button
                    v-if="state.ticket.status === 'WaitingCustomer' || state.ticket.status === 'WaitingThirdParty'"
                    @click="continueAction"
                    :disabled="actions.continuing"
                    class="px-3 py-1 bg-blue-500 text-white rounded"
                  >
                    Continue
                  </button>
                  <button v-else @click="modals.showPause = true" class="px-3 py-1 bg-yellow-500 text-white rounded">Pause</button>
                  <button @click="modals.showResolve = true" class="px-3 py-1 bg-green-600 text-white rounded">Resolve</button>
                </div>
              </div>
            </div>

            <Modal v-if="modals.showAssign" title="Assign Team" @close="modals.showAssign = false">
              <div class="space-y-3 modal-body">
                <label class="block text-sm">Select team</label>
                <div>
                  <select v-model="assign.teamId" class="px-3 py-2 border rounded w-full">
                    <option value="">-- choose team --</option>
                    <option v-for="t in lists.teams" :key="t.teamId" :value="t.teamId">{{ t.name }}</option>
                  </select>
                  <div v-if="lists.loadingTeams" class="text-sm text-gray-500 mt-2">Loading teams…</div>
                </div>
                <p v-if="assign.error" class="text-sm text-red-500">{{ assign.error }}</p>
              </div>
              <template #footer>
                <div class="flex justify-end gap-2">
                  <button @click="modals.showAssign = false" class="px-3 py-2 bg-gray-200 rounded">Cancel</button>
                  <button @click="assignTeamAction" :disabled="assign.loading" class="px-4 py-2 bg-green-600 text-white rounded">
                    <span v-if="assign.loading">Assigning…</span>
                    <span v-else>Assign</span>
                  </button>
                </div>
              </template>
            </Modal>

            <Modal v-if="modals.showPause" title="Pause Ticket" @close="modals.showPause = false">
              <div class="space-y-3 modal-body">
                <label class="block text-sm">Type</label>
                <select v-model="pause.type" class="px-3 py-2 border rounded w-full">
                  <option :value="TicketPauseType.WaitingCustomer">WaitingCustomer</option>
                  <option :value="TicketPauseType.WaitingThirdParty">WaitingThirdParty</option>
                </select>
                <label class="block text-sm">Reason</label>
                <textarea v-model="pause.reason" class="px-3 py-2 border rounded w-full" rows="3"></textarea>
                <p v-if="pause.error" class="text-sm text-red-500">{{ pause.error }}</p>
              </div>
              <template #footer>
                <div class="flex justify-end gap-2">
                  <button @click="modals.showPause = false" class="px-3 py-2 bg-gray-200 rounded">Cancel</button>
                  <button @click="pauseAction" :disabled="pause.loading" class="px-4 py-2 bg-yellow-500 text-white rounded">
                    <span v-if="pause.loading">Pausing…</span>
                    <span v-else>Pause</span>
                  </button>
                </div>
              </template>
            </Modal>

            <Modal v-if="modals.showResolve" title="Resolve Ticket" @close="modals.showResolve = false">
              <div class="space-y-3 modal-body">
                <label class="block text-sm">Resolution note</label>
                <textarea v-model="resolve.note" class="px-3 py-2 border rounded w-full" rows="4"></textarea>
                <p v-if="resolve.error" class="text-sm text-red-500">{{ resolve.error }}</p>
              </div>
              <template #footer>
                <div class="flex justify-end gap-2">
                  <button @click="modals.showResolve = false" class="px-3 py-2 bg-gray-200 rounded">Cancel</button>
                  <button @click="resolveAction" :disabled="resolve.loading" class="px-4 py-2 bg-green-600 text-white rounded">
                    <span v-if="resolve.loading">Resolving…</span>
                    <span v-else>Resolve</span>
                  </button>
                </div>
              </template>
            </Modal>

            <Modal v-if="modals.showAssignMember" title="Assign Member" @close="modals.showAssignMember = false">
              <div class="space-y-3 modal-body">
                <label class="block text-sm">Team</label>
                <select v-model="members.selectedTeamId" class="px-3 py-2 border rounded w-full" disabled>
                  <option :value="state.ticket.teamId">{{ state.ticket.teamName }}</option>
                </select>

                <label class="block text-sm mt-2">Member</label>
                <div>
                  <select v-model="members.assigneeId" class="px-3 py-2 border rounded w-full">
                    <option value="">-- choose member --</option>
                    <option v-for="m in members.list" :key="m.userId" :value="m.userId">{{ m.email }}</option>
                  </select>
                  <div v-if="members.loading" class="text-sm text-gray-500 mt-2">Loading members…</div>
                </div>

                <p v-if="members.error" class="text-sm text-red-500">{{ members.error }}</p>
              </div>
              <template #footer>
                <div class="flex justify-end gap-2">
                  <button @click="modals.showAssignMember = false" class="px-3 py-2 bg-gray-200 rounded">Cancel</button>
                  <button @click="assignMemberAction" :disabled="members.assigning" class="px-4 py-2 bg-blue-600 text-white rounded">
                    <span v-if="members.assigning">Assigning…</span>
                    <span v-else>Assign</span>
                  </button>
                </div>
              </template>
            </Modal>

            <div class="mt-6 prose max-w-none text-gray-800" v-html="state.ticket.body"></div>
          </div>
        </div>

        <aside class="space-y-4">
          <div class="bg-white rounded shadow p-4">
            <h4 class="text-sm font-semibold text-gray-700">SLA</h4>
            <div class="mt-2 text-sm text-gray-600">
              <div>First response: <strong>{{ state.ticket.slaFirstResponseMinutes ?? '-' }}m</strong></div>
              <div>Resolution: <strong>{{ state.ticket.slaResolutionMinutes ?? '-' }}m</strong></div>
              <div class="mt-2 text-sm">Response breached: <strong>{{ state.ticket.isResponseBreached ? 'Yes' : 'No' }}</strong></div>
              <div class="text-sm">Resolution breached: <strong>{{ state.ticket.isResolutionBreached ? 'Yes' : 'No' }}</strong></div>
            </div>
          </div>

          <div class="bg-white rounded shadow p-4">
            <h4 class="text-sm font-semibold text-gray-700">Meta</h4>
            <div class="mt-2 text-sm text-gray-600 space-y-1">
              <div><strong>Priority:</strong> {{ state.ticket.priority ?? 'Normal' }}</div>
              <div><strong>Assignee:</strong> {{ state.ticket.assigneeName ?? 'Unassigned' }}</div>
              <div><strong>Category:</strong> {{ state.ticket.categoryName }}</div>
            </div>
          </div>

          <div class="bg-white rounded shadow p-4">
            <h3 class="text-lg font-semibold mb-3">Activity / Logs</h3>
            <div v-if="!state.ticket.logs || state.ticket.logs.length === 0" class="text-sm text-gray-500">No logs available.</div>
            <ul v-else class="space-y-2">
              <li v-for="log in state.ticket.logs" :key="log.id || log.timestamp || log.createdAt" class="flex items-start gap-3">
                <div class="text-xs text-gray-500 w-36">
                  <div class="text-sm font-medium text-gray-700">
                    <span v-if="(log.fromStatus || log.toStatus) && log.fromStatus !== log.toStatus">{{ log.fromStatus }} → {{ log.toStatus }}</span>
                    <span v-else>{{ log.toStatus ?? log.fromStatus ?? state.ticket.status }}</span>
                  </div>
                  <div class="mt-1 text-xs text-gray-500">{{ formatTimestamp(log.timestamp || log.createdAt || log.occurredAt) }}</div>
                </div>
                <div class="flex-1 text-sm text-gray-700">
                  <div class="font-medium text-gray-800">{{ log.action }}</div>
                  <div class="text-sm text-gray-700 mt-1">{{ log.note ?? log.message ?? '' }}</div>
                  <div v-if="log.actorId" class="text-xs text-gray-500 mt-1">Actor: {{ log.actorName ?? log.actorId }}</div>
                </div>
              </li>
            </ul>
          </div>
        </aside>
      </div>

      <div v-else class="text-sm text-gray-600">Ticket not found.</div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, reactive } from 'vue'
import { assignMemberToTicket, assignTeam, continueTicket, getTicketByCode, getTicketLogs, pauseTicket, resolveTicket, startTicket } from '~/api/tickets'
import { getCurrentCompany } from '~/api/companies'
import { getTeamMembers, getTeamsByCompany } from '~/api/teams'
import Modal from '~/components/Modal.vue'
import { useAuthStore } from '~/store/authStore'
import { TicketPauseType } from '~/types/pause'
import type { CurrentCompany, MemberItem } from '~/types/auth'
import type { TeamItem } from '~/api/teams'

definePageMeta({
  middleware: 'auth',
})

const route = useRoute()

const state = reactive({
  ticket: null as any | null,
  loading: false,
})

const lists = reactive({
  teams: [] as TeamItem[],
  loadingTeams: false,
})

const actions = reactive({
  continuing: false,
  starting: false,
})

const modals = reactive({
  showAssign: false,
  showPause: false,
  showResolve: false,
  showAssignMember: false,
})

const assign = reactive({
  teamId: '',
  loading: false,
  error: '',
})

const pause = reactive({
  type: TicketPauseType.WaitingCustomer as TicketPauseType,
  reason: '',
  loading: false,
  error: '',
})

const resolve = reactive({
  note: '',
  loading: false,
  error: '',
})

const members = reactive({
  selectedTeamId: null as string | null,
  list: [] as MemberItem[],
  assigneeId: '',
  loading: false,
  assigning: false,
  error: '',
})

const isAssignee = computed(() => {
  const me = useAuthStore().me
  if (!me || !state.ticket?.assigneeId) return false
  const meId = String(me.id || '').toLowerCase()
  const assigneeId = String(state.ticket.assigneeId || '').toLowerCase()
  return meId !== '' && assigneeId !== '' && meId === assigneeId
})

function goBack() {
  const slug = String(route.params.slug || '')
  navigateTo(`/${slug}/tickets`)
}

async function loadTeams() {
  lists.loadingTeams = true
  try {
    let currentCompany: CurrentCompany | null = useAuthStore().getCurrentCompanyRef().value
    if (!currentCompany) {
      currentCompany = await getCurrentCompany()
    }
    lists.teams = currentCompany?.id ? await getTeamsByCompany(currentCompany.id) : []
  } catch {
    lists.teams = []
  } finally {
    lists.loadingTeams = false
  }
}

async function loadTicket() {
  const ticketCode = String(route.params.ticketCode || '')
  if (!ticketCode) return

  state.loading = true
  try {
    state.ticket = await getTicketByCode(ticketCode)
    try {
      const logs = await getTicketLogs(state.ticket.id)
      state.ticket.logs = logs?.items || logs || []
    } catch {
      state.ticket.logs = state.ticket.logs || []
    }
  } catch {
    state.ticket = null
  } finally {
    state.loading = false
  }
}

function openAssign() {
  assign.teamId = state.ticket?.teamId ?? ''
  assign.error = ''
  modals.showAssign = true
}

async function assignTeamAction() {
  if (!state.ticket || !assign.teamId) return
  assign.loading = true
  assign.error = ''
  try {
    await assignTeam(state.ticket.id, assign.teamId)
    await loadTicket()
    modals.showAssign = false
    assign.teamId = ''
  } catch (err: any) {
    assign.error = err?.response?.data?.message ?? err?.message ?? 'Failed to assign team'
  } finally {
    assign.loading = false
  }
}

function openAssignMember() {
  const teamId = state.ticket?.teamId
  if (!teamId) return
  members.selectedTeamId = teamId
  members.error = ''
  members.assigneeId = ''
  modals.showAssignMember = true
  loadMembersForTeam(teamId)
}

async function loadMembersForTeam(teamId: string) {
  if (!teamId) {
    members.list = []
    return
  }
  members.loading = true
  try {
    const res = await getTeamMembers(teamId)
    members.list = (res.items || []) as MemberItem[]
  } catch {
    members.list = []
  } finally {
    members.loading = false
  }
}

async function pauseAction() {
  if (!state.ticket) return
  pause.loading = true
  pause.error = ''
  try {
    await pauseTicket(state.ticket.id, pause.type, pause.reason)
    await loadTicket()
    modals.showPause = false
    pause.reason = ''
  } catch (err: any) {
    pause.error = err?.response?.data?.message ?? err?.message ?? 'Failed to pause ticket'
  } finally {
    pause.loading = false
  }
}

async function continueAction() {
  if (!state.ticket) return
  actions.continuing = true
  try {
    await continueTicket(state.ticket.id)
    await loadTicket()
  } finally {
    actions.continuing = false
  }
}

async function resolveAction() {
  if (!state.ticket) return
  resolve.loading = true
  resolve.error = ''
  try {
    await resolveTicket(state.ticket.id, resolve.note || undefined)
    await loadTicket()
    modals.showResolve = false
    resolve.note = ''
  } catch (err: any) {
    resolve.error = err?.response?.data?.message ?? err?.message ?? 'Failed to resolve ticket'
  } finally {
    resolve.loading = false
  }
}

async function startWorking() {
  if (!state.ticket) return
  actions.starting = true
  try {
    await startTicket(state.ticket.id)
    await loadTicket()
  } finally {
    actions.starting = false
  }
}

async function assignMemberAction() {
  if (!state.ticket || !members.assigneeId) return
  members.assigning = true
  members.error = ''
  try {
    await assignMemberToTicket(state.ticket.id, members.assigneeId)
    await loadTicket()
    modals.showAssignMember = false
    members.assigneeId = ''
  } catch (err: any) {
    members.error = err?.response?.data?.message ?? err?.message ?? 'Failed to assign member'
  } finally {
    members.assigning = false
  }
}

function formatTimestamp(ts: string | undefined | null) {
  if (!ts) return '-'
  try {
    return new Date(ts).toLocaleString()
  } catch {
    return String(ts)
  }
}

onMounted(async () => {
  await Promise.all([loadTicket(), loadTeams()])
})
</script>
