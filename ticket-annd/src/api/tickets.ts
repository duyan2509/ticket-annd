import api from './axios'

export async function getTicketByCode(slug: string, ticketCode: string) {
  const { data } = await api.get(`/tickets/by-code`, { params: { slug, ticketCode } })
  return data
}

export async function createTicket(categoryId: string, slaRuleId: string, subject: string, body?: string | null, teamId?: string | null) {
  const payload: any = { categoryId, slaRuleId, subject }
  if (body) payload.body = body
  if (teamId) payload.teamId = teamId
  const { data } = await api.post('/tickets', payload)
  return data
}
