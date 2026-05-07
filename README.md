# JS, TS, Vue, React and Express HW Deployed to VPS

This repository contains frontend course projects and the Express.js API assignment deployed to a VPS as Docker containers behind nginx.

## Public URLs

- Landing page: `https://ansiin.proxy.itcollege.ee/`
- JavaScript project: `https://ansiin.proxy.itcollege.ee/javascript-task-manager/`
- TypeScript project: `https://ansiin.proxy.itcollege.ee/typescript-task-manager/`
- Vue project: `https://ansiin.proxy.itcollege.ee/vue-task-manager/`
- Express API assignment: `https://ansiin.proxy.itcollege.ee/express-hw-api/`
- React client against Express API: `https://ansiin.proxy.itcollege.ee/react-hw-express/`
- Vue client against Express API: `https://ansiin.proxy.itcollege.ee/vue-task-manager-express/`

## What Is Deployed

- `javasciprt-task-manager/` - modular JavaScript task manager
- `typescript-task-manager/` - strict TypeScript task manager
- `vue-task-manager/` - Vue 3 + Pinia + Router task manager with JWT and refresh-token security
- `react-hw/` - React/Next.js client reused for the Express backend deployment
- `express-hw/` - Express.js reimplementation of the TalTech ToDo API auth and ToDo endpoints

The JavaScript and TypeScript apps are served from the main nginx container under separate subpaths:

- `/javascript-task-manager/`
- `/typescript-task-manager/`

The Vue app is built into its own Docker container and reverse-proxied by nginx under:

- `/vue-task-manager/`

The Express assignment uses three separate containers:

- `/express-hw-api/` proxies to the Express.js backend
- `/react-hw-express/` proxies to the React client rebuilt with the Express API base URL
- `/vue-task-manager-express/` proxies to the Vue client rebuilt with the Express API base URL

## Deployment Setup

- VPS host is exposed through the university proxy on port `80`
- GitLab Runner is installed on the VPS
- Deployment is triggered from the `main` branch through GitLab CI
- Docker uses a mixed setup:
  - JavaScript and TypeScript stay on the main web container
  - Vue 3 is built and served from a separate container
  - Express HW is built and served from a separate API container with a persistent Docker volume
  - React and Vue are rebuilt into separate containers for the Express backend
  - nginx reverse-proxies `/vue-task-manager/` to that container

## Important Files

- `Dockerfile` - builds and packages the JavaScript and TypeScript apps for the main web container
- `docker-compose.yml` - starts the main nginx container and the dedicated Vue container
- `.gitlab-ci.yml` - deploys on push to `main`
- `deploy/nginx.conf` - nginx routing for all public paths
- `deploy/index.html` - landing page with links to all projects
- `vue-task-manager/Dockerfile` - builds and serves the Vue app separately
- `express-hw/Dockerfile` - builds and serves the Express.js backend
- `react-hw/Dockerfile` - builds and serves the React client reused for Express API deployment

## Run Locally With Docker

```bash
docker compose up --build
```

Then open:

- `http://localhost/`
- `http://localhost/javascript-task-manager/`
- `http://localhost/typescript-task-manager/`
- `http://localhost/vue-task-manager/`
- `http://localhost/express-hw-api/health`
- `http://localhost/react-hw-express/`
- `http://localhost/vue-task-manager-express/`

## CI/CD Deploy Command

The GitLab pipeline runs this command on the VPS:

```bash
docker compose -p ansiin-task-managers up --build --remove-orphans --detach
```
