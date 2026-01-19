import type { Config } from 'tailwindcss';
import typography from '@tailwindcss/typography';

const config: Config = {
  content: [
    './pages/**/*.{js,ts,jsx,tsx,mdx}',
    './components/**/*.{js,ts,jsx,tsx,mdx}',
    './app/**/*.{js,ts,jsx,tsx,mdx}'
  ],
  theme: {
    extend: {
      // Extending fontFamily to use @next/font loaded families
      fontFamily: {
        mono: ['var(--font-mono)', 'monospace'],
        sans: ['var(--font-sans)', 'sans-serif'],
        serif: ['var(--font-serif)', 'serif']
      },
      keyframes: {
        'bounce-dot': {
          '0%, 100%': { transform: 'translateY(0)' },
          '50%': { transform: 'translateY(-8px)' } // Adjust bounce height
        }
      },
      animation: {
        'bounce-dot-1': 'bounce-dot 0.6s ease-in-out infinite',
        'bounce-dot-2': 'bounce-dot 0.6s ease-in-out 0.2s infinite',
        'bounce-dot-3': 'bounce-dot 0.6s ease-in-out 0.4s infinite'
      }
    }
  },
  plugins: [typography]
};

export default config;
