# DentalSaaS

ASP.NET Core MVC + EF Core + ASP.NET Core Identity solution for a multi-tenant dental practice SaaS.

## Public URL

- `https://ansiin-first-project.proxy.itcollege.ee/acme`

Default proxy route:

- `https://ansiin-first-project.proxy.itcollege.ee => http://192.168.181.136:84`

The seeded tenant slug is `acme`, so the full public path for the app is `/acme`.

## Architecture

- `DentalSaaS.Web`
  MVC controllers, views, authentication endpoints and tenant middleware.
- `DentalSaaS.Application`
  Business services, DTOs, orchestration and permission checks.
- `DentalSaaS.Domain`
  Entities and business models.
- `DentalSaaS.Infrastructure`
  EF Core persistence, repositories, Identity integration, PostgreSQL migrations, seeding and authorization handlers.
- `DentalSaaS.Shared`
  Shared constants, enums and result models.
- `DentalSaaS.Application.Tests`
  Unit tests for application services.

## Database And Deployment

- Production uses PostgreSQL in Docker Compose.
- The app is built into a Docker image with [Dockerfile](Dockerfile).
- PostgreSQL and the web app are started together with [docker-compose.yml](docker-compose.yml).
- EF Core migrations are configured for PostgreSQL.
- Application startup runs `Database.Migrate()` and then seeds demo data.
- The current PostgreSQL migration is [DentalSaaS.Infrastructure/Persistence/Migrations/20260324104702_InitialPostgreSql.cs](DentalSaaS.Infrastructure/Persistence/Migrations/20260324104702_InitialPostgreSql.cs).

## Seeded Users

- System users:
  - `systemadmin@dentalsaas.local / SystemAdmin123!`
  - `support@dentalsaas.local / SystemSupport123!`
  - `billing@dentalsaas.local / SystemBilling123!`
- Demo tenant:
  - `acme`
- Demo tenant users:
  - `owner@acme.local / Owner123!`
  - `admin@acme.local / Admin123!`
  - `manager@acme.local / Manager123!`
  - `employee@acme.local / Employee123!`

## Local Run

For local Docker testing:

```powershell
Copy-Item .env.example .env
docker compose up --build -d
```

The default local URL is `http://localhost:84/acme`.

## CI/CD

The pipeline is defined in [.gitlab-ci.yml](.gitlab-ci.yml) and follows the lecture structure:

- `build`
  Runs `dotnet restore` and `dotnet build` inside the official `.NET 9` SDK Docker image.
- `test`
  Runs `dotnet test` inside the official `.NET 9` SDK Docker image.
- `deploy`
  Runs on the VPS GitLab Runner with `shell` executor and executes:

```bash
docker compose -p ansiin-cweb up --build --remove-orphans --detach
```

The deploy job is configured for branch `main` and runner tag `shared`.

## VPS Setup

Docker is already installed on the VPS. The remaining setup is:

1. SSH into the VPS.
2. Install GitLab Runner:

```bash
curl -L "https://packages.gitlab.com/install/repositories/runner/gitlab-runner/script.deb.sh" -o script.deb.sh
bash script.deb.sh
apt install gitlab-runner
```

3. Add the runner user to the Docker group:

```bash
sudo usermod -aG docker gitlab-runner
sudo systemctl restart gitlab-runner
```

4. In GitLab, create a project runner with tag `shared`.
5. Register the runner on the VPS and choose `shell` executor:

```bash
gitlab-runner register --url https://gitlab.proxy.itcollege.ee --token glrt-<YOUR_TOKEN>
```

6. If `gitlab.proxy.itcollege.ee` does not resolve to the internal network IP, add this to `/etc/hosts`:

```text
192.168.183.251 gitlab.proxy.itcollege.ee
```

7. Make sure port `84` is free on the VPS, because the university proxy forwards traffic there.

## GitLab CI/CD Variables

For this setup you do not need SSH deploy variables in GitLab. The runner executes directly on the VPS.

Add at least these CI/CD variables:

- `DENTAL_POSTGRES_PASSWORD`
  Strong production password for DentalSaaS PostgreSQL.

Optional variables:

- `POSTGRES_DB`
  Defaults to `dentalsaas`.
- `POSTGRES_USER`
  Defaults to `dentalsaas`.
- `POSTGRES_PASSWORD`
  Legacy fallback variable. Prefer `DENTAL_POSTGRES_PASSWORD` so this project does not conflict with other VPS deployments.
- `APP_PORT`
  Optional for local Docker testing. The CI/CD deploy binds the production app to host port `84`.

## Verification

These checks were run locally before preparing the deployment:

- `dotnet build DentalSaaS.sln`
- `dotnet test DentalSaaS.sln`
- `docker compose config`
- `docker compose build app`

## Notes

- HTTPS is terminated by the university proxy, so the container itself serves plain HTTP on host port `84` and container port `8080`.
- This deployment uses a GitLab Runner with `shell` executor directly on the VPS, so a separate SSH-based deploy step is not used in the final pipeline.
- If your GitLab Runner tag is not `shared`, update the `tags` section in [.gitlab-ci.yml](.gitlab-ci.yml).
