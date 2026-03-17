<template>
  <div class="bg-white rounded-lg shadow p-6">
    <h2 class="text-lg font-semibold text-gray-800 mb-4">Your companies</h2>
    <ul v-if="companies.length" class="space-y-2">
      <li v-for="c in companies" :key="c.companyId"
        class="flex items-center justify-between py-2 border-b border-gray-100 last:border-0">
        <span class="text-gray-800">{{ c.companyName }}</span>
        <span class="text-sm text-gray-500">{{ roleLabel(c.role) }}</span>
        <button v-if="canSwitch" @click="$emit('switch', c.companyId)"
          class="text-blue-600 hover:underline text-sm">Switch</button>
      </li>
    </ul>
    <p v-else class="text-gray-500">No companies yet.</p>
  </div>
</template>

<script setup lang="ts">
import type { CompanyOption } from '../types/auth'
import { AppRoles } from '../types/appRoles'

withDefaults(
  defineProps<{
    companies?: CompanyOption[]
    canSwitch?: boolean
  }>(),
  { companies: () => [], canSwitch: true }
)
defineEmits<{ (e: 'switch', companyId: string): void }>()

function roleLabel(role: string): string {
  return role === AppRoles.CompanyAdmin ? 'Owner' : 'Member'
}
</script>
