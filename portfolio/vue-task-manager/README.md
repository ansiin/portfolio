# Vue Task Manager

Vue 3 + Vite + TypeScript frontend for the TalTech ToDo API at `https://taltech.akaver.com`.

## Stack

- Vue 3
- Vue Router
- Pinia
- Fetch-based API client with automatic JWT refresh retry
- Docker + nginx for production serving

## Security Base

- JWT bearer token is attached centrally for protected requests.
- Refresh token rotation is handled in one place and retried only once per failed request.
- Session is restored from local storage and refreshed on startup if the JWT is already near expiry.
- Route guards block access to `/app/*` while the session is missing or expired.

## Local Development

```bash
npm install
npm run dev
```

Optional environment variables:

```bash
VITE_API_BASE_URL=https://taltech.akaver.com/api/v1.0
VITE_APP_BASE_PATH=/
```

## Production Build

```bash
npm install
npm run build
docker build -t vue-task-manager .
```

The container serves the built app with nginx. In the parent repository deployment, the app is expected under `/vue-task-manager/`.
