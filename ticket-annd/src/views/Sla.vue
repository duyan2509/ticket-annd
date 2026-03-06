<template>
  <div class="min-h-screen bg-gray-100">
    <AppHeader />

    <main class="max-w-4xl mx-auto p-6">
      <h1 class="text-2xl font-semibold text-gray-800">SLA Policies</h1>
      <SlaContent />
    </main>
  </div>
</template>

<script setup lang="ts">
import AppHeader from '../components/AppHeader.vue'
import SlaContent from '../components/SlaContent.vue'
</script>
