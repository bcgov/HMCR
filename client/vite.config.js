import { defineConfig, loadEnv } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '');
  const apiHost = env.VITE_API_HOST || 'localhost:27238';

  return {
    plugins: [react()],
    server: {
      port: 3000,
      proxy: {
        '/api': {
          target: `http://${apiHost}`,
          changeOrigin: true,
        },
        '/swagger': {
          target: `http://${apiHost}`,
          changeOrigin: true,
        },
      },
    },
    build: {
      outDir: 'build',
      sourcemap: env.GENERATE_SOURCEMAP !== 'false',
    },
  };
});
