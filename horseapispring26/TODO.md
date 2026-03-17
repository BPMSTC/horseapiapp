# Horse API SQL Repository TODO

Created: 2025-09-29

Legend: [ ] = pending, [x] = done

## 0) Up-front decisions (confirm before implementation)
- [ ] Data access: Use EF Core for initial implementation (option to add Dapper later for hot paths)
- [ ] Concurrency: Use optimistic concurrency with `rowversion` column (translate conflicts to HTTP 409)
- [ ] Delete behavior: Hard delete (match current JSON repo). Consider soft delete later (`IsDeleted`)
- [ ] Keys & uniqueness: `Id` as INT primary key, `RegistrationNumber` unique index (enforce at DB)
- [ ] Collation & case sensitivity: Case-insensitive comparisons for search; rely on database default CI collation
- [ ] Paging: Introduce paging parameters before large datasets (API + repo signature changes)
- [ ] Migration of existing JSON data: One-time import preserving `Id` and `RegistrationNumber`
- [ ] Security: Use non-SA SQL login for app; keep secrets out of source control

## 1) Schema design (EF Core model)
- [ ] Table `Horses` with columns mapping to `horseapispring26.Models.Horse`
  - [ ] `Id` INT PK (identity or preserved from import)
  - [ ] `Name` NVARCHAR(200) NOT NULL
  - [ ] `RegistrationNumber` NVARCHAR(50) NOT NULL UNIQUE
  - [ ] `DateOfBirth` DATE NOT NULL
  - [ ] `Gender` SMALLINT (enum) NOT NULL
  - [ ] `Color` NVARCHAR(50) NOT NULL
  - [ ] `Sire` NVARCHAR(200) NULL
  - [ ] `Dam` NVARCHAR(200) NULL
  - [ ] `BreederName` NVARCHAR(200) NULL
  - [ ] `PictureUrl` NVARCHAR(512) NULL
  - [ ] `TotalRacesRun`, `Wins`, `Places`, `Shows` INT NOT NULL DEFAULT 0
  - [ ] `CareerEarnings` DECIMAL(18,2) NOT NULL DEFAULT 0
  - [ ] `CurrentOwner` NVARCHAR(200) NULL
  - [ ] `Trainer` NVARCHAR(200) NULL
  - [ ] `RowVersion` ROWVERSION NOT NULL (for optimistic concurrency)
- [ ] Indexes: UNIQUE on `RegistrationNumber`; nonclustered on `Gender`, `CareerEarnings` (top earners)

## 2) Packages
- [ ] Add packages to `horseapispring26` project:
  - [ ] `Microsoft.EntityFrameworkCore`
  - [ ] `Microsoft.EntityFrameworkCore.SqlServer`
  - [ ] `Microsoft.EntityFrameworkCore.Tools`
  - [ ] `Microsoft.Extensions.Diagnostics.HealthChecks`
  - [ ] `AspNetCore.HealthChecks.SqlServer` (optional) or use EF-based health check
  - [ ] `Testcontainers.MsSql` (for integration tests)

## 3) EF Core setup
- [ ] Create `Data/HorseDbContext.cs` with `DbSet<Horse>` and fluent config for lengths, required fields, unique index, enum mapping, rowversion
- [ ] Optionally add `Data/Configurations/HorseConfiguration.cs` for entity type configuration
- [ ] Update `Program.cs` to register DbContext when `DataSources:SqlServer:Enabled` is true
  - [ ] Connection string from `ConnectionStrings:DefaultConnection`
  - [ ] Enable sensitive data logging only in Development

## 4) SQL repository
- [ ] Implement `Repositories/SqlHorseRepository.cs : IHorseRepository`
  - [ ] Ensure behavior parity with JSON repo:
    - [ ] Duplicate `RegistrationNumber` → throw `InvalidOperationException`
    - [ ] `Get*` return `null` when not found
    - [ ] `GetTopEarnersAsync(count)` → ordered by `CareerEarnings` desc, `Take(count)`
    - [ ] `Search` performs case-insensitive contains across the same fields
  - [ ] Add cancellation tokens internally (optional for now)
  - [ ] Translate `DbUpdateConcurrencyException` to 409-friendly domain result if controller later adopts it

## 5) Migrations & database
- [ ] Add initial migration `AddHorseSchema`
- [ ] Apply migration locally
- [ ] Seed dev data from json for smoke tests

## 6) Docker & compose
- [ ] Add `docker-compose.yml` with services:
  - [ ] `database`: `mcr.microsoft.com/mssql/server:2022-latest`
    - [ ] env: `ACCEPT_EULA=Y`, `SA_PASSWORD` (from `.env`/secret), `MSSQL_PID=Express` (or Developer)
    - [ ] volume: `sqlserver-data:/var/opt/mssql`
    - [ ] healthcheck: `SELECT 1`
  - [ ] `api`: build from `horseapispring26/Dockerfile`
    - [ ] depends_on: database (with condition: service_healthy)
    - [ ] env: `ConnectionStrings__DefaultConnection=Server=database;Database=HorseDB;User Id=app_user;Password=...;TrustServerCertificate=true;`
- [ ] Add `.env` to hold secrets locally (not committed) or Docker secrets
- [ ] Ensure API uses SQL path by setting `DataSources:SqlServer:Enabled=true` in container env

## 7) Startup behavior
- [ ] On app start (in Development only), ensure DB exists and apply migrations automatically
- [ ] Optional: one-time JSON import behind flag `DataSources:ImportFromJsonOnStart=true`

## 8) Configuration & security
- [ ] Replace SA usage with app-specific SQL login/user
- [ ] Store secrets outside source control (User Secrets for dev; env vars/secrets in containers)
- [ ] Review TLS/TrustServerCertificate usage; restrict public SQL port exposure

## 9) Health checks & observability
- [ ] Add ASP.NET Core health checks and map `/health` endpoint
- [ ] Add SQL health check (connection/open)
- [ ] Structured logging for DB operations (minimal noise in production)

## 10) Tests
- [ ] Unit tests for `SqlHorseRepository` using in-memory or SQLite? Prefer real SQL via Testcontainers
- [ ] Integration tests: spin up SQL Server container and run repo tests (CRUD, search, unique constraint, top earners)

## 11) Optional API enhancements (post-SQL)
- [ ] Introduce paging to `GetAllHorsesAsync` and `SearchHorsesAsync`
- [ ] Add filter/sort parameters (owner, trainer, gender, min/max DOB)
- [ ] ETags or If-Match header support using rowversion

## 12) Data migration tool (if chosen)
- [ ] Implement a one-time import job `Tools/JsonToSqlImporter` (console) that:
  - [ ] Reads `data/horses.json`
  - [ ] Upserts into SQL preserving `Id` and `RegistrationNumber`
  - [ ] Validates uniqueness and reports conflicts

## Acceptance criteria
- [ ] With `DataSources:SqlServer:Enabled=true` and compose up, API uses SQL repository and passes smoke tests
- [ ] Duplicate registration numbers rejected at DB and surfaced as `InvalidOperationException`
- [ ] Health endpoint reports healthy when DB is reachable
- [ ] Integration tests green on CI (using Testcontainers)
