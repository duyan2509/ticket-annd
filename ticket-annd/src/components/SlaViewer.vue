<template>
    <div class="bg-white rounded shadow p-4">
        <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-semibold">SLA Policies</h3>
            <div class="flex items-center gap-2">
                <button @click="showNewPolicy = !showNewPolicy"
                    class="px-3 py-1 bg-green-600 text-white rounded text-sm">New Policy</button>
                <button @click="$emit('close')" class="px-2 py-1 bg-gray-200 rounded">Close</button>
            </div>
        </div>

        <div v-if="loading" class="text-sm text-gray-600">Loading policies...</div>
        <div v-else>
            <div v-if="showNewPolicy" class="mb-3 p-3 bg-gray-50 rounded">
                <div class="flex gap-2">
                    <input v-model="newPolicyName" placeholder="Policy name" class="px-2 py-1 border rounded w-full" />
                    <button @click="createPolicy()" class="px-3 py-1 bg-blue-600 text-white rounded">Create</button>
                    <button @click="() => (showNewPolicy = false)" class="px-3 py-1 bg-gray-200 rounded">Cancel</button>
                </div>
                <p v-if="newPolicyError" class="text-sm text-red-500 mt-1">{{ newPolicyError }}</p>
            </div>
            <div v-if="policies.length === 0" class="text-sm text-gray-500">No SLA policies found.</div>
            <div class="space-y-2">
                <div v-for="p in policies" :key="p.id" class="border rounded">
                    <div class="flex items-center justify-between px-4 py-2 cursor-pointer">
                        <div class="flex items-center gap-3" @click.stop="toggleExpanded(p)">
                            <div class="flex items-center gap-3">
                                <svg v-if="expandedId === p.id" xmlns="http://www.w3.org/2000/svg"
                                    class="h-4 w-4 text-gray-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                        d="M20 12H4" />
                                </svg>
                                <svg v-else xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 text-gray-500" fill="none"
                                    viewBox="0 0 24 24" stroke="currentColor">
                                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                        d="M12 4v16m8-8H4" />
                                </svg>
                                <div class="text-sm font-medium text-gray-800">
                                    <template v-if="editingPolicyId === p.id">
                                        <input v-model="editingPolicyName" class="px-2 py-1 border rounded text-sm" />
                                    </template>
                                    <template v-else>{{ p.name }}</template>
                                </div>
                            </div>
                            <div class="flex items-center gap-3">
                                <label class="flex items-center gap-2 text-sm">
                                    <input type="checkbox" :checked="p.isActive" @click.stop="onActivate(p)" />
                                    <span class="text-xs text-gray-600">Active</span>
                                </label>
                                <div class="flex items-center gap-2">
                                    <button v-if="editingPolicyId !== p.id" @click.stop="startEditPolicy(p)"
                                        class="px-2 py-1 bg-yellow-400 text-white rounded text-sm">Edit</button>
                                    <div v-else class="flex items-center gap-2">
                                        <button @click.stop="savePolicy(p)"
                                            class="px-2 py-1 bg-green-600 text-white rounded text-sm">Save</button>
                                        <button @click.stop="cancelEditPolicy()"
                                            class="px-2 py-1 bg-gray-200 rounded text-sm">Cancel</button>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div v-if="expandedId === p.id" class="px-4 pb-3">
                            <div v-if="rulesLoadingMap[p.id]" class="text-sm text-gray-600">Loading rules...</div>
                            <div v-else>
                                <div v-if="!p.rules || p.rules.length === 0" class="text-sm text-gray-500">No rules for
                                    this policy.</div>
                                <div class="mt-2">
                                    <div v-for="r in p.rules || []" :key="r.id"
                                        class="flex items-center justify-between px-3 py-2 bg-gray-50 rounded mb-2">
                                        <div>
                                            <template v-if="editingRuleId === r.id">
                                                <input v-model="editingRuleName"
                                                    class="px-2 py-1 border rounded text-sm w-56" />
                                                <div class="flex gap-2 mt-1">
                                                    <input type="number" v-model.number="editingRuleFirst"
                                                        class="px-2 py-1 border rounded text-sm w-32" />
                                                    <input type="number" v-model.number="editingRuleResolution"
                                                        class="px-2 py-1 border rounded text-sm w-32" />
                                                </div>
                                            </template>
                                            <template v-else>
                                                <div class="text-sm font-medium text-gray-800">{{ r.name }}</div>
                                                <div class="text-xs text-gray-600">First response: {{
                                                    r.firstResponseMinutes }}m · Resolution: {{ r.resolutionMinutes }}m
                                                </div>
                                            </template>
                                        </div>
                                        <div class="flex items-center gap-2">
                                            <button v-if="editingRuleId !== r.id" @click.stop="startEditRule(p.id, r)"
                                                class="px-2 py-1 bg-yellow-400 text-white rounded text-sm">Edit</button>
                                            <div v-else class="flex items-center gap-2">
                                                <button @click.stop="saveRule(p.id, r.id)"
                                                    class="px-2 py-1 bg-green-600 text-white rounded text-sm">Save</button>
                                                <button @click.stop="cancelEditRule()"
                                                    class="px-2 py-1 bg-gray-200 rounded text-sm">Cancel</button>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="mt-2 p-3 bg-gray-50 rounded">
                                        <div class="flex gap-2 items-center">
                                            <input v-model="newRuleNameMap[p.id]" placeholder="Rule name"
                                                class="px-2 py-1 border rounded w-48" />
                                            <input type="number" v-model.number="newRuleFirstMap[p.id]"
                                                placeholder="First response (min)"
                                                class="px-2 py-1 border rounded w-36" />
                                            <input type="number" v-model.number="newRuleResolutionMap[p.id]"
                                                placeholder="Resolution (min)" class="px-2 py-1 border rounded w-36" />
                                            <button @click.stop="createRule(p.id)"
                                                class="px-3 py-1 bg-blue-600 text-white rounded">Add
                                                Rule</button>
                                        </div>
                                        <p v-if="newRuleErrorMap[p.id]" class="text-sm text-red-500 mt-1">{{
                                            newRuleErrorMap[p.id] }}</p>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</template>

