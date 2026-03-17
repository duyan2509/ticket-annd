import api from './axios'
import type { PagedResult } from '~/types/auth'

export interface SlaPolicyItem {
  id: string
  name: string
  isActive: boolean
}

export interface SlaRuleItem {
  id: string
  name: string
  firstResponseMinutes: number
  resolutionMinutes: number
}

export function getSlaPolicies(page = 1, pageSize = 10) {
  return api.get<PagedResult<SlaPolicyItem>>('/sla/policies', { params: { page, pageSize } }).then((r) => r.data)
}

export function getSlaRulesByPolicy(policyId: string) {
  return api.get<SlaRuleItem[]>(`/sla/policies/${policyId}/rules`).then((r) => r.data)
}

export function getActiveSlaRules() {
  return api.get<SlaRuleItem[]>('/sla/rules').then((r) => r.data)
}

export function activateSlaPolicy(policyId: string) {
  return api.post(`/sla/policies/${policyId}/activate`).then((r) => r.data)
}

export function createSlaPolicy(name: string) {
  return api.post<{ id: string }>('/sla/policies', { name }).then((r) => r.data)
}

export function updateSlaPolicy(id: string, name: string) {
  return api.put(`/sla/policies/${id}`, { name }).then((r) => r.data)
}

export function createSlaRule(policyId: string, name: string, firstResponseMinutes: number, resolutionMinutes: number) {
  return api.post<{ id: string }>(`/sla/policies/${policyId}/rules`, { name, firstResponseMinutes, resolutionMinutes }).then((r) => r.data)
}

export function updateSlaRule(policyId: string, ruleId: string, data: { name?: string | null; firstResponseMinutes?: number | null; resolutionMinutes?: number | null }) {
  return api.put(`/sla/policies/${policyId}/rules/${ruleId}`, data).then((r) => r.data)
}
