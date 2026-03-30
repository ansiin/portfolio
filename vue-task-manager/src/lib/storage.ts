import type { StoredAuthSession } from '../types/auth';

const STORAGE_KEY = 'vue-task-manager.auth-session';

const isValidStoredSession = (value: unknown): value is StoredAuthSession => {
  if (!value || typeof value !== 'object') {
    return false;
  }

  const session = value as Partial<StoredAuthSession>;
  return (
    typeof session.token === 'string' &&
    typeof session.refreshToken === 'string' &&
    typeof session.firstName === 'string' &&
    typeof session.lastName === 'string' &&
    typeof session.email === 'string'
  );
};

export const authStorage = {
  load(): StoredAuthSession | null {
    if (typeof window === 'undefined') {
      return null;
    }

    const raw = window.localStorage.getItem(STORAGE_KEY);
    if (!raw) {
      return null;
    }

    try {
      const parsed = JSON.parse(raw) as unknown;
      return isValidStoredSession(parsed) ? parsed : null;
    } catch {
      return null;
    }
  },

  save(session: StoredAuthSession): void {
    if (typeof window === 'undefined') {
      return;
    }

    window.localStorage.setItem(STORAGE_KEY, JSON.stringify(session));
  },

  clear(): void {
    if (typeof window === 'undefined') {
      return;
    }

    window.localStorage.removeItem(STORAGE_KEY);
  },
};
