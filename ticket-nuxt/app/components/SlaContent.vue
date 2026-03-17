<template>
  <div>
    <SlaPolicyList @view-rules="handleViewRules" />

    <SlaRuleList :policy="selectedPolicy" v-if="selectedPolicy" />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import SlaPolicyList from './SlaPolicyList.vue'
import SlaRuleList from './SlaRuleList.vue'
import type { SlaPolicyItem } from '../api/sla'

const selectedPolicy = ref<SlaPolicyItem | null>(null)

function handleViewRules(p: SlaPolicyItem) {
  selectedPolicy.value = p
}
</script>

