import api from './axios'
import type { CategoryItem } from '../types/auth'

export async function getCategories(): Promise<CategoryItem[]> {
  const { data } = await api.get<CategoryItem[]>('/categories')
  return data
}

export async function createCategory(name: string): Promise<{ id: string } | void> {
  const { data } = await api.post('/categories', { name })
  return data
}

export async function updateCategory(id: string, name: string): Promise<void> {
  await api.put(`/categories/${encodeURIComponent(id)}`, { name })
}

export async function deleteCategory(id: string): Promise<void> {
  await api.delete(`/categories/${encodeURIComponent(id)}`)
}
