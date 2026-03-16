import axios, { type InternalAxiosRequestConfig } from 'axios'
import { useAuthStore } from '../store/authStore'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '/api',
  headers: { 'Content-Type': 'application/json' },
  withCredentials: true,
})

api.interceptors.request.use((config: InternalAxiosRequestConfig) => {
  const token = useAuthStore().getAccessToken()
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

let refreshPromise: Promise<string> | null = null

function doRefresh(): Promise<string> {
  if (refreshPromise) return refreshPromise
  refreshPromise = api
    .post<{ accessToken: string }>('/auth/refresh')
    .then(({ data }) => {
      try {
        useAuthStore().setAccessToken(data.accessToken)
      } catch {
        // noop
      }
      return data.accessToken
    })
    .finally(() => {
      refreshPromise = null
    })
  return refreshPromise
}

const publicAuthPaths = ['/auth/login', '/auth/register', '/auth/forgot-password', '/auth/reset-password']
function isPublicAuthRequest(config: InternalAxiosRequestConfig): boolean {
  const url = config.url ?? ''
  return publicAuthPaths.some((p) => url.includes(p))
}

api.interceptors.response.use(
  (res) => res,
  async (err) => {
    const original = err.config as InternalAxiosRequestConfig & { _retry?: boolean }
    if (err.response?.status === 401 && !original._retry && !isPublicAuthRequest(original)) {
      original._retry = true
      try {
        const newToken = await doRefresh()
        original.headers.Authorization = `Bearer ${newToken}`
        return api(original)
      } catch {
        refreshPromise = null
          try { useAuthStore().clearAccessToken() } catch {}
        window.location.href = '/login'
      }
    }
    return Promise.reject(err)
  }
)

export default api
