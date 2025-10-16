import { Speaker } from '@/types';
import { useQuery } from '@tanstack/react-query';

export const useGetSpeakers = () => {
  return useQuery<Speaker[]>({
    queryKey: ['speakers'],
    queryFn: async () => {
      const response = await fetch('/api/speakers', {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json'
        }
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      return response.json();
    }
  });
};
