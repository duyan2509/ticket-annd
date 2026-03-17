import { refresh } from '~/api/auth'
import { useAuth } from '~/composables/useAuth'

export default defineNuxtPlugin(
    async () => {
        const { isLoggedIn, setTokens } = useAuth()
        const publicAuthPages = ['/login', '/register', '/forgot-password', '/reset-password']

        if (publicAuthPages.includes(window.location.pathname)) {
            return
        }

        if (!isLoggedIn()) {
            try {
                const result = await refresh()
                setTokens(result.accessToken)
            } catch {
                await navigateTo('/login')
            }
        }
    })
