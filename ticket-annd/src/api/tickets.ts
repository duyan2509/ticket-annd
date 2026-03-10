import { TicketPagedResult } from '@/types/auth'
import api from './axios'

export async function getTicketByCode(ticketCode: string) {
  const { data } = await api.get(`/tickets/by-code`, { params: { ticketCode } })
  return data
}

export async function createTicket(categoryId: string, slaRuleId: string, subject: string, body?: string | null, teamId?: string | null) {
  const payload: any = { categoryId, slaRuleId, subject }
  if (body) payload.body = body
  if (teamId) payload.teamId = teamId
  const { data } = await api.post('/tickets', payload)
  return data
}

export async function getTickets(params: Record<string, any> = {}) {
  const { data } = await api.get<TicketPagedResult>('/tickets', { params })
  return data
}

export async function assignTeam(ticketId: string, teamId: string) {
  const { data } = await api.post(`/tickets/${ticketId}/team/${teamId}`)
  return data
}
