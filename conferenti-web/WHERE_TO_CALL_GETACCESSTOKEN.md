# Where to Call `getAccessToken()` Method

## ✅ **Answer: Call it in your CLIENT-SIDE React Component**

The `getAccessToken()` function should be called in your React components (client-side), NOT in your API routes.

## 📍 **Current Implementation in Your Project**

I've added it to `app/speakers/page.tsx` - here's how it works:

### **1. The Method (Added to your component)**

```typescript
async function getAccessToken() {
  const response = await fetch('/api/auth/token', {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json'
    }
  });

  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }

  const data = await response.json();
  return data.accessToken;
}
```

### **2. When to Call It (Using useEffect)**

```typescript
const [accessToken, setAccessToken] = useState<string>('');

useEffect(() => {
  const fetchToken = async () => {
    try {
      const token = await getAccessToken();
      setAccessToken(token);
      console.log('✅ Access token retrieved:', token);
    } catch (error) {
      console.error('❌ Error fetching access token:', error);
    }
  };

  fetchToken(); // Call when component mounts
}, []);
```

### **3. Use the Token in API Calls**

```typescript
// Use authenticated speakers if token is available
const { data: authenticatedData } = useAuthenticatedSpeakers('', accessToken);
const { data: publicData } = usePublicSpeakers('');

// Choose data source based on token availability
const apiSpeakers = accessToken ? authenticatedData : publicData;
```

## 🔄 **Complete Flow**

### **Current Setup (Ready to Enable)**

```
1. Component Mounts
   ↓
2. useEffect runs → calls getAccessToken()
   ↓
3. Fetches from /api/auth/token (your API route)
   ↓
4. API route calls Auth0 getAccessToken() server-side
   ↓
5. Returns token to your component
   ↓
6. Component stores token in state
   ↓
7. useAuthenticatedSpeakers hook uses token
   ↓
8. API calls include Authorization: Bearer {token}
   ↓
9. Backend receives authenticated request
```

## 🚀 **To Enable Authentication**

In `app/speakers/page.tsx`, find this line (around line 60):

```typescript
// Uncomment when Auth0 is configured:
// fetchToken();
```

Change it to:

```typescript
// Uncomment when Auth0 is configured:
fetchToken(); // ← Remove the slashes to enable
```

## 🎯 **Why This Approach?**

### **Client-Side (React Component)** ✅

- Can use React hooks (useState, useEffect)
- Has access to browser cookies/session
- Can trigger on component mount or user actions
- Perfect for getting tokens when user is already signed in

### **Server-Side (API Route)** ❌

- No component lifecycle
- No user interaction triggers
- Used for processing requests, not initiating them

## 📋 **Summary**

| Location                                   | Purpose                                 | When to Use                            |
| ------------------------------------------ | --------------------------------------- | -------------------------------------- |
| **Client Component** (`page.tsx`)          | Call `getAccessToken()` to fetch token  | When component mounts or user signs in |
| **API Route** (`/api/auth/token/route.ts`) | Provide access token from Auth0 session | Called by client components            |
| **API Route** (`/api/speakers/route.ts`)   | Forward tokens to backend               | Called by React Query hooks            |

## ✨ **Your Current Status**

✅ Method added to `app/speakers/page.tsx`  
✅ useEffect configured to fetch token  
✅ State management set up  
✅ Authenticated/Public data switching ready  
⏸️ Currently commented out until Auth0 is configured

**To activate:** Just uncomment `fetchToken();` in the useEffect when Auth0 is ready!
