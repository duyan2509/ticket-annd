<template>
    <div class="min-h-screen bg-gray-100">
        <AppHeader />
        <main class="max-w-4xl mx-auto p-6">
            <div class="flex items-center justify-between mb-4">
                <h1 class="text-2xl font-semibold mb-4">Tickets</h1>
                <div>
                    <button @click="goGeneral" class="px-3 py-1 bg-gray-200 rounded">Back to Dashboard</button>
                </div>
            </div>
            <div class="bg-white rounded shadow p-4">
                <h2 class="text-lg font-medium mb-3">Create ticket</h2>
                <div class="grid grid-cols-1 gap-3">
                    <select v-model="form.categoryId" class="px-2 py-2 border rounded">
                        <option value="">Select category</option>
                        <option v-for="c in categories" :key="c.id" :value="c.id">{{ c.name }}</option>
                    </select>

                    <select v-model="selectedPolicyId" class="px-2 py-2 border rounded">
                        <option value="">Select SLA policy</option>
                        <option v-for="p in policies" :key="p.id" :value="p.id">{{ p.name }} {{ p.isActive ? '' : '(inactive)' }}</option>
                    </select>

                    <select v-model="form.slaRuleId" class="px-2 py-2 border rounded">
                        <option value="">Select SLA rule</option>
                        <option v-for="r in rules" :key="r.id" :value="r.id">{{ r.name }}</option>
                    </select>

                    <select v-model="form.teamId" class="px-2 py-2 border rounded">
                        <option value="">Assign to team (optional)</option>
                        <option v-for="t in teams" :key="t.teamId" :value="t.teamId">{{ t.name }}</option>
                    </select>

                    <input v-model="form.subject" placeholder="Subject" class="px-2 py-2 border rounded" />
                    <textarea v-model="form.body" placeholder="Body (optional)" class="px-2 py-2 border rounded h-28"></textarea>

                    <div class="flex items-center gap-2">
                        <button @click="onCreate" class="px-3 py-2 bg-blue-600 text-white rounded">Create</button>
                        <div v-if="loading" class="text-sm text-gray-600">Creating...</div>
                        <div v-if="success" class="text-sm text-green-600">Created successfully.</div>
                        <div v-if="error" class="text-sm text-red-600">{{ error }}</div>
                    </div>
                </div>
            </div>
        </main>
    </div>
</template>

<script setup lang="ts">
import AppHeader from '../components/AppHeader.vue'
import { ref, onMounted, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { getCurrentCompany } from '../api/companies'
import { getCategories } from '../api/categories'
import { getSlaPolicies, getSlaRulesByPolicy } from '../api/sla'
import { getTeamsByCompany } from '../api/teams'
import { createTicket } from '../api/tickets'

const router = useRouter()
const route = useRoute()
const companySlug = String(route.params.slug || '')

const companyId = ref<string | null>(null)
const categories = ref<any[]>([])
const policies = ref<any[]>([])
const rules = ref<any[]>([])
const teams = ref<{ teamId: string; name: string }[]>([])

const form = ref({ categoryId: '', slaRuleId: '', teamId: '', subject: '', body: '' })
const selectedPolicyId = ref('')
const loading = ref(false)
const success = ref(false)
const error = ref('')

async function goGeneral() {
    router.push(`/${companySlug}`)
}

async function loadLists() {
    try {
        const c = await getCurrentCompany()
        companyId.value = c?.id ?? null
        categories.value = await getCategories()
        const p = await getSlaPolicies(1, 50)
        policies.value = p.items ?? p
        if (companyId.value) {
            teams.value = await getTeamsByCompany(companyId.value)
        }
    } catch (e) {
        // ignore
    }
}

async function loadRules() {
    rules.value = []
    if (!selectedPolicyId.value) return
    try {
        rules.value = await getSlaRulesByPolicy(selectedPolicyId.value)
    } catch {}
}

watch(selectedPolicyId, () => {
    form.value.slaRuleId = ''
    loadRules()
})

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
    } catch (ex: any) {
        error.value = ex?.response?.data?.message ?? (ex?.message ?? 'Failed to create ticket')
    } finally {
        loading.value = false
    }
}

onMounted(loadLists)
</script>
