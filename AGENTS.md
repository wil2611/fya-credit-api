# AGENTS.md

## Project overview

This repository contains the backend API for the FYA credit management technical assessment. The application registers credits, stores them in PostgreSQL and exposes the required operations through a REST API.

## Technology stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL and Npgsql
- Swagger / OpenAPI
- Docker for the local database
- Hangfire
- MailKit

## Project structure

```text
Controllers/
Data/
DTOs/
Entities/
Migrations/
```

- `Controllers`: handle HTTP requests, responses and status codes. Keep business logic out of controllers.
- `Data`: contains database configuration and the `AppDbContext`.
- `DTOs`: define endpoint request and response data. Prefer specific DTOs over exposing entities directly.
- `Entities`: persisted database models. The main entity is currently `Credit`.
- `Migrations`: Entity Framework Core database schema changes.

Create and apply migrations with:

```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

## Development guidelines

- Keep the implementation simple and readable.
- Prefer asynchronous database operations.
- Validate incoming API data in the backend.
- Do not concatenate user input into SQL queries.
- Use Entity Framework Core for database access.
- Keep controllers focused on HTTP responsibilities.
- Use clear names for classes, methods and properties.
- Do not commit generated build folders such as `bin` or `obj`.
- Do not commit production credentials or secrets.
- Keep Swagger documentation working.
- Keep API error responses consistent.
- Do not expose exception details or sensitive information to clients.
- Keep rate limiting enabled for API endpoints.

## API conventions

Use REST-oriented endpoints such as `POST /api/credits` and `GET /api/credits`.

Use appropriate HTTP status codes, including `200 OK`, `201 Created`, `400 Bad Request`, `404 Not Found` and `500 Internal Server Error`.

Invalid requests should return an appropriate `400 Bad Request` response.

## Database

PostgreSQL is the selected database engine and Entity Framework Core is the ORM. Represent schema changes through migrations instead of modifying the database manually.

## Before completing a change

Run:

```bash
dotnet build
```

The project should compile without errors. For database-related changes, verify the corresponding migrations. For endpoint changes, test them through Swagger.

## Current functional scope

Currently implemented:

- PostgreSQL database integration.
- Entity Framework Core configuration and initial migration.
- Credit entity.
- Credit creation endpoint.
- Credit consultation.
- Search filters.
- Sorting.
- Background email notifications with Hangfire.
- SMTP email delivery with MailKit.
- Global exception handling with ProblemDetails.
- Rate limiting per client IP.
- Backend validation.
- Swagger / OpenAPI.

Still to be implemented:

- Additional security features.
