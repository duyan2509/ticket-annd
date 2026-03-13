<template>
  <AppHeader />
  <div class="max-w-6xl mx-auto p-6">
    <div v-if="loading" class="text-center py-12">Loading ticket…</div>

    <div v-else-if="ticket" class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <!-- Main content -->
      <div class="md:col-span-2 space-y-4">
        <div class="bg-white rounded shadow p-5">
          <div class="flex items-start justify-between gap-4">
            <div>
              <h1 class="text-2xl font-semibold">{{ ticket.subject }}</h1>
              <div class="mt-2 text-sm text-gray-600">#{{ ticket.ticketCode }} · Raised by <strong>{{ ticket.raiserName }}</strong></div>
              <div class="mt-3 flex flex-wrap gap-2 text-sm">
                <span class="px-2 py-1 bg-gray-100 rounded">Status: <strong class="ml-1">{{ ticket.status }}</strong></span>
                <span class="px-2 py-1 bg-gray-100 rounded">Category: {{ ticket.categoryName }}</span>
                <span class="px-2 py-1 bg-gray-100 rounded">Team: {{ ticket.teamName ?? 'Unassigned' }}</span>
                <span class="px-2 py-1 bg-gray-100 rounded">SLA: {{ ticket.slaRuleName ?? '-' }}</span>
              </div>
            </div>

            <div class="flex flex-col items-end gap-2">
              <div class="text-sm text-gray-500">Created: {{ ticket.createdAt ?? '-' }}</div>
              <div class="flex gap-2">
                <button @click="openAssign" class="px-3 py-1 bg-blue-600 text-white rounded">Assign Team</button>
                <button @click="openAssignMember" :disabled="!ticket.teamId" class="px-3 py-1 bg-indigo-600 text-white rounded disabled:opacity-50">Assign Member</button>
                <button v-if="ticket.status === 'WaitingCustomer' || ticket.status === 'WaitingThirdParty'" @click="continueAction" :disabled="continuing" class="px-3 py-1 bg-blue-500 text-white rounded">Continue</button>
                <button v-else @click="showPause = true" class="px-3 py-1 bg-yellow-500 text-white rounded">Pause</button>
                <button @click="showResolve = true" class="px-3 py-1 bg-green-600 text-white rounded">Resolve</button>
              </div>
            </div>
          </div>
          
          <!-- Modals -->
          <Modal v-if="showAssign" title="Assign Team" @close="showAssign = false">
            <div class="space-y-3 modal-body">
              <label class="block text-sm">Select team</label>
              <div>
                <select v-model="assignTeamId" class="px-3 py-2 border rounded w-full">
                  <option value="">-- choose team --</option>
                  <option v-for="t in teams" :key="t.teamId" :value="t.teamId">{{ t.name }}</option>
                </select>
                <div v-if="loadingTeams" class="text-sm text-gray-500 mt-2">Loading teams…</div>
              </div>
              <p v-if="assignError" class="text-sm text-red-500">{{ assignError }}</p>
            </div>
            <template #footer>
              <div class="flex justify-end gap-2">
                <button @click="showAssign = false" class="px-3 py-2 bg-gray-200 rounded">Cancel</button>
                <button @click="assignTeamAction" :disabled="assigning" class="px-4 py-2 bg-green-600 text-white rounded">
                  <span v-if="assigning">Assigning…</span>
                  <span v-else>Assign</span>
                </button>
              </div>
            </template>
          </Modal>

          <Modal v-if="showPause" title="Pause Ticket" @close="showPause = false">
            <div class="space-y-3 modal-body">
                  <label class="block text-sm">Type</label>
                  <select v-model="pauseType" class="px-3 py-2 border rounded w-full">
                    <option :value="TicketPauseType.WaitingCustomer">WaitingCustomer</option>
                    <option :value="TicketPauseType.WaitingThirdParty">WaitingThirdParty</option>
                  </select>
              <label class="block text-sm">Reason</label>
              <textarea v-model="pauseReason" class="px-3 py-2 border rounded w-full" rows="3"></textarea>
              <!-- resumeAt removed -->
              <p v-if="pauseError" class="text-sm text-red-500">{{ pauseError }}</p>
            </div>
            <template #footer>
              <div class="flex justify-end gap-2">
                <button @click="showPause = false" class="px-3 py-2 bg-gray-200 rounded">Cancel</button>
                <button @click="pauseAction" :disabled="pausing" class="px-4 py-2 bg-yellow-500 text-white rounded">
                  <span v-if="pausing">Pausing…</span>
                  <span v-else>Pause</span>
                </button>
              </div>
            </template>
          </Modal>

          <Modal v-if="showResolve" title="Resolve Ticket" @close="showResolve = false">
            <div class="space-y-3 modal-body">
              <label class="block text-sm">Resolution note</label>
              <textarea v-model="resolveNote" class="px-3 py-2 border rounded w-full" rows="4"></textarea>
              <p v-if="resolveError" class="text-sm text-red-500">{{ resolveError }}</p>
            </div>
            <template #footer>
              <div class="flex justify-end gap-2">
                <button @click="showResolve = false" class="px-3 py-2 bg-gray-200 rounded">Cancel</button>
                <button @click="resolveAction" :disabled="resolving" class="px-4 py-2 bg-green-600 text-white rounded">
                  <span v-if="resolving">Resolving…</span>
                  <span v-else>Resolve</span>
                </button>
              </div>
            </template>
          </Modal>

          <Modal v-if="showAssignMember" title="Assign Member" @close="showAssignMember = false">
            <div class="space-y-3 modal-body">
              <label class="block text-sm">Team</label>
              <select v-model="selectedMemberTeam" class="px-3 py-2 border rounded w-full" :disabled="true">
                <option :value="ticket.teamId">{{ ticket.teamName }}</option>
              </select>

              <label class="block text-sm mt-2">Member</label>
              <div>
                <select v-model="assignMemberId" class="px-3 py-2 border rounded w-full">
                  <option value="">-- choose member --</option>
                  <option v-for="m in teamMembers" :key="m.userId || m.id" :value="m.userId ?? m.id">{{ m.name || m.displayName || m.email }}</option>
                </select>
                <div v-if="loadingMembers" class="text-sm text-gray-500 mt-2">Loading members…</div>
              </div>

              <p v-if="assignMemberError" class="text-sm text-red-500">{{ assignMemberError }}</p>
            </div>
            <template #footer>
              <div class="flex justify-end gap-2">
                <button @click="showAssignMember = false" class="px-3 py-2 bg-gray-200 rounded">Cancel</button>
                <button @click="assignMemberAction" :disabled="assigningMember" class="px-4 py-2 bg-blue-600 text-white rounded">
                  <span v-if="assigningMember">Assigning…</span>
                  <span v-else>Assign</span>
                </button>
              </div>
            </template>
          </Modal>

          <div class="mt-6 prose max-w-none text-gray-800" v-html="ticket.body"></div>
        </div>

          <!-- Logs moved to sidebar -->

        <div class="bg-white rounded shadow p-4">
          <h3 class="text-lg font-semibold mb-3">Comments</h3>
          <div v-if="!ticket.comments || ticket.comments.length === 0" class="text-sm text-gray-500">No comments yet.</div>
          <div class="space-y-3">
            <div v-for="c in ticket.comments || []" :key="c.id" class="p-3 bg-gray-50 rounded">
              <div class="text-sm text-gray-800">{{ c.body }}</div>
              <div class="mt-2 text-xs text-gray-500">By {{ c.authorName }} · {{ c.createdAt }}</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Sidebar -->
      <aside class="space-y-4">
        <div class="bg-white rounded shadow p-4">
          <h4 class="text-sm font-semibold text-gray-700">SLA</h4>
          <div class="mt-2 text-sm text-gray-600">
            <div>First response: <strong>{{ ticket.slaFirstResponseMinutes ?? '-' }}m</strong></div>
            <div>Resolution: <strong>{{ ticket.slaResolutionMinutes ?? '-' }}m</strong></div>
            <div class="mt-2 text-sm">Response breached: <strong>{{ ticket.isResponseBreached ? 'Yes' : 'No' }}</strong></div>
            <div class="text-sm">Resolution breached: <strong>{{ ticket.isResolutionBreached ? 'Yes' : 'No' }}</strong></div>
          </div>
        </div>

        <div class="bg-white rounded shadow p-4">
          <h4 class="text-sm font-semibold text-gray-700">Meta</h4>
          <div class="mt-2 text-sm text-gray-600 space-y-1">
            <div><strong>Priority:</strong> {{ ticket.priority ?? 'Normal' }}</div>
            <div><strong>Assignee:</strong> {{ ticket.assigneeName ?? 'Unassigned' }}</div>
            <div><strong>Category:</strong> {{ ticket.categoryName }}</div>
          </div>
        </div>

        <div class="bg-white rounded shadow p-4">
          <h3 class="text-lg font-semibold mb-3">Activity / Logs</h3>
          <div v-if="!ticket.logs || ticket.logs.length === 0" class="text-sm text-gray-500">No logs available.</div>
          <ul v-else class="space-y-2">
            <li v-for="log in ticket.logs" :key="log.id" class="flex items-start gap-3">
              <div class="text-xs text-gray-500 w-36">
                <div class="text-sm font-medium text-gray-700">
                  <span v-if="(log.fromStatus || log.toStatus) && log.fromStatus !== log.toStatus">{{ log.fromStatus }} → {{ log.toStatus }}</span>
                  <span v-else>{{ log.toStatus ?? log.fromStatus ?? ticket.status }}</span>
                </div>
                <div class="mt-1 text-xs text-gray-500">{{ formatTimestamp(log.timestamp || log.createdAt || log.occurredAt) }}</div>
              </div>
              <div class="flex-1 text-sm text-gray-700">
                <div class="font-medium text-gray-800">{{ log.action }}</div>
                <div class="text-sm text-gray-700 mt-1">{{ log.note ?? log.message ?? '' }}</div>
                <div v-if="log.actorId" class="text-xs text-gray-500 mt-1">Actor: {{ log.actorName??log.actorId }}</div>
              </div>
            </li>
          </ul>
        </div>
      </aside>
    </div>

    <div v-else class="text-sm text-gray-600">Ticket not found.</div>
  </div>
