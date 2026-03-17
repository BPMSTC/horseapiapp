# Horse API Architecture

## Overview

This is an ASP.NET Core Web API (targeting .NET 10) that supports two data-access modes:

1. **JSON mode** (file-backed repository) for simple/demo operation.
2. **SQL mode** (EF Core + SQL Server) for relational persistence.

The API also supports two authentication modes:

1. **Auth OFF**: demo authentication handler.
2. **Auth ON**: ASP.NET Core Identity endpoints + role-based authorization.

Because SQL + auth-on and SQL + auth-off have different schema requirements, the solution uses **two DbContexts and two databases**.

---

## Project Structure

- `Controllers/`
  - API endpoints (`HorseController`, etc.)
- `Models/`
  - Domain model (`Horse`) and DTOs
- `Repositories/`
  - `IHorseRepository` abstraction
  - `JsonHorseRepository` (JSON file)
  - `SqlHorseRepository` (EF Core)
- `Services/`
  - Mapping/services used by controllers
- `Data/`
  - EF contexts, migrations, startup initialization/seed logic
- `Infrastructure/`
  - Global exception handling, demo auth handler

---

## Runtime Mode Selection

### Data source selection

Controlled by config (`appsettings*.json`):

- `DataSources:JsonFile:Enabled`
- `DataSources:SqlServer:Enabled`

When JSON is enabled, `JsonHorseRepository` is used.
When JSON is disabled and SQL is enabled, `SqlHorseRepository` is used.

### Authentication selection

Controlled by:

- `Auth:Enabled`

- `false` -> demo auth handler
- `true` -> ASP.NET Core Identity APIs + EF Identity store

---

## Why Two DbContexts and Two Databases?

## Problem being solved

Identity adds many tables/entities (`AspNetUsers`, `AspNetRoles`, etc.).
A single context that sometimes includes Identity and sometimes does not causes EF model/migration drift.

## Implemented solution

- **No-auth context**: `HorseDbContext : DbContext`
- **Auth context**: `AuthHorseDbContext : IdentityDbContext<IdentityUser>`

Both contexts share horse entity mapping through:

- `HorseModelConfiguration.ConfigureHorseModel(...)`

This keeps horse schema consistent while allowing Identity schema only in auth mode.

## Database separation

Connection strings:

- `ConnectionStrings:HorseNoAuthConnection`
- `ConnectionStrings:HorseAuthConnection`

Using separate databases avoids migration-history conflicts and mode-switch instability.

---

## Migrations Strategy

Separate migration tracks are maintained:

- `data/Migrations/HorseOnly` for `HorseDbContext`
- `data/Migrations/Auth` for `AuthHorseDbContext`

Always create/apply migrations against the context for the mode being changed.

---

## Startup Initialization

`DbInitializer` handles:

1. Applying migrations for the active SQL context.
2. Optional Identity seed (auth mode).
3. Optional import from `data/horses.json` if SQL table is empty.

This preserves first-run convenience while keeping schema correctness tied to migrations.

---

## Soft Delete Model

`Horse.IsDeleted` is used for soft delete.
A global query filter excludes deleted records by default.

---

## Request Flow (high level)

1. Request reaches controller.
2. Controller uses `IHorseRepository`.
3. Repository implementation depends on config (JSON or SQL).
4. For SQL mode, EF context depends on auth mode (no-auth context or auth context).

This layered design keeps controllers stable while infrastructure varies by configuration.