<script setup lang="ts">
import { ref } from 'vue'
import { getSlaPolicies, getSlaRulesByPolicy, activateSlaPolicy, createSlaPolicy, updateSlaPolicy, createSlaRule, updateSlaRule, SlaPolicyItem, SlaRuleItem } from '../api/sla'

defineProps<{ companyId?: string }>()
const emit = defineEmits(['close'] as const)

const policies = ref<Array<SlaPolicyItem & { rules?: SlaRuleItem[] }>>([])
const loading = ref(false)
const expandedId = ref<string | null>(null)
const rulesLoadingMap = ref<Record<string, boolean>>({})
// new policy state
const showNewPolicy = ref(false)
const newPolicyName = ref('')
const newPolicyError = ref('')
// edit policy
const editingPolicyId = ref<string | null>(null)
const editingPolicyName = ref('')
// rules create/edit state
const newRuleNameMap = ref<Record<string, string>>({})
const newRuleFirstMap = ref<Record<string, number>>({})
const newRuleResolutionMap = ref<Record<string, number>>({})
const newRuleErrorMap = ref<Record<string, string>>({})
const editingRuleId = ref<string | null>(null)
const editingRuleName = ref('')
const editingRuleFirst = ref<number | null>(null)
const editingRuleResolution = ref<number | null>(null)

async function load() {
    loading.value = true
    try {
        const paged = await getSlaPolicies(1, 50)
        policies.value = paged.items.map((i) => ({ ...i }))
    } finally {
        loading.value = false
    }
}

async function toggleExpanded(p: SlaPolicyItem & { rules?: SlaRuleItem[] }) {
    if (expandedId.value === p.id) {
        expandedId.value = null
        return
    }
    expandedId.value = p.id
    if (!p.rules) {
        rulesLoadingMap.value[p.id] = true
        try {
            const rules = await getSlaRulesByPolicy(p.id)
            p.rules = rules
        } finally {
            rulesLoadingMap.value[p.id] = false
        }
    }
}

async function createPolicy() {
    newPolicyError.value = ''
    if (!newPolicyName.value || newPolicyName.value.trim() === '') {
        newPolicyError.value = 'Name is required'
        return
    }
    try {
        const res = await createSlaPolicy(newPolicyName.value.trim())
        // reload list or push new
        await load()
        newPolicyName.value = ''
        showNewPolicy.value = false
    } catch (err: any) {
        newPolicyError.value = err?.response?.data?.message ?? err.message ?? 'Failed to create policy'
    }
}

function startEditPolicy(p: SlaPolicyItem) {
    editingPolicyId.value = p.id
    editingPolicyName.value = p.name
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
        alert('Failed to update policy')
    }
}

async function onActivate(p: SlaPolicyItem) {
    try {
        await activateSlaPolicy(p.id)
        const target = policies.value.find((x) => x.id === p.id)
        if (target) target.isActive = true
    } catch (err) {
        console.error('Activate failed', err)
        alert('Failed to activate policy')
    }
}

async function createRule(policyId: string) {
    newRuleErrorMap.value[policyId] = ''
    const name = newRuleNameMap.value[policyId]
    const first = newRuleFirstMap.value[policyId] ?? 0
    const resol = newRuleResolutionMap.value[policyId] ?? 0
    if (!name || name.trim() === '') {
        newRuleErrorMap.value[policyId] = 'Name is required'
        return
    }
    try {
        const res = await createSlaRule(policyId, name.trim(), first, resol)
        // reload rules for policy
        const p = policies.value.find((x) => x.id === policyId)
        if (p) {
            const rules = await getSlaRulesByPolicy(policyId)
            p.rules = rules
        }
        newRuleNameMap.value[policyId] = ''
        newRuleFirstMap.value[policyId] = 0
        newRuleResolutionMap.value[policyId] = 0
    } catch (err: any) {
        newRuleErrorMap.value[policyId] = err?.response?.data?.message ?? err.message ?? 'Failed to create rule'
    }
}

function startEditRule(policyId: string, r: SlaRuleItem) {
    editingRuleId.value = r.id
    editingRuleName.value = r.name
    editingRuleFirst.value = r.firstResponseMinutes
    editingRuleResolution.value = r.resolutionMinutes
}

function cancelEditRule() {
    editingRuleId.value = null
    editingRuleName.value = ''
    editingRuleFirst.value = null
    editingRuleResolution.value = null
}

async function saveRule(policyId: string, ruleId: string) {
    try {
        await updateSlaRule(policyId, ruleId, {
            name: editingRuleName.value || null,
            firstResponseMinutes: editingRuleFirst.value ?? null,
            resolutionMinutes: editingRuleResolution.value ?? null,
        })
        const p = policies.value.find((x) => x.id === policyId)
        if (p) {
            const rules = await getSlaRulesByPolicy(policyId)
            p.rules = rules
        }
        cancelEditRule()
    } catch (err) {
        alert('Failed to update rule')
    }
}

load()
</script>
