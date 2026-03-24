# Docker Deployment for JS and TS Task Managers

This repository deploys both task manager projects from a single nginx Docker container on the VPS.

## Public URLs

- JavaScript task manager: `https://ansiin.proxy.itcollege.ee/javascript-task-manager/`
- TypeScript task manager: `https://ansiin.proxy.itcollege.ee/typescript-task-manager/`
- Landing page: `https://ansiin.proxy.itcollege.ee/`

These paths are served from one container with these internal nginx directories:

- `/usr/share/nginx/html/javascript-task-manager/`
- `/usr/share/nginx/html/typescript-task-manager/`

## Repository Layout

- `javasciprt-task-manager/` - plain JavaScript task manager
- `typescript-task-manager/` - strict TypeScript task manager
- `deploy/` - nginx config and landing page for Docker image
- `Dockerfile` - multi-stage image build
- `docker-compose.yml` - VPS runtime definition
- `.gitlab-ci.yml` - GitLab CI deploy job

## Local Docker Run

```bash
docker compose up --build
```

Open:

- `http://localhost/`
- `http://localhost/javascript-task-manager/`
- `http://localhost/typescript-task-manager/`

## VPS Setup Summary

1. SSH into the VPS.
2. If you want to clone over SSH from the VPS, generate a key and add the public key in GitLab:

```bash
ssh-keygen -t ed25519 -C "ansiin-vps"
cat ~/.ssh/id_ed25519.pub
```

3. Install GitLab Runner and add it to the Docker group:

```bash
curl -L "https://packages.gitlab.com/install/repositories/runner/gitlab-runner/script.deb.sh" -o script.deb.sh
bash script.deb.sh
apt install gitlab-runner
usermod -aG docker gitlab-runner
```

4. Register the runner against `https://gitlab.proxy.itcollege.ee` with the `shell` executor and tag `shared`.
5. Make sure the proxy points `https://ansiin.proxy.itcollege.ee` to your VPS internal IP on port `80`.
6. Push to `main`. GitLab runs:

```bash
docker compose -p ansiin-task-managers up --build --remove-orphans --detach
```

## Deployment Notes

- The Docker image builds the TypeScript project inside `node:20-alpine`.
- The final image uses `nginx:alpine` and only serves static files.
- Both apps are available side-by-side under their own subpaths.
- If the proxy host or uni-id changes, update the URLs in this README and the VPS proxy mapping.
