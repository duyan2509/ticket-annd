import { createRouter, createWebHistory } from 'vue-router'
import { useAuth } from '../composables/useAuth'
import { setAccessToken } from '../store/authStore'
import { refresh } from '../api/auth'
import { AppRoles } from '../types/appRoles'

const routes = [
  { path: '/login', name: 'Login', component: () => import('../views/Login.vue'), meta: { guest: true } },
  { path: '/register', name: 'Register', component: () => import('../views/Register.vue'), meta: { guest: true } },
  { path: '/forgot-password', name: 'ForgotPassword', component: () => import('../views/ForgotPassword.vue'), meta: { guest: true } },
  { path: '/reset-password', name: 'ResetPassword', component: () => import('../views/ResetPassword.vue'), meta: { guest: true } },
  { path: '/', name: 'Dashboard', component: () => import('../views/Dashboard.vue'), meta: { requiresAuth: true } },
  { path: '/admin', name: 'Admin', component: () => import('../views/Admin.vue'), meta: { requiresAuth: true, role: AppRoles.SupperAdmin } },
  { path: '/:slug', name: 'Company', component: () => import('../views/Company.vue'), meta: { requiresAuth: true } },
]

let restorePromise: Promise<string | null> | null = null

const router = createRouter({ history: createWebHistory(), routes })

router.beforeEach(async (to, _from, next) => {
  const { isLoggedIn, getUserContext } = useAuth()
  const loggedIn = isLoggedIn()

  if (to.meta.requiresAuth && !loggedIn) {
    if (!restorePromise) {
      restorePromise = refresh()
        .then((data) => {
          setAccessToken(data.accessToken)
          return data.accessToken
        })
        .catch(() => null)
        .finally(() => {
          restorePromise = null
        })
    }
    const token = await restorePromise
    if (!token) return next('/login')
  }

  if (to.meta.requiresAuth && !isLoggedIn()) return next('/login')
  if (to.meta.guest && isLoggedIn()) return next('/')
  if (to.meta.role) {
    const ctx = getUserContext()
    if (ctx?.role !== to.meta.role) return next('/')
  }
  next()
})

export default router
