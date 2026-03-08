<template>
  <div class="bg-white rounded shadow p-4">
    <div class="mb-4">
      <label class="block text-sm text-gray-700">Create team</label>
      <div class="flex gap-2 mt-2">
        <input v-model="newTeamName" placeholder="Team name" class="px-3 py-2 border rounded w-full" />
        <button @click="onCreateTeam" class="px-3 py-2 bg-blue-600 text-white rounded">Create</button>
      </div>
      <p v-if="teamError" class="text-sm text-red-500 mt-1">{{ teamError }}</p>
    </div>

    <div class="overflow-x-auto">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Name</th>
            <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-100">
          <tr v-for="t in teams" :key="t.teamId" class="hover:bg-gray-50 even:bg-gray-50">
            <td class="px-4 py-3 text-sm text-gray-700 truncate">{{ t.name }}</td>
            <td class="px-4 py-3 text-sm text-gray-700">
              <button @click="onModifyTeam(t.teamId)" class="px-2 py-1 bg-yellow-400 text-white rounded text-sm mr-2">Modify</button>
              <button @click="onViewTeamMembers(t.teamId, t.name)" class="px-2 py-1 bg-blue-600 text-white rounded text-sm">View Members</button>
            </td>
          </tr>
          <tr v-if="teams.length === 0">
            <td colspan="2" class="px-4 py-6 text-center text-sm text-gray-500">No teams found.</td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-if="selectedTeamId" class="mt-4">
      <h3 class="text-lg font-medium text-gray-800">Members of {{ selectedTeamName }}</h3>
      <div v-if="loadingTeamMembers" class="text-sm text-gray-600">Loading members...</div>
      <div v-else>
        <div class="overflow-x-auto mt-2">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Email</th>
                <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Switch To</th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-100">
              <tr v-for="m in teamMembers" :key="m.userId" class="hover:bg-gray-50 even:bg-gray-50">
                <td class="px-4 py-3 text-sm text-gray-700 truncate">{{ m.email }}</td>
                <td class="px-4 py-3 text-sm text-gray-700">
                  <div class="flex gap-2 items-center">
                    <select v-model="switchTargets[m.userId]" class="px-2 py-1 border rounded">
                      <option v-for="opt in teams" :key="opt.teamId" :value="opt.teamId">{{ opt.name }}</option>
                    </select>
                    <button @click="onSwitchMember(m.userId)" class="px-2 py-1 bg-green-600 text-white rounded text-sm">Switch</button>
                  </div>
                </td>
              </tr>
              <tr v-if="teamMembers.length === 0">
                <td colspan="3" class="px-4 py-6 text-center text-sm text-gray-500">No members found for this team.</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { getCurrentCompany } from '../api/companies'
import { getTeamsByCompany, createTeam, getTeamMembers, switchMember } from '../api/teams'

const teams = ref<{ teamId: string; name: string }[]>([])
const newTeamName = ref('')
const teamError = ref('')
const selectedTeamId = ref<string | null>(null)
const selectedTeamName = ref('')    
const teamMembers = ref<{ userId: string; email: string; role: string }[]>([])
const loadingTeamMembers = ref(false)
const switchTargets = ref<Record<string, string>>({})

async function loadTeams() {
  try {
    const c = await getCurrentCompany()
    const companyId = c?.id
    if (!companyId) {
      teams.value = []
      return
    }
    const items = await getTeamsByCompany(companyId)
    teams.value = items
  } catch {
    teams.value = []
  }
}

async function onCreateTeam() {
  teamError.value = ''
  if (!newTeamName.value || newTeamName.value.trim() === '') {
    teamError.value = 'Name is required'
    return
  }
  try {
    await createTeam(newTeamName.value.trim())
    newTeamName.value = ''
    await loadTeams()
  } catch (err: unknown) {
    const axiosErr = err as any
    teamError.value = axiosErr?.response?.data?.message ?? (err instanceof Error ? err.message : 'Failed to create team')
  }
}

function onModifyTeam(_teamId: string) {
  alert('Modify team not implemented')
}

async function onViewTeamMembers(teamId: string, teamName: string) {
  selectedTeamId.value = teamId
  selectedTeamName.value = teamName
  loadingTeamMembers.value = true
  try {
    const paged = await getTeamMembers(teamId, 1, 100)
    teamMembers.value = paged.items
    const map: Record<string, string> = {}
    for (const t of teams.value) map[t.teamId] = t.teamId
    for (const m of teamMembers.value) switchTargets.value[m.userId] = map[selectedTeamId.value!] ?? (teams.value[0]?.teamId ?? '')
  } catch {
    teamMembers.value = []
  } finally {
    loadingTeamMembers.value = false
  }
}

async function onSwitchMember(userId: string) {
  const toTeamId = switchTargets.value[userId]
  if (!selectedTeamId.value || !toTeamId) return
  try {
    await switchMember(selectedTeamId.value, userId, toTeamId)
    await onViewTeamMembers(selectedTeamId.value, selectedTeamName.value)
  } catch (err: unknown) {
    const axiosErr = err as any
    alert(axiosErr?.response?.data?.message ?? (err instanceof Error ? err.message : 'Failed to switch member'))
  }
}

loadTeams()
</script>
