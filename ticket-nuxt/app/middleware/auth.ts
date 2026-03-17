export default defineNuxtRouteMiddleware(() => {
  if (import.meta.server) {
    return
  }

  const { isLoggedIn } = useAuth()

  if (!isLoggedIn()) {
    return navigateTo('/login')
  }
})