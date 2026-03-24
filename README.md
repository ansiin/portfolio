# JS and TS Task Managers Deployed to VPS

This repository contains two frontend course projects deployed to a VPS as Docker containers behind nginx.

## Public URLs

- Landing page: `https://ansiin.proxy.itcollege.ee/`
- JavaScript project: `https://ansiin.proxy.itcollege.ee/javascript-task-manager/`
- TypeScript project: `https://ansiin.proxy.itcollege.ee/typescript-task-manager/`

## What Is Deployed

- `javasciprt-task-manager/` - modular JavaScript task manager
- `typescript-task-manager/` - strict TypeScript task manager

Both apps are served from one nginx container under separate subpaths:

- `/javascript-task-manager/`
- `/typescript-task-manager/`

## Deployment Setup

- VPS host is exposed through the university proxy on port `80`
- GitLab Runner is installed on the VPS
- Deployment is triggered from the `main` branch through GitLab CI
- Docker uses a multi-stage build:
  - TypeScript app is built in `node:20-alpine`
  - Final static files are served from `nginx:alpine`

## Important Files

- `Dockerfile` - builds and packages both apps
- `docker-compose.yml` - starts the nginx container on the VPS
- `.gitlab-ci.yml` - deploys on push to `main`
- `deploy/nginx.conf` - nginx routing for both apps
- `deploy/index.html` - landing page with links to both projects

## Run Locally With Docker

```bash
docker compose up --build
```

Then open:

- `http://localhost/`
- `http://localhost/javascript-task-manager/`
- `http://localhost/typescript-task-manager/`

## CI/CD Deploy Command

The GitLab pipeline runs this command on the VPS:

```bash
docker compose -p ansiin-task-managers up --build --remove-orphans --detach
```
