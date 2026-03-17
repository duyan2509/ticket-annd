import api from './axios'
import type { MemberPagedResult } from '../types/auth'

export async function getCompanyMembers(page = 1, size = 20): Promise<MemberPagedResult> {
  const { data } = await api.get<MemberPagedResult>(`/companies/members?page=${page}&size=${size}`)
  return data
}
