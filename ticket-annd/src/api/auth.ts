import api from './axios'
import { setMeCache } from '../store/authStore'
import type { LoginResponse } from '../types/auth'

export async function login(email: string, password: string): Promise<LoginResponse> {
  const { data } = await api.post<LoginResponse>('/auth/login', { email, password })
  return data
}

export async function register(email: string, password: string): Promise<void> {
  await api.post('/auth/register', { email, password })
}

export async function forgotPassword(email: string): Promise<void> {
  await api.post('/auth/forgot-password', { email })
}

export async function resetPassword(email: string, token: string, newPassword: string): Promise<void> {
  await api.post('/auth/reset-password', { email, token, newPassword })
}

export async function logout(): Promise<void> {
  await api.post('/auth/logout')
}

export async function refresh(): Promise<LoginResponse> {
  const { data } = await api.post<LoginResponse>('/auth/refresh')
  return data
}

export interface MeResponse {
  id: string
  email: string
  isActive: boolean
  currentRole: string
}

export async function getMe(): Promise<MeResponse> {
  const { data } = await api.get<MeResponse>('/auth/me')
  setMeCache(data)
  return data
}

export async function switchRole(companyId: string): Promise<LoginResponse> {
  const { data } = await api.post<LoginResponse>('/auth/switch-role', { companyId })
  return data
}
