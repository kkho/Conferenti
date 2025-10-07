import { Speaker } from '@/types';
import { useQuery } from '@tanstack/react-query';

export const useGetSpeaker = (speakerId: number) => {
  
}

export const useGetSpeakers = (filter: string) => {
  return useQuery<Speaker[]>({
    queryKey: ['speakers', filter],
    queryFn: async () => {
      const response = await fetch(
        `${process.env.NEXT_PUBLIC_API_URL}/api/speakers?filter=${filter}`,
        {
          method: 'GET',
          headers: {
            'Content-Type': 'application/json'
          }
        }
      );

      if (response.status === 401) {
        return null;
      }

      return response.json();
    }
  });
};
