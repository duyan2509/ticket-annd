import { defineStore } from 'pinia'
import { ref } from 'vue'

export interface MeCache {
  id: string
  email: string
  isActive: boolean
  currentRole: string
  companyName?: string | null
}

export interface CurrentCompany {
  id: string
  name: string
  slug: string
  role?: string
}

export const useAuthStore = defineStore('auth', () => {
  const accessToken = ref<string | null>(null)
  const me = ref<MeCache | null>(null)
  const currentCompany = ref<CurrentCompany | null>(null)

  function getAccessToken() {
    return accessToken.value
  }
  function setAccessToken(token: string) {
    accessToken.value = token
  }
  function clearAccessToken() {
    accessToken.value = null
    me.value = null
    currentCompany.value = null
  }

  function getMeCache() {
    return me.value
  }
  function setMeCache(data: MeCache | null) {
    me.value = data
  }

  function getUserContextRef() {
    return me
  }

  function setCurrentCompany(c: CurrentCompany | null) {
    currentCompany.value = c
  }
  function getCurrentCompanyRef() {
    return currentCompany
  }

  return {
    accessToken,
    me,
    currentCompany,
    getAccessToken,
    setAccessToken,
    clearAccessToken,
    getMeCache,
    setMeCache,
    getUserContextRef,
    setCurrentCompany,
    getCurrentCompanyRef,
  }
})

export default useAuthStore
