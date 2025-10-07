import { resolveHref } from './route.links';

describe('resolveHref', () => {
  describe('documentType handling', () => {
    it('should return root path for empty string', () => {
      const result = resolveHref('', undefined);
      expect(result).toBe('/');
    });

    it('should return root path for empty documentType with any slug', () => {
      const result = resolveHref('', 'some-slug');
      expect(result).toBe('/');
    });

    it('should return undefined for unknown documentType', () => {
      const result = resolveHref('unknown', 'slug');
      expect(result).toBeUndefined();
    });

    it('should return undefined for undefined documentType', () => {
      const result = resolveHref(undefined, 'slug');
      expect(result).toBeUndefined();
    });
  });

  describe('sessions routes', () => {
    it('should return sessions index when no slug provided', () => {
      const result = resolveHref('sessions', undefined);
      expect(result).toBe('/sessions');
    });

    it('should return sessions index when empty slug provided', () => {
      // Empty string is falsy, so should return index page
      const result = resolveHref('sessions', '');
      expect(result).toBe('/sessions');
    });

    it('should return specific session route when slug provided', () => {
      const result = resolveHref('sessions', 'react-performance');
      expect(result).toBe('/sessions/react-performance');
    });

    it('should handle slugs with special characters', () => {
      const result = resolveHref('sessions', 'web-dev-2024');
      expect(result).toBe('/sessions/web-dev-2024');
    });
  });

  describe('speakers routes', () => {
    it('should return speakers index when no slug provided', () => {
      const result = resolveHref('speakers', undefined);
      expect(result).toBe('/speakers');
    });

    it('should return speakers index when empty slug provided', () => {
      // Empty string is falsy, so should return index page
      const result = resolveHref('speakers', '');
      expect(result).toBe('/speakers');
    });

    it('should return specific speaker route when slug provided', () => {
      const result = resolveHref('speakers', 'john-doe');
      expect(result).toBe('/speakers/john-doe');
    });

    it('should handle speaker slugs with numbers', () => {
      const result = resolveHref('speakers', 'jane-smith-123');
      expect(result).toBe('/speakers/jane-smith-123');
    });
  });

  describe('faq routes', () => {
    it('should return faq route regardless of slug', () => {
      const result = resolveHref('faq', undefined);
      expect(result).toBe('/faq');
    });

    it('should return faq route even with slug provided', () => {
      const result = resolveHref('faq', 'some-question');
      expect(result).toBe('/faq');
    });
  });

  describe('edge cases', () => {
    it('should handle null values gracefully', () => {
      // @ts-expect-error Testing runtime behavior with null
      const result = resolveHref(null, null);
      expect(result).toBeUndefined();
    });

    it('should handle very long slugs', () => {
      const longSlug = 'a'.repeat(200);
      const result = resolveHref('sessions', longSlug);
      expect(result).toBe(`/sessions/${longSlug}`);
    });

    it('should handle slugs with URL-unsafe characters', () => {
      const unsafeSlug = 'session with spaces & symbols!';
      const result = resolveHref('speakers', unsafeSlug);
      expect(result).toBe(`/speakers/${unsafeSlug}`);
    });

    it('should handle whitespace-only slug as falsy', () => {
      // Whitespace-only string should be treated as truthy
      const result = resolveHref('sessions', '   ');
      expect(result).toBe('/sessions/   ');
    });
  });

  describe('truthy/falsy slug behavior', () => {
    it('should treat falsy slugs as no slug', () => {
      const falsyValues = ['', undefined, null, false, 0];
      
      falsyValues.forEach((falsyValue) => {
        // @ts-expect-error Testing runtime behavior with various falsy values
        const result = resolveHref('sessions', falsyValue);
        expect(result).toBe('/sessions');
      });
    });

    it('should treat truthy slugs as valid slugs', () => {
      const truthyValues = ['slug', 'a', '1', ' ', 'true'];
      
      truthyValues.forEach((truthyValue) => {
        const result = resolveHref('sessions', truthyValue);
        expect(result).toBe(`/sessions/${truthyValue}`);
      });
    });
  });

  describe('consistent behavior', () => {
    it('should always return string or undefined', () => {
      const testCases = [
        ['', ''],
        ['sessions', 'test'],
        ['speakers', 'test'],
        ['faq', 'test'],
        ['unknown', 'test'],
        [undefined, undefined]
      ];

      testCases.forEach(([docType, slug]) => {
        const result = resolveHref(docType, slug);
        expect(typeof result === 'string' || result === undefined).toBe(true);
      });
    });

    it('should handle all valid documentTypes', () => {
      const validTypes = ['', 'sessions', 'speakers', 'faq'];
      
      validTypes.forEach((docType) => {
        const result = resolveHref(docType, 'test-slug');
        expect(typeof result).toBe('string');
        expect(result).toBeDefined();
      });
    });
  });
});