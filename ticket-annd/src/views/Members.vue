<template>
  <div class="min-h-screen bg-gray-100">
    <AppHeader />

    <main class="max-w-4xl mx-auto p-6">
      <h1 class="text-2xl font-semibold text-gray-800">Members</h1>

      <div class="mt-6 bg-white rounded shadow p-4">
        <div v-if="loading" class="text-sm text-gray-600">Loading...</div>
        <div v-else>
          <div class="overflow-x-auto">
            <table class="min-w-full divide-y divide-gray-200">
              <thead class="bg-gray-50">
                <tr>
                  <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Email</th>
                  <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Role</th>
                </tr>
              </thead>
              <tbody class="bg-white divide-y divide-gray-100">
                <tr v-for="m in members" :key="m.userId" class="hover:bg-gray-50 even:bg-gray-50">
                  <td class="px-4 py-3 text-sm text-gray-700 truncate">{{ m.email }}</td>
                  <td class="px-4 py-3 text-sm text-gray-700">
                    <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800">{{ m.role }}</span>
                  </td>
                </tr>
                <tr v-if="members.length === 0">
                  <td colspan="2" class="px-4 py-6 text-center text-sm text-gray-500">No members found.</td>
                </tr>
              </tbody>
            </table>
          </div>

          <div class="mt-4 flex items-center gap-3">
            <button @click="prevPage" :disabled="page <= 1" class="px-3 py-1 bg-gray-200 rounded disabled:opacity-50">Prev</button>
            <span class="text-sm text-gray-600">Page {{ page }} / {{ Math.max(1, Math.ceil(total / size)) }}</span>
            <button @click="nextPage" :disabled="page >= Math.ceil(total / size)" class="px-3 py-1 bg-gray-200 rounded disabled:opacity-50">Next</button>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import AppHeader from '../components/AppHeader.vue'
import { getCompanyMembers } from '../api/members'
import type { MemberItem } from '../types/auth'

const members = ref<MemberItem[]>([])
const loading = ref(false)
const page = ref(1)
const size = ref(20)
const total = ref(0)

async function load() {
  loading.value = true
  try {
    const res = await getCompanyMembers(page.value, size.value)
    members.value = res.items
    total.value = res.total
  } catch (err) {
    console.error('Failed to load members', err)
    members.value = []
    total.value = 0
  } finally {
    loading.value = false
  }
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

onMounted(load)
</script>
