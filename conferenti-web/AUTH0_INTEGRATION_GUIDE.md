# Complete Auth0 Integration Guide

## Overview

Here's how to properly integrate Auth0 with your Next.js application for Bearer token authentication.

## 1. Server-Side Token Endpoint (`app/api/auth/token/route.ts`)

```typescript
import { getAccessToken } from '@auth0/nextjs-auth0';
import { NextResponse } from 'next/server';

export async function GET() {
  try {
    // For Auth0 v4.10.0, getAccessToken returns a string directly
    const accessToken = await getAccessToken();

    if (!accessToken) {
      return NextResponse.json(
        { error: 'No access token available' },
        { status: 401 }
      );
    }

    return NextResponse.json({ accessToken });
  } catch (error) {
    console.error('Error getting access token:', error);
    return NextResponse.json(
      { error: 'Failed to get access token' },
      { status: 500 }
    );
  }
}
```

## 2. Client-Side Hook Usage (`app/speakers/page.tsx`)

```typescript
'use client';

import { useUser } from '@auth0/nextjs-auth0/client';
import {
  useAuthenticatedSpeakers,
  usePublicSpeakers
} from '@/hooks/speakers-auth';

function SpeakersContent() {
  const { user, isLoading } = useUser();
  const [accessToken, setAccessToken] = useState<string>('');

  // Get access token when user is authenticated
  useEffect(() => {
    if (user) {
      fetch('/api/auth/token')
        .then((res) => res.json())
        .then((data) => setAccessToken(data.accessToken))
        .catch(console.error);
    }
  }, [user]);

  // Use authenticated hook when user is logged in with token
  // Fall back to public hook otherwise
  const { data: speakers, error } =
    user && accessToken
      ? useAuthenticatedSpeakers('', accessToken)
      : usePublicSpeakers('');

  if (isLoading) return <div>Loading...</div>;

  // Rest of component...
}
```

## 3. Auth0 App Setup

### Environment Variables (`.env.local`)

```env
AUTH0_SECRET='use [openssl rand -hex 32] to generate a 32 bytes value'
AUTH0_BASE_URL='http://localhost:3000'
AUTH0_ISSUER_BASE_URL='https://your-domain.auth0.com'
AUTH0_CLIENT_ID='your-client-id'
AUTH0_CLIENT_SECRET='your-client-secret'
```

### App Layout with UserProvider (`app/layout.tsx`)

```typescript
import { UserProvider } from '@auth0/nextjs-auth0/client';

export default function RootLayout({
  children
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body>
        <UserProvider>{children}</UserProvider>
      </body>
    </html>
  );
}
```

### Auth API Routes (`app/api/auth/[...auth0]/route.ts`)

```typescript
import { handleAuth } from '@auth0/nextjs-auth0';

export const GET = handleAuth();
```

## 4. Login/Logout Component

```typescript
import { useUser } from '@auth0/nextjs-auth0/client';

export function AuthButton() {
  const { user, error, isLoading } = useUser();

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>{error.message}</div>;

  if (user) {
    return (
      <div className="flex items-center gap-4">
        <span>Welcome {user.name}!</span>
        <a
          href="/api/auth/logout"
          className="bg-red-500 text-white px-4 py-2 rounded"
        >
          Logout
        </a>
      </div>
    );
  }

  return (
    <a
      href="/api/auth/login"
      className="bg-blue-500 text-white px-4 py-2 rounded"
    >
      Login
    </a>
  );
}
```

## 5. Updated Speakers Hooks (`hooks/speakers-auth.ts`)

```typescript
import { Speaker } from '@/types';
import { useQuery } from '@tanstack/react-query';

export const useAuthenticatedSpeakers = (
  filter?: string,
  accessToken?: string
) => {
  return useQuery<Speaker[]>({
    queryKey: ['speakers', filter, !!accessToken],
    queryFn: async () => {
      const headers: Record<string, string> = {
        'Content-Type': 'application/json'
      };

      if (accessToken) {
        headers['Authorization'] = `Bearer ${accessToken}`;
      }

      const url = filter
        ? `/api/speakers?filter=${encodeURIComponent(filter)}`
        : '/api/speakers';

      const response = await fetch(url, {
        method: 'GET',
        headers
      });

      if (response.status === 401) {
        throw new Error('Authentication required');
      }

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      return response.json();
    },
    enabled: !!accessToken, // Only run when we have a token
    retry: (failureCount, error: unknown) => {
      if (
        error instanceof Error &&
        error.message.includes('Authentication required')
      ) {
        return false;
      }
      return failureCount < 3;
    }
  });
};
```

## 6. Integration Steps

### Step 1: Install Auth0 (if needed)

```bash
npm install @auth0/nextjs-auth0
```

### Step 2: Set up environment variables

Create `.env.local` with your Auth0 configuration

### Step 3: Add UserProvider to layout

Wrap your app with the Auth0 UserProvider

### Step 4: Create auth API routes

Add the `[...auth0]/route.ts` file

### Step 5: Update your components

Use the patterns shown above in your speakers page

### Step 6: Test the flow

1. Visit `/api/auth/login` to log in
2. Visit `/speakers` to see authenticated data
3. Visit `/api/auth/logout` to log out

## Current Working State

Your application currently works with:

- ✅ Public speaker data via `usePublicSpeakers`
- ✅ Bearer token forwarding in API routes
- ✅ CORS handling and SSL certificate bypassing
- ✅ Modal functionality and dynamic routing

When you're ready to add authentication, follow the steps above!
