import api from './axios'
import type { CompanyOption, UserContext } from '../types/auth'
import { AppRoles } from '../types/appRoles'
/** mock */
export async function getCompanies(userContext: UserContext | null): Promise<CompanyOption[]> {
  const role = userContext?.role ?? AppRoles.EmptyUser
  const companyId = userContext?.company_id
  const companyName = 'My Company'

  if (role === AppRoles.EmptyUser) return []
  if (role === AppRoles.SupperAdmin) {
    return [
      { companyId: '00000000-0000-0000-0000-000000000001', companyName: 'Admin Company', role: AppRoles.SupperAdmin },
    ]
  }
  if (companyId && companyId !== '00000000-0000-0000-0000-000000000000') {
    return [{ companyId, companyName, role }]
  }
  return []
}

export async function createCompany(name: string): Promise<{ companyId: string; name: string }> {
  const { data } = await api.post<{ companyId: string; name: string }>('/companies', { name })
  return data
}
