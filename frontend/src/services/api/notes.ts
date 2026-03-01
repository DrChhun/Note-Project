import { http } from '@/services/http'
import type { NoteApi } from '@/types/note'

/** Backend wraps list responses in { payload, page, pageSize, totalCount, totalPages } */
export interface PaginatedResponse<T> {
  payload: T[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
}

/** Backend may wrap single-item responses in { payload } */
interface WrappedResponse<T> {
  payload?: T
}

function unwrapPayload<T>(data: T | (WrappedResponse<T> & T)): T {
  if (data && typeof data === 'object' && 'payload' in data && data.payload !== undefined) {
    return data.payload as T
  }
  return data as T
}

export const getNotes = async (params?: {
  title?: string
  type?: number
}): Promise<NoteApi[]> => {
  const response = await http.get<NoteApi[] | PaginatedResponse<NoteApi>>('/notes/', { params })
  const data = response.data
  if (data && typeof data === 'object' && 'payload' in data && Array.isArray((data as PaginatedResponse<NoteApi>).payload)) {
    return (data as PaginatedResponse<NoteApi>).payload
  }
  return (data as NoteApi[]) ?? []
}

const DEFAULT_PAGE_SIZE = 15

export const getNotesPaginated = async (params: {
  page?: number
  pageSize?: number
  title?: string
  type?: number
}): Promise<PaginatedResponse<NoteApi>> => {
  const { page = 1, pageSize = DEFAULT_PAGE_SIZE, ...rest } = params
  const response = await http.get<PaginatedResponse<NoteApi>>('/notes/', {
    params: { page, pageSize, ...rest },
  })
  const data = response.data
  if (data && typeof data === 'object' && 'payload' in data && Array.isArray((data as PaginatedResponse<NoteApi>).payload)) {
    return data as PaginatedResponse<NoteApi>
  }
  return {
    payload: (data as NoteApi[]) ?? [],
    page: 1,
    pageSize,
    totalCount: 0,
    totalPages: 0,
  }
}

export const getNoteById = async (id: string | number): Promise<NoteApi> => {
  const response = await http.get<NoteApi | WrappedResponse<NoteApi>>(`/notes/${id}`)
  return unwrapPayload(response.data)
}

export const createNote = async (data: {
  title: string
  content: string
  type: number
}): Promise<NoteApi> => {
  const response = await http.post<NoteApi | WrappedResponse<NoteApi>>('/notes/', data)
  return unwrapPayload(response.data)
}

export const updateNote = async (
  id: string | number,
  data: { title: string; content: string; type: number },
): Promise<NoteApi> => {
  const response = await http.put<NoteApi | WrappedResponse<NoteApi>>(`/notes/${id}`, data)
  return unwrapPayload(response.data)
}

export interface DeleteNoteResponse {
  success: boolean
}

export const deleteNote = async (id: string | number): Promise<DeleteNoteResponse> => {
  const response = await http.delete(`/notes/${id}`)
  const success = response.status >= 200 && response.status < 300
  return { success }
}
