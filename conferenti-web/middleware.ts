import type { NextRequest } from 'next/server';
import { auth0 } from './lib/auth/auth0';

export async function middleware(request: NextRequest) {
  return await auth0.middleware(request);
}

export const config = {
  matcher: [
    /*
     * Only run Auth0 middleware on auth routes to avoid blocking navigation
     * This prevents delays during page navigation
     */
    '/api/auth/:path*'
  ]
};
