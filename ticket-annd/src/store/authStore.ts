import { ref, type Ref } from 'vue'

let accessToken: string | null = null

export function getAccessToken(): string | null {
  return accessToken
}

export function setAccessToken(token: string): void {
  accessToken = token
}

export function clearAccessToken(): void {
  accessToken = null
  // clear reactive user context as well
  userContext.value = null
}

export interface MeCache {
  id: string
  email: string
  isActive: boolean
  currentRole: string
  companyName?: string | null
}

// reactive user context accessible across the app
const userContext: Ref<MeCache | null> = ref(null)

export function getMeCache(): MeCache | null {
  return userContext.value
}

export function setMeCache(data: MeCache | null): void {
  userContext.value = data
}

export { userContext }
