const trimTrailingSlash = (value: string): string => value.replace(/\/+$/, '');

export const API_BASE_URL = trimTrailingSlash(
  import.meta.env.VITE_API_BASE_URL || 'https://taltech.akaver.com/api/v1.0',
);

export const APP_BASE_PATH = import.meta.env.BASE_URL || '/';
export const APP_NAME = 'Vue Task Manager';
