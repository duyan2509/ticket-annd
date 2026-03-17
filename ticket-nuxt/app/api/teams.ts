import api from './axios'
import type { MemberItem, PagedResult } from '../types/auth'

export interface TeamItem {
  teamId: string
  name: string
}

export async function getTeamsByCompany(companyId: string): Promise<TeamItem[]> {
  const { data } = await api.get<TeamItem[]>(`/teams`)
  return data
}

export async function createTeam(name: string) {
  const { data } = await api.post('/teams', { name })
  return data
}

export async function getTeamMembers(teamId: string, page = 1, size = 100): Promise<PagedResult<MemberItem>> {
  const { data } = await api.get<PagedResult<MemberItem>>(`/teams/${teamId}/members?page=${page}&size=${size}`)
  return data
}

export async function switchMember(teamId: string, userId: string, toTeamId: string) {
  const { data } = await api.post(`/teams/${teamId}/members/switch`, { userId, toTeamId })
  return data
}

export async function assignMember(teamId: string, userId: string) {
  const { data } = await api.post(`/teams/${teamId}/members`, { userId })
  return data
}

export async function setLeader(teamId: string, userId: string) {
  const { data } = await api.post(`/teams/${teamId}/leader`, { userId })
  return data
}

export async function updateTeam(teamId: string, name: string): Promise<void> {
  await api.put(`/teams/${teamId}`, { name })
}
