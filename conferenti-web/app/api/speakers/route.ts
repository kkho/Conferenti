import { NextRequest, NextResponse } from 'next/server';
import https from 'https';
// Commenting out Auth0 until it's properly configured
// import { getAccessToken } from '@auth0/nextjs-auth0';

const API_BASE_URL =
  process.env['services__conferenti-api__https__0'] || 'https://localhost:7027';

// Create an agent that accepts self-signed certificates (for development only)
const httpsAgent = new https.Agent({
  rejectUnauthorized: false
});

export async function GET(request: NextRequest) {
  process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';
  try {
    // Extract Bearer token from Authorization header
    const authHeader = request.headers.get('Authorization');
    
    if (authHeader) {
      console.log('Auth Header Present:', authHeader.substring(0, 20) + '...');
    }

    const apiUrl = `${API_BASE_URL}/v1/speakers`;
    console.log('Fetching from:', apiUrl);
    
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