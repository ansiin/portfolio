import { clearRegisteredSession, getRegisteredAccessToken, refreshRegisteredTokens } from './auth-session';
import { API_BASE_URL } from './config';

export class ApiError extends Error {
  status: number;
  details: unknown;

  constructor(message: string, status: number, details: unknown) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
    this.details = details;
  }
}

interface ApiRequestOptions extends Omit<RequestInit, 'body'> {
  auth?: boolean;
  retryOnAuth?: boolean;
  body?: BodyInit | object | null;
}

const buildUrl = (path: string): string => {
  const normalizedPath = path.startsWith('/') ? path : `/${path}`;
  return `${API_BASE_URL}${normalizedPath}`;
};

const normalizeBody = (body: ApiRequestOptions['body']): BodyInit | undefined => {
  if (body == null) {
    return undefined;
  }

  if (body instanceof FormData || body instanceof Blob || typeof body === 'string') {
    return body;
  }

  return JSON.stringify(body);
};

const extractErrorMessage = (payload: unknown, fallback = 'Request failed'): string => {
  if (!payload) {
    return fallback;
  }

  if (typeof payload === 'string') {
    return payload;
  }

  if (typeof payload === 'object') {
    const data = payload as Record<string, unknown>;

    if (Array.isArray(data.messages)) {
      const messages = data.messages.filter((value): value is string => typeof value === 'string');
      if (messages.length > 0) {
        return messages.join(' ');
      }
    }

    if (typeof data.message === 'string') {
      return data.message;
    }

    if (typeof data.title === 'string') {
      return data.title;
    }

    if (typeof data.detail === 'string') {
      return data.detail;
    }

    if (data.errors && typeof data.errors === 'object') {
      const validationErrors = Object.values(data.errors as Record<string, unknown>)
        .flatMap((value) => (Array.isArray(value) ? value : [value]))
        .filter((value): value is string => typeof value === 'string');

      if (validationErrors.length > 0) {
        return validationErrors.join(' ');
      }
    }
  }

  return fallback;
};

const parseResponsePayload = async (response: Response): Promise<unknown> => {
  if (response.status === 204) {
    return null;
  }

  const contentType = response.headers.get('content-type') || '';

  if (contentType.includes('application/json')) {
    return response.json();
  }

  const text = await response.text();
  if (!text) {
    return null;
  }

  try {
    return JSON.parse(text) as unknown;
  } catch {
    return text;
  }
};

const requestInternal = async <T>(
  path: string,
  options: ApiRequestOptions,
  canRetryOnAuth: boolean,
): Promise<T> => {
  const headers = new Headers(options.headers);
  const requestBody = normalizeBody(options.body);

  if (!headers.has('Accept')) {
    headers.set('Accept', 'application/json');
  }

  if (requestBody && !(requestBody instanceof FormData) && !headers.has('Content-Type')) {
    headers.set('Content-Type', 'application/json');
  }

  if (options.auth !== false) {
    const token = getRegisteredAccessToken();
    if (token) {
      headers.set('Authorization', `Bearer ${token}`);
    }
  }

  const response = await fetch(buildUrl(path), {
    ...options,
    headers,
    body: requestBody,
  });

  if (response.status === 401 && options.auth !== false && canRetryOnAuth) {
    const refreshed = await refreshRegisteredTokens();
    if (refreshed) {
      return requestInternal<T>(path, options, false);
    }

    clearRegisteredSession();
  }

  const payload = await parseResponsePayload(response);

  if (!response.ok) {
    throw new ApiError(extractErrorMessage(payload, `Request failed with status ${response.status}`), response.status, payload);
  }

  return payload as T;
};

export const apiRequest = <T>(path: string, options: ApiRequestOptions = {}): Promise<T> =>
  requestInternal<T>(path, options, options.retryOnAuth ?? true);

export const getErrorMessage = (error: unknown, fallback = 'Something went wrong'): string => {
  if (error instanceof ApiError) {
    return error.message;
  }

  if (error instanceof Error) {
    return error.message;
  }

  return fallback;
};

