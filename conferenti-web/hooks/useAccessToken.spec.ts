import { renderHook, waitFor } from '@testing-library/react';
import { useAccessToken } from './useAccessToken';
import { useQuery } from '@tanstack/react-query';
import type { UseQueryResult } from '@tanstack/react-query';

// Mock the entire React Query module
jest.mock('@tanstack/react-query', () => ({
  useQuery: jest.fn(),
  useMutation: jest.fn()
}));

// Mock the authStore with correct syntax
jest.mock('@/stores/authStore', () => ({
  useAuthStore: jest.fn()
}));

// Mock fetch
global.fetch = jest.fn();

describe('useAccessToken', () => {
  const originalEnv = process.env;
  const mockSetAccessToken = jest.fn();
  const mockClearAccessToken = jest.fn();
  const mockRefetch = jest.fn();
  const mockUseQuery = useQuery as jest.MockedFunction<typeof useQuery>;

  beforeEach(() => {
    jest.clearAllMocks();
    process.env = { ...originalEnv };

    // Setup authStore mock to return the expected functions
    const { useAuthStore } = jest.requireMock('@/stores/authStore');
    useAuthStore.mockReturnValue({
      accessToken: null,
      setAccessToken: mockSetAccessToken,
      clearAccessToken: mockClearAccessToken
    });

    // Setup useQuery mock to return default values
    mockUseQuery.mockReturnValue({
      data: undefined,
      isLoading: false,
      error: null,
      refetch: mockRefetch,
      isError: false,
      isSuccess: false,
      status: 'pending',
      fetchStatus: 'idle',
    } as unknown as UseQueryResult);
  });

  afterAll(() => {
    process.env = originalEnv;
  });

  it('should call useQuery with correct parameters', () => {
    renderHook(() => useAccessToken());
    
    expect(mockUseQuery).toHaveBeenCalledWith({
      queryKey: ['accessToken'],
      queryFn: expect.any(Function),
      enabled: true,
      staleTime: 5 * 60 * 1000, // 5 minutes
      retry: 1
    });
  });

  it('should call useQuery with enabled=false when passed', () => {
    renderHook(() => useAccessToken(false));
    
    expect(mockUseQuery).toHaveBeenCalledWith(
      expect.objectContaining({
        enabled: false
      })
    );
  });

  it('should return accessToken from useQuery data', () => {
    mockUseQuery.mockReturnValue({
      data: 'test-token-123',
      isLoading: false,
      error: null,
      refetch: mockRefetch,
      isError: false,
      isSuccess: true,
      status: 'success',
      fetchStatus: 'idle',
    } as unknown as UseQueryResult);

    const { result } = renderHook(() => useAccessToken());
    
    expect(result.current.accessToken).toBe('test-token-123');
    expect(result.current.isLoading).toBe(false);
    expect(result.current.error).toBe(null);
    expect(result.current.isAuthenticated).toBe(true);
  });

  it('should return accessToken from authStore when no data from query', () => {
    const { useAuthStore } = jest.requireMock('@/stores/authStore');
    useAuthStore.mockReturnValue({
      accessToken: 'stored-token-456',
      setAccessToken: mockSetAccessToken,
      clearAccessToken: mockClearAccessToken
    });

    mockUseQuery.mockReturnValue({
      data: undefined,
      isLoading: false,
      error: null,
      refetch: mockRefetch,
      isError: false,
      isSuccess: false,
      status: 'pending',
      fetchStatus: 'idle',
    } as unknown as UseQueryResult);

    const { result } = renderHook(() => useAccessToken());
    
    expect(result.current.accessToken).toBe('stored-token-456');
    expect(result.current.isAuthenticated).toBe(true);
  });

  it('should call setAccessToken when data is received', async () => {
    mockUseQuery.mockReturnValue({
      data: 'new-token-456',
      isLoading: false,
      error: null,
      refetch: mockRefetch,
      isError: false,
      isSuccess: true,
      status: 'success',
      fetchStatus: 'idle',
    } as unknown as UseQueryResult);

    renderHook(() => useAccessToken());

    await waitFor(() => {
      expect(mockSetAccessToken).toHaveBeenCalledWith('new-token-456');
    });
  });

  it('should call clearAccessToken when error occurs', async () => {
    const mockError = new Error('Failed to fetch token');
    mockUseQuery.mockReturnValue({
      data: undefined,
      isLoading: false,
      error: mockError,
      refetch: mockRefetch,
      isError: true,
      isSuccess: false,
      status: 'error',
      fetchStatus: 'idle',
    } as unknown as UseQueryResult);

    renderHook(() => useAccessToken());

    await waitFor(() => {
      expect(mockClearAccessToken).toHaveBeenCalled();
    });
  });

  it('should return isAuthenticated as false when no token', () => {
    mockUseQuery.mockReturnValue({
      data: undefined,
      isLoading: false,
      error: null,
      refetch: mockRefetch,
      isError: false,
      isSuccess: false,
      status: 'pending',
      fetchStatus: 'idle',
    } as unknown as UseQueryResult);

    const { result } = renderHook(() => useAccessToken());
    
    expect(result.current.accessToken).toBe(null);
    expect(result.current.isAuthenticated).toBe(false);
  });

  it('should expose refetch function', () => {
    const { result } = renderHook(() => useAccessToken());
    
    expect(result.current.refetch).toBe(mockRefetch);
  });

  it('should expose clearToken function', () => {
    const { result } = renderHook(() => useAccessToken());
    
    result.current.clearToken();
    expect(mockClearAccessToken).toHaveBeenCalled();
  });

  describe('queryFn', () => {
    it('should fetch token from API endpoint', async () => {
      const mockFetch = global.fetch as jest.MockedFunction<typeof fetch>;
      mockFetch.mockResolvedValueOnce({
        ok: true,
        json: async () => ({ accessToken: 'api-token-123' })
      } as Response);

      // Get the queryFn that was passed to useQuery
      renderHook(() => useAccessToken());
      const queryConfig = mockUseQuery.mock.calls[0][0];
      const queryFn = queryConfig.queryFn as (context: unknown) => Promise<string>;

      if (queryFn) {
        const result = await queryFn({});

        expect(mockFetch).toHaveBeenCalledWith('/api/auth/token', {
          method: 'GET',
          headers: {
            'Content-Type': 'application/json'
          },
          credentials: 'include'
        });
        expect(result).toBe('api-token-123');
      }
    });

    it('should throw error when API request fails', async () => {
      const mockFetch = global.fetch as jest.MockedFunction<typeof fetch>;
      mockFetch.mockResolvedValueOnce({
        ok: false,
        status: 401
      } as Response);

      renderHook(() => useAccessToken());
      const queryConfig = mockUseQuery.mock.calls[0][0];
      const queryFn = queryConfig.queryFn as (context: unknown) => Promise<string>;

      if (queryFn) {
        await expect(queryFn({})).rejects.toThrow('HTTP error! status: 401');
      }
    });
  });
});