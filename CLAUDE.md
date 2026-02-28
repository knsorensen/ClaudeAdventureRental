# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with this repository.

## Project

**AdventureRental** — a motorcycle rental web application built with .NET 9. Customers browse and rent Voge motorcycles; admins manage inventory, reservations, and customers.

## Tech Stack

| Layer | Technology |
|---|---|
| Backend API | ASP.NET Core Web API (.NET 9) |
| Frontend | Blazor WebAssembly (.NET 9), hosted by the API |
| Database | PostgreSQL 16 via EF Core 9 + Npgsql |
| Auth | ASP.NET Core Identity + JWT Bearer |
| API Docs | Swashbuckle (Swagger UI) v6 |
| Tests | xUnit with EF Core InMemory provider |

## Solution Structure

```
AdventureRental/
├── src/
│   ├── AdventureRental.Core/           # Domain entities, interfaces, DTOs, enums
│   ├── AdventureRental.Infrastructure/ # EF Core DbContext, repositories, migrations, services
│   ├── AdventureRental.Api/            # ASP.NET Core Web API — also serves Blazor WASM
│   └── AdventureRental.Web/            # Blazor WebAssembly frontend
└── tests/
    └── AdventureRental.Tests/          # xUnit integration tests
```

## Running Locally

### Prerequisites
- .NET 9 SDK (`~/.dotnet/` if user-installed; set `PATH` and `DOTNET_ROOT` accordingly)
- PostgreSQL 16 running locally
- Database and user created (see below)

### Database setup (one-time)
```sql
CREATE DATABASE "AdventureRental";
CREATE USER adventureuser WITH PASSWORD 'your-password';
GRANT ALL PRIVILEGES ON DATABASE "AdventureRental" TO adventureuser;
\c "AdventureRental"
GRANT ALL ON SCHEMA public TO adventureuser;
```

### Configuration
Real credentials go in `src/AdventureRental.Api/appsettings.Development.json` (gitignored):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=AdventureRental;Username=adventureuser;Password=your-password"
  }
}
```

JWT secret is stored in user-secrets (never in appsettings):
```bash
dotnet user-secrets set "JwtSettings:SecretKey" "your-32+-char-secret" --project src/AdventureRental.Api
dotnet user-secrets set "SeedData:AdminPassword" "your-admin-password" --project src/AdventureRental.Api
```

### Run
```bash
export PATH="$HOME/.dotnet:$HOME/.dotnet/tools:$PATH"
export DOTNET_ROOT="$HOME/.dotnet"

dotnet ef database update -p src/AdventureRental.Infrastructure -s src/AdventureRental.Api
dotnet run --project src/AdventureRental.Api
```

The API runs on `http://localhost:5228`. On startup it auto-migrates and seeds Admin/Customer roles plus the default admin account.

## Key Architecture Decisions

- **Soft delete for Equipment**: `DELETE /equipment/{id}` sets `Status = Retired` rather than hard-deleting, preserving reservation history.
- **Price snapshot**: `ReservationItem.DailyRate` captures the rate at booking time — price changes don't affect existing reservations.
- **Customer ≠ ApplicationUser**: Admin accounts have no `Customer` record. A `Customer` is linked to a user via `CustomerId` on `ApplicationUser`.
- **JWT auth in Blazor**: `JwtAuthStateProvider` reads the token from `localStorage` and parses claims client-side. `CustomAuthMessageHandler` attaches the `Authorization: Bearer` header to all `HttpClient` requests.
- **Swagger v6**: Swashbuckle must be pinned to v6.x — v10 has breaking API changes incompatible with the current setup.
- **EF Core / dotnet-ef versioning**: All EF Core packages and the global `dotnet-ef` tool must be v9.x. The default `dotnet tool install` pulls v10, which is incompatible with .NET 9 projects.

## API Routes (`/api/v1/`)

| Controller | Endpoints |
|---|---|
| Auth | `POST /auth/register`, `POST /auth/login`, `GET /auth/me` |
| Equipment | `GET /equipment` (paginated+filterable), `GET /equipment/{id}`, `GET /equipment/{id}/availability`, `POST /equipment` (Admin), `PUT /equipment/{id}` (Admin), `DELETE /equipment/{id}` (Admin) |
| Categories | `GET /equipment-categories`, `GET /equipment-categories/{id}`, `POST`, `PUT`, `DELETE` (Admin) |
| Reservations | `GET /reservations` (Admin), `GET /reservations/my` (Customer), `POST /reservations`, `PUT /reservations/{id}/confirm`, `PUT /reservations/{id}/cancel`, `PUT /reservations/{id}/complete` |
| Customers | `GET /customers` (Admin), `GET /customers/me`, `POST /customers` (Admin), `PUT /customers/{id}` |

## EF Core Migrations

```bash
export DOTNET_ROOT="$HOME/.dotnet"
# Add a new migration
dotnet ef migrations add <MigrationName> -p src/AdventureRental.Infrastructure -s src/AdventureRental.Api -o Data/Migrations
# Apply migrations
dotnet ef database update -p src/AdventureRental.Infrastructure -s src/AdventureRental.Api
```

## Blazor Pages

| Page | Route | Auth |
|---|---|---|
| Home | `/` | Public |
| Equipment list | `/equipment` | Public |
| Equipment detail | `/equipment/{Id}` | Public |
| Equipment form | `/admin/equipment/new`, `/admin/equipment/{Id}/edit` | Admin |
| Login / Register | `/auth/login`, `/auth/register` | Anonymous |
| My Reservations | `/reservations` | Authenticated |
| Reservation detail | `/reservations/{Id}` | Authenticated |
| New Reservation | `/reservations/new` | Authenticated |
| Customer list | `/admin/customers` | Admin |
| Customer detail | `/admin/customers/{Id}` | Admin |

## Common Gotchas

- When using `@page` as a C# variable name inside Blazor HTML, write `@(page)` to avoid the Razor parser treating it as the `@page` directive.
- Nested `<AuthorizeView>` components require a unique `Context="name"` attribute on inner components to avoid parameter name conflicts.
- `DOTNET_ROOT` must be set before running `dotnet ef` or other global tools when .NET is user-installed.
