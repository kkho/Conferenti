import { useQuery, useMutation } from '@tanstack/react-query';
import { useEffect } from 'react';
import { useAuthStore } from '@/stores/authStore';

/**
 * Custom hook to manage access token retrieval and storage
 * Uses React Query for data fetching and Zustand for state management
 * 
 * @param enabled - Whether to fetch the token (default: true)
 * @returns Object with token, loading state, and error from React Query
 */
export function useAccessToken(enabled: boolean = true) {
  const { accessToken, setAccessToken, clearAccessToken } = useAuthStore();

  const { data, isLoading, error, refetch } = useQuery({
    queryKey: ['accessToken'],
    queryFn: async () => {
      const response = await fetch('/api/auth/token', {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json'
        },
        credentials: 'include'
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const result = await response.json();
      return result.accessToken as string;
    },
    enabled,
    staleTime: 5 * 60 * 1000, // 5 minutes
    retry: 1
  });

  // Sync React Query data with Zustand store
  useEffect(() => {
    if (data) {
      setAccessToken(data);
      console.log('✅ Access token retrieved and stored in Zustand');
    }
  }, [data, setAccessToken]);

  // Clear token on error
  useEffect(() => {
    if (error) {
      console.error('❌ Error fetching access token:', error);
      clearAccessToken();
    }
  }, [error, clearAccessToken]);

  return {
    accessToken: data || accessToken,
    isAuthenticated: !!(data || accessToken),
    isLoading,
    error,
    clearToken: clearAccessToken,
    refetch
  };
}

/**
 * Hook to manually refresh the access token
 * Uses React Query mutation for imperative token refresh
 */
export function useRefreshToken() {
  const { setAccessToken, clearAccessToken } = useAuthStore();

  const { mutateAsync: refreshToken, isPending: isRefreshing } = useMutation({
    mutationFn: async () => {
      const response = await fetch('/api/auth/token', {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json'
        },
        credentials: 'include'
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const data = await response.json();
      return data.accessToken as string;
    }
  });

  // Sync with Zustand on successful refresh
  const handleRefresh = async () => {
    try {
      const token = await refreshToken();
      setAccessToken(token);
      console.log('✅ Access token refreshed');
      return token;
    } catch (error) {
      console.error('❌ Error refreshing access token:', error);
      clearAccessToken();
      throw error;
    }
  };

  return { 
    refreshToken: handleRefresh,
    isRefreshing 
  };
}
