import { auth0 } from '@/lib/auth/auth0';
import { NextResponse, NextRequest } from 'next/server';

export async function GET(req: NextRequest) {
  const session = await auth0.getSession();
  return NextResponse.json({ accessToken: session?.tokenSet.accessToken });
}
