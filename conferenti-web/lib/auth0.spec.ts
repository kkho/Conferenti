// Mock the Auth0Client constructor
const mockAuth0ClientConstructor = jest.fn();

jest.mock('@auth0/nextjs-auth0/server', () => ({
  Auth0Client: mockAuth0ClientConstructor
}));

describe('auth0 module', () => {
  const originalEnv = process.env;

  beforeEach(() => {
    jest.clearAllMocks();
    mockAuth0ClientConstructor.mockClear();
    process.env = { ...originalEnv };
    jest.resetModules();
  });

  afterAll(() => {
    process.env = originalEnv;
  });

  it('should create Auth0Client with environment variables', async () => {
    process.env.AUTH0_SCOPE = 'openid profile email';
    process.env.AUTH0_AUDIENCE = 'https://api.conferenti.com';

    await import('./auth0');

    expect(mockAuth0ClientConstructor).toHaveBeenCalledWith({
      authorizationParameters: {
        scope: 'openid profile email',
        audience: 'https://api.conferenti.com'
      }
    });
  });

  it('should handle missing environment variables', async () => {
    delete process.env.AUTH0_SCOPE;
    delete process.env.AUTH0_AUDIENCE;

    await import('./auth0');

    expect(mockAuth0ClientConstructor).toHaveBeenCalledWith({
      authorizationParameters: {
        scope: undefined,
        audience: undefined
      }
    });
  });

  it('should call Auth0Client constructor once', async () => {
    process.env.AUTH0_SCOPE = 'test';
    process.env.AUTH0_AUDIENCE = 'test';

    await import('./auth0');

    expect(mockAuth0ClientConstructor).toHaveBeenCalledTimes(1);
  });

  it('should export auth0 instance', async () => {
    const mockInstance = { test: 'value' };
    mockAuth0ClientConstructor.mockReturnValue(mockInstance);

    const { auth0 } = await import('./auth0');

    expect(auth0).toBe(mockInstance);
  });
});