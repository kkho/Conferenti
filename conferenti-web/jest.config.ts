import type { Config } from 'jest';
import nextJest from 'next/jest.js';

const createJestConfig = nextJest({
  // Provide the path to your Next.js app to load next.config.js and .env files
  dir: './'
});

// Add any custom config to be passed to Jest
const config: Config = {
  coverageProvider: 'v8',
  testEnvironment: 'jsdom',

  // Setup files
  setupFilesAfterEnv: ['<rootDir>/jest.setup.tsx'],

  // GitHub Actions optimizations
  ...(process.env.CI && {
    // Use multiple workers in CI for faster parallel execution
    maxWorkers: '50%',
    // Cache directory for CI
    cacheDirectory: '/tmp/jest_rs',
    // Verbose output for CI debugging
    verbose: true,
    // Force exit to prevent hanging in CI
    forceExit: true,
    // Detect open handles in CI
    detectOpenHandles: true,
    // Fail fast in CI - stop on first test failure
    bail: 1,
    // CI-optimized timeouts
    testTimeout: 30000,
    // Silent mode in CI to reduce log noise
    silent: false,
    // Coverage in CI
    collectCoverage: true,
    // CI-specific reporters
    reporters: [
      'default',
      [
        'jest-junit',
        {
          outputDirectory: 'test-results',
          outputName: 'junit.xml',
          suiteNameTemplate: '{filepath}',
          classNameTemplate: '{classname}',
          titleTemplate: '{title}'
        }
      ]
    ]
  }),

  // Module paths
  moduleNameMapper: {
    '^@/(.*)$': '<rootDir>/$1',
    '^@/app/(.*)$': '<rootDir>/app/$1',
    '^@/lib/(.*)$': '<rootDir>/lib/$1',
    '^@/components/(.*)$': '<rootDir>/components/$1',
    '^@/hooks/(.*)$': '<rootDir>/hooks/$1'
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
    '<rootDir>/__tests__/**/*.{js,jsx,ts,tsx}'
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
    '!app/globals.css'
  ],

  // Transform files
  moduleFileExtensions: ['ts', 'tsx', 'js', 'jsx'],

  // Ignore patterns
  testPathIgnorePatterns: [
    '<rootDir>/.next/',
    '<rootDir>/node_modules/',
    '<rootDir>/e2e/'
  ],
  transformIgnorePatterns: ['/node_modules/(?!(hooks)/)'],

  // Coverage settings
  coverageDirectory: 'coverage',
  coverageReporters: process.env.CI
    ? ['text', 'lcov', 'clover', 'json'] // CI optimized reporters
    : ['text', 'lcov', 'html'], // Local development reporters
  coverageThreshold: {
    global: {
      branches: 50,
      functions: 50,
      lines: 50,
      statements: 50
    }
  },

  // GitHub Actions specific settings
  ...(process.env.GITHUB_ACTIONS && {
    // GitHub Actions specific cache directory
    cacheDirectory: '/tmp/jest_cache',
    // Optimized for GitHub Actions runners (2 CPU cores typically)
    maxWorkers: 2
  })
};

// createJestConfig is exported this way to ensure that next/jest can load the Next.js config which is async
export default createJestConfig(config);
