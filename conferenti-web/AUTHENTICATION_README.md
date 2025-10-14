# Authentication Integration Guide

## Current Status

- ✅ API routes with Bearer token forwarding are working
- ✅ Custom hooks for authenticated and public speaker data
- ✅ Component structure ready for Auth0 integration
- ⏳ Auth0 client integration pending

## How to Complete Auth0 Integration

### 1. Install Auth0 Dependencies (if not already installed)

```bash
npm install @auth0/nextjs-auth0
```

### 2. Environment Variables

Make sure these are in your `.env.local`:

```env
AUTH0_SECRET='your-secret-key'
AUTH0_BASE_URL='http://localhost:3000'
AUTH0_ISSUER_BASE_URL='https://your-domain.auth0.com'
AUTH0_CLIENT_ID='your-client-id'
AUTH0_CLIENT_SECRET='your-client-secret'
```

### 3. Wrap your app with Auth0Provider

In `app/layout.tsx`, add:

```tsx
import { UserProvider } from '@auth0/nextjs-auth0/client';

export default function RootLayout({
  children
}: {
  children: React.ReactNode;
}) {
  return (
    <html>
      <body>
        <UserProvider>{children}</UserProvider>
      </body>
    </html>
  );
}
```

### 4. Add Auth0 API routes

Create `app/api/auth/[...auth0]/route.ts`:

```tsx
import { handleAuth } from '@auth0/nextjs-auth0';

export const GET = handleAuth();
```

### 5. Update speakers page for authentication

In `app/speakers/page.tsx`, replace the import and hook usage:

```tsx
// Replace this import
import { usePublicSpeakers } from '@/hooks/speakers-auth';

// With this
import {
  useAuthenticatedSpeakers,
  usePublicSpeakers
} from '@/hooks/speakers-auth';
import { useUser } from '@auth0/nextjs-auth0/client';

// In the component:
function SpeakersContent() {
  const { user } = useUser();
  const [accessToken, setAccessToken] = useState<string>('');

  useEffect(() => {
    if (user) {
      // Get access token when user is logged in
      fetch('/api/auth/token')
        .then((res) => res.text())
        .then(setAccessToken)
        .catch(console.error);
    }
  }, [user]);

  // Use authenticated hook when user is logged in, otherwise use public
  const { data: apiSpeakers, error } = user
    ? useAuthenticatedSpeakers('', accessToken)
    : usePublicSpeakers('');

  // Rest of component...
}
```

### 6. Add login/logout buttons

```tsx
import { useUser } from '@auth0/nextjs-auth0/client';

function LoginButton() {
  const { user, isLoading } = useUser();

  if (isLoading) return <div>Loading...</div>;

  if (user) {
    return (
      <div>
        <span>Welcome {user.name}!</span>
        <a href="/api/auth/logout">Logout</a>
      </div>
    );
  }

  return <a href="/api/auth/login">Login</a>;
}
```

## Current Working Features

### 1. Bearer Token Forwarding

The API route at `app/api/speakers/route.ts` already supports Bearer tokens:

- Extracts `Authorization` header from requests
- Forwards token to backend API at `https://localhost:7027/api/speakers`
- Handles CORS and SSL certificate issues

### 2. Custom Hooks

Two hooks are available in `hooks/speakers-auth.ts`:

- `useAuthenticatedSpeakers(filter, accessToken)` - for authenticated requests
- `usePublicSpeakers(filter)` - for public requests

### 3. Error Handling

- Authentication errors (401) are handled gracefully
- Network errors have retry logic
- SSL certificate issues are bypassed in development

## Testing the Current Setup

1. **Public Access**: The speakers page currently works with public data
2. **Bearer Token Support**: You can test by adding an Authorization header manually
3. **API Proxy**: The `/api/speakers` endpoint forwards requests to your backend

## Next Steps

1. Complete Auth0 setup with environment variables
2. Add the UserProvider wrapper
3. Create the auth API routes
4. Update the speakers page to use authentication
5. Add login/logout UI components

The foundation is ready - just need to complete the Auth0 configuration!
