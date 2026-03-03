<template>
  <div class="bg-white rounded-lg shadow p-6">
    <h2 class="text-lg font-semibold text-gray-800 mb-4">Invitations</h2>
    <ul v-if="invitations.length" class="space-y-2">
      <li v-for="inv in invitations" :key="inv.id"
        class="flex items-center justify-between py-2 border-b border-gray-100 last:border-0">
        <div>
          <span class="font-medium text-gray-800">{{ inv.companyName }}</span>
          <span class="text-sm text-gray-500 ml-2">{{ inv.role }} · {{ inv.status }}</span>
        </div>
        <span class="text-sm text-gray-400">{{ formatDate(inv.expires) }}</span>
      </li>
    </ul>
    <p v-else class="text-gray-500">No invitations.</p>
  </div>
</template>

<script setup lang="ts">
import type { InvitationItem } from '../types/auth'

withDefaults(
  defineProps<{
    invitations?: InvitationItem[]
  }>(),
  { invitations: () => [] }
)

function formatDate(iso: string | undefined): string {
  if (!iso) return ''
  try {
    return new Date(iso).toLocaleDateString()
  } catch {
    return iso
  }
}
</script>
