import { defineConfig, loadEnv } from 'vite';
import vue from '@vitejs/plugin-vue';

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '');

  return {
    plugins: [vue()],
    base: env.VITE_APP_BASE_PATH || '/',
    server: {
      host: '0.0.0.0',
      port: 5173,
    },
  };
});
