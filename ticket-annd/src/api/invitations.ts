import type { InvitationItem, UserContext } from '../types/auth'
import { AppRoles } from '../types/appRoles'
/** mock */

export async function getInvitations(userContext: UserContext | null): Promise<InvitationItem[]> {
  const email = userContext?.email ?? ''
  const mock: InvitationItem[] = [
    { id: '1', companyId: 'c1', companyName: 'Acme Corp', email, role: AppRoles.Agent, status: 'Pending', expires: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000).toISOString() },
    { id: '2', companyId: 'c2', companyName: 'Beta Inc', email, role: AppRoles.Customer, status: 'Pending', expires: new Date(Date.now() + 3 * 24 * 60 * 60 * 1000).toISOString() },
  ]
  return mock
}