</template>

<script setup lang="ts">
import AppHeader from '../components/AppHeader.vue'
import { ref, onMounted } from 'vue'
import { TicketPauseType } from '../types/pause'
import { useRoute } from 'vue-router'
import { getTicketByCode, assignTeam, getTicketLogs, pauseTicket, continueTicket, resolveTicket, assignMemberToTicket } from '../api/tickets'
import { getTeamsByCompany, getTeamMembers } from '../api/teams'
import Modal from '../components/Modal.vue'

const route = useRoute()
const ticket = ref<any | null>(null)
const loading = ref(false)

const showAssign = ref(false)
const assignTeamId = ref('')
const teams = ref<Array<{ teamId: string; name: string }>>([])
const loadingTeams = ref(false)
const assigning = ref(false)
const assignError = ref('')

// logs
const loadingLogs = ref(false)

// pause
const showPause = ref(false)
const pauseType = ref<TicketPauseType>(TicketPauseType.WaitingCustomer)
const pauseReason = ref('')
const pausing = ref(false)
const pauseError = ref('')

// continue
const continuing = ref(false)

// resolve
const showResolve = ref(false)
const resolveNote = ref('')
const resolving = ref(false)
const resolveError = ref('')

// assign member
const showAssignMember = ref(false)
const assignMemberId = ref('')
const teamMembers = ref<Array<any>>([])
const loadingMembers = ref(false)
const selectedMemberTeam = ref<string | null>(null)
const assigningMember = ref(false)
const assignMemberError = ref('')

