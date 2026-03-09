<template>
  <div class="min-h-screen bg-gray-100">
    <AppHeader />

    <main class="max-w-4xl mx-auto p-6">
      <h1 class="text-2xl font-semibold text-gray-800">{{ company?.name ?? 'Company' }}</h1>
      <p class="text-sm text-gray-600 mt-2">Slug: {{ company?.slug }}</p>

      <div class="mt-6 bg-white rounded shadow p-4 flex items-center justify-between">
        <p class="text-sm text-gray-700">Role: {{ company?.role ?? '-' }}</p>
          <button @click="onViewTickets"
            :class="showTickets ? 'px-3 py-1 bg-blue-600 text-white rounded' : 'px-3 py-1 bg-gray-200 rounded'">
            Go to Tickets page
          </button>
      </div>

      <div class="mt-6 flex items-center justify-between">
        <div>
          <button @click="goDashboard" class="px-3 py-1 bg-gray-200 rounded">Back to Dashboard</button>
        </div>
        <div class="flex items-center gap-2">
        
          <button @click="onViewInvitations"
            :class="(!showMembers && !showCategories && !showSla && !showTeams) ? 'px-3 py-1 bg-blue-600 text-white rounded' : 'px-3 py-1 bg-gray-200 rounded'">
            View Invitations
          </button>
          <button @click="onViewMembers"
            :class="showMembers ? 'px-3 py-1 bg-blue-600 text-white rounded' : 'px-3 py-1 bg-gray-200 rounded'">
            View Members
          </button>
          <button @click="onViewCategories"
            :class="showCategories ? 'px-3 py-1 bg-blue-600 text-white rounded' : 'px-3 py-1 bg-gray-200 rounded'">
            View Categories
          </button>
          <button @click="onViewSla"
            :class="showSla ? 'px-3 py-1 bg-blue-600 text-white rounded' : 'px-3 py-1 bg-gray-200 rounded'">
            View SLA
          </button>
          <button @click="onViewTeams"
            :class="showTeams ? 'px-3 py-1 bg-blue-600 text-white rounded' : 'px-3 py-1 bg-gray-200 rounded'">
            View Teams
          </button>
        </div>
      </div>


      <div class="mt-6">
        <div class="mt-3 grid grid-cols-1 gap-4">
          <template v-if="showSla">
            <SlaContent />
          </template>

          <template v-else-if="showCategories">
            <div class="bg-white rounded shadow p-4">
              <div class="mb-4">
                <label class="block text-sm text-gray-700">Add category</label>
                <div class="flex gap-2 mt-2">
                  <input v-model="newCategoryName" placeholder="Category name"
                    class="px-3 py-2 border rounded w-full" />
                  <button @click="onCreateCategory" class="px-3 py-2 bg-blue-600 text-white rounded">Add</button>
                </div>
                <p v-if="categoryError" class="text-sm text-red-500 mt-1">{{ categoryError }}</p>
              </div>

              <div class="overflow-x-auto">
                <table class="min-w-full divide-y divide-gray-200">
                  <thead class="bg-gray-50">
                    <tr>
                      <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Name
                      </th>
                      <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions
                      </th>
                    </tr>
                  </thead>
                  <tbody class="bg-white divide-y divide-gray-100">
                    <tr v-for="c in categories" :key="c.id" class="hover:bg-gray-50 even:bg-gray-50">
                      <td class="px-4 py-3 text-sm text-gray-700 truncate">
                        <template v-if="editingCategoryId === c.id">
                          <input v-model="editingCategoryName" class="px-2 py-1 border rounded w-full" />
                          <p v-if="editingError" class="text-sm text-red-500 mt-1">{{ editingError }}</p>
                        </template>
                        <template v-else>
                          {{ c.name }}
                        </template>
                      </td>
                      <td class="px-4 py-3 text-sm text-gray-700">
                        <template v-if="editingCategoryId === c.id">
                          <button @click="onSaveEdit(c.id)"
                            class="px-2 py-1 bg-green-600 text-white rounded text-sm mr-2">Save</button>
                          <button @click="onCancelEdit()" class="px-2 py-1 bg-gray-200 rounded text-sm">Cancel</button>
                        </template>
                        <template v-else>
                          <button @click="onStartEdit(c.id, c.name)"
                            class="px-2 py-1 bg-yellow-400 text-white rounded text-sm mr-2">Edit</button>
                          <button @click="onDeleteCategory(c.id)"
                            class="px-2 py-1 bg-red-600 text-white rounded text-sm">Delete</button>
                        </template>
                      </td>
                    </tr>
                    <tr v-if="categories.length === 0">
                      <td colspan="2" class="px-4 py-6 text-center text-sm text-gray-500">No categories found.</td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </template>
          <template v-else-if="showMembers">
            <Members />
          </template>
          <template v-else-if="showTeams">
            <Teams />
          </template>
          <template v-else>
            <div>
              <label class="block text-sm text-gray-700">Invite by email</label>
              <div class="flex gap-2 mt-2">
                <input v-model="inviteEmail" placeholder="email@example.com" class="px-3 py-2 border rounded w-full" />
                <select v-model="inviteRole" class="px-3 py-2 border rounded">
                  <option value="Customer">Customer</option>
                  <option value="Agent">Agent</option>
                </select>
                <button @click="onInvite" class="px-3 py-2 bg-blue-600 text-white rounded">Invite</button>
              </div>
              <p v-if="inviteError" class="text-sm text-red-500 mt-1">{{ inviteError }}</p>
            </div>
            <InvitationList :invitations="invitations" :showActions="false" />
          </template>

          <div v-if="!showCategories && companyTotal > companySize" class="flex items-center gap-3">
            <button @click="prevCompanyPage" :disabled="companyPage <= 1"
              class="px-3 py-1 bg-gray-200 rounded">Prev</button>
            <span class="text-sm text-gray-600">Page {{ companyPage }} / {{ Math.max(1, Math.ceil(companyTotal /
              companySize)) }}</span>
            <button @click="nextCompanyPage" :disabled="companyPage >= Math.ceil(companyTotal / companySize)"
              class="px-3 py-1 bg-gray-200 rounded">Next</button>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuth } from '../composables/useAuth'
