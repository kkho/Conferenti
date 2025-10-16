/**
 * @jest-environment node
 */

import { NextRequest } from 'next/server';

global.fetch = jest.fn();

describe('GET /api/speakers', () => {
  const originalEnv = process.env;
  const mockFetch = global.fetch as jest.MockedFunction<typeof fetch>;
  let GET: (request: NextRequest) => Promise<Response>;

  beforeEach(async () => {
    jest.clearAllMocks();
    process.env = { ...originalEnv };
    // Set default API URL
    process.env['services__conferenti-api__https__0'] = 'https://localhost:7027';
    
    // Clear the module cache and re-import to pick up new env variables
    jest.resetModules();
    
    // Re-setup https mock after resetModules
    jest.doMock('https', () => ({
      Agent: jest.fn().mockImplementation(() => ({})),
      default: {
        Agent: jest.fn().mockImplementation(() => ({}))
      }
    }));
    
    const routeModule = await import('./route');
    GET = routeModule.GET;
  });

  afterAll(() => {
    process.env = originalEnv;
  });

  // Helper function to create NextRequest with headers
  const createMockRequest = (headers: Record<string, string> = {}) => {
    const headersObj = new Headers(headers);
    return {
      headers: headersObj,
      url: 'http://localhost:3000/api/speakers'
    } as NextRequest;
  };

  describe('Success Cases', () => {
    it('should successfully fetch speakers from API', async () => {
      const mockSpeakers = [
        { id: '1', name: 'John Doe', bio: 'Speaker bio' },
        { id: '2', name: 'Jane Smith', bio: 'Another speaker' }
      ];

      mockFetch.mockResolvedValueOnce({
        ok: true,
        status: 200,
        json: async () => mockSpeakers
      } as Response);

      const request = createMockRequest();
      const response = await GET(request);

      expect(mockFetch).toHaveBeenCalledWith(
        'https://localhost:7027/v1/speakers',
        expect.objectContaining({
          method: 'GET',
          headers: expect.objectContaining({
            'Content-Type': 'application/json'
          })
        })
      );

      const data = await response.json();
      expect(data).toEqual(mockSpeakers);
      expect(response.status).toBe(200);
    });

    it('should return response with cache headers', async () => {
      const mockSpeakers = [{ id: '1', name: 'Test Speaker' }];

      mockFetch.mockResolvedValueOnce({
        ok: true,
        status: 200,
        json: async () => mockSpeakers
      } as Response);

      const request = createMockRequest();
      const response = await GET(request);

      expect(response.headers.get('Cache-Control')).toBe(
        'public, s-maxage=60, stale-while-revalidate=300'
      );
    });

    it('should handle empty speakers array', async () => {
      mockFetch.mockResolvedValueOnce({
        ok: true,
        status: 200,
        json: async () => []
      } as Response);

      const request = createMockRequest();
      const response = await GET(request);

      const data = await response.json();
      expect(data).toEqual([]);
      expect(Array.isArray(data)).toBe(true);
    });
  });

  describe('Authorization Header Forwarding', () => {
    it('should forward Bearer token from Authorization header', async () => {
      const mockSpeakers = [{ id: '1', name: 'Speaker' }];
      const bearerToken = 'Bearer test-token-123';

      mockFetch.mockResolvedValueOnce({
        ok: true,
        status: 200,
        json: async () => mockSpeakers
      } as Response);

      const request = createMockRequest({
        Authorization: bearerToken
      });

      await GET(request);

      expect(mockFetch).toHaveBeenCalledWith(
        'https://localhost:7027/v1/speakers',
        expect.objectContaining({
          headers: expect.objectContaining({
            'Content-Type': 'application/json',
            Authorization: bearerToken
          })
        })
      );
    });

    it('should work without Authorization header', async () => {
      const mockSpeakers = [{ id: '1', name: 'Speaker' }];

      mockFetch.mockResolvedValueOnce({
        ok: true,
        status: 200,
        json: async () => mockSpeakers
      } as Response);

      const request = createMockRequest(); // No auth header

      await GET(request);

      const fetchCall = mockFetch.mock.calls[0][1];
      expect(fetchCall?.headers).not.toHaveProperty('Authorization');
      expect(fetchCall?.headers).toHaveProperty('Content-Type', 'application/json');
    });

    it('should forward different Bearer tokens correctly', async () => {
      const mockSpeakers = [{ id: '1', name: 'Speaker' }];
      const tokens = [
        'Bearer token-abc-123',
        'Bearer token-xyz-789',
        'Bearer different-token'
      ];

      for (const token of tokens) {
        mockFetch.mockResolvedValueOnce({
          ok: true,
          status: 200,
          json: async () => mockSpeakers
        } as Response);

        const request = createMockRequest({ Authorization: token });
        await GET(request);

        expect(mockFetch).toHaveBeenCalledWith(
          expect.any(String),
          expect.objectContaining({
            headers: expect.objectContaining({
              Authorization: token
            })
          })
        );

        mockFetch.mockClear();
      }
    });
  });

  describe('Error Handling', () => {
    let consoleErrorSpy: jest.SpyInstance;

    beforeEach(() => {
      consoleErrorSpy = jest.spyOn(console, 'error').mockImplementation();
    });

    afterEach(() => {
      consoleErrorSpy.mockRestore();
    });

    it('should return 500 when API responds with 404', async () => {
      mockFetch.mockResolvedValueOnce({
        ok: false,
        status: 404,
        statusText: 'Not Found'
      } as Response);

      const request = createMockRequest();
      const response = await GET(request);

      expect(response.status).toBe(500);
      const data = await response.json();
      expect(data).toEqual({ error: 'Failed to fetch' });
      
      // Verify error was logged
      expect(consoleErrorSpy).toHaveBeenCalledWith(
        'API Proxy Error:',
        expect.any(Error)
      );
    });

    it('should return 500 when API responds with 500', async () => {
      mockFetch.mockResolvedValueOnce({
        ok: false,
        status: 500,
        statusText: 'Internal Server Error'
      } as Response);

      const request = createMockRequest();
      const response = await GET(request);

      expect(response.status).toBe(500);
      const data = await response.json();
      expect(data).toEqual({ error: 'Failed to fetch' });
      
      // Verify error was logged
      expect(consoleErrorSpy).toHaveBeenCalledWith(
        'API Proxy Error:',
        expect.any(Error)
      );
    });

    it('should return 500 when API responds with 401 Unauthorized', async () => {
      mockFetch.mockResolvedValueOnce({
        ok: false,
        status: 401,
        statusText: 'Unauthorized'
      } as Response);

      const request = createMockRequest();
      const response = await GET(request);

      expect(response.status).toBe(500);
      const data = await response.json();
      expect(data).toEqual({ error: 'Failed to fetch' });
      
      // Verify error was logged
      expect(consoleErrorSpy).toHaveBeenCalledWith(
        'API Proxy Error:',
        expect.any(Error)
      );
    });

    it('should handle network errors gracefully', async () => {
      mockFetch.mockRejectedValueOnce(new Error('Network request failed'));

      const request = createMockRequest();
      const response = await GET(request);

      expect(response.status).toBe(500);
      const data = await response.json();
      expect(data).toEqual({ error: 'Failed to fetch' });
      
      // Verify error was logged
      expect(consoleErrorSpy).toHaveBeenCalledWith(
        'API Proxy Error:',
        expect.any(Error)
      );
    });

    it('should handle fetch timeout errors', async () => {
      mockFetch.mockRejectedValueOnce(new Error('Request timeout'));

      const request = createMockRequest();
      const response = await GET(request);

      expect(response.status).toBe(500);
      const data = await response.json();
      expect(data).toEqual({ error: 'Failed to fetch' });
      
      // Verify error was logged
      expect(consoleErrorSpy).toHaveBeenCalledWith(
        'API Proxy Error:',
        expect.any(Error)
      );
    });

    it('should handle JSON parsing errors', async () => {
      mockFetch.mockResolvedValueOnce({
        ok: true,
        status: 200,
        json: async () => {
          throw new Error('Invalid JSON');
        }
      } as unknown as Response);

      const request = createMockRequest();
      const response = await GET(request);

      expect(response.status).toBe(500);
      const data = await response.json();
      expect(data).toEqual({ error: 'Failed to fetch' });
      
      // Verify error was logged
      expect(consoleErrorSpy).toHaveBeenCalledWith(
        'API Proxy Error:',
        expect.any(Error)
      );
    });
  });

  describe('API URL Configuration', () => {
    it('should use default API URL when env variable not set', async () => {
      delete process.env['services__conferenti-api__https__0'];

      mockFetch.mockResolvedValueOnce({
        ok: true,
        status: 200,
        json: async () => []
      } as Response);

      const request = createMockRequest();
      await GET(request);

      expect(mockFetch).toHaveBeenCalledWith(
        'https://localhost:7027/v1/speakers',
        expect.any(Object)
      );
    });

    it('should append correct endpoint path to base URL', async () => {
      mockFetch.mockResolvedValueOnce({
        ok: true,
        status: 200,
        json: async () => []
      } as Response);

      const request = createMockRequest();
      await GET(request);

      const fetchUrl = mockFetch.mock.calls[0][0] as string;
      expect(fetchUrl).toContain('/v1/speakers');
      expect(fetchUrl).toMatch(/^https?:\/\/.+\/v1\/speakers$/);
    });
  });

  describe('HTTP Headers', () => {
    it('should always include Content-Type header', async () => {
      mockFetch.mockResolvedValueOnce({
        ok: true,
        status: 200,
        json: async () => []
      } as Response);

      const request = createMockRequest();
      await GET(request);

      expect(mockFetch).toHaveBeenCalledWith(
        expect.any(String),
        expect.objectContaining({
          headers: expect.objectContaining({
            'Content-Type': 'application/json'
          })
        })
      );
    });

    it('should use GET method', async () => {
      mockFetch.mockResolvedValueOnce({
        ok: true,
        status: 200,
        json: async () => []
      } as Response);

      const request = createMockRequest();
      await GET(request);

      expect(mockFetch).toHaveBeenCalledWith(
        expect.any(String),
        expect.objectContaining({
          method: 'GET'
        })
      );
    });
  });

  describe('Response Data Integrity', () => {
    it('should return exact data from upstream API', async () => {
      const complexSpeakerData = [
        {
          id: '1',
          name: 'Tony Stark',
          position: 'CEO',
          company: 'Stark Industries',
          bio: 'Genius, billionaire, playboy, philanthropist',
          photoUrl: 'https://example.com/tony.jpg',
          sessions: [
            {
              sessionId: 'session-1',
              title: 'AI in Modern Apps',
              description: 'Deep dive into AI',
              startTime: '2024-10-20T10:00:00Z',
              endTime: '2024-10-20T11:00:00Z'
            }
          ]
        }
      ];

      mockFetch.mockResolvedValueOnce({
        ok: true,
        status: 200,
        json: async () => complexSpeakerData
      } as Response);

      const request = createMockRequest();
      const response = await GET(request);

      const data = await response.json();
      expect(data).toEqual(complexSpeakerData);
      expect(data[0].sessions).toHaveLength(1);
      expect(data[0].sessions[0].title).toBe('AI in Modern Apps');
    });

    it('should not modify or transform API response', async () => {
      const originalData = {
        speakers: [{ id: '1' }],
        metadata: { total: 1, page: 1 }
      };

      mockFetch.mockResolvedValueOnce({
        ok: true,
        status: 200,
        json: async () => originalData
      } as Response);

      const request = createMockRequest();
      const response = await GET(request);

      const data = await response.json();
      expect(data).toEqual(originalData);
    });
  });
});
