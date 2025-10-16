import { renderHook } from '@testing-library/react';
import { useGetSpeakers } from './speakers';
import { useQuery, UseQueryResult } from '@tanstack/react-query';

// Mock the entire React Query module
jest.mock('@tanstack/react-query', () => ({
  useQuery: jest.fn()
}));

// Mock fetch
global.fetch = jest.fn();

describe('useGetSpeakers', () => {
  const originalEnv = process.env;
  const mockUseQuery = useQuery as jest.MockedFunction<typeof useQuery>;
  const mockRefetch = jest.fn();

  beforeEach(() => {
    jest.clearAllMocks();
    process.env = { ...originalEnv };

    mockUseQuery.mockReturnValue({
      data: undefined,
      isLoading: false,
      error: null,
      refetch: mockRefetch,
      isError: false,
      isSuccess: false,
      status: 'pending',
      fetchStatus: 'idle'
    } as unknown as UseQueryResult);
  });

  afterAll(() => {
    process.env = originalEnv;
  });

  it('should call useQuery with correct parameters', () => {
    const { useQuery } = jest.requireMock('@tanstack/react-query');

    renderHook(() => useGetSpeakers());

    expect(useQuery).toHaveBeenCalledWith({
      queryKey: ['speakers'],
      queryFn: expect.any(Function)
    });
  });

  it('should return loading state when fetching speakers', () => {
    mockUseQuery.mockReturnValue({
      data: undefined,
      isLoading: true,
      error: null,
      refetch: mockRefetch,
      isError: false,
      isSuccess: false,
      status: 'pending',
      fetchStatus: 'fetching'
    } as unknown as UseQueryResult);

    const { result } = renderHook(() => useGetSpeakers());

    expect(result.current.isLoading).toBe(true);
    expect(result.current.data).toBeUndefined();
    expect(result.current.isError).toBe(false);
  });

  it('should handle empty speakers array', () => {
    mockUseQuery.mockReturnValue({
      data: [],
      isLoading: false,
      error: null,
      refetch: mockRefetch,
      isError: false,
      isSuccess: true,
      status: 'success',
      fetchStatus: 'idle'
    } as unknown as UseQueryResult);

    const { result } = renderHook(() => useGetSpeakers());

    expect(result.current.data).toEqual([]);
    expect(result.current.isSuccess).toBe(true);
    expect(Array.isArray(result.current.data)).toBe(true);
    expect(result.current.data?.length).toBe(0);
  });

  it('should expose refetch function', () => {
    const { result } = renderHook(() => useGetSpeakers());

    expect(result.current.refetch).toBe(mockRefetch);
    expect(typeof result.current.refetch).toBe('function');
  });

  it('should call refetch when invoked', () => {
    const { result } = renderHook(() => useGetSpeakers());

    result.current.refetch();

    expect(mockRefetch).toHaveBeenCalled();
    expect(mockRefetch).toHaveBeenCalledTimes(1);
  });

  it('should throw error when error occurs', () => {
    const mockError = new Error('HTTP error! status: 400');
    mockUseQuery.mockReturnValue({
      data: undefined,
      isLoading: false,
      error: mockError,
      refetch: mockRefetch,
      isError: true,
      isSuccess: false,
      status: 'error',
      fetchStatus: 'idle'
    } as unknown as UseQueryResult);

    const { result } = renderHook(() => useGetSpeakers());

    expect(result.current.error).toBe(mockError);
    expect(result.current.error?.message).toBe('HTTP error! status: 400');
    expect(result.current.isError).toBe(true);
    expect(result.current.isSuccess).toBe(false);
    expect(result.current.data).toBeUndefined();
  });

  it('should call useGetSpeakers when data is received', () => {
    const mockData = [
      { id: 1, name: 'John Doe' },
      { id: 2, name: 'Jane Doe' }
    ];
    mockUseQuery.mockReturnValue({
      data: mockData,
      isLoading: false,
      error: null,
      refetch: mockRefetch,
      isError: false,
      isSuccess: true,
      status: 'success',
      fetchStatus: 'idle'
    } as unknown as UseQueryResult);

    const { result } = renderHook(() => useGetSpeakers());

    expect(result.current.data).toEqual(mockData);
    expect(result.current.isError).toBe(false);
    expect(result.current.isSuccess).toBe(true);
  });
});
