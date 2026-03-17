import axios, { type InternalAxiosRequestConfig } from 'axios'
import { useAuthStore } from '../store/authStore'

const api = axios.create({
  baseURL: process.env.NUXT_PUBLIC_API_BASE_URL || 'http://localhost:5001/api',
  headers: { 'Content-Type': 'application/json' },
  withCredentials: true,
})

if (import.meta.client) {
  api.interceptors.request.use((config: InternalAxiosRequestConfig) => {
    try {
      const token = useAuthStore().getAccessToken()
      if (token && config.headers) config.headers.Authorization = `Bearer ${token}`
    } catch {}
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
        } catch {}
        return data.accessToken
      })
      .finally(() => {
        refreshPromise = null
      })
    return refreshPromise
  }

  const publicAuthPaths = ['/auth/login', '/auth/register', '/auth/forgot-password', '/auth/reset-password', '/auth/refresh']
  function isPublicAuthRequest(config: InternalAxiosRequestConfig): boolean {
    const url = config.url ?? ''
    return publicAuthPaths.some((p) => url.includes(p))
  }

  api.interceptors.response.use(
    (res) => res,
    async (err) => {
      const original = err.config as InternalAxiosRequestConfig & { _retry?: boolean }
      if (err.response?.status === 401 && !original?._retry && !isPublicAuthRequest(original)) {
        original._retry = true
        try {
          const newToken = await doRefresh()
          if (original.headers) original.headers.Authorization = `Bearer ${newToken}`
          return api(original)
        } catch {
          refreshPromise = null
          try {
            useAuthStore().clearAccessToken()
          } catch {}
          if (import.meta.client && window.location.pathname !== '/login') {
            window.location.href = '/login'
          }
        }
      }
      return Promise.reject(err)
    }
  )
}

export default api
