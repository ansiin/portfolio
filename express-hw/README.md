# Express HW

Assignment 6 backend reimplementation for the TalTech ToDo API.

This project implements the API surface needed by the existing Vue and React clients:

- `POST /api/v1/Account/Register`
- `POST /api/v1/Account/Login`
- `POST /api/v1/Account/RefreshToken`
- `GET/POST /api/v1/TodoCategories`
- `GET/PUT/DELETE /api/v1/TodoCategories/:id`
- `GET/POST /api/v1/TodoPriorities`
- `GET/PUT/DELETE /api/v1/TodoPriorities/:id`
- `GET/POST /api/v1/TodoTasks`
- `GET/PUT/DELETE /api/v1/TodoTasks/:id`

The same routes are also mounted under `/api/v1.0` for the older Vue client base URL.

## Public URLs

Planned VPS paths:

- Express API: `https://ansiin.proxy.itcollege.ee/express-hw-api/`
- React client using Express API: `https://ansiin.proxy.itcollege.ee/react-hw-express/`
- Vue client using Express API: `https://ansiin.proxy.itcollege.ee/vue-task-manager-express/`

Client API base URLs:

- React: `https://ansiin.proxy.itcollege.ee/express-hw-api/api/v1`
- Vue: `https://ansiin.proxy.itcollege.ee/express-hw-api/api/v1.0`

## Local Development

```bash
npm install
npm run dev
```

API health check:

```bash
curl http://localhost:4000/health
```

## Docker

```bash
docker compose up --build
```

The container listens on `http://localhost:4000`.

## Notes

- Data is stored in JSON at `DATA_FILE`.
- Passwords are hashed with bcrypt.
- Access tokens are JWT Bearer tokens.
- Refresh tokens are opaque random values stored server-side and rotated on refresh.
- No Swagger UI is included, per assignment.
