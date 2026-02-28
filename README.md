# AdventureRental

A motorcycle rental web application built with .NET 9. Customers can browse and rent Voge motorcycles online; administrators manage inventory, reservations, and customer accounts.

## Features

- **Equipment catalogue** — browse motorcycles by category, search by name, paginated grid with photos
- **Reservations** — authenticated customers select dates and equipment, system checks live availability
- **Admin panel** — manage equipment, categories, customers; view and action all reservations (confirm, complete, cancel)
- **JWT authentication** — register/login with email; roles: `Admin` and `Customer`
- **Swagger UI** — interactive API docs with JWT bearer support at `/swagger`

## Tech Stack

| | |
|---|---|
| Backend | ASP.NET Core Web API (.NET 9) |
| Frontend | Blazor WebAssembly (.NET 9) |
| Database | PostgreSQL 16, EF Core 9 (Npgsql) |
| Auth | ASP.NET Core Identity + JWT Bearer |
| Tests | xUnit, EF Core InMemory |

## Project Structure

```
src/
├── AdventureRental.Core/           # Entities, interfaces, DTOs, enums
├── AdventureRental.Infrastructure/ # DbContext, repositories, migrations, TokenService
├── AdventureRental.Api/            # Web API controllers + serves Blazor WASM
└── AdventureRental.Web/            # Blazor WebAssembly client
tests/
└── AdventureRental.Tests/          # xUnit tests
```

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL 16

### 1. Database

Create the database and user in psql:

```sql
CREATE DATABASE "AdventureRental";
CREATE USER adventureuser WITH PASSWORD 'your-password';
GRANT ALL PRIVILEGES ON DATABASE "AdventureRental" TO adventureuser;
\c "AdventureRental"
GRANT ALL ON SCHEMA public TO adventureuser;
```

### 2. Configuration

Create `src/AdventureRental.Api/appsettings.Development.json` (gitignored):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=AdventureRental;Username=adventureuser;Password=your-password"
  }
}
```

Store secrets with the .NET user-secrets tool:

```bash
dotnet user-secrets set "JwtSettings:SecretKey" "replace-with-32+-char-random-string" --project src/AdventureRental.Api
dotnet user-secrets set "SeedData:AdminPassword" "YourAdminPassword123!" --project src/AdventureRental.Api
```

### 3. Run

```bash
dotnet ef database update -p src/AdventureRental.Infrastructure -s src/AdventureRental.Api
dotnet run --project src/AdventureRental.Api
```

On first startup the app automatically applies migrations, creates `Admin` and `Customer` roles, and seeds a default admin account using the email from `appsettings.json` (`SeedData:AdminEmail`) and the password from user-secrets.

Open `http://localhost:5228` for the Blazor frontend or `http://localhost:5228/swagger` for the API.

### 4. Tests

```bash
dotnet test
```

## API Overview

| Area | Endpoints |
|---|---|
| Auth | `POST /api/v1/auth/register` · `POST /api/v1/auth/login` · `GET /api/v1/auth/me` |
| Equipment | `GET /api/v1/equipment` · `GET /api/v1/equipment/{id}` · `GET /api/v1/equipment/{id}/availability` · CRUD (Admin) |
| Categories | `GET /api/v1/equipment-categories` · CRUD (Admin) |
| Reservations | `GET /api/v1/reservations` (Admin) · `GET /api/v1/reservations/my` · `POST /api/v1/reservations` · status transitions |
| Customers | `GET /api/v1/customers` (Admin) · `GET /api/v1/customers/me` · CRUD |

Full interactive docs available at `/swagger`.

## Sample Data

The database ships with seven [Voge](https://www.vogeglobal.com) motorcycle models across three categories:

| Category | Models |
|---|---|
| Sport | Voge 300RR · Voge RR525 · Voge RR660S |
| Adventure | Voge DS625X · Voge DS900X |
| Naked / Roadster | Voge 525R · Voge 900 DSX |