async function load() {
  const ticketCode = String(route.params.ticketCode || '')
  if (!ticketCode) return
  loading.value = true
  try {
    ticket.value = await getTicketByCode(ticketCode)
    // load logs separately (merge into ticket)
    loadingLogs.value = true
    try {
      const logs = await getTicketLogs(ticket.value.id)
      ticket.value.logs = logs.items || logs
    } catch {
      ticket.value.logs = ticket.value.logs || []
    } finally {
      loadingLogs.value = false
    }
    // load teams for select
    loadingTeams.value = true
    try {
      teams.value = await getTeamsByCompany('')
    } catch {
      teams.value = []
    } finally {
      loadingTeams.value = false
    }
  } catch (err) {
    ticket.value = null
  } finally {
    loading.value = false
  }
}

async function assignTeamAction() {
  if (!assignTeamId.value || !ticket.value) return
  assigning.value = true
  assignError.value = ''
  try {
    await assignTeam(ticket.value.id, assignTeamId.value)
    // reload ticket
    await load()
    showAssign.value = false
    assignTeamId.value = ''
  } catch (err: any) {
    assignError.value = err?.response?.data?.message ?? err?.message ?? 'Failed to assign team'
  } finally {
    assigning.value = false
  }
}

function openAssign() {
  assignTeamId.value = ticket.value?.teamId ?? ''
  showAssign.value = true
}

