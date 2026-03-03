import {
  getAccessToken,
  setAccessToken,
  clearAccessToken,
  getMeCache,
} from '../store/authStore'
import type { UserContext } from '../types/auth'

export function useAuth() {
  function isLoggedIn(): boolean {
    return !!getAccessToken()
  }

  function getUserContext(): UserContext | null {
    const me = getMeCache()
    if (!me) return null
    return {
      userId: me.id,
      email: me.email,
      role: me.currentRole,
      company_id: '',
    }
  }

  function setTokens(accessToken: string): void {
    setAccessToken(accessToken)
  }

  function clearTokens(): void {
    clearAccessToken()
  }

  return {
    isLoggedIn,
    getUserContext,
    setTokens,
    clearTokens,
  }
}
