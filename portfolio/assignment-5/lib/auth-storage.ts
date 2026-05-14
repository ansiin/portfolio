import type { JwtResponse } from "@/types/api";

export type AuthSession = {
  token: string;
  refreshToken: string;
  firstName: string | null;
  lastName: string | null;
};

const storageKey = "react-hw.auth";

export function normalizeSession(response: JwtResponse): AuthSession {
  if (!response.token || !response.refreshToken) {
    throw new Error("Backend did not return both JWT and refresh token.");
  }

  return {
    token: response.token,
    refreshToken: response.refreshToken,
    firstName: response.firstName,
    lastName: response.lastName
  };
}

export function loadSession(): AuthSession | null {
  if (typeof window === "undefined") return null;

  const raw = window.localStorage.getItem(storageKey);
  if (!raw) return null;

  try {
    const parsed = JSON.parse(raw) as AuthSession;
    if (!parsed.token || !parsed.refreshToken) return null;
    return parsed;
  } catch {
    return null;
  }
}

export function saveSession(session: AuthSession): void {
  window.localStorage.setItem(storageKey, JSON.stringify(session));
}

export function clearSession(): void {
  window.localStorage.removeItem(storageKey);
}

export function isJwtExpiring(token: string, skewSeconds = 30): boolean {
  const parts = token.split(".");
  if (parts.length < 2) return true;

  try {
    const payload = JSON.parse(atob(parts[1].replace(/-/g, "+").replace(/_/g, "/"))) as { exp?: number };
    if (!payload.exp) return true;
    return payload.exp * 1000 <= Date.now() + skewSeconds * 1000;
  } catch {
    return true;
  }
}
