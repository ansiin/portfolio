# React HW

React/Next.js ToDo client for the TalTech Akaver backend.

The assignment explicitly requires React usage. This project uses Next.js with React 18, React context, and reducers. The backend in `react-hw/Backend` is reference source only and is not modified by this frontend.

## Public URL

Full public path after VPS deployment:

`https://ansiin.proxy.itcollege.ee/react-hw/`

## Backend

- API base URL: `https://taltech.akaver.com/api/v1`
- Swagger source in this repo: `react-hw/swagger.json`
- Swagger public URL: `https://taltech.akaver.com/swagger/index.html`

## Implemented

- Login and registration against `/api/v1/Account`
- JWT Authorization header for all ToDo requests
- Refresh-token flow through `/api/v1/Account/RefreshToken`
- Automatic token refresh before expiry and retry after `401`
- Auth state with React context + reducer
- ToDo state with React context + reducer
- No prop drilling for auth or ToDo data
- TodoTasks CRUD basics
- TodoCategories and TodoPriorities creation
- Dockerfile for a separate frontend container

## Local Development

```bash
npm install
npm run dev
```

Open `http://localhost:3000`.

## Docker

```bash
docker compose up --build
```

Open `http://localhost:3000/`. The container redirects to `http://localhost:3000/react-hw/` because the production build uses the `/react-hw` base path.

## VPS Deployment

Build as a separate Docker container:

```bash
docker compose -f react-hw/docker-compose.yml up --build --detach
```

When reverse-proxying through nginx, route `/react-hw/` to this container on port `3000` without stripping the `/react-hw` prefix because Next.js is built with `NEXT_PUBLIC_BASE_PATH=/react-hw`.

Example:

```nginx
location /react-hw/ {
    proxy_pass http://react-hw:3000;
    proxy_set_header Host $host;
    proxy_set_header X-Real-IP $remote_addr;
    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    proxy_set_header X-Forwarded-Proto $scheme;
}
```
