<template>
  <div class="min-h-screen bg-gray-100">
    <main class="max-w-4xl mx-auto p-6">
      <h1 class="text-2xl font-semibold text-gray-800">{{ company?.name ?? 'Company' }}</h1>
      <p class="text-sm text-gray-600 mt-2">Slug: {{ company?.slug ?? '-' }}</p>

      <div class="mt-6 bg-white rounded shadow p-4 flex items-center justify-between">
        <p class="text-sm text-gray-700">Role: {{ company?.role ?? '-' }}</p>
        <button
          @click="onViewTickets"
          class="px-3 py-1 bg-blue-600 text-white rounded"
        >
          Go to Tickets page
        </button>
      </div>

      <div class="mt-6 flex items-center justify-between gap-4 flex-wrap">
        <button @click="goDashboard" class="px-3 py-1 bg-gray-200 rounded">Back to Dashboard</button>

        <div class="flex items-center gap-4 flex-wrap text-sm">
          <label class="inline-flex items-center gap-2">
            <input v-model="selectedView" type="radio" value="invitations" />
            <span>Invitations</span>
          </label>
          <label class="inline-flex items-center gap-2">
            <input v-model="selectedView" type="radio" value="members" />
            <span>Members</span>
          </label>
          <label class="inline-flex items-center gap-2">
            <input v-model="selectedView" type="radio" value="categories" />
            <span>Categories</span>
          </label>
          <label class="inline-flex items-center gap-2">
            <input v-model="selectedView" type="radio" value="sla" />
            <span>SLA</span>
          </label>
          <label class="inline-flex items-center gap-2">
            <input v-model="selectedView" type="radio" value="teams" />
            <span>Teams</span>
          </label>
        </div>
      </div>

      <div class="mt-6">
        <template v-if="selectedView === 'sla'">
          <SlaContent />
        </template>

        <template v-else-if="selectedView === 'categories'">
          <div class="bg-white rounded shadow p-4">
            <div class="mb-4">
              <CategoryForm @submit="onCreateCategory" :error="errors.category" />
            </div>

            <div class="overflow-x-auto">
              <table class="min-w-full divide-y divide-gray-200">
                <thead class="bg-gray-50">
                  <tr>
                    <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Name</th>
                    <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
                  </tr>
                </thead>
                <tbody class="bg-white divide-y divide-gray-100">
                  <tr v-for="category in categories" :key="category.id" class="hover:bg-gray-50 even:bg-gray-50">
                    <td class="px-4 py-3 text-sm text-gray-700 truncate">
                      <template v-if="editingCategory.id === category.id">
                        <input v-model="editingCategory.name" class="px-2 py-1 border rounded w-full" />
                        <p v-if="editingCategory.error" class="text-sm text-red-500 mt-1">{{ editingCategory.error }}</p>
                      </template>
                      <template v-else>
                        {{ category.name }}
                      </template>
                    </td>
                    <td class="px-4 py-3 text-sm text-gray-700">
                      <template v-if="editingCategory.id === category.id">
                        <button @click="onSaveEdit(category.id)" class="px-2 py-1 bg-green-600 text-white rounded text-sm mr-2">Save</button>
                        <button @click="onCancelEdit" class="px-2 py-1 bg-gray-200 rounded text-sm">Cancel</button>
                      </template>
                      <template v-else>
                        <button @click="onStartEdit(category.id, category.name)" class="px-2 py-1 bg-yellow-500 text-white rounded text-sm mr-2">Edit</button>
                        <button @click="onDeleteCategory(category.id)" class="px-2 py-1 bg-red-600 text-white rounded text-sm">Delete</button>
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

        <template v-else-if="selectedView === 'members'">
          <Members />
        </template>

        <template v-else-if="selectedView === 'teams'">
          <Teams />
        </template>

        <template v-else>
          <div class="space-y-4">
            <div class="bg-white rounded shadow p-4">
              <InviteForm @submit="onInvite" :error="errors.invite" />
            </div>
            <InvitationList :invitations="invitations" :show-actions="false" />
          </div>
        </template>

        <div
          v-if="selectedView === 'invitations' && invitationPagination.total > invitationPagination.size"
          class="flex items-center gap-3 mt-4"
        >
          <button @click="prevCompanyPage" :disabled="invitationPagination.page <= 1" class="px-3 py-1 bg-gray-200 rounded">Prev</button>
          <span class="text-sm text-gray-600">Page {{ invitationPagination.page }} / {{ Math.max(1, Math.ceil(invitationPagination.total / invitationPagination.size)) }}</span>
          <button @click="nextCompanyPage" :disabled="invitationPagination.page >= Math.ceil(invitationPagination.total / invitationPagination.size)" class="px-3 py-1 bg-gray-200 rounded">Next</button>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, watch } from 'vue'
