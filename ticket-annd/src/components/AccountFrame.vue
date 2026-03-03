<template>
  <div class="account-frame rounded-lg border border-gray-200 bg-white px-4 py-3 shadow-sm">
    <div class="flex flex-col gap-0.5">
      <span class="text-sm font-medium text-gray-800">{{ email || '—' }}</span>
      <span class="text-xs text-gray-500">{{ roleLabel }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { AppRoles } from '../types/appRoles'

const props = withDefaults(
  defineProps<{
    email?: string
    currentRole?: string
  }>(),
  { email: '', currentRole: AppRoles.EmptyUser }
)

const roleLabels: Record<string, string> = {
  [AppRoles.SupperAdmin]: 'Super Admin',
  [AppRoles.CompanyAdmin]: 'Owner',
  [AppRoles.Agent]: 'Agent',
  [AppRoles.Customer]: 'Customer',
  [AppRoles.EmptyUser]: 'Member',
}

const roleLabel = computed(() => {
  const r = props.currentRole || AppRoles.EmptyUser
  return roleLabels[r] ?? r
})
</script>
