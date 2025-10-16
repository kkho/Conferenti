import { NextRequest, NextResponse } from 'next/server';
import https from 'https';

const API_BASE_URL =
  process.env['services__conferenti-api__https__0'] || 'https://localhost:7027';

// Create an agent that accepts self-signed certificates (for development only)
const httpsAgent = new https.Agent({
  rejectUnauthorized: false
});

export async function GET(request: NextRequest) {
  try {
    const authHeader = request.headers.get('Authorization');    
    const apiUrl = `${API_BASE_URL}/v1/speakers`;
    
    const response = await fetch(apiUrl, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        ...(authHeader && { Authorization: authHeader })
      },
      // @ts-expect-error - agent is valid but TypeScript doesn't recognize it in fetch
      agent: apiUrl.startsWith('https') ? httpsAgent : undefined
    });

    if (!response.ok) {
      throw new Error(`API responded with status: ${response.status}`);
    }

    const data = await response.json();

    return NextResponse.json(data, {
      headers: {
        'Cache-Control': 'public, s-maxage=60, stale-while-revalidate=300'
      }
    });
  } catch (error) {
    console.error('API Proxy Error:', error);
    return NextResponse.json({ error: 'Failed to fetch' }, { status: 500 });
  }
}