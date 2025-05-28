import path from 'path';

import react from '@vitejs/plugin-react';
import { defineConfig, loadEnv } from 'vite';

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '');

  const API_HOST = env.VITE_API_HOST?.replace(/\/$/, '');

  return {
    plugins: [react()],
    resolve: {
      alias: {
        '@': path.resolve(__dirname, 'src'),
      },
    },
    server: {
      host: true,
      port: 3000,
      strictPort: true,
      headers: {
        'Cache-Control': 'no-store',
        'Access-Control-Allow-Origin': env.VITE_CLIENT_ORIGIN || '*',
      },
      proxy: {
        '/api': {
          target: API_HOST,
          changeOrigin: true,
          secure: false,
          rewrite: (path) => path.replace(/^\/api/, '/api'),
        },
        '/swagger': {
          target: API_HOST,
          changeOrigin: true,
          secure: false,
        },
      },
    },
    build: {
      outDir: 'build',
      sourcemap: mode !== 'production',
    },
    base: env.VITE_BASE_URL || '/',
    define: {
      __VITE_SSO_CLIENT__: JSON.stringify(env.VITE_SSO_CLIENT),
      __VITE_SSO_REALM__: JSON.stringify(env.VITE_SSO_REALM),
      __VITE_SSO_HOST__: JSON.stringify(env.VITE_SSO_HOST),
      __VITE_DEFAULT_PAGE_SIZE__: JSON.stringify(env.VITE_DEFAULT_PAGE_SIZE),
      __VITE_DEFAULT_PAGE_SIZE_OPTIONS__: JSON.stringify(env.VITE_DEFAULT_PAGE_SIZE_OPTIONS),
    },
  };
});
