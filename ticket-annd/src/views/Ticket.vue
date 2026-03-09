<template>
  <div class="max-w-4xl mx-auto p-6">
    <h1 class="text-2xl font-semibold mb-4">Ticket {{ ticket?.ticketCode ?? '' }}</h1>
    <div v-if="loading">Loading...</div>
    <div v-else-if="ticket">
      <div class="bg-white rounded shadow p-4">
        <h2 class="text-lg font-medium">{{ ticket.subject }}</h2>
        <p class="text-sm text-gray-600 mt-2">Status: {{ ticket.status }}</p>
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
  const slug = String(route.params.slug || '')
  const ticketCode = String(route.params.ticketCode || '')
  if (!slug || !ticketCode) return
  loading.value = true
  try {
    ticket.value = await getTicketByCode(slug, ticketCode)
  } catch {
    ticket.value = null
  } finally {
    loading.value = false
  }
}

onMounted(load)
</script>
