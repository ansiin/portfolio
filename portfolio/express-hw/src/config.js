export const config = {
  port: Number(process.env.PORT || 4000),
  jwtSecret: process.env.JWT_SECRET || "dev-only-change-me-access-secret",
  dataFile: process.env.DATA_FILE || "./data/db.json",
  corsOrigin: process.env.CORS_ORIGIN || "*",
  defaultAccessTokenSeconds: 15 * 60,
  refreshTokenDays: Number(process.env.REFRESH_TOKEN_DAYS || 14)
};
