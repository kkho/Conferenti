import { create } from 'zustand';
import { persist } from 'zustand/middleware';

interface AuthStore {
  accessToken: string | null;
  setAccessToken: (token: string | null) => void;
  clearAccessToken: () => void;
}

// Zustand store for authentication state
export const useAuthStore = create<AuthStore>()(
  persist(
    (set) => ({
      accessToken: null,
      setAccessToken: (token) => set({ accessToken: token }),
      clearAccessToken: () => set({ accessToken: null })
    }),
    {
      name: 'auth-storage', // localStorage key
      // Optional: Only persist in browser, not during SSR
      skipHydration: true
    }
  )
);
