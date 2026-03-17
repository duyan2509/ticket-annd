import type { PagedResult, TicketPageItem } from '~/types/auth'
import api from './axios'

export async function getTicketByCode(ticketCode: string) {
  const { data } = await api.get(`/tickets/by-code`, { params: { ticketCode } })
  return data
}

export async function getTicketLogs(ticketId: string, page = 1, size = 50) {
  const { data } = await api.get(`/tickets/${ticketId}/logs`, { params: { page, size } })
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
  const { data } = await api.get<PagedResult<TicketPageItem>>('/tickets', { params })
  return data
}

export async function assignTeam(ticketId: string, teamId: string) {
  const { data } = await api.post(`/tickets/${ticketId}/team/${teamId}`)
  return data
}

import { TicketPauseType } from '~/types/pause'

export async function pauseTicket(ticketId: string, pauseType: TicketPauseType, reason: string) {
  const payload: any = { pauseType, reason }
  const { data } = await api.post(`/tickets/${ticketId}/pause`, payload)
  return data
}

export async function continueTicket(ticketId: string) {
  const { data } = await api.post(`/tickets/${ticketId}/continue`)
  return data
}

export async function resolveTicket(ticketId: string, note?: string | null) {
  const payload: any = {}
  if (note) payload.note = note
  const { data } = await api.post(`/tickets/${ticketId}/resolve`, payload)
  return data
}

export async function assignMemberToTicket(ticketId: string, userId: string) {
  const { data } = await api.post(`/tickets/${ticketId}/member/${userId}`)
  return data
}

export async function startTicket(ticketId: string) {
  const { data } = await api.post(`/tickets/${ticketId}/start`)
  return data
}
