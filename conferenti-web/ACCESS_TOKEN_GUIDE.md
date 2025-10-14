# Complete Guide: User Signed In → Get Access Token → Call API

## Overview

This guide shows exactly how to implement authentication flow where a signed-in user gets an access token to call your API.

## The Complete Flow

### 1. User Authentication State

```typescript
import { useUser } from '@auth0/nextjs-auth0/client';

function MyComponent() {
  const { user, error, isLoading } = useUser();

  // user will be null if not signed in
  // user will contain user info if signed in
}
```

### 2. Get Access Token When User Signs In

```typescript
import { useUser } from '@auth0/nextjs-auth0/client';
import { useState, useEffect } from 'react';

function MyComponent() {
  const { user } = useUser();
  const [accessToken, setAccessToken] = useState<string>('');

  useEffect(() => {
    const getToken = async () => {
      if (user) {
        try {
          // This is the key function - gets token from Auth0
          const token = await getAccessTokenSilently({
            authorizationParams: {
              audience: 'https://your-api-audience', // Set in Auth0 dashboard
              scope: 'read:speakers write:speakers' // Define what user can do
            }
          });

          setAccessToken(token);
          console.log('✅ Got access token:', token);
        } catch (error) {
          console.error('❌ Error getting token:', error);
        }
      }
    };

    getToken();
  }, [user]); // Run when user changes (signs in/out)
}
```

### 3. Use Access Token in API Calls

```typescript
// Your hook already supports this!
const { data: speakers, error } = useAuthenticatedSpeakers('', accessToken);

// This will:
// 1. Add "Authorization: Bearer {token}" header
// 2. Call /api/speakers with the token
// 3. Your API route forwards token to backend
// 4. Backend validates token and returns data
```

### 4. Complete Working Example

```typescript
'use client';

import { useUser } from '@auth0/nextjs-auth0/client';
import { useState, useEffect } from 'react';
import {
  useAuthenticatedSpeakers,
  usePublicSpeakers
} from '@/hooks/speakers-auth';

export function SpeakersWithAuth() {
  const { user, error, isLoading } = useUser();
  const [accessToken, setAccessToken] = useState<string>('');

  // Get access token when user signs in
  useEffect(() => {
    if (user) {
      getAccessTokenSilently({
        authorizationParams: {
          audience: process.env.NEXT_PUBLIC_AUTH0_AUDIENCE,
          scope: 'read:speakers'
        }
      })
        .then(setAccessToken)
        .catch(console.error);
    }
  }, [user]);

  // Use authenticated data if user + token, otherwise public data
  const { data: speakers, error: dataError } =
    user && accessToken
      ? useAuthenticatedSpeakers('', accessToken) // 🔒 Authenticated call
      : usePublicSpeakers(''); // 🌐 Public call

  // Loading states
  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Auth error: {error.message}</div>;

  return (
    <div>
      {/* Authentication status */}
      {user ? (
        <div className="bg-green-100 p-4 rounded mb-4">
          <p>✅ Signed in as: {user.name}</p>
          <p>🔑 Token: {accessToken ? 'Available' : 'Getting...'}</p>
          <a href="/api/auth/logout">Logout</a>
        </div>
      ) : (
        <div className="bg-yellow-100 p-4 rounded mb-4">
          <p>❌ Not signed in - showing public data</p>
          <a href="/api/auth/login">Sign In</a>
        </div>
      )}

      {/* Speakers list */}
      <div>
        <h2>Speakers ({speakers?.length})</h2>
        {dataError && <p>Error: {dataError.message}</p>}
        <ul>
          {speakers?.map((speaker) => (
            <li key={speaker.id}>
              {speaker.name} - {speaker.position}
            </li>
          ))}
        </ul>
      </div>
    </div>
  );
}
```

## How It Works in Your App

### Step 1: User Flow

1. User visits `/speakers`
2. Not signed in → sees "Sign In" button
3. Clicks "Sign In" → redirected to Auth0
4. Signs in → redirected back to `/speakers`
5. Now signed in → gets access token → sees authenticated data

### Step 2: API Call Flow

```
User signed in → getAccessTokenSilently() → JWT token
     ↓
useAuthenticatedSpeakers('', token)
     ↓
fetch('/api/speakers', { headers: { Authorization: 'Bearer {token}' } })
     ↓
Your API route extracts token → forwards to backend
     ↓
Backend validates token → returns authenticated data
```

### Step 3: Your Current Setup (Already Working!)

- ✅ `/api/speakers` route forwards Bearer tokens
- ✅ `useAuthenticatedSpeakers` hook sends tokens
- ✅ Error handling for auth failures
- ✅ Fallback to public data

## To Enable This in Your App:

1. **Set up Auth0 environment variables**
2. **Add UserProvider to layout**
3. **Create auth API routes**
4. **Replace your current speakers component with the pattern above**

Your authentication infrastructure is already built - just need to complete the Auth0 setup!

## Testing the Flow

1. Visit `/api/auth/login` → Should redirect to Auth0
2. Sign in → Should redirect back with session
3. Visit `/speakers` → Should show authenticated data
4. Visit `/api/auth/logout` → Should sign out

The access token is automatically managed by Auth0 and refreshed as needed!
