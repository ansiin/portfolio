import { computed, ref } from 'vue';
import { defineStore } from 'pinia';

import { apiRequest, getErrorMessage } from '../lib/api';
import { registerAuthSessionProvider } from '../lib/auth-session';
import { formatDateTime } from '../lib/dates';
import { getJwtEmail, getJwtExpiration, isJwtExpired } from '../lib/jwt';
import { authStorage } from '../lib/storage';
import type { JwtResponse, LoginCredentials, RefreshTokenPayload, RegisterPayload, StoredAuthSession } from '../types/auth';

const REFRESH_BUFFER_SECONDS = 60;

let refreshInFlight: Promise<boolean> | null = null;

export const useAuthStore = defineStore('auth', () => {
  const session = ref<StoredAuthSession | null>(null);
  const initialized = ref(false);
  const working = ref(false);
  const lastError = ref<string | null>(null);

  const isAuthenticated = computed(() => Boolean(session.value?.token));
  const tokenExpiresAt = computed(() => getJwtExpiration(session.value?.token));
  const sessionExpiresLabel = computed(() =>
    tokenExpiresAt.value ? formatDateTime(tokenExpiresAt.value.toISOString()) : 'Unknown',
  );
  const fullName = computed(() => {
    if (!session.value) {
      return '';
    }

    return [session.value.firstName, session.value.lastName].filter(Boolean).join(' ');
  });

  const setSession = (nextSession: StoredAuthSession | null) => {
    session.value = nextSession;

    if (nextSession) {
      authStorage.save(nextSession);
    } else {
      authStorage.clear();
    }
  };

  const applyJwtResponse = (payload: JwtResponse, fallbackEmail?: string) => {
    const nextSession: StoredAuthSession = {
      ...payload,
      email: getJwtEmail(payload.token) ?? fallbackEmail ?? session.value?.email ?? '',
    };

    setSession(nextSession);
  };

  const clearSession = () => {
    setSession(null);
  };

  const refreshTokens = async (): Promise<boolean> => {
    if (!session.value) {
      return false;
    }

    if (refreshInFlight) {
      return refreshInFlight;
    }

    refreshInFlight = (async () => {
      const currentSession = session.value;
      if (!currentSession) {
        return false;
      }

      try {
        const payload: RefreshTokenPayload = {
          jwt: currentSession.token,
          refreshToken: currentSession.refreshToken,
        };

        const response = await apiRequest<JwtResponse>('/Account/RefreshToken', {
          method: 'POST',
          auth: false,
          retryOnAuth: false,
          body: payload,
        });

        applyJwtResponse(response, currentSession.email);
        lastError.value = null;
        return true;
      } catch (error) {
        clearSession();
        lastError.value = getErrorMessage(error, 'Session refresh failed');
        return false;
      } finally {
        refreshInFlight = null;
      }
    })();

    return refreshInFlight;
  };

  registerAuthSessionProvider({
    getAccessToken: () => session.value?.token ?? null,
    refreshTokens,
    clearSession,
  });

  const init = async () => {
    if (initialized.value) {
      return;
    }

    const restoredSession = authStorage.load();
    if (restoredSession) {
      setSession(restoredSession);

      if (isJwtExpired(restoredSession.token, REFRESH_BUFFER_SECONDS)) {
        await refreshTokens();
      }
    }

    initialized.value = true;
  };

  const ensureActiveSession = async (): Promise<boolean> => {
    if (!session.value) {
      return false;
    }

    if (!isJwtExpired(session.value.token, REFRESH_BUFFER_SECONDS)) {
      return true;
    }

    return refreshTokens();
  };

  const login = async (credentials: LoginCredentials) => {
    working.value = true;
    lastError.value = null;

    try {
      const response = await apiRequest<JwtResponse>('/Account/Login', {
        method: 'POST',
        auth: false,
        retryOnAuth: false,
        body: credentials,
      });

      applyJwtResponse(response, credentials.email);
    } catch (error) {
      clearSession();
      lastError.value = getErrorMessage(error, 'Unable to sign in');
      throw error;
    } finally {
      working.value = false;
    }
  };

  const register = async (payload: RegisterPayload) => {
    working.value = true;
    lastError.value = null;

    try {
      const response = await apiRequest<JwtResponse>('/Account/Register', {
        method: 'POST',
        auth: false,
        retryOnAuth: false,
        body: payload,
      });

      applyJwtResponse(response, payload.email);
    } catch (error) {
      clearSession();
      lastError.value = getErrorMessage(error, 'Unable to create account');
      throw error;
    } finally {
      working.value = false;
    }
  };

  const logout = () => {
    lastError.value = null;
    clearSession();
  };

  return {
    session,
    initialized,
    working,
    lastError,
    isAuthenticated,
    tokenExpiresAt,
    sessionExpiresLabel,
    fullName,
    init,
    login,
    register,
    logout,
    refreshTokens,
    ensureActiveSession,
  };
});