import { getMe } from '../api/auth'
import { getCurrentCompany } from '../api/companies'
import { getCompanyInvitations, createInvitation } from '../api/invitations'
import { getCompanyMembers } from '../api/members'
import Teams from './Teams.vue'
import { getCategories, createCategory, updateCategory, deleteCategory } from '../api/categories'
import type { CompanyInvitationItem, CategoryItem } from '../types/auth'
import AppHeader from '../components/AppHeader.vue'
import InvitationList from '../components/InvitationList.vue'
import SlaContent from '../components/SlaContent.vue'
import Members from './Members.vue'

const route = useRoute()
const router = useRouter()
const { getUserContextRef } = useAuth()
const company = ref<{ id: string; name: string; slug: string; role?: string } | null>(null)
const invitations = ref<CompanyInvitationItem[]>([])
const members = ref<{ userId: string; email: string; role: string }[]>([])
const showMembers = ref(false)
const loadingMembers = ref(false)
const showTeams = ref(false)
const showTickets = ref(false)
const showCategories = ref(false)
const showSla = ref(false)
const categories = ref<CategoryItem[]>([])
const loadingCategories = ref(false)
const newCategoryName = ref('')
const categoryError = ref('')
const editingCategoryId = ref<string | null>(null)
const editingCategoryName = ref('')
const editingError = ref('')
const inviteEmail = ref('')
const inviteRole = ref('Customer')
const inviteError = ref('')
const companyTotal = ref(0)
const companyPage = ref(1)
const companySize = ref(10)
const userContext = getUserContextRef()

async function load() {
  try {
    const c = await getCurrentCompany()
    if (route.params.slug && String(route.params.slug) !== c.slug) {
      router.replace('/')
      return
    }
    company.value = c
    try {
      await getMe()
    } catch {
      // ignore
    }
    try {
      const paged = await getCompanyInvitations(companyPage.value, companySize.value)
      invitations.value = paged.items
      companyTotal.value = paged.total
    } catch {
      invitations.value = []
    }
  } catch {
    router.replace('/')
  }
}
function goDashboard() {
  router.push('/')
}



async function onInvite() {
  inviteError.value = ''
  if (!inviteEmail.value || !/.+@.+\..+/.test(inviteEmail.value)) {
    inviteError.value = 'Invalid email'
    return
  }
  try {
    await createInvitation(inviteEmail.value, inviteRole.value)
    inviteEmail.value = ''
    // refresh
    if (showMembers.value) {
      const paged = await getCompanyMembers(companyPage.value, companySize.value)
      members.value = paged.items
      companyTotal.value = paged.total
    } else {
      const paged = await getCompanyInvitations(companyPage.value, companySize.value)
      invitations.value = paged.items
      companyTotal.value = paged.total
    }
  } catch (err: unknown) {
    let message = 'Failed to send invitation'
    const axiosErr = err as any
    if (axiosErr?.response) {
      const status = axiosErr.response.status
      const data = axiosErr.response.data
      if (status === 403) {
        message = data?.message ?? 'You do not have permission to invite users.'
      } else if (status === 400) {
        if (data?.errors && Array.isArray(data.errors) && data.errors.length > 0) {
          message = data.errors
            .map((e: any) => e.ErrorMessage ?? e.errorMessage ?? `${e.PropertyName ?? e.propertyName}: ${e.ErrorMessage ?? e.errorMessage}`)
            .join('; ')
        } else {
          message = data?.message ?? 'Bad request'
        }
      } else {
        message = data?.message ?? axiosErr.message ?? message
      }
    } else if (err instanceof Error) {
      message = err.message
    }
    inviteError.value = message
  }
}

