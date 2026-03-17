<template>
  <div class="mt-6 bg-white rounded shadow p-4">
    <div v-if="loading" class="text-sm text-gray-600">Loading...</div>
    <div v-else>
      <div class="mb-4 flex items-center justify-between">
        <h2 class="text-lg font-semibold pb-2">SLA Policies</h2>
        <div class="mb-4 flex flex-col items-center gap-2">
          <div>
            <input v-model="newPolicyName" placeholder="New policy name" class="mr-2 px-2 py-1 border rounded text-sm" />
            <button @click="addPolicy" :disabled="creatingPolicy || !newPolicyName.trim()"
              class="px-3 py-1 bg-green-600 text-white rounded text-sm">
              <span v-if="creatingPolicy">Creating...</span>
              <span v-else>Add Policy</span>
            </button>
          </div>
          <p v-if="createPolicyError" class="text-sm text-red-500 ml-3">{{ createPolicyError }}</p>
        </div>
      </div>

      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Name</th>
              <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Active</th>
              <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-100">
            <tr v-for="p in policies" :key="p.id" class="hover:bg-gray-50 even:bg-gray-50">
              <td class="px-4 py-3 text-sm text-gray-700 truncate">
                <template v-if="editingPolicyId === p.id">
                  <input v-model="editingPolicyName" class="px-2 py-1 border rounded w-full text-sm" />
                  <p v-if="editingPolicyError[p.id]" class="text-sm text-red-500 mt-1">{{ editingPolicyError[p.id] }}</p>
                </template>
                <template v-else>{{ p.name }}</template>
              </td>
              <td class="px-4 py-3 text-sm text-gray-700">{{ p.isActive ? 'Yes' : 'No' }}</td>
              <td class="px-4 py-3 text-sm text-gray-700">
                <template v-if="editingPolicyId !== p.id">
                  <button @click="viewRules(p)" class="px-2 py-1 bg-blue-600 text-white rounded text-sm mr-2">View Rules</button>
                  <button @click="startEditPolicy(p)" class="px-2 py-1 bg-yellow-400 text-white rounded text-sm">Edit</button>
                </template>
                <template v-else>
                  <button @click="savePolicy(p)" class="px-2 py-1 bg-green-600 text-white rounded text-sm mr-2">Save</button>
                  <button @click="cancelEditPolicy()" class="px-2 py-1 bg-gray-200 rounded text-sm">Cancel</button>
                </template>
              </td>
            </tr>
            <tr v-if="policies.length === 0">
              <td colspan="3" class="px-4 py-6 text-center text-sm text-gray-500">No SLA policies found.</td>
            </tr>
          </tbody>
        </table>
      </div>

      <div class="mt-4 flex items-center gap-3">
        <div class="flex items-center gap-2">
          <label class="text-sm text-gray-700">Activate policy:</label>
          <select v-model="activatePolicyId" class="px-2 py-1 border rounded text-sm">
            <option :value="null">-- Select policy --</option>
            <option v-for="p in policies" :key="p.id" :value="p.id">{{ p.name }}</option>
          </select>
          <button @click="activateSelectedPolicy" :disabled="!activatePolicyId || activating"
            class="px-3 py-1 bg-blue-600 text-white rounded text-sm">
            <span v-if="activating">Activating...</span>
            <span v-else>Activate</span>
          </button>
          <p v-if="activateError" class="text-sm text-red-500 ml-3">{{ activateError }}</p>
        </div>

        <button @click="prevPage" :disabled="page <= 1" class="px-3 py-1 bg-gray-200 rounded disabled:opacity-50">Prev</button>
        <span class="text-sm text-gray-600">Page {{ page }} / {{ Math.max(1, Math.ceil(total / size)) }}</span>
        <button @click="nextPage" :disabled="page >= Math.ceil(total / size)" class="px-3 py-1 bg-gray-200 rounded disabled:opacity-50">Next</button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getSlaPolicies, updateSlaPolicy, activateSlaPolicy, createSlaPolicy } from '../api/sla'
import type { SlaPolicyItem } from '../api/sla'

const emit = defineEmits<{
  (e: 'view-rules', p: SlaPolicyItem): void
}>()

const page = ref(1)
const size = ref(10)
const total = ref(0)
const loading = ref(false)
const policies = ref<SlaPolicyItem[]>([])

const editingPolicyId = ref<string | null>(null)
const editingPolicyName = ref('')
const editingPolicyError = ref<Record<string, string>>({})

const activatePolicyId = ref<string | null>(null)
const activating = ref(false)
const activateError = ref('')

const newPolicyName = ref('')
const creatingPolicy = ref(false)
const createPolicyError = ref('')

async function loadPolicies() {
  loading.value = true
  try {
    const res = await getSlaPolicies(page.value, size.value)
    policies.value = res.items
    total.value = res.total
  } catch (err) {
    policies.value = []
    total.value = 0
  } finally {
    loading.value = false
  }
}

function startEditPolicy(p: SlaPolicyItem) {
  editingPolicyId.value = p.id
  editingPolicyName.value = p.name
  editingPolicyError.value[p.id] = ''
}

function cancelEditPolicy() {
  editingPolicyId.value = null
  editingPolicyName.value = ''
}

async function savePolicy(p: SlaPolicyItem) {
  if (!editingPolicyName.value || editingPolicyName.value.trim() === '') return
  try {
    await updateSlaPolicy(p.id, editingPolicyName.value.trim())
    p.name = editingPolicyName.value.trim()
    cancelEditPolicy()
  } catch (err) {
    const axiosErr = err as any
    editingPolicyError.value[p.id] = axiosErr?.response?.data?.message ?? (err instanceof Error ? err.message : 'Failed to update policy')
  }
}

async function addPolicy() {
  if (!newPolicyName.value || !newPolicyName.value.trim()) return
  creatingPolicy.value = true
  createPolicyError.value = ''
  try {
    await createSlaPolicy(newPolicyName.value.trim())
    await loadPolicies()
    newPolicyName.value = ''
  } catch (err) {
    const axiosErr = err as any
    createPolicyError.value = axiosErr?.response?.data?.message ?? (err instanceof Error ? err.message : 'Failed to create policy')
  } finally {
    creatingPolicy.value = false
  }
}

function prevPage() {
  if (page.value <= 1) return
  page.value -= 1
  loadPolicies()
}

function nextPage() {
  const totalPages = Math.max(1, Math.ceil(total.value / size.value))
  if (page.value >= totalPages) return
  page.value += 1
  loadPolicies()
}

function viewRules(p: SlaPolicyItem) {
  emit('view-rules', p)
}

async function activateSelectedPolicy() {
  if (!activatePolicyId.value) return
  activating.value = true
  activateError.value = ''
  try {
    await activateSlaPolicy(activatePolicyId.value)
    await loadPolicies()
  } catch (err) {
    const axiosErr = err as any
    activateError.value = axiosErr?.response?.data?.message ?? (err instanceof Error ? err.message : 'Failed to activate policy')
  } finally {
    activating.value = false
  }
}

onMounted(loadPolicies)
</script>