function openAssignMember() {
  // only enable when ticket has team
  const teamId = ticket.value?.teamId
  if (!teamId) return
  selectedMemberTeam.value = teamId
  showAssignMember.value = true
  loadMembersForTeam(teamId)
}

async function loadMembersForTeam(teamId: string) {
  if (!teamId) {
    teamMembers.value = []
    return
  }
  loadingMembers.value = true
  try {
    const res = await getTeamMembers(teamId)
    teamMembers.value = (res.items || res) as any[]
  } catch {
    teamMembers.value = []
  } finally {
    loadingMembers.value = false
  }
}

async function pauseAction() {
  if (!ticket.value) return
  pausing.value = true
  pauseError.value = ''
    try {
    await pauseTicket(ticket.value.id, pauseType.value, pauseReason.value)
    await load()
    showPause.value = false
    pauseReason.value = ''
  } catch (err: any) {
    pauseError.value = err?.response?.data?.message ?? err?.message ?? 'Failed to pause ticket'
  } finally {
    pausing.value = false
  }
}

async function continueAction() {
  if (!ticket.value) return
  continuing.value = true
  try {
    await continueTicket(ticket.value.id)
    await load()
  } catch (err: any) {
    console.error('Continue failed', err)
  } finally {
    continuing.value = false
  }
}

async function resolveAction() {
  if (!ticket.value) return
  resolving.value = true
  resolveError.value = ''
  try {
    await resolveTicket(ticket.value.id, resolveNote.value || undefined)
    await load()
    showResolve.value = false
    resolveNote.value = ''
  } catch (err: any) {
    resolveError.value = err?.response?.data?.message ?? err?.message ?? 'Failed to resolve ticket'
  } finally {
    resolving.value = false
  }
}

async function assignMemberAction() {
  if (!ticket.value || !assignMemberId.value) return
  assigningMember.value = true
  assignMemberError.value = ''
  try {
    await assignMemberToTicket(ticket.value.id, assignMemberId.value)
    await load()
    showAssignMember.value = false
    assignMemberId.value = ''
  } catch (err: any) {
    assignMemberError.value = err?.response?.data?.message ?? err?.message ?? 'Failed to assign member'
  } finally {
    assigningMember.value = false
  }
}

onMounted(load)

function formatTimestamp(ts: string | undefined | null) {
  if (!ts) return '-'
  try {
    return new Date(ts).toLocaleString()
  } catch {
    return String(ts)
  }
}
</script>