async function prevCompanyPage() {
  if (companyPage.value <= 1) return
  companyPage.value -= 1
  if (showMembers.value) {
    const paged = await getCompanyMembers(companyPage.value, companySize.value)
    members.value = paged.items
    companyTotal.value = paged.total
  } else {
    const paged = await getCompanyInvitations(companyPage.value, companySize.value)
    invitations.value = paged.items
    companyTotal.value = paged.total
  }
}

async function nextCompanyPage() {
  const totalPages = Math.max(1, Math.ceil(companyTotal.value / companySize.value))
  if (companyPage.value >= totalPages) return
  companyPage.value += 1
  if (showMembers.value) {
    const paged = await getCompanyMembers(companyPage.value, companySize.value)
    members.value = paged.items
    companyTotal.value = paged.total
  } else {
    const paged = await getCompanyInvitations(companyPage.value, companySize.value)
    invitations.value = paged.items
    companyTotal.value = paged.total
  }
}

async function onViewMembers() {
  showCategories.value = false
  showSla.value = false
  showMembers.value = !showMembers.value
  showTeams.value = false
  if (!showMembers.value) return
  loadingMembers.value = true
  try {
    const paged = await getCompanyMembers(companyPage.value, companySize.value)
    members.value = paged.items
    companyTotal.value = paged.total
  } catch (err) {
    members.value = []
  } finally {
    loadingMembers.value = false
  }
}

async function onViewTeams() {
  showMembers.value = false
  showCategories.value = false
  showSla.value = false
  showTeams.value = !showTeams.value
  if (!showTeams.value) return
}

function onViewTickets() {
  // navigate to tickets list page for this company
  if (!company.value) return
  router.push({ name: 'Tickets', params: { slug: company.value.slug } })
}

async function onViewInvitations() {
  showMembers.value = false
  showCategories.value = false
  showSla.value = false
  try {
    const paged = await getCompanyInvitations(companyPage.value, companySize.value)
    invitations.value = paged.items
    companyTotal.value = paged.total
  } catch {
    invitations.value = []
  }
}

async function onViewCategories() {
  showMembers.value = false
  showSla.value = false
  showCategories.value = !showCategories.value
  if (!showCategories.value) return
  loadingCategories.value = true
  try {
    const data = await getCategories()
    categories.value = data
  } catch (err) {
    categories.value = []
  } finally {
    loadingCategories.value = false
  }
}

function onViewSla() {
  showMembers.value = false
  showCategories.value = false
  showTeams.value = false
  showSla.value = !showSla.value
}

async function onCreateCategory() {
  categoryError.value = ''
  if (!newCategoryName.value || newCategoryName.value.trim() === '') {
    categoryError.value = 'Name is required'
    return
  }
  try {
    await createCategory(newCategoryName.value.trim())
    newCategoryName.value = ''
    const data = await getCategories()
    categories.value = data
  } catch (err: unknown) {
    const axiosErr = err as any
    if (axiosErr?.response) {
      const status = axiosErr.response.status
      const data = axiosErr.response.data
      if (status === 403) categoryError.value = data?.message ?? 'Forbidden'
      else if (status === 409) categoryError.value = data?.message ?? 'Conflict'
      else categoryError.value = data?.message ?? axiosErr.message ?? 'Failed to create category'
    } else if (err instanceof Error) categoryError.value = err.message
  }
}

function onStartEdit(id: string, name: string) {
  editingCategoryId.value = id
  editingCategoryName.value = name
  editingError.value = ''
}

function onCancelEdit() {
  editingCategoryId.value = null
  editingCategoryName.value = ''
  editingError.value = ''
}

async function onSaveEdit(id: string) {
  editingError.value = ''
  if (!editingCategoryName.value || editingCategoryName.value.trim() === '') {
    editingError.value = 'Name is required'
    return
  }
  try {
    await updateCategory(id, editingCategoryName.value.trim())
    editingCategoryId.value = null
    editingCategoryName.value = ''
    const data = await getCategories()
    categories.value = data
  } catch (err: unknown) {
    const axiosErr = err as any
    if (axiosErr?.response) {
      const status = axiosErr.response.status
      const data = axiosErr.response.data
      if (status === 403) editingError.value = data?.message ?? 'Forbidden'
      else if (status === 409) editingError.value = data?.message ?? 'Conflict'
      else editingError.value = data?.message ?? axiosErr.message ?? 'Failed to update category'
    } else if (err instanceof Error) editingError.value = err.message
  }
}

async function onDeleteCategory(id: string) {
  if (!confirm('Delete this category?')) return
  try {
    await deleteCategory(id)
    const data = await getCategories()
    categories.value = data
  } catch (err: unknown) {
    const axiosErr = err as any
    const msg = axiosErr?.response?.data?.message ?? axiosErr?.message ?? 'Failed to delete category'
    alert(msg)
  }
}

onMounted(load)
</script>
