import React from 'react';
import { render, screen } from '@testing-library/react';
import { ReactQueryClientProvider } from './index';
import { QueryClient } from '@tanstack/react-query';

// QueryClient is already mocked in jest.setup.tsx
const MockedQueryClient = jest.mocked(QueryClient);

describe('ReactQueryClientProvider', () => {
  beforeEach(() => {
    // Clear mock calls before each test
    MockedQueryClient.mockClear();
  });

  it('renders without crashing', () => {
    render(
      <ReactQueryClientProvider>
        <div>Test content</div>
      </ReactQueryClientProvider>
    );
    
    // Should not throw any errors
    expect(true).toBe(true);
  });

  it('renders children correctly', () => {
    const testContent = 'Hello, React Query!';
    
    render(
      <ReactQueryClientProvider>
        <div data-testid="test-child">{testContent}</div>
      </ReactQueryClientProvider>
    );

    // Check that children are rendered
    expect(screen.getByTestId('test-child')).toBeInTheDocument();
    expect(screen.getByText(testContent)).toBeInTheDocument();
  });

  it('wraps children with QueryClientProvider', () => {
    render(
      <ReactQueryClientProvider>
        <span>Wrapped content</span>
      </ReactQueryClientProvider>
    );

    // Check that the QueryClientProvider wrapper is present
    expect(screen.getByTestId('query-client-provider')).toBeInTheDocument();
    expect(screen.getByText('Wrapped content')).toBeInTheDocument();
  });

  it('creates a new QueryClient instance', () => {
    render(
      <ReactQueryClientProvider>
        <div>Test</div>
      </ReactQueryClientProvider>
    );

    // Verify QueryClient constructor was called
    expect(MockedQueryClient).toHaveBeenCalledTimes(1);
  });

  it('handles multiple children', () => {
    render(
      <ReactQueryClientProvider>
        <div data-testid="child-1">First child</div>
        <div data-testid="child-2">Second child</div>
        <span data-testid="child-3">Third child</span>
      </ReactQueryClientProvider>
    );

    // All children should be rendered
    expect(screen.getByTestId('child-1')).toBeInTheDocument();
    expect(screen.getByTestId('child-2')).toBeInTheDocument();
    expect(screen.getByTestId('child-3')).toBeInTheDocument();
    expect(screen.getByText('First child')).toBeInTheDocument();
    expect(screen.getByText('Second child')).toBeInTheDocument();
    expect(screen.getByText('Third child')).toBeInTheDocument();
  });

  it('handles null or undefined children gracefully', () => {
    render(
      <ReactQueryClientProvider>
        {null}
        {undefined}
        <div>Valid child</div>
      </ReactQueryClientProvider>
    );

    // Should still render the valid child
    expect(screen.getByText('Valid child')).toBeInTheDocument();
    expect(screen.getByTestId('query-client-provider')).toBeInTheDocument();
  });
});