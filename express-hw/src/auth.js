import bcrypt from "bcryptjs";
import jwt from "jsonwebtoken";
import { randomBytes } from "node:crypto";
import { v4 as uuid } from "uuid";
import { config } from "./config.js";
import { getStore, saveStore } from "./store.js";
import { message } from "./problem.js";

function accessTokenSeconds(req) {
  const value = Number(req.query.expiresInSeconds);
  if (Number.isInteger(value) && value > 0 && value <= 24 * 60 * 60) return value;
  return config.defaultAccessTokenSeconds;
}

function signJwt(user, expiresInSeconds) {
  return jwt.sign(
    {
      email: user.email,
      firstName: user.firstName,
      lastName: user.lastName
    },
    config.jwtSecret,
    {
      subject: user.id,
      expiresIn: expiresInSeconds
    }
  );
}

function createRefreshToken(userId) {
  const now = new Date();
  const expiresAt = new Date(now.getTime() + config.refreshTokenDays * 24 * 60 * 60 * 1000);

  return {
    id: uuid(),
    userId,
    token: randomBytes(48).toString("base64url"),
    createdAt: now.toISOString(),
    expiresAt: expiresAt.toISOString(),
    revokedAt: null
  };
}

function jwtResponse(user, refreshToken, expiresInSeconds) {
  return {
    token: signJwt(user, expiresInSeconds),
    refreshToken: refreshToken.token,
    firstName: user.firstName,
    lastName: user.lastName
  };
}

export async function register(req, res) {
  const { email, password, firstName, lastName } = req.body || {};
  if (!email || !password || !firstName || !lastName) {
    return message(res, 400, "email, password, firstName and lastName are required.");
  }

  const store = await getStore();
  const normalizedEmail = String(email).trim().toLowerCase();
  if (store.users.some((user) => user.email === normalizedEmail)) {
    return message(res, 400, "Account with this email already exists.");
  }

  const user = {
    id: uuid(),
    email: normalizedEmail,
    passwordHash: await bcrypt.hash(String(password), 12),
    firstName: String(firstName),
    lastName: String(lastName),
    createdAt: new Date().toISOString()
  };

  const refreshToken = createRefreshToken(user.id);
  store.users.push(user);
  store.refreshTokens.push(refreshToken);
  await saveStore();

  return res.json(jwtResponse(user, refreshToken, accessTokenSeconds(req)));
}

export async function login(req, res) {
  const { email, password } = req.body || {};
  const store = await getStore();
  const user = store.users.find((item) => item.email === String(email || "").trim().toLowerCase());

  if (!user || !(await bcrypt.compare(String(password || ""), user.passwordHash))) {
    return message(res, 404, "Invalid email or password.");
  }

  const refreshToken = createRefreshToken(user.id);
  store.refreshTokens.push(refreshToken);
  await saveStore();

  return res.json(jwtResponse(user, refreshToken, accessTokenSeconds(req)));
}

export async function refresh(req, res) {
  const { jwt: oldJwt, refreshToken } = req.body || {};
  if (!oldJwt || !refreshToken) return message(res, 400, "jwt and refreshToken are required.");

  let decoded;
  try {
    decoded = jwt.verify(oldJwt, config.jwtSecret, { ignoreExpiration: true });
  } catch {
    return message(res, 400, "JWT is invalid.");
  }

  const store = await getStore();
  const stored = store.refreshTokens.find((item) => item.userId === decoded.sub && item.token === refreshToken && !item.revokedAt);
  if (!stored || new Date(stored.expiresAt).getTime() <= Date.now()) {
    return message(res, 400, "Refresh token is invalid or expired.");
  }

  const user = store.users.find((item) => item.id === decoded.sub);
  if (!user) return message(res, 400, "User no longer exists.");

  stored.revokedAt = new Date().toISOString();
  const nextRefreshToken = createRefreshToken(user.id);
  store.refreshTokens.push(nextRefreshToken);
  await saveStore();

  return res.json(jwtResponse(user, nextRefreshToken, accessTokenSeconds(req)));
}

export async function requireAuth(req, res, next) {
  const header = req.header("authorization") || "";
  const [scheme, token] = header.split(" ");

  if (scheme !== "Bearer" || !token) return message(res, 401, "Bearer token is required.");

  try {
    const decoded = jwt.verify(token, config.jwtSecret);
    const store = await getStore();
    const user = store.users.find((item) => item.id === decoded.sub);
    if (!user) return message(res, 401, "User no longer exists.");
    req.user = user;
    return next();
  } catch {
    return message(res, 401, "Bearer token is invalid or expired.");
  }
}
