import { clearSession, isJwtExpiring, loadSession, normalizeSession, saveSession, type AuthSession } from "@/lib/auth-storage";
import type { ApiMessage, JwtResponse, LoginRequest, RefreshTokenRequest, RegisterRequest, TodoCategory, TodoCategoryCreate, TodoPriority, TodoPriorityCreate, TodoTask, TodoTaskCreate } from "@/types/api";

const apiBaseUrl = process.env.NEXT_PUBLIC_API_BASE_URL ?? "https://taltech.akaver.com/api/v1";
const tokenLifetimeSeconds = 60 * 15;

let refreshPromise: Promise<AuthSession> | null = null;
let onSessionChanged: ((session: AuthSession | null) => void) | null = null;

export function subscribeToApiSessionChanges(handler: (session: AuthSession | null) => void): void {
  onSessionChanged = handler;
}

function notifySessionChanged(session: AuthSession | null): void {
  onSessionChanged?.(session);
}

function authUrl(path: string): string {
  return `${apiBaseUrl}${path}?expiresInSeconds=${tokenLifetimeSeconds}`;
}

async function parseError(response: Response): Promise<Error> {
  let message = `${response.status} ${response.statusText}`;

  try {
    const body = (await response.json()) as ApiMessage & { errors?: Record<string, string[]> };
    const validationMessages = body.errors ? Object.values(body.errors).flat() : [];

    if (validationMessages.length) message = validationMessages.join(" ");
    else if (body.messages?.length) message = body.messages.join(" ");
    else if (body.detail) message = body.detail;
    else if (body.title) message = body.title;
  } catch {
    const text = await response.text().catch(() => "");
    if (text) message = text;
  }

  return new Error(message);
}

async function requestJson<T>(url: string, init: RequestInit): Promise<T> {
  const response = await fetch(url, {
    ...init,
    headers: {
      "Content-Type": "application/json",
      Accept: "application/json",
      ...init.headers
    }
  });

  if (!response.ok) throw await parseError(response);
  if (response.status === 204) return undefined as T;
  return (await response.json()) as T;
}

async function refreshSession(): Promise<AuthSession> {
  const current = loadSession();
  if (!current) throw new Error("No active session.");

  if (!refreshPromise) {
    const body: RefreshTokenRequest = {
      jwt: current.token,
      refreshToken: current.refreshToken
    };

    refreshPromise = requestJson<JwtResponse>(authUrl("/Account/RefreshToken"), {
      method: "POST",
      body: JSON.stringify(body)
    })
      .then(normalizeSession)
      .then((session) => {
        saveSession(session);
        notifySessionChanged(session);
        return session;
      })
      .catch((error) => {
        clearSession();
        notifySessionChanged(null);
        throw error;
      })
      .finally(() => {
        refreshPromise = null;
      });
  }

  return refreshPromise;
}

async function authorizedRequest<T>(path: string, init: RequestInit = {}, retry = true): Promise<T> {
  let session = loadSession();
  if (!session) throw new Error("Please sign in first.");

  if (isJwtExpiring(session.token)) {
    session = await refreshSession();
  }

  const response = await fetch(`${apiBaseUrl}${path}`, {
    ...init,
    headers: {
      "Content-Type": "application/json",
      Accept: "application/json",
      Authorization: `Bearer ${session.token}`,
      ...init.headers
    }
  });

  if (response.status === 401 && retry) {
    await refreshSession();
    return authorizedRequest<T>(path, init, false);
  }

  if (!response.ok) throw await parseError(response);
  if (response.status === 204) return undefined as T;
  return (await response.json()) as T;
}

export const authApi = {
  async login(payload: LoginRequest): Promise<AuthSession> {
    const session = normalizeSession(
      await requestJson<JwtResponse>(authUrl("/Account/Login"), {
        method: "POST",
        body: JSON.stringify(payload)
      })
    );
    saveSession(session);
    notifySessionChanged(session);
    return session;
  },

  async register(payload: RegisterRequest): Promise<AuthSession> {
    const session = normalizeSession(
      await requestJson<JwtResponse>(authUrl("/Account/Register"), {
        method: "POST",
        body: JSON.stringify(payload)
      })
    );
    saveSession(session);
    notifySessionChanged(session);
    return session;
  },

  logout(): void {
    clearSession();
    notifySessionChanged(null);
  }
};

export const todoApi = {
  categories: {
    list: () => authorizedRequest<TodoCategory[]>("/TodoCategories"),
    create: (payload: TodoCategoryCreate) =>
      authorizedRequest<TodoCategory>("/TodoCategories", {
        method: "POST",
        body: JSON.stringify(payload)
      }),
    update: (payload: TodoCategory) =>
      authorizedRequest<TodoCategory>(`/TodoCategories/${payload.id}`, {
        method: "PUT",
        body: JSON.stringify(payload)
      }),
    remove: (id: string) => authorizedRequest<void>(`/TodoCategories/${id}`, { method: "DELETE" })
  },
  priorities: {
    list: () => authorizedRequest<TodoPriority[]>("/TodoPriorities"),
    create: (payload: TodoPriorityCreate) =>
      authorizedRequest<TodoPriority>("/TodoPriorities", {
        method: "POST",
        body: JSON.stringify(payload)
      }),
    update: (payload: TodoPriority) =>
      authorizedRequest<void>(`/TodoPriorities/${payload.id}`, {
        method: "PUT",
        body: JSON.stringify(payload)
      }),
    remove: (id: string) => authorizedRequest<void>(`/TodoPriorities/${id}`, { method: "DELETE" })
  },
  tasks: {
    list: () => authorizedRequest<TodoTask[]>("/TodoTasks"),
    create: (payload: TodoTaskCreate) =>
      authorizedRequest<TodoTask>("/TodoTasks", {
        method: "POST",
        body: JSON.stringify(payload)
      }),
    update: (payload: TodoTask) =>
      authorizedRequest<TodoTask>(`/TodoTasks/${payload.id}`, {
        method: "PUT",
        body: JSON.stringify(payload)
      }),
    remove: (id: string) => authorizedRequest<void>(`/TodoTasks/${id}`, { method: "DELETE" })
  }
};
