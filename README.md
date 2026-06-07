# BookingHack

> **Test / learning project.** Not intended for production use.

A sandbox ASP.NET Core Web API built to explore and practice common backend patterns: authentication, resource management, and team membership — wired together with a realistic tech stack.

## What it does

- **Auth** — JWT-based registration and login with refresh token rotation (tokens stored in Redis)
- **Companies** — create and manage companies
- **Company Members** — assign users to companies with role-based membership

## Tech stack

| Concern | Choice |
|---|---|
| Runtime | .NET 10 |
| Database | PostgreSQL (EF Core) |
| Cache / sessions | Redis |
| Validation | FluentValidation |
| API docs | Swagger / Swashbuckle (JWT auth wired in) |
| Logging | Serilog → Console + Seq |
| Tests | NUnit + NSubstitute + FluentAssertions |
| CI | GitHub Actions (`.github/workflows/ci.yml`) |

## Architecture

Loosely follows Clean Architecture with four layers:

```
BookingHack.Domain              — entities, core abstractions
BookingHack.Application         — use cases, validators, service interfaces
BookingHack.Infrastructure.*    — EF Core + PostgreSQL, Redis
BookingHack.Api                 — controllers, DI wiring, Swagger config
BookingHack.Tests               — unit tests (application layer)
```

## CI

The GitHub Actions workflow runs on every push and PR to `master`:

1. `dotnet restore`
2. `dotnet build --configuration Release`
3. `dotnet test --configuration Release`

No deployment step.

## Running locally

**Prerequisites:** .NET 10 SDK, Docker (for Postgres + Redis)

```bash
# Start dependencies
docker run -d -p 5432:5432 -e POSTGRES_PASSWORD=postgres postgres:17
docker run -d -p 6379:6379 redis:7

# Run the API
dotnet run --project BookingHack.Api
```

Swagger UI is available at `https://localhost:{port}/swagger`.