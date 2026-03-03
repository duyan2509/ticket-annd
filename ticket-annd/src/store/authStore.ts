let accessToken: string | null = null

export function getAccessToken(): string | null {
  return accessToken
}

export function setAccessToken(token: string): void {
  accessToken = token
}

export function clearAccessToken(): void {
  accessToken = null
  meCache = null
}

export interface MeCache {
  id: string
  email: string
  isActive: boolean
  currentRole: string
}

let meCache: MeCache | null = null

export function getMeCache(): MeCache | null {
  return meCache
}

export function setMeCache(data: MeCache | null): void {
  meCache = data
}
