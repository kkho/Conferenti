import { useGetSpeakers } from './speakers';

// Mock the entire React Query module
jest.mock('@tanstack/react-query', () => ({
  useQuery: jest.fn()
}));

// Mock fetch
global.fetch = jest.fn();

describe('useGetSpeakers', () => {
  const originalEnv = process.env;

  beforeEach(() => {
    jest.clearAllMocks();
    process.env = { ...originalEnv };
  });

  afterAll(() => {
    process.env = originalEnv;
  });

  it('should call useQuery with correct parameters', () => {
    const { useQuery } = jest.requireMock('@tanstack/react-query');
    
    useGetSpeakers('frontend');
    
    expect(useQuery).toHaveBeenCalledWith({
      queryKey: ['speakers', 'frontend'],
      queryFn: expect.any(Function)
    });
  });

  it('should use different query keys for different filters', () => {
    const { useQuery } = jest.requireMock('@tanstack/react-query');
    
    useGetSpeakers('backend');
    
    expect(useQuery).toHaveBeenCalledWith({
      queryKey: ['speakers', 'backend'],
      queryFn: expect.any(Function)
    });
  });

  it('should pass a queryFn to useQuery', () => {
    const { useQuery } = jest.requireMock('@tanstack/react-query');
    
    useGetSpeakers('test');
    
    const callArgs = useQuery.mock.calls[0][0];
    expect(callArgs).toHaveProperty('queryFn');
    expect(typeof callArgs.queryFn).toBe('function');
  });

  describe('integration', () => {
    it('should be a function that accepts a filter parameter', () => {
      expect(typeof useGetSpeakers).toBe('function');
      expect(() => useGetSpeakers('test')).not.toThrow();
    });

    it('should handle different filter values', () => {
      const filters = ['frontend', 'backend', 'fullstack', 'mobile', ''];
      
      filters.forEach(filter => {
        expect(() => useGetSpeakers(filter)).not.toThrow();
      });
    });
  });
});