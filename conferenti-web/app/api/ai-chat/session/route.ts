import { randomUUID } from 'crypto';
import { cookies } from 'next/headers';
import { NextRequest, NextResponse } from 'next/server';

const API_BASE_URL =
  process.env['services__conferenti-api__https__0'] || 'https://localhost:7027';

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
    maxAge: 259200, // 3 days,
    path: '/'
  });

  return response;
}

export async function POST(request: NextRequest) {
  const cookieStore = await cookies();
  const sessionId = cookieStore.get('ai_chat_session')?.value;

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

  const data = await response.json();
  return NextResponse.json(data);
}

export async function DELETE(request: NextRequest) {
  const response = NextResponse.json({ success: true });
  response.cookies.delete('ai_chat_session');
  return response;
}
