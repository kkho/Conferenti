import type { Config } from 'jest'
import nextJest from 'next/jest.js'

const createJestConfig = nextJest({
  // Provide the path to your Next.js app to load next.config.js and .env files
  dir: './',
})

// Add any custom config to be passed to Jest
const config: Config = {
  coverageProvider: 'v8',
  testEnvironment: 'jsdom',
  
  // Setup files
  setupFilesAfterEnv: ['<rootDir>/jest.setup.ts'],
  
  // Module paths
  moduleNameMapping: {
    '^@/(.*)$': '<rootDir>/$1',
    '^@/app/(.*)$': '<rootDir>/app/$1',
    '^@/lib/(.*)$': '<rootDir>/lib/$1',
    '^@/components/(.*)$': '<rootDir>/components/$1',
    '^@/hooks/(.*)$': '<rootDir>/hooks/$1',
  },
  
  // Test match patterns - include app and lib directories
  testMatch: [
    '<rootDir>/app/**/__tests__/**/*.{js,jsx,ts,tsx}',
    '<rootDir>/app/**/*.{test,spec}.{js,jsx,ts,tsx}',
    '<rootDir>/lib/**/__tests__/**/*.{js,jsx,ts,tsx}',
    '<rootDir>/lib/**/*.{test,spec}.{js,jsx,ts,tsx}',
    '<rootDir>/components/**/__tests__/**/*.{js,jsx,ts,tsx}',
    '<rootDir>/components/**/*.{test,spec}.{js,jsx,ts,tsx}',
    '<rootDir>/hooks/**/__tests__/**/*.{js,jsx,ts,tsx}',
    '<rootDir>/hooks/**/*.{test,spec}.{js,jsx,ts,tsx}',
    '<rootDir>/test/**/*.{test,spec}.{js,jsx,ts,tsx}',
    '<rootDir>/__tests__/**/*.{js,jsx,ts,tsx}',
  ],
  
  // Collect coverage from app and lib directories
  collectCoverageFrom: [
    'app/**/*.{js,jsx,ts,tsx}',
    'lib/**/*.{js,jsx,ts,tsx}',
    'components/**/*.{js,jsx,ts,tsx}',
    'hooks/**/*.{js,jsx,ts,tsx}',
    '!**/*.d.ts',
    '!**/node_modules/**',
    '!app/**/layout.{js,jsx,ts,tsx}',
    '!app/**/loading.{js,jsx,ts,tsx}',
    '!app/**/not-found.{js,jsx,ts,tsx}',
    '!app/**/error.{js,jsx,ts,tsx}',
    '!app/globals.css',
  ],
  
  // Transform files
  moduleFileExtensions: ['ts', 'tsx', 'js', 'jsx'],
  
  // Ignore patterns
  testPathIgnorePatterns: [
    '<rootDir>/.next/',
    '<rootDir>/node_modules/',
    '<rootDir>/e2e/',
  ],
  
  // Coverage settings
  coverageDirectory: 'coverage',
  coverageReporters: ['text', 'lcov', 'html'],
  coverageThreshold: {
    global: {
      branches: 70,
      functions: 70,
      lines: 70,
      statements: 70,
    },
  },
}

// createJestConfig is exported this way to ensure that next/jest can load the Next.js config which is async
export default createJestConfig(config)
