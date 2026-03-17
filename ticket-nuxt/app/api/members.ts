import api from './axios'
import type { MemberItem, PagedResult } from '../types/auth'

export async function getCompanyMembers(page = 1, size = 20): Promise<PagedResult<MemberItem>> {
  const { data } = await api.get<PagedResult<MemberItem>>(`/companies/members?page=${page}&size=${size}`)
  return data
}
