// Global application constants
export const API_ENDPOINTS = {
  SPEAKERS: '/api/speakers',
  SESSIONS: '/api/sessions',
  AUTH: '/api/auth'
} as const;

export const APP_CONFIG = {
  APP_NAME: 'Conferenti',
  VERSION: '1.0.0',
  DEFAULT_PAGE_SIZE: 12,
  DEBOUNCE_DELAY: 500
} as const;

export const UI_CONSTANTS = {
  MODAL_ANIMATION_DURATION: 300,
  TOAST_DURATION: 5000,
  SEARCH_MIN_LENGTH: 2
} as const;

export const ROUTES = {
  HOME: '/',
  SPEAKERS: '/speakers',
  SESSIONS: '/sessions',
  ABOUT: '/about'
} as const;

// Speaker-related constants
export const SPEAKER_LEVELS = ['Beginner', 'Intermediate', 'Advanced'] as const;
export const SESSION_FORMATS = ['Presentation', 'Workshop', 'Panel'] as const;


// API Configuration
export const APIEndpoint =
  typeof window === 'undefined'
    ? process.env["services__conferenti-api__http__0"] || 'http://localhost:7027'
    : "/api";

// Type helpers
export type SpeakerLevel = typeof SPEAKER_LEVELS[number];
export type SessionFormat = typeof SESSION_FORMATS[number];