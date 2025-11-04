/**
 * @jest-environment node
 */

import { NextRequest } from 'next/server';

global.fetch = jest.fn();

describe('GET /api/sessions', () => {
  const originalEnv = process.env;
  const mockFetch = global.fetch as jest.MockedFunction<typeof fetch>;
  let GET: (
    request: NextRequest,
    { params }: { params: Record<string, string> }
  ) => Promise<Response>;

  beforeEach(async () => {
    jest.clearAllMocks();
    process.env = { ...originalEnv };
    process.env['services__conferenti-api__https__0'] =
      'https://localhost:7027';
    jest.resetModules();

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

  const createMockRequest = (headers: Record<string, string> = {}) => {
    const headerObject = new Headers(headers);
    return {
      headers: headerObject,
      url: 'https://localhost:3000/api/sessions'
    } as NextRequest;
  };

  describe('Success cases', () => {
    it('should successfully fetch sessions from API', async () => {
      const mockSessions = [
        { id: '1', title: 'Session 1', description: 'Description 1' }
      ];

      mockFetch.mockResolvedValueOnce({
        ok: true,
        status: 200,
        json: async () => mockSessions
      } as Response);

      const request = createMockRequest();
      const response = await GET(request, { params: {} });

      expect(mockFetch).toHaveBeenCalledWith(
        'https://localhost:7027/v1/sessions/',
        expect.objectContaining({
          method: 'GET',
          headers: expect.objectContaining({
            'Content-Type': 'application/json'
          })
        })
      );

      const data = await response.json();
      expect(data).toEqual(mockSessions);
      expect(response.status).toBe(200);
    });
  });

  it('sould return response with cache headers', async () => {
    const mockSessions = [
      { id: '1', title: 'Session 1', description: 'Description 1' }
    ];

    mockFetch.mockResolvedValueOnce({
      ok: true,
      status: 200,
      json: async () => mockSessions
    } as Response);

    const request = createMockRequest();
    const response = await GET(request, { params: {} });

    expect(response.headers.get('Cache-Control')).toBe(
      'public, s-maxage=60, stale-while-revalidate=300'
    );
  });

  it('should handle empty sessions array', async () => {
    mockFetch.mockResolvedValueOnce({
      ok: true,
      status: 200,
      json: async () => []
    } as Response);

    const request = createMockRequest();
    const response = await GET(request, { params: {} });

    const data = await response.json();
    expect(data).toEqual([]);
    expect(Array.isArray(data)).toBe(true);
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
      const response = await GET(request, { params: {} });

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
      const response = await GET(request, { params: {} });

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
      const response = await GET(request, { params: {} });

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
      const response = await GET(request, { params: {} });

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
      const response = await GET(request, { params: {} });

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
      const response = await GET(request, { params: {} });

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
      await GET(request, { params: {} });

      expect(mockFetch).toHaveBeenCalledWith(
        'https://localhost:7027/v1/sessions/',
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
      await GET(request, { params: {} });

      const fetchUrl = mockFetch.mock.calls[0][0] as string;
      expect(fetchUrl).toContain('/v1/sessions');
      expect(fetchUrl).toMatch(/^https?:\/\/.+\/v1\/sessions\/?$/);
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
      await GET(request, { params: {} });

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
      await GET(request, { params: {} });

      expect(mockFetch).toHaveBeenCalledWith(
        expect.any(String),
        expect.objectContaining({
          method: 'GET'
        })
      );
    });
  });
});
