<template>
  <div class="max-w-4xl mx-auto p-6">
    <h1 class="text-2xl font-semibold mb-4">Ticket {{ ticket?.ticketCode ?? '' }}</h1>
    <div v-if="loading">Loading...</div>
    <div v-else-if="ticket">
      <div class="bg-white rounded shadow p-4 space-y-3">
        <div class="flex items-center justify-between">
          <h2 class="text-lg font-medium">{{ ticket.subject }}</h2>
          <span class="px-2 py-1 bg-gray-200 rounded text-sm">Status: {{ ticket.status }}</span>
        </div>

        <div class="grid grid-cols-2 gap-4 text-sm text-gray-700">
          <div><strong>Ticket code:</strong> {{ ticket.ticketCode }}</div>
          <div><strong>Raiser:</strong> {{ ticket.raiserName }}</div>
          <div><strong>Category:</strong> {{ ticket.categoryName }}</div>
          <div><strong>Team:</strong> {{ ticket.teamName ?? 'Unassigned' }}</div>
          <div><strong>SLA rule:</strong> {{ ticket.slaRuleName }}</div>
          <div><strong>First response mins:</strong> {{ ticket.slaFirstResponseMinutes }}</div>
          <div><strong>Resolution mins:</strong> {{ ticket.slaResolutionMinutes }}</div>
          <div><strong>First response at:</strong> {{ ticket.firstResponseAt ?? '-' }}</div>
          <div><strong>Response breached:</strong> {{ ticket.isResponseBreached ? 'Yes' : 'No' }}</div>
          <div><strong>Resolution breached:</strong> {{ ticket.isResolutionBreached ? 'Yes' : 'No' }}</div>
        </div>

        <div class="mt-4 text-gray-800" v-html="ticket.body"></div>
      </div>
    </div>
    <div v-else class="text-sm text-gray-600">Ticket not found.</div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { getTicketByCode } from '../api/tickets'

const route = useRoute()
const ticket = ref<any | null>(null)
const loading = ref(false)

async function load() {
  const ticketCode = String(route.params.ticketCode || '')
  if (!ticketCode) return
  loading.value = true
  try {
    ticket.value = await getTicketByCode(ticketCode)
  } catch {
    ticket.value = null
  } finally {
    loading.value = false
  }
}

onMounted(load)
</script>
