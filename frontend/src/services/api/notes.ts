import { http } from '@/services/http'
import type { NoteApi } from '@/types/note'

export const getNotes = async (params?: {
  title?: string
  type?: number
}): Promise<NoteApi[]> => {
  const response = await http.get<NoteApi[]>('/notes/', { params })
  return response.data
}

export const getNoteById = async (id: string | number): Promise<NoteApi> => {
  const response = await http.get<NoteApi>(`/notes/${id}`)
  return response.data
}

export const createNote = async (data: {
  title: string
  content: string
  type: number
}): Promise<NoteApi> => {
  const response = await http.post<NoteApi>('/notes/', data)
  return response.data
}

export const updateNote = async (
  id: string | number,
  data: { title: string; content: string; type: number },
): Promise<NoteApi> => {
  const response = await http.put<NoteApi>(`/notes/${id}`, data)
  return response.data
}

export const deleteNote = async (id: string | number): Promise<void> => {
  await http.delete(`/notes/${id}`)
}
