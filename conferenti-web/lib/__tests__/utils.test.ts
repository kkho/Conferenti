import { formatDate, capitalize, slugify } from '../utils'

describe('Utils', () => {
  describe('formatDate', () => {
    it('formats date correctly', () => {
      const date = new Date('2025-01-01')
      const formatted = formatDate(date)
      expect(formatted).toBe('January 1, 2025')
    })
  })

  describe('capitalize', () => {
    it('capitalizes first letter and lowercases rest', () => {
      expect(capitalize('hello')).toBe('Hello')
      expect(capitalize('WORLD')).toBe('World')
      expect(capitalize('hELLo WoRLD')).toBe('Hello world')
    })
  })

  describe('slugify', () => {
    it('converts text to slug format', () => {
      expect(slugify('Hello World')).toBe('hello-world')
      expect(slugify('Hello, World!')).toBe('hello-world')
      expect(slugify('Multiple   Spaces')).toBe('multiple-spaces')
    })
  })
})