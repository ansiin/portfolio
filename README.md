# JS, TS and Vue Task Managers Deployed to VPS

This repository contains three frontend course projects deployed to a VPS as Docker containers behind nginx.

## Public URLs

- Landing page: `https://ansiin.proxy.itcollege.ee/`
- JavaScript project: `https://ansiin.proxy.itcollege.ee/javascript-task-manager/`
- TypeScript project: `https://ansiin.proxy.itcollege.ee/typescript-task-manager/`
- Vue project: `https://ansiin.proxy.itcollege.ee/vue-task-manager/`

## What Is Deployed

- `javasciprt-task-manager/` - modular JavaScript task manager
- `typescript-task-manager/` - strict TypeScript task manager
- `vue-task-manager/` - Vue 3 + Pinia + Router task manager with JWT and refresh-token security

The JavaScript and TypeScript apps are served from the main nginx container under separate subpaths:

- `/javascript-task-manager/`
- `/typescript-task-manager/`

The Vue app is built into its own Docker container and reverse-proxied by nginx under:

- `/vue-task-manager/`

## Deployment Setup

- VPS host is exposed through the university proxy on port `80`
- GitLab Runner is installed on the VPS
- Deployment is triggered from the `main` branch through GitLab CI
- Docker uses a mixed setup:
  - JavaScript and TypeScript stay on the main web container
  - Vue 3 is built and served from a separate container
  - nginx reverse-proxies `/vue-task-manager/` to that container

## Important Files

- `Dockerfile` - builds and packages the JavaScript and TypeScript apps for the main web container
- `docker-compose.yml` - starts the main nginx container and the dedicated Vue container
- `.gitlab-ci.yml` - deploys on push to `main`
- `deploy/nginx.conf` - nginx routing for all public paths
- `deploy/index.html` - landing page with links to all projects
- `vue-task-manager/Dockerfile` - builds and serves the Vue app separately

## Run Locally With Docker

```bash
docker compose up --build
```

Then open:

- `http://localhost/`
- `http://localhost/javascript-task-manager/`
- `http://localhost/typescript-task-manager/`
- `http://localhost/vue-task-manager/`

## CI/CD Deploy Command

The GitLab pipeline runs this command on the VPS:

```bash
docker compose -p ansiin-task-managers up --build --remove-orphans --detach
```
