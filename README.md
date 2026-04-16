# Products API

A .NET 8 Web API built with Vertical Slice Architecture, MediatR, and Entity Framework Core.

## Tech Stack

- .NET 8 Minimal APIs
- Entity Framework Core 8 (SQL Server)
- MediatR 12
- FluentValidation 11
- JWT Bearer authentication
- xUnit (unit + integration tests)

## Endpoints

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| GET | `/health` | No | Health check |
| POST | `/auth/token` | No | Get a JWT token |
| POST | `/api/products` | Yes | Create a product |
| GET | `/api/products?colour={value}` | Yes | List products (optional colour filter) |

## Getting Started

### 1. Configure the database

Set your SQL Server connection string in `appsettings.Development.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=...;Database=ProductsApi;Trusted_Connection=True;"
}
```

### 2. Run EF migrations

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 3. Run the API

```bash
dotnet run
```

Swagger UI is available at `https://localhost:{port}/swagger`.

## Using the API

1. Open Swagger UI and call `POST /auth/token` with:
   ```json
   { "username": "admin", "password": "password123" }
   ```
2. Copy the returned token, click **Authorize**, and paste it in.
3. All secured endpoints are now unlocked.

> **Note:** The `/auth/token` endpoint uses hardcoded credentials for development and testing only. It must be replaced before any production deployment. As this is a coding test exercise, no user management or secure password storage is implemented.

## Running Tests

```bash
dotnet test
```

Tests use an EF Core in-memory database — no SQL Server required.
