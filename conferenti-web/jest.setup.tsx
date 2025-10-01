import '@testing-library/jest-dom'
import React from 'react'

// Mock Next.js router
jest.mock('next/navigation', () => ({
  useRouter() {
    return {
      push: jest.fn(),
      replace: jest.fn(),
      prefetch: jest.fn(),
      back: jest.fn(),
      forward: jest.fn(),
      refresh: jest.fn(),
    }
  },
  useSearchParams() {
    return new URLSearchParams()
  },
  usePathname() {
    return '/'
  },
}))

// Mock Next.js Image component
jest.mock('next/image', () => {
  const MockedImage = (props: Record<string, unknown>) => {
    // Extract Next.js specific props to filter them out
    const nextJSProps = [
      'priority', 'placeholder', 'blurDataURL', 'quality', 'fill', 
      'sizes', 'loader', 'unoptimized', 'onLoad', 'onLoadingComplete', 'onError'
    ]
    
    // Create clean props object without Next.js specific props
    const cleanProps = Object.fromEntries(
      Object.entries(props).filter(([key]) => !nextJSProps.includes(key))
    )
    
    // Ensure alt is always present
    const imgProps = {
      ...cleanProps,
      alt: (cleanProps.alt as string) || ''
    }
    
    // eslint-disable-next-line @next/next/no-img-element
    return <img {...imgProps} alt={imgProps.alt} />
  }
  MockedImage.displayName = 'Image'
  return {
    __esModule: true,
    default: MockedImage,
  }
})

// Mock Auth0
jest.mock('@auth0/nextjs-auth0', () => ({
  useUser: () => ({
    user: null,
    error: null,
    isLoading: false,
  }),
  withPageAuthRequired: (component: unknown) => component,
  withApiAuthRequired: (handler: unknown) => handler,
}))

// Mock Heroicons
jest.mock('@heroicons/react/24/outline', () => ({
  Bars3Icon: () => <svg data-testid="bars3-icon" />,
  BellIcon: () => <svg data-testid="bell-icon" />,
  XMarkIcon: () => <svg data-testid="xmark-icon" />,
}))

// Mock React Query
jest.mock('@tanstack/react-query', () => ({
  QueryClient: jest.fn().mockImplementation(() => ({
    setQueryData: jest.fn(),
    getQueryData: jest.fn(),
    invalidateQueries: jest.fn(),
    clear: jest.fn(),
    removeQueries: jest.fn(),
  })),
  QueryClientProvider: ({ children }: { children: React.ReactNode }) => (
    <div data-testid="query-client-provider">{children}</div>
  ),
  useQuery: jest.fn(),
  useMutation: jest.fn(),
  useQueryClient: jest.fn(),
}))