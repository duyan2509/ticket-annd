<template>
  <div class="fixed inset-0 z-50 flex items-center justify-center">
    <div class="absolute inset-0 bg-black/40" @click="$emit('close')"></div>
    <div class="relative bg-white rounded shadow-lg w-full max-w-xl mx-4 overflow-hidden">
      <div class="px-4 py-3 border-b">
        <div class="flex items-center justify-between">
          <h3 class="text-lg font-medium">{{ title }}</h3>
          <button @click="$emit('close')" class="text-gray-500 hover:text-gray-700">✕</button>
        </div>
      </div>
      <div class="p-4">
        <slot />
      </div>
      <div class="px-4 py-3 border-t bg-gray-50">
        <slot name="footer" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
defineProps<{ title?: string }>()
</script>

<style scoped>
.modal-body { max-height: 70vh; overflow: auto; }
</style>
