export interface LoginCredentials {
  email: string;
  password: string;
}

export interface RegisterPayload extends LoginCredentials {
  firstName: string;
  lastName: string;
}

export interface JwtResponse {
  token: string;
  refreshToken: string;
  firstName: string;
  lastName: string;
}

export interface RefreshTokenPayload {
  jwt: string;
  refreshToken: string;
}

export interface StoredAuthSession extends JwtResponse {
  email: string;
}

export interface JwtPayload {
  exp?: number;
  email?: string;
  unique_name?: string;
  upn?: string;
  sub?: string;
  given_name?: string;
  family_name?: string;
  [key: string]: unknown;
}
