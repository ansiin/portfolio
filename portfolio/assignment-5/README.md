# Assignment 5: React 1

React/Next.js ToDo client against the official TalTech Akaver backend.

This folder is kept as the complete Assignment 5 deliverable. It uses React/Next.js, JWT access tokens, refresh-token security, React context, and reducers. There is no prop drilling for auth or ToDo state.

## Public URL

Full public path after VPS deployment:

`https://ansiin.proxy.itcollege.ee/assignment-5/`

## Backend

- API base URL: `https://taltech.akaver.com/api/v1`
- Swagger source in this folder: `assignment-5/swagger.json`
- Swagger public URL: `https://taltech.akaver.com/swagger/index.html`
- Backend source: `https://git2.akaver.com/taltech-public/com.akaver.taltech`

The backend was overhauled on 2026-04-05 and its database was recreated.

## Implemented

- Login and registration against `/api/v1/Account`
- JWT Authorization header for all ToDo requests
- Refresh-token flow through `/api/v1/Account/RefreshToken`
- Automatic token refresh before expiry and retry after `401`
- Auth state with React context + reducer
- ToDo state with React context + reducer
- TodoTasks CRUD basics
- TodoCategories and TodoPriorities creation
- Demo seed button for starter categories, priorities, and tasks
- Dockerfile for a separate frontend container

## Local Development

```bash
npm install
npm run dev
```

Open `http://localhost:3000`.

## Docker

This assignment is deployed through the repository root compose file as a separate container with:

- `NEXT_PUBLIC_API_BASE_URL=https://taltech.akaver.com/api/v1`
- `NEXT_PUBLIC_BASE_PATH=/assignment-5`
