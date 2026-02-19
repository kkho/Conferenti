import { randomUUID } from 'crypto';
import { cookies } from 'next/headers';
import { NextRequest, NextResponse } from 'next/server';

const API_BASE_URL =
  process.env['services__conferenti-api__https__0'] || 'https://localhost:7027';

const THREE_DAYS_IN_SECONDS = 259200; // 3 days * 24 hours * 60 minutes * 60 seconds

// Note: Do not disable TLS certificate validation in code. For local development
// with self-signed certificates, configure your environment or use proper
// development certificates instead of setting NODE_TLS_REJECT_UNAUTHORIZED.

export async function GET(request: NextRequest) {
  const cookieStore = await cookies();
  let sessionId = cookieStore.get('ai_chat_session')?.value;

  if (!sessionId) {
    sessionId = randomUUID();
  }

  const response = NextResponse.json({ sessionId });

  response.cookies.set('ai_chat_session', sessionId, {
    httpOnly: true,
    secure: process.env.NODE_ENV === 'production',
    sameSite: 'strict',
    maxAge: THREE_DAYS_IN_SECONDS,
    path: '/'
  });

  return response;
}

export async function POST(request: NextRequest) {
  const cookieStore = await cookies();
  let sessionId = cookieStore.get('ai_chat_session')?.value;

  // If no session exists, create one
  if (!sessionId) {
    sessionId = randomUUID();
  }

  const { message } = await request.json();

  const response = await fetch(`${API_BASE_URL}/v1/ai/chat/`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      message,
      sessionId
    })
  });

  console.log(response);
  const data = await response.json();
  
  // Set session cookie and ensure sessionId is in response
  const apiResponse = NextResponse.json(data);
  apiResponse.cookies.set('ai_chat_session', sessionId, {
    httpOnly: true,
    secure: process.env.NODE_ENV === 'production',
    sameSite: 'strict',
    maxAge: THREE_DAYS_IN_SECONDS,
    path: '/'
  });

  return apiResponse;
}

export async function DELETE(request: NextRequest) {
  const response = NextResponse.json({ success: true });
  response.cookies.delete('ai_chat_session');
  return response;
}
