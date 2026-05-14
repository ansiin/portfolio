## Investing Tracker

ASP.NET Core investing tracker with:
- MVC client UX
- protected admin area
- versioned REST API with public DTOs
- JWT + refresh token authentication
- separate JavaScript client in sibling `MyProjectUI` repo folder
- PostgreSQL + EF Core migrations
- UI i18n via `.resx`
- DB i18n via `LangStr`

## Repo Layout

- `MyProject` contains the ASP.NET Core backend, MVC app, API, tests and deployment files
- `MyProjectUI` contains the extracted standalone JavaScript client
- `.github/workflows/myproject-ci.yml` runs CI for `MyProject` and `MyProjectUI`
- `.github/workflows/myproject-deploy.yml` deploys `MyProject` from GitHub Actions

## Local Run

Prerequisites:
- `.NET 8` SDK
- PostgreSQL if running without Docker
- Docker Desktop if running the full container setup

### Option 1: full local stack with Docker

```bash
docker compose --env-file .env.docker.example up --build -d
```

This starts:
- backend on `http://localhost:8080`
- JS client on `http://localhost:8081`
- PostgreSQL on `localhost:5432`

Swagger:
- `http://localhost:8080/swagger`

### Option 2: backend only without Docker

Make sure PostgreSQL is running with the connection string values from `WebApp/appsettings.json`.

```bash
dotnet build HWDemo.sln -v:minimal
dotnet run --project WebApp/WebApp.csproj --launch-profile https
```

This starts the MVC app and API on:
- `https://localhost:7192`
- `http://localhost:5065`

Swagger:
- `https://localhost:7192/swagger`

## Default Users

- `admin@taltech.ee` / `Kala.12345`
- `user@taltech.ee` / `Kala.12345`

## EF Core Migrations

Run from the solution root:

```bash
dotnet ef migrations add YourMigrationName --project App.DAL.EF --startup-project WebApp
dotnet ef migrations remove --project App.DAL.EF --startup-project WebApp
dotnet ef database update --project App.DAL.EF --startup-project WebApp
```

## VPS Deploy Without Breaking The Existing App

Existing VPS services that must stay untouched:
- `ansiin-task-managers-web` on host port `80`
- `ansiin-cweb-app` on host port `81`

This project is prepared to deploy separately with:
- compose project name: `ansiin-investing`
- backend host port: `82`
- JS client host port: `83`
- separate PostgreSQL volume name derived from `ansiin-investing`

Direct host access before proxy:
- backend: `http://192.168.181.136:82`
- swagger: `http://192.168.181.136:82/swagger`
- JS client: `http://192.168.181.136:83`

Files for VPS deployment:
- `docker-compose.vps.yml`
- `.env.vps.example`
- `../.github/workflows/myproject-deploy.yml`
- `deploy/vps-deploy.sh`

### Manual VPS bootstrap

1. Clone this repo to the VPS.
2. Copy `.env.vps.example` to `.env.vps`.
3. Set at minimum:
   - `POSTGRES_PASSWORD`
   - `JWT__Key`
4. Run:

```bash
sh deploy/vps-deploy.sh
```

This uses:
- `docker-compose.vps.yml`
- `.env.vps`
- `COMPOSE_PROJECT_NAME=ansiin-investing`

It does **not** run `docker compose down` on the old `ansiin-cweb` project.

### Suggested public URLs

- JS client: `https://ansiin-investing.proxy.itcollege.ee`
- backend/API/Swagger: `https://ansiin-investing-api.proxy.itcollege.ee`

### Reverse proxy routes to add

Assuming the proxy uses the same path-routing behavior as the existing `/acme` route:

- `https://ansiin-investing.proxy.itcollege.ee` -> `http://192.168.181.136:83`
- `https://ansiin-investing-api.proxy.itcollege.ee` -> `http://192.168.181.136:82`

Do **not** change the existing route:
- `https://ansiin.proxy.itcollege.ee/acme` -> `http://192.168.181.136:81`

Current proxy setup uses separate hostnames, so `PATH_BASE` is not needed.

### GitHub Actions

The repo now includes:
- `../.github/workflows/myproject-ci.yml` for build, test and frontend syntax checks
- `../.github/workflows/myproject-deploy.yml` for VPS deployment over SSH

Deploy assumptions:
- GitHub Actions can reach the VPS over SSH
- Docker and docker compose are installed on the VPS

Required GitHub Actions secrets:
- `VPS_HOST`
- `VPS_USERNAME`
- `VPS_SSH_KEY`
- `VPS_PORT`
- `VPS_APP_PATH`

`VPS_APP_PATH` may point either to the repo root or directly to the `MyProject` folder on the VPS.

Required values in VPS-side `.env.vps`:
- `POSTGRES_PASSWORD`
- `JWT__Key`

The deploy workflow runs:

```bash
docker compose -f docker-compose.vps.yml -p ansiin-investing up --build --remove-orphans --detach
```

This keeps the existing `ansiin-cweb` project intact because the compose project name and host ports are different.

## Verification

Verified in the current repo state:
- `dotnet build HWDemo.sln -v:minimal`
- `dotnet test App.Tests/App.Tests.csproj -v:minimal --no-build`
- `node --check ../MyProjectUI/app.js`
