<template>
  <div>
    <label class="block text-sm text-gray-700">Invite by email</label>
    <div class="flex gap-2 mt-2">
      <input v-model="email" placeholder="email@example.com" class="px-3 py-2 border rounded w-full" />
      <select v-model="role" class="px-3 py-2 border rounded">
        <option value="Customer">Customer</option>
        <option value="Agent">Agent</option>
      </select>
      <button @click.prevent="onSubmit" class="px-3 py-2 bg-blue-600 text-white rounded">Invite</button>
    </div>
    <p v-if="error" class="text-sm text-red-500 mt-1">{{ error }}</p>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useInviteValidation } from '../composables/useInviteValidation'

const props = defineProps<{ error?: string }>()
const emit = defineEmits<{ (e: 'submit', payload: { email: string; role: string }): void }>()

const email = ref('')
const role = ref('Customer')
const error = props.error ?? ''
const { v$ } = useInviteValidation({ email, role })

function onSubmit() {
  v$.value.$touch()
  if (!v$.value.$invalid) emit('submit', { email: email.value, role: role.value })
}
</script>
