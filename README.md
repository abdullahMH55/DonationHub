# DonationHub

DonationHub is a .NET 10 REST API for donation management and crowdfunding. It uses Clean Architecture, EF Core with SQL Server, ASP.NET Core Identity with JWT, and CQRS with MediatR.

## Sprint 2 additions

- Every use case is a MediatR command or query; controllers only map HTTP requests to `IMediator.Send`.
- `Result` / `Result<T>` and the error catalogue are shared from the Domain project.
- `ValidationBehavior<TRequest, TResponse>` runs FluentValidation before handlers and returns `Validation.Failed` with field-level errors.
- Expected failures are returned as RFC 9457 `ProblemDetails` with `code`, `traceId` and optional `errors` fields.
- Unhandled exceptions are logged and returned as a generic 500 without leaking internals.
- Campaigns module: create, update, delete, public list, success stories, and public detail with `progressPercent` and `daysLeft`.
- Optimistic concurrency on campaigns through `RowVersion`; conflicts return `Campaign.ConcurrencyConflict` (409).
- Public campaign endpoints localize title and description from `Accept-Language` (`ar` or `en`, fallback `en`); admin responses return both languages.
- 41 passing unit tests covering domain rules, entity equality, validators, handlers, auth error handling, the validation pipeline, and the soft-delete interceptor.

## Project layout

- `DonationHub.Domain` - entities, value objects, enums, domain errors, and the Result kernel.
- `DonationHub.Application` - CQRS slices, DTOs, validators, repository ports, error catalogue, and pipeline behaviors.
- `DonationHub.Infrastructure` - EF Core, SQL Server, Identity, repositories, JWT/auth implementation, and migrations.
- `DonationHub.Api` - thin controllers, ProblemDetails mapping, exception handler, OpenAPI/Scalar.
- `DonationHub.UnitTests` - xUnit, Moq, FluentAssertions.

## Run locally

Requirements: .NET 10 SDK, SQL Server, and a JWT secret configured outside source control.

```powershell
dotnet user-secrets set "Jwt:SecretKey" "<a-long-random-secret>" --project DonationHub.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<sql-server-connection-string>" --project DonationHub.Api
dotnet ef database update --project DonationHub.Infrastructure --startup-project DonationHub.Api
dotnet run --project DonationHub.Api
```

In Development, Scalar is available at `/scalar/v1`.

Default seed configuration is under `SeedAdmin` in `appsettings.json`; override `SeedAdmin:Email`, `SeedAdmin:Password`, and `SeedAdmin:FullName` with user-secrets or environment variables. Do not commit secrets.

## Try the main endpoints

- `POST /api/v1/auth/register`
- `POST /api/v1/auth/login`
- `POST /api/v1/auth/refresh`
- `POST /api/v1/auth/logout` (authenticated)
- `GET /api/v1/auth/me` (authenticated)
- `POST /api/v1/admin/staff` (Admin only)
- `GET /api/v1/categories`
- `GET /api/v1/categories/{id}`
- `POST /api/v1/categories` (Admin only)
- `PUT /api/v1/categories/{id}` (Admin only)
- `DELETE /api/v1/categories/{id}` (Admin only)
- `GET /api/v1/campaigns`
- `GET /api/v1/campaigns/success-stories`
- `GET /api/v1/campaigns/{id}`
- `POST /api/v1/campaigns` (Admin only)
- `PUT /api/v1/campaigns/{id}` (Admin only, send the last `rowVersion`)
- `DELETE /api/v1/campaigns/{id}` (Admin only)

Public campaign calls accept `Accept-Language: ar` or `en`. Any other or missing value falls back to English.

## Read/write split

Commands load tracked entities through repository ports, enforce invariants in the domain, and save through `IUnitOfWork`. Query handlers use dedicated read methods with `AsNoTracking` (or `UserManager` for Identity reads) and map the results to DTOs; they never call `IUnitOfWork` and never expose domain entities. We kept reads on repository read methods instead of exposing `DbContext` to the Application layer: this preserves the layer boundary and avoids change tracking, at the cost of a small amount of read-specific repository code and mapping.

## Tests

```powershell
dotnet test
```

Pinned test packages: Moq `[4.18.4]` and FluentAssertions `[7.0.0]`. MediatR is pinned to `[12.5.0]`.

## Error handling

- Expected failures: handlers return `Result<T>`; `ToActionResult` maps `ErrorType` to the HTTP status and writes `application/problem+json`.
- Validation failures: 400 with `code: Validation.Failed` and an `errors` dictionary.
- Campaign conflicts: 409 with `code: Campaign.ConcurrencyConflict`.
- Missing or invalid JWTs: 401 with `code: Auth.Unauthorized`; role failures: 403 with `code: Auth.Forbidden`.
- Unexpected exceptions: 500 with `code: Common.UnexpectedError`, `traceId`, and no internal details.
