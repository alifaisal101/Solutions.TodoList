# Todo List API – Technical and Architectural Documentation

This document outlines the technical design, requirements, structure, and API contract for the Todo List API. The primary focus is correctness, maintainability, and clarity. No fluff or “cutting edge” marketing talk—just the facts.

---

## 1. Requirements

- **.NET**: ASP.NET Core 9 (Web API)
- **Architecture**: Clean Architecture (Onion), layered as Domain, Application, Infrastructure, Presentation
- **CQRS & Mediation**: MediatR for commands/queries, separate read/write flows
- **Database**: PostgreSQL (EF Core), multiple migrations, UTC timestamps, seeding (users, roles, todos)
- **Identity & Auth**: ASP.NET Core Identity, JWT (RSA256, key pair), refresh tokens (rotation & revocation, DB-persisted)
- **Validation**: FluentValidation pipeline for all request models
- **Error Handling**: Global middleware, RFC 7807 ProblemDetails everywhere, no exceptions for business logic flow
- **OpenAPI/Swagger**: Enabled, enums as strings, examples, JWT integration, grouped endpoints
- **HTTP Client Factory**: Used for external calls (e.g., http://ip-api.com/json/8.8.8.8)
- **Security**: CORS configured, rate limiting policies applied
- **Docker**: Dockerfile for API, optional docker-compose (API + Postgres)
- **API Design**: Versioned (`/api/v1`), pagination/filtering/sorting, batch endpoints, consistent base responses
- **Testing**: xUnit + FluentAssertions + Moq for unit, Testcontainers for integration (Postgres)
- **Other**: Multi-layer cache (in-memory + materialized views), Dapper + EF Core, API auditing/logging, correlation IDs, health checks, feature flags, localization, DB migrations automated on startup (except production), extensible validation, command validation pipeline, ProblemDetails extensions, consistent responses

---

## 2. Project Structure

```
├── src
│   ├── Core
│   │   ├── Solutions.TodoList.Application
│   │   └── Solutions.TodoList.Domain
│   ├── Infrastructure
│   │   ├── Read
│   │   │   ├── Solutions.TodoList.Cache
│   │   │   └── Solutions.TodoList.Projections
│   │   ├── Solutions.TodoList.Identity
│   │   └── Solutions.TodoList.Persistence
│   └── Presentation
│       └── Solutions.TodoList.WebApi
└── test
    ├── Solutions.TodoList.IntegrationTests
    └── Solutions.TodoList.UnitTests
```

- **Core**
  - `Application`: Use cases, MediatR handlers, CQRS logic, DTOs, interfaces, validation, pipeline behaviors.
  - `Domain`: Entities, value objects, enums, domain events.
- **Infrastructure**
  - `Persistence`: EF Core context, migrations, PostgreSQL, repositories, data seeding.
  - `Identity`: ASP.NET Core Identity, JWT, refresh tokens, RBAC policies.
  - `Read/Cache`: Dapper, multi-layer cache, projections, materialized views, cache strategies.
- **Presentation**
  - `WebApi`: Startup, DI, controllers, middleware, error handling, health checks, Swagger, versioning, CORS, rate limiting.
- **Tests**
  - `IntegrationTests`: Testcontainers, end-to-end.
  - `UnitTests`: xUnit, Moq, FluentAssertions.

---

## 3. Technologies Used

- **.NET 9**
- **ASP.NET Core Web API**
- **MediatR (CQRS, Domain Events)**
- **EF Core (Postgres) + Dapper (Read)**
- **ASP.NET Core Identity**
- **JWT (RSA256, key pair)**
- **FluentValidation**
- **Serilog (Structured Logging)**
- **Swagger/Swashbuckle**
- **Testcontainers (Integration Tests)**
- **xUnit, Moq, FluentAssertions**
- **HealthChecks**
- **FeatureManagement (Feature Flags)**
- **IStringLocalizer (Localization)**
- **Docker, docker-compose**

---

## 4. API Base Responses

All API responses follow HTTP standards. Errors are surfaced as RFC 7807 ProblemDetails.

### 4.1 Success (2xx)
```json
{
  "data": { ... }, // actual response DTO
  "meta": { /* pagination, correlationId, etc. */ }
}
```
- All list endpoints include `meta` for pagination.
- Single resource endpoints omit `meta` if not needed.

### 4.2 Errors

**Validation Error (400)**
```json
{
  "type": "https://httpstatuses.com/400",
  "title": "Validation Failed",
  "status": 400,
  "errors": {
    "field1": ["error1", "error2"]
  },
  "traceId": "..."
}
```

**Unauthorized (401)**
```json
{
  "type": "https://httpstatuses.com/401",
  "title": "Unauthorized",
  "status": 401,
  "traceId": "..."
}
```

**Forbidden (403)**
```json
{
  "type": "https://httpstatuses.com/403",
  "title": "Forbidden",
  "status": 403,
  "traceId": "..."
}
```

**Not Found (404)**
```json
{
  "type": "https://httpstatuses.com/404",
  "title": "Resource Not Found",
  "status": 404,
  "traceId": "..."
}
```

**Internal Server Error (500+)**
```json
{
  "type": "https://httpstatuses.com/500",
  "title": "Internal Server Error",
  "status": 500,
  "traceId": "..."
}
```

- All errors include `traceId` for correlation.
- Extensions: In some cases, error responses may include `code` and `links` for docs.

---

## 5. Endpoints

All endpoints are under `/api/v1/todos`. JWT Bearer required except login/register.

### 5.1 Todo Endpoints

#### Create Todo
- **POST** `/api/v1/todos`
- Request: `{ "title": "...", "description": "...", ... }`
- Response: `201 Created` with full Todo DTO.

#### Get Todo By Id
- **GET** `/api/v1/todos/{id}`
- Response: Todo DTO

#### Update Todo (Full Replace)
- **PUT** `/api/v1/todos/{id}`
- Request: Full DTO
- Response: Updated Todo DTO

#### Patch Todo (Toggle Done)
- **PATCH** `/api/v1/todos/{id}/done`
- Request: `{ "done": true }`
- Response: Updated Todo DTO

#### Delete Todo (Hard Delete)
- **DELETE** `/api/v1/todos/{id}`
- Response: `204 No Content`

#### List Todos (With Pagination/Sorting/Filtering)
- **GET** `/api/v1/todos?search=...&sort=createdAt_desc&skip=0&take=20`
- Response: `{ "data": [ ... ], "meta": { ... } }`

---

### 5.2 Batch Endpoints

#### Batch Create
- **POST** `/api/v1/todos/batch`
- Request: `[ { todoDTO1 }, { todoDTO2 }, ... ]`
- Response: Array of created Todos

#### Batch Update
- **PUT** `/api/v1/todos/batch`
- Request: `[ { id, fields... }, ... ]`
- Response: Array of updated Todos

#### Batch Mark Done
- **PATCH** `/api/v1/todos/batch/done`
- Request: `{ "ids": [ ... ], "done": true }`
- Response: Array of updated Todos

#### Batch Delete
- **DELETE** `/api/v1/todos/batch`
- Request: `{ "ids": [ ... ] }`
- Response: `{ "deleted": n }`

---

### 5.3 Auth Endpoints

#### Register
- **POST** `/api/v1/auth/register`
- Request: `{ "username": "...", "password": "...", ... }`
- Response: JWT tokens

#### Login
- **POST** `/api/v1/auth/login`
- Request: `{ "username": "...", "password": "..." }`
- Response: JWT + Refresh token

#### Refresh Token
- **POST** `/api/v1/auth/refresh`
- Request: `{ "refreshToken": "..." }`
- Response: new JWT + refresh token

#### JWKS (for JWT public keys)
- **GET** `/api/v1/auth/.well-known/jwks.json`

---

### 5.4 Misc Endpoints

- **GET** `/api/v1/health` — Health/liveness/readiness
- **GET** `/api/v1/ip/geo` — Calls http://ip-api.com/json/8.8.8.8 via HTTP Client Factory, returns geo info

---

## 6. Database Design

### Tables

- **Users**
  - Id (Guid, PK)
  - UserName
  - PasswordHash
  - Email
  - Role (enum: User/Admin)
  - CreatedAt/UpdatedAt (UTC)
  - ... (identity fields)

- **Roles**
  - Id (Guid, PK)
  - Name

- **Todos**
  - Id (Guid, PK)
  - Title
  - Description
  - Done (bool)
  - CreatedAt/UpdatedAt/CompletedAt (UTC)
  - UserId (FK)
  - ... (other fields as needed)

- **RefreshTokens**
  - Id (Guid, PK)
  - UserId (FK)
  - Token (string)
  - ExpiresAt (UTC)
  - Revoked (bool)
  - CreatedAt (UTC)

- **MaterializedViews_Todos**
  - (Read-optimized denormalized structure for batch/list endpoints)

- **AuditLogs**
  - Id (Guid, PK)
  - UserId
  - Action (string)
  - ResourceId (Guid)
  - Timestamp (UTC)
  - CorrelationId

---

## 7. Caching

- **Hot Cache**: In-memory (using .NET 9 caching abstractions), per-node, for most-read todos
- **Cold Cache**: Materialized views in Postgres, updated asynchronously
- **Cache Invalidation**: On any write (create/update/delete), emit domain event → Outbox pattern → background worker updates cache/materialized view
- **Multi-level fallback**: Try in-memory, then materialized view, then DB

---

## 8. Security

- **JWT (RSA256)**: Private key for signing, public key for validation/JWKS. Key rotation is supported.
- **Refresh Token**: Rotated and stored in DB, revoked on use/expiry.
- **Rate Limiting**: Per-user/IP, uses .NET rate limiting middleware.
- **CORS**: Only allowed origins/methods/headers accepted.
- **Role-based Policies**: Only admin can delete, all others can CRUD their own.

---

## 9. Logging & Auditing

- **Serilog** for structured logs.
- **Correlation ID**: propagated via headers/logs/ProblemDetails.
- **Audit Log**: All writes tracked by user, action, resource, and correlation.

---

## 10. Health & Readiness

- `/health` endpoint checks DB, cache, and readiness.
- Liveness/readiness probes for Docker/Kubernetes.

---

## 11. Feature Flags

- Use .NET FeatureManagement for toggling features at runtime.

---

## 12. Validation

- **FluentValidation** for all requests, with pipeline behaviors for MediatR.
- **No exceptions** for validation/business logic—use result types and ProblemDetails.

---

## 13. Localization

- API error responses/localized strings via `IStringLocalizer`.

---

## 14. Automated Migrations

- DB migrations run on startup except in production.

---

## 15. Testing

- **Unit tests**: xUnit/Moq/FluentAssertions.
- **Integration tests**: Testcontainers (Postgres).
- **Coverage reports** for unit tests.

---

## 16. CQRS Command Validation Pipeline

- MediatR pipeline behaviors for validation, logging, performance, and correlation.

---

## 17. Response Format Examples

**Paged List**
```json
{
  "data": [ { ... }, ... ],
  "meta": {
    "page": 1,
    "pageSize": 20,
    "totalCount": 135,
    "traceId": "..."
  }
}
```

**Single Resource**
```json
{
  "data": { ... },
  "meta": {
    "traceId": "..."
  }
}
```

**Error**
```json
{
  "type": "...",
  "title": "...",
  "status": ...,
  "errors": { ... },
  "traceId": "..."
}
```

---

## 18. Swagger/OpenAPI

- Enums as strings, example responses for all endpoints, JWT authorize button, grouped endpoints (CRUD, batch, auth, misc), request/response examples, versioned documentation.

---

## 19. Batch Operations

- All batch endpoints accept arrays or id lists, return array of results or summary.
- E.g., batch create, batch update, batch mark-done, batch delete.

---