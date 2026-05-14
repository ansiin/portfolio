import type { JwtPayload } from '../types/auth';

const decodeBase64Url = (value: string): string => {
  const base64 = value.replace(/-/g, '+').replace(/_/g, '/');
  const padded = base64.padEnd(base64.length + ((4 - (base64.length % 4)) % 4), '=');

  try {
    const decoded = atob(padded);
    const bytes = Array.from(decoded, (char) => `%${char.charCodeAt(0).toString(16).padStart(2, '0')}`);

    return decodeURIComponent(bytes.join(''));
  } catch {
    return atob(padded);
  }
};

export const decodeJwtPayload = (token?: string | null): JwtPayload | null => {
  if (!token) {
    return null;
  }

  const parts = token.split('.');
  if (parts.length < 2) {
    return null;
  }

  try {
    return JSON.parse(decodeBase64Url(parts[1])) as JwtPayload;
  } catch {
    return null;
  }
};

export const getJwtExpiration = (token?: string | null): Date | null => {
  const payload = decodeJwtPayload(token);
  if (typeof payload?.exp !== 'number') {
    return null;
  }

  return new Date(payload.exp * 1000);
};

export const isJwtExpired = (token?: string | null, skewSeconds = 30): boolean => {
  const expiration = getJwtExpiration(token);
  if (!expiration) {
    return true;
  }

  return expiration.getTime() <= Date.now() + skewSeconds * 1000;
};

export const getJwtEmail = (token?: string | null): string | null => {
  const payload = decodeJwtPayload(token);
  const candidates = [payload?.email, payload?.unique_name, payload?.upn, payload?.sub];

  return (
    candidates.find((value): value is string => typeof value === 'string' && value.trim().length > 0) ?? null
  );
};
