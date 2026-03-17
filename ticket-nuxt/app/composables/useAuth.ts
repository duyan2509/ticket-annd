import { storeToRefs } from 'pinia'
import { useAuthStore } from '../store/authStore'
import type { UserContext } from '../types/auth'

export function useAuth() {
  function isLoggedIn(): boolean {
    return !!useAuthStore().getAccessToken()
  }

  function getUserContext(): UserContext | null {
    const me = useAuthStore().getMeCache()
    if (!me) return null
    return {
      userId: me.id,
      email: me.email,
      role: me.currentRole,
      company_id: '',
    }
  }

  function getUserContextRef() {
    const { me } = storeToRefs(useAuthStore())
    return me
  }

  function setTokens(accessToken: string): void {
    useAuthStore().setAccessToken(accessToken)
  }

  function clearTokens(): void {
    useAuthStore().clearAccessToken()
  }

  return {
    isLoggedIn,
    getUserContext,
    setTokens,
    clearTokens,
    getUserContextRef,
  }
}
