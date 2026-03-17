export interface UserContext {
  userId: string
  email: string
  role: string
  company_id: string
}

export interface LoginResponse {
  accessToken: string
}

export interface CompanyOption {
  companyId: string
  companyName: string
  role: string
}

export interface CompanyPagedResult {
  items: CompanyOption[]
  total: number
  page: number
  size: number
}

export interface InvitationItem {
  id: string
  companyId: string
  companyName: string
  email: string
  role: string
  status: string
  expires: string
}

export interface InvitationPagedResult {
  items: InvitationItem[]
  total: number
  page: number
  size: number
}

export interface CompanyInvitationItem {
  id: string
  email: string
  response_at: string | null
  status: string
  expire_at: string
}

export interface CompanyInvitationPagedResult {
  items: CompanyInvitationItem[]
  total: number
  page: number
  size: number
}

export interface MemberItem {
  userId: string
  email: string
  role: string
  teamId?: string
  teamName?: string
  isLeader?: boolean|false
}

export interface MemberPagedResult {
  items: MemberItem[]
  total: number
  page: number
  size: number
}

export interface CategoryItem {
  id: string
  name: string
}

export interface TicketPagedResult {
  items: any[]
  total: number
  page: number
  size: number
}
export interface TicketPageItem {
  id: string
  subject: string
  status: string
  categoryName: string
  categoryId: string
  slaRuleId: string
  teamId?: string
  teamName?: string
  slaRuleName: string
  firstResponseAt: string | null
  isResoluionBreached: boolean
  isFirstResponseBreached: boolean
  raiserId: string
  raiserName: string
  slaResolutionMinutes: number
  slaResponseMinutes: number
  ticketCode: string
}

