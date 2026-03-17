import api from './axios'
import type { CompanyOption, UserContext, PagedResult } from '../types/auth'
import { useAuthStore } from '../store/authStore'

export async function getCompanies(userContext: UserContext | null, page = 1, size = 10): Promise<PagedResult<CompanyOption>> {
  if (!userContext) return { items: [], total: 0, page, size }

  const { data } = await api.get<PagedResult<CompanyOption>>('/companies', { params: { page, size } })
  return data
}

export async function createCompany(name: string): Promise<{ companyId: string; name: string }> {
  const { data } = await api.post<{ companyId: string; name: string }>('/companies', { name })
  return data
}

export async function getCurrentCompany(): Promise<{ id: string; name: string; slug: string; role?: string }> {
  const { data } = await api.get<{ id: string; name: string; slug: string; role?: string }>('/companies/current')
  try {
    useAuthStore().setCurrentCompany(data)
  } catch {}
  return data
}
