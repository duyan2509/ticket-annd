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

export interface InvitationItem {
  id: string
  companyId: string
  companyName: string
  email: string
  role: string
  status: string
  expires: string
}
