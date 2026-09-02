# AGENTS.md

## Project overview

This repository contains the backend API for the FYA credit management technical assessment.

The API is responsible for:

- Registering credits.
- Persisting credits in PostgreSQL.
- Listing, filtering and sorting credits.
- Validating incoming data.
- Scheduling email notifications in the background.
- Exposing the application through a REST API documented with Swagger / OpenAPI.

## Technology stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Npgsql
- Swagger / OpenAPI
- Hangfire
- SendGrid
- Docker
- Railway
- Supabase

## Architecture

### Main request flow

```text
Frontend / Android
      ↓
ASP.NET Core API
      ↓
Entity Framework Core
      ↓
PostgreSQL
```

### Background email flow

```text
POST /api/credits
      ↓
Persist credit
      ↓
Enqueue Hangfire job
      ↓
EmailService
      ↓
SendGrid HTTPS API
```

> **Rule:** Credit creation must not wait for the email provider to finish sending the notification.

## Project structure

```text
Configuration/
Controllers/
Data/
DTOs/
Entities/
Migrations/
Services/
Properties/
Program.cs
Dockerfile
```

### Responsibilities

- `Configuration/`: configuration models such as `SendGridSettings`.
- `Controllers/`: HTTP routing, request handling, response codes and orchestration.
- `Data/`: Entity Framework Core configuration and `AppDbContext`.
- `DTOs/`: request and response contracts.
- `Entities/`: persistent database models.
- `Migrations/`: Entity Framework Core schema changes.
- `Services/`: application services such as email delivery (`EmailService`).

## Development guidelines

- Keep the implementation simple and readable.
- Prefer asynchronous database and external-service operations.
- Keep controllers focused on HTTP responsibilities.
- Do not move email delivery directly into the credit creation request.
- Keep Hangfire responsible for background email execution.
- Use Entity Framework Core for database access.
- Do not concatenate user input into SQL queries.
- Validate incoming API data in the backend.
- Treat backend validation as authoritative.
- Prefer specific request and response DTOs instead of exposing entities directly.
- Do not commit `bin`, `obj` or other generated build outputs.
- Do not commit database credentials, SendGrid API keys or other secrets.
- Keep Swagger / OpenAPI working.
- Keep error responses consistent.
- Do not expose exception details or sensitive information to clients.
- Keep rate limiting enabled.
- Keep production configuration outside source-controlled files.
- Run a build before completing meaningful changes.

## API conventions

Main endpoints:

- `POST /api/credits`
- `GET /api/credits`

Use appropriate HTTP status codes:

- `200 OK`
- `201 Created`
- `400 Bad Request`
- `404 Not Found`
- `429 Too Many Requests`
- `500 Internal Server Error`

Invalid request data should return `400 Bad Request`.
Successful credit creation should return `201 Created`.

## Credit query behavior

`GET /api/credits` supports:

- `clientName`
- `clientDocument`
- `salesperson`
- `sortBy`
- `sortOrder`

Filtering and sorting should remain server-side. Do not duplicate these operations in the frontend when the API already provides them.

## Database

PostgreSQL is the selected database engine and Entity Framework Core is the ORM. Schema changes must be represented through migrations.

Create a migration:

```bash
dotnet ef migrations add MigrationName
```

Apply migrations:

```bash
dotnet ef database update
```

Do not manually modify production tables when a migration should represent the change.

### Local database

A local PostgreSQL 16 instance may be run with Docker.

### Production database

The deployed environment uses PostgreSQL hosted in Supabase. Production database credentials must be provided through configuration / environment variables and must never be committed.

## Background jobs

Hangfire is used for asynchronous processing.

When a credit is created:

1. Save the credit successfully.
2. Enqueue the email notification job.
3. Return the HTTP response without waiting for SendGrid.

Do not call SendGrid synchronously from `CreditsController`.
Hangfire currently uses PostgreSQL for its job storage.
The Hangfire dashboard should only be exposed in development unless authentication is added.

## Email delivery

SendGrid is the email provider. The application uses the SendGrid HTTPS API rather than SMTP.

Required configuration:

```text
SendGrid:ApiKey
SendGrid:FromEmail
SendGrid:FromName
```

Equivalent production environment variables:

```text
SendGrid__ApiKey
SendGrid__FromEmail
SendGrid__FromName
```

`FromEmail` must correspond to a verified SendGrid sender. Do not log or expose the API key.

Before final delivery, confirm that the notification recipient matches the technical assessment requirement: `fyasocialcapital@gmail.com`.

## Validation

The credit creation request validates:

- Client name (required, max 120 chars).
- Client document (required, max 30 chars).
- Credit amount (greater than zero).
- Interest rate (between 0 and 100).
- Term in months (greater than zero and within configured limit, e.g., 1-600).
- Salesperson (required, max 120 chars).

Validation changes should be implemented in the backend first.

## Error handling

The application uses ASP.NET Core `ProblemDetails` for global error responses.
Unexpected errors return a consistent response without exposing internal exception details.
The implementation adds a `traceId` to assist with troubleshooting.

## Rate limiting

API routes use per-IP rate limiting.
The current policy allows **60 requests per minute**.
Requests above the limit receive `429 Too Many Requests`.
Do not remove rate limiting without a clear project requirement.

## CORS

CORS must permit only the origins required by the frontend and Capacitor application:

- Local Ionic development.
- Capacitor / Android.
- Deployed mobile application consuming the Railway API.

Avoid `AllowAnyOrigin` unless there is an explicit reason.

## Swagger / OpenAPI

Swagger documentation must remain available for evaluation. The deployed API exposes Swagger publicly while the Hangfire dashboard remains development-only.

- Production API: `https://fya-credit-api-production.up.railway.app`
- Swagger UI: `https://fya-credit-api-production.up.railway.app/swagger`

## Docker and deployment

The repository contains a `Dockerfile`. Before deployment-related changes, verify that the image builds:

```bash
docker build -t fya-credit-api .
```

The backend is deployed on Railway from the GitHub repository. Railway production configuration is provided through environment variables. The application connects to Supabase for PostgreSQL and SendGrid for email delivery.

## Secrets and configuration

- Development secrets should use .NET User Secrets when appropriate.
- Production secrets belong in Railway environment variables.
- Never commit PostgreSQL production passwords, complete connection strings with credentials, SendGrid API keys, or other sensitive secrets.

## Before completing a change

1. Run build:
   ```bash
   dotnet build
   ```
2. For database changes, verify migrations with `dotnet ef database update`.
3. For endpoint changes, test through Swagger.
4. For email changes, verify:
   - The credit is persisted.
   - A Hangfire job is created.
   - The job succeeds.
   - SendGrid accepts the email.
5. For deployment changes, verify the public Railway endpoint after the deployment completes.

## Current functional scope

Implemented:

- PostgreSQL integration.
- Entity Framework Core.
- Database migrations.
- Credit entity.
- Credit creation endpoint (`POST /api/credits`).
- Credit listing endpoint (`GET /api/credits`).
- Search filters & sorting.
- Backend validation.
- Background jobs with Hangfire.
- SendGrid email delivery.
- Global exception handling with `ProblemDetails`.
- Per-IP rate limiting.
- CORS configuration.
- Swagger / OpenAPI.
- Docker support.
- Supabase production database.
- Railway deployment.
