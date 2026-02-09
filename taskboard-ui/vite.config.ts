import react from '@vitejs/plugin-react-swc'
import { defineConfig } from 'vitest/config'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  test: {
    environment: 'jsdom',
    setupFiles: './spec/setup.ts',
    globals: true,
    css: true,
    include: ['spec/**/*-spec.{ts,tsx}', 'spec/**/*.{test,spec}.{ts,tsx}'],
    exclude: ['spec/setup.ts'],
  },
  server: {
    proxy: {
      "/api": "http://localhost:5217",
    },
  },
})
