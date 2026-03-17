<template>
  <div class=" rounded-lg   bg-white px-4 py-3 ">
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
    companyName?: string|null
  }>(),
  { email: '', currentRole: AppRoles.EmptyUser }
)

const roleLabels: Record<string, string> = {
  [AppRoles.SupperAdmin]: 'Super Admin',
  [AppRoles.CompanyAdmin]: 'Owner',
  [AppRoles.Agent]: 'Agent',
  [AppRoles.Customer]: 'Customer',
  [AppRoles.EmptyUser]: 'EmptyUser',
}

const roleLabel = computed(() => {
  const r = props.currentRole || AppRoles.EmptyUser
  const base = roleLabels[r] ?? r
  if (r === AppRoles.SupperAdmin || r === AppRoles.EmptyUser) return base
  return `${base} of ${props.companyName ?? '-'} `
})
</script>
