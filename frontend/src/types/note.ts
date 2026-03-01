/** Note from API */
export interface NoteApi {
  id: string | number
  title: string
  content: string
  type: number
  createdAt?: string
  updatedAt?: string
}

/** Note type mapping: 0=personal, 1=work, 2=learn */
export const NOTE_TYPE_MAP = {
  0: 'personal',
  1: 'work',
  2: 'learn',
} as const

export type NoteTypeKey = keyof typeof NOTE_TYPE_MAP
