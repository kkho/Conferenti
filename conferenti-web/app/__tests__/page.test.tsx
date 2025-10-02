import { render, screen } from '@testing-library/react'
import Page from '../page'

// Mock next/navigation
jest.mock('next/navigation', () => ({
  useRouter: () => ({
    push: jest.fn(),
    replace: jest.fn(),
    prefetch: jest.fn(),
    back: jest.fn(),
    forward: jest.fn(),
    refresh: jest.fn(),
  }),
  useSearchParams: () => new URLSearchParams(),
  usePathname: () => '/',
}))

describe('Home Page', () => {
  it('renders without crashing', () => {
    render(<Page />)
    // Add your specific test assertions here
    expect(true).toBe(true) // Placeholder test
  })
})