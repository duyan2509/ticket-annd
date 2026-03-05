import api from './axios'
import type { InvitationItem, InvitationPagedResult, UserContext, CompanyInvitationPagedResult } from '../types/auth'
import { AppRoles } from '../types/appRoles'

export async function getInvitations(userContext: UserContext | null): Promise<InvitationItem[]> {
  const { data } = await api.get<InvitationItem[]>('/invitations/me')
  return data
}

export async function getCompanyInvitations(page = 1, size = 20): Promise<CompanyInvitationPagedResult> {
  const { data } = await api.get<CompanyInvitationPagedResult>(`/invitations/company?page=${page}&size=${size}`)
  return data
}

export async function acceptInvitation(invitationId: string): Promise<void> {
  await api.post(`/invitations/${invitationId}/accept`)
}

export async function rejectInvitation(invitationId: string): Promise<void> {
  await api.post(`/invitations/${invitationId}/reject`)
}

export async function createInvitation(email: string, role: string): Promise<{ invitationId: string } | void> {
  const { data } = await api.post('/invitations', { email, role })
  return data
}
