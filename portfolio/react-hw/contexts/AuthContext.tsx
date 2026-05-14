"use client";

import { createContext, useCallback, useContext, useEffect, useMemo, useReducer, type ReactNode } from "react";
import { authApi, subscribeToApiSessionChanges } from "@/lib/api-client";
import { loadSession, type AuthSession } from "@/lib/auth-storage";
import type { LoginRequest, RegisterRequest } from "@/types/api";

type AuthState = {
  session: AuthSession | null;
  loading: boolean;
  error: string | null;
};

type AuthAction =
  | { type: "restore"; session: AuthSession | null }
  | { type: "start" }
  | { type: "success"; session: AuthSession }
  | { type: "logout" }
  | { type: "error"; error: string };

type AuthContextValue = AuthState & {
  login: (payload: LoginRequest) => Promise<void>;
  register: (payload: RegisterRequest) => Promise<void>;
  logout: () => void;
};

const AuthContext = createContext<AuthContextValue | null>(null);

function authReducer(state: AuthState, action: AuthAction): AuthState {
  switch (action.type) {
    case "restore":
      return { ...state, session: action.session, loading: false };
    case "start":
      return { ...state, loading: true, error: null };
    case "success":
      return { session: action.session, loading: false, error: null };
    case "logout":
      return { session: null, loading: false, error: null };
    case "error":
      return { ...state, loading: false, error: action.error };
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [state, dispatch] = useReducer(authReducer, {
    session: null,
    loading: true,
    error: null
  });

  useEffect(() => {
    dispatch({ type: "restore", session: loadSession() });
    subscribeToApiSessionChanges((session) => {
      dispatch(session ? { type: "success", session } : { type: "logout" });
    });
  }, []);

  const login = useCallback(async (payload: LoginRequest) => {
    dispatch({ type: "start" });
    try {
      dispatch({ type: "success", session: await authApi.login(payload) });
    } catch (error) {
      dispatch({ type: "error", error: error instanceof Error ? error.message : "Login failed." });
    }
  }, []);

  const register = useCallback(async (payload: RegisterRequest) => {
    dispatch({ type: "start" });
    try {
      dispatch({ type: "success", session: await authApi.register(payload) });
    } catch (error) {
      dispatch({ type: "error", error: error instanceof Error ? error.message : "Registration failed." });
    }
  }, []);

  const logout = useCallback(() => {
    authApi.logout();
    dispatch({ type: "logout" });
  }, []);

  const value = useMemo<AuthContextValue>(() => ({ ...state, login, register, logout }), [state, login, register, logout]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useAuth must be used inside AuthProvider.");
  return context;
}
