'use client';
import { ReactNode, useState } from 'react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';


const ReactQueryClientProvider = ({ children }: { children: ReactNode }) => {
  // Create QueryClient only once per session to preserve cache
  const [queryClient] = useState(
    () =>
      new QueryClient({
        defaultOptions: {
          queries: {
            // Data stays fresh for 10 minutes - instant navigation
            staleTime: 10 * 60 * 1000,
            // Keep data in cache for 30 minutes even if unused
            gcTime: 30 * 60 * 1000,
            // Don't refetch on window focus (reduce API calls)
            refetchOnWindowFocus: false,
            // Don't refetch on mount if data is fresh
            refetchOnMount: false,
            // Don't refetch on reconnect
            refetchOnReconnect: false,
            // Retry failed requests once
            retry: 1
          }
        }
      })
  );
  return <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>;
};

export { ReactQueryClientProvider };