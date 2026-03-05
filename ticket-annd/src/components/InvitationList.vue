<template>
  <div class="bg-white rounded-lg shadow p-6">
    <h2 class="text-lg font-semibold text-gray-800 mb-4">Invitations</h2>
    <ul v-if="invitations.length" class="space-y-2">
      <li v-for="inv in invitations" :key="inv.id ?? inv.email"
        class="flex items-center justify-between py-2 border-b border-gray-100 last:border-0">
        <div>
          <template v-if="'companyName' in inv">
            <span class="font-medium text-gray-800">{{ (inv as any).companyName }}</span>
            <span class="text-sm text-gray-500 ml-2">{{ (inv as any).role }} · {{ (inv as any).status }}</span>
          </template>
          <template v-else>
            <div>
              <span class="font-medium text-gray-800">{{ (inv as any).email }}</span>
              <div class="text-sm text-gray-500 mt-1">
                <span>{{ (inv as any).status }}</span>
                <span v-if="(inv as any).response_at"> · {{ formatDate((inv as any).response_at) }}</span>
              </div>
            </div>
          </template>
        </div>
        <div class="flex items-center gap-3">
          <span class="text-sm text-gray-400">{{ formatDate((inv as any).expires ?? (inv as any).expire_at) }}</span>
          <div v-if="showActions && 'id' in inv">
            <button v-if="(inv as any).status === 'Pending'" @click="onAcceptClick((inv as any).id)" class="px-2 py-1 bg-green-500 text-white rounded text-sm">Accept</button>
            <button v-if="(inv as any).status === 'Pending'" @click="onRejectClick((inv as any).id)" class="px-2 py-1 bg-red-500 text-white rounded text-sm">Reject</button>
          </div>
        </div>
      </li>
    </ul>
    <p v-else class="text-gray-500">No invitations.</p>
  </div>
</template>

<script setup lang="ts">
import type { InvitationItem, CompanyInvitationItem } from '../types/auth'

type AnyInvitation = InvitationItem | CompanyInvitationItem

const emits = defineEmits<{
  (e: 'accept', id: string): void
  (e: 'reject', id: string): void
}>()

withDefaults(
  defineProps<{
    invitations?: AnyInvitation[]
    showActions?: boolean
  }>(),
  { invitations: () => [], showActions: true }
)

function formatDate(iso: string | undefined): string {
  if (!iso) return ''
  try {
    return new Date(iso).toLocaleDateString()
  } catch {
    return iso
  }
}

function onAcceptClick(id: string) {
  emits('accept', id)
}

function onRejectClick(id: string) {
  emits('reject', id)
}
</script>
