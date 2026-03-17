<template>
  <div class="mt-6 bg-white rounded shadow p-4">
    <div v-if="rulesLoading" class="text-sm text-gray-600">Loading rules...</div>
    <div v-else>
      <h2 class="text-lg font-semibold mb-4">{{ policy?.name }} Rules</h2>
      <div class="mb-4 flex items-center gap-2">
        <SlaRuleForm @submit="(payload) => addRule(payload)" :error="createRuleError" />
      </div>

      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Name</th>
              <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">First response (min)</th>
              <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Resolution (min)</th>
              <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-100">
            <tr v-for="r in rules" :key="r.id" class="hover:bg-gray-50 even:bg-gray-50">
              <template v-if="editingRuleId !== r.id">
                <td class="px-4 py-3 text-sm text-gray-700 truncate">{{ r.name }}</td>
                <td class="px-4 py-3 text-sm text-gray-700">{{ r.firstResponseMinutes }}</td>
                <td class="px-4 py-3 text-sm text-gray-700">{{ r.resolutionMinutes }}</td>
                <td class="px-4 py-3 text-sm text-gray-700">
                  <button @click="startEditRule(r)" class="px-2 py-1 bg-yellow-400 text-white rounded text-sm">Edit</button>
                </td>
              </template>
              <template v-else>
                <td class="px-4 py-3 text-sm text-gray-700 truncate">
                  <input v-model="editingRuleData.name" class="px-2 py-1 border rounded w-full text-sm" />
                </td>
                <td class="px-4 py-3 text-sm text-gray-700">
                  <input v-model.number="editingRuleData.firstResponseMinutes" type="number" class="px-2 py-1 border rounded w-24 text-sm" />
                </td>
                <td class="px-4 py-3 text-sm text-gray-700">
                  <input v-model.number="editingRuleData.resolutionMinutes" type="number" class="px-2 py-1 border rounded w-24 text-sm" />
                </td>
                <td class="px-4 py-3 text-sm text-gray-700">
                  <button @click="saveRule(r.id)" class="px-2 py-1 bg-green-600 text-white rounded text-sm mr-2">Save</button>
                  <button @click="cancelEditRule()" class="px-2 py-1 bg-gray-200 rounded text-sm">Cancel</button>
                  <p v-if="editingRuleError[r.id]" class="text-sm text-red-500 mt-1">{{ editingRuleError[r.id] }}</p>
                </td>
              </template>
            </tr>
            <tr v-if="rules.length === 0">
              <td colspan="4" class="px-4 py-6 text-center text-sm text-gray-500">No rules found for this policy.</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'
import { getSlaRulesByPolicy, createSlaRule, updateSlaRule } from '../api/sla'
import SlaRuleForm from './SlaRuleForm.vue'
import type { SlaPolicyItem, SlaRuleItem } from '../api/sla'

const props = defineProps<{
  policy: SlaPolicyItem | null
}>()

const rules = ref<SlaRuleItem[]>([])
const rulesLoading = ref(false)

const newRuleName = ref('')
const newRuleFirstResponse = ref<number | null>(0)
const newRuleResolution = ref<number | null>(0)
const creatingRule = ref(false)
const createRuleError = ref('')

const editingRuleId = ref<string | null>(null)
const editingRuleData = ref<{ name: string; firstResponseMinutes: number; resolutionMinutes: number }>({ name: '', firstResponseMinutes: 0, resolutionMinutes: 0 })
const editingRuleError = ref<Record<string, string>>({})

async function loadRules() {
  if (!props.policy) { rules.value = []; return }
  rulesLoading.value = true
  try {
    const r = await getSlaRulesByPolicy(props.policy.id)
    rules.value = r
  } catch {
    rules.value = []
  } finally {
    rulesLoading.value = false
  }
}

watch(() => props.policy, () => loadRules(), { immediate: true })

async function addRule(payload: { name: string; firstResponseMinutes: number | null; resolutionMinutes: number | null }) {
  if (!props.policy) return
  if (!payload.name || !payload.name.trim()) return
  creatingRule.value = true
  createRuleError.value = ''
  try {
    await createSlaRule(props.policy.id, payload.name.trim(), Number(payload.firstResponseMinutes ?? 0), Number(payload.resolutionMinutes ?? 0))
    await loadRules()
  } catch (err) {
    const axiosErr = err as any
    createRuleError.value = axiosErr?.response?.data?.message ?? (err instanceof Error ? err.message : 'Failed to create rule')
  } finally {
    creatingRule.value = false
  }
}

function startEditRule(r: SlaRuleItem) {
  editingRuleId.value = r.id
  editingRuleData.value = { name: r.name, firstResponseMinutes: r.firstResponseMinutes, resolutionMinutes: r.resolutionMinutes }
  editingRuleError.value[r.id] = ''
}

function cancelEditRule() {
  if (editingRuleId.value) editingRuleError.value[editingRuleId.value] = ''
  editingRuleId.value = null
  editingRuleData.value = { name: '', firstResponseMinutes: 0, resolutionMinutes: 0 }
}

async function saveRule(ruleId: string) {
  if (!props.policy) return
  const pid = props.policy.id
  const payload = {
    name: editingRuleData.value.name?.trim() ?? null,
    firstResponseMinutes: editingRuleData.value.firstResponseMinutes ?? null,
    resolutionMinutes: editingRuleData.value.resolutionMinutes ?? null,
  }
  try {
    await updateSlaRule(pid, ruleId, payload)
    await loadRules()
    cancelEditRule()
  } catch (err) {
    const axiosErr = err as any
    let message = 'Failed to update rule'
    if (axiosErr?.response) {
      const status = axiosErr.response.status
      const data = axiosErr.response.data
      if (status === 403) message = data?.message ?? 'You do not have permission.'
      else if (status === 409) message = data?.message ?? 'Conflict'
      else if (status === 400) {
        if (data?.errors && Array.isArray(data.errors) && data.errors.length > 0) {
          message = data.errors.map((e: any) => e.ErrorMessage ?? e.errorMessage ?? `${e.PropertyName ?? e.propertyName}: ${e.ErrorMessage ?? e.errorMessage}`).join('; ')
        } else {
          message = data?.message ?? 'Bad request'
        }
      } else message = data?.message ?? axiosErr.message ?? message
    } else if (err instanceof Error) {
      message = err.message
    }
    editingRuleError.value[ruleId] = message
  }
}

onMounted(loadRules)
</script>
