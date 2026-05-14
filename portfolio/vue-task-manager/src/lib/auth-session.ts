export interface AuthSessionProvider {
  getAccessToken: () => string | null;
  refreshTokens: () => Promise<boolean>;
  clearSession: () => void;
}

let provider: AuthSessionProvider | null = null;

export const registerAuthSessionProvider = (nextProvider: AuthSessionProvider) => {
  provider = nextProvider;
};

export const getRegisteredAccessToken = (): string | null => provider?.getAccessToken() ?? null;

export const refreshRegisteredTokens = (): Promise<boolean> =>
  provider?.refreshTokens() ?? Promise.resolve(false);

export const clearRegisteredSession = (): void => {
  provider?.clearSession();
};
