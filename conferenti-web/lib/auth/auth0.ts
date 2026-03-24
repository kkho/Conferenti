// lib/auth0.ts

import { Auth0Client } from '@auth0/nextjs-auth0/server';

export const auth0 = new Auth0Client({
  // In v4, the AUTH0_SCOPE and AUTH0_AUDIENCE environment variables for API authorized applications are no longer automatically picked up by the SDK.
  // Instead, we need to provide the values explicitly.
  authorizationParameters: {
    scope: process.env.AUTH0_SCOPE,
    audience: process.env.AUTH0_AUDIENCE
  },
  session: {
    // Use rolling sessions to keep cookies smaller
    rolling: true,
    rollingDuration: 86400, // 24 hours
    // Store only essential user data in the session
    storeIDToken: false,
    storeAccessToken: true,
    storeRefreshToken: false
  }
});