import { getMe } from '~/api/auth'
import { getCurrentCompany } from '~/api/companies'
import { getCompanyInvitations, createInvitation } from '~/api/invitations'
import { getCategories, createCategory, updateCategory, deleteCategory } from '~/api/categories'
import type { CompanyInvitationItem, CategoryItem, CurrentCompany } from '~/types/auth'
import InvitationList from '~/components/InvitationList.vue'
import CategoryForm from '~/components/CategoryForm.vue'
import InviteForm from '~/components/InviteForm.vue'
import Members from '~/components/Members.vue'
import Teams from '~/components/Teams.vue'
import SlaContent from '~/components/SlaContent.vue'
import useAuthStore from '~/store/authStore'

definePageMeta({
  middleware: 'auth',
})

type CompanyView = 'invitations' | 'members' | 'categories' | 'sla' | 'teams'

const route = useRoute()
const company = ref<CurrentCompany | null>(null)

const selectedView = ref<CompanyView>('invitations')

const invitations = ref<CompanyInvitationItem[]>([])
const categories = ref<CategoryItem[]>([])

const errors = reactive({
  category: '',
  invite: '',
})
const editingCategory = reactive({
  id: null as string | null,
  name: '',
  error: '',
})

const invitationPagination = reactive({
  total: 0,
  page: 1,
  size: 2,
})

async function load() {
  try {
    let currentCompany: CurrentCompany | null = useAuthStore().getCurrentCompanyRef().value
    if (!currentCompany) {
      try {
        currentCompany = await getCurrentCompany()
      } catch {
        currentCompany = null
      }
    }
    const slugParam = route.params.slug ? String(route.params.slug) : null
    if (!slugParam || !currentCompany || slugParam !== currentCompany.slug) {
      await navigateTo('/dashboard', { replace: true })
      return
    }

    company.value = currentCompany
    try {
      await getMe()
    } catch {
      // ignore
    }

    await loadByView()
  } catch {
    await navigateTo('/dashboard', { replace: true })
  }
}

async function loadByView() {
  if (selectedView.value === 'categories') {
    try {
      categories.value = await getCategories()
    } catch {
      categories.value = []
    }
    return
  }

  if (selectedView.value === 'sla') {
    return
  }

  if (selectedView.value === 'members' || selectedView.value === 'teams') {
    return
  }

  try {
    const paged = await getCompanyInvitations(invitationPagination.page, invitationPagination.size)
    invitations.value = paged.items
    invitationPagination.total = paged.total
  } catch {
    invitations.value = []
    invitationPagination.total = 0
  }
}

function goDashboard() {
  navigateTo('/dashboard')
}

function onViewTickets() {
  const slug = route.params.slug ? String(route.params.slug) : ''
  if (!slug) return
  if (company.value) {
    useAuthStore().setCurrentCompany(company.value)
  }
  navigateTo(`/${slug}/tickets`)
}

async function onInvite(payload: { email: string; role: string }) {
  errors.invite = ''
  if (!payload.email || !/.+@.+\..+/.test(payload.email)) {
    errors.invite = 'Invalid email'
    return
  }

  try {
    await createInvitation(payload.email, payload.role)
    await loadByView()
  } catch (error: unknown) {
    const axiosErr = error as any
    errors.invite = axiosErr?.response?.data?.message ?? axiosErr?.message ?? 'Failed to send invitation'
  }
}

async function prevCompanyPage() {
  if (invitationPagination.page <= 1) return
  invitationPagination.page -= 1
  await loadByView()
}

async function nextCompanyPage() {
  const totalPages = Math.max(1, Math.ceil(invitationPagination.total / invitationPagination.size))
  if (invitationPagination.page >= totalPages) return
  invitationPagination.page += 1
  await loadByView()
}

async function onCreateCategory(name: string) {
  errors.category = ''
  if (!name || name.trim() === '') {
    errors.category = 'Name is required'
    return
  }

  try {
    await createCategory(name.trim())
    categories.value = await getCategories()
  } catch (error: unknown) {
    const axiosErr = error as any
    errors.category = axiosErr?.response?.data?.message ?? axiosErr?.message ?? 'Failed to create category'
  }
}

function onStartEdit(id: string, name: string) {
  editingCategory.id = id
  editingCategory.name = name
  editingCategory.error = ''
}

function onCancelEdit() {
  editingCategory.id = null
  editingCategory.name = ''
  editingCategory.error = ''
}

async function onSaveEdit(id: string) {
  editingCategory.error = ''
  if (!editingCategory.name || editingCategory.name.trim() === '') {
    editingCategory.error = 'Name is required'
    return
  }

  try {
    await updateCategory(id, editingCategory.name.trim())
    onCancelEdit()
    categories.value = await getCategories()
  } catch (error: unknown) {
    const axiosErr = error as any
    editingCategory.error = axiosErr?.response?.data?.message ?? axiosErr?.message ?? 'Failed to update category'
  }
}

async function onDeleteCategory(id: string) {
  if (!confirm('Delete this category?')) return
  try {
    await deleteCategory(id)
    categories.value = await getCategories()
  } catch {
    // ignore
  }
}

watch(selectedView, async () => {
  invitationPagination.page = 1
  await loadByView()
})

onMounted(load)
</script>