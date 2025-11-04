import { useQuery, UseQueryResult } from '@tanstack/react-query';
import { renderHook } from '@testing-library/react';
import { useGetSessions } from './sessions';

jest.mock('@tanstack/react-query', () => ({
  useQuery: jest.fn()
}));

global.fetch = jest.fn();

describe('useGetSessions', () => {
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

    renderHook(() => useGetSessions());
    expect(useQuery).toHaveBeenCalledWith({
      queryKey: ['sessions'],
      queryFn: expect.any(Function)
    });
  });

  it('should call useQuery with correct parameters', () => {
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

    const { result } = renderHook(() => useGetSessions());
    expect(result.current.isLoading).toBe(true);
    expect(result.current.data).toBeUndefined();
    expect(result.current.isError).toBe(false);
  });

  it('should handle empty sessions array', () => {
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

    const { result } = renderHook(() => useGetSessions());
    expect(result.current.data).toEqual([]);
    expect(result.current.isSuccess).toBe(true);
    expect(Array.isArray(result.current.data)).toBe(true);
    expect(result.current.data?.length).toBe(0);
  });

  it('should expose refetch function', () => {
    const { result } = renderHook(() => useGetSessions());
    expect(result.current.refetch).toBe(mockRefetch);
  });

  it('should call refetch when invoked', () => {
    const { result } = renderHook(() => useGetSessions());

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

    const { result } = renderHook(() => useGetSessions());

    expect(result.current.error).toBe(mockError);
    expect(result.current.error?.message).toBe('HTTP error! status: 400');
    expect(result.current.isError).toBe(true);
    expect(result.current.isSuccess).toBe(false);
    expect(result.current.data).toBeUndefined();
  });

  it('should call useGetSessions when data is received', () => {
    const mockData = [
      { id: 1, description: 'John Doe' },
      { id: 2, description: 'Jane Doe' }
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

    const { result } = renderHook(() => useGetSessions());

    expect(result.current.data).toEqual(mockData);
    expect(result.current.isError).toBe(false);
    expect(result.current.isSuccess).toBe(true);
  });
});
