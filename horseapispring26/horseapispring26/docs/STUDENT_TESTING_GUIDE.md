# Student Testing Guide

This guide explains how to test the API in JSON mode, SQL no-auth mode, and SQL auth mode.

---

## Prerequisites

- SQL Server is running (for SQL tests).
- App settings are set for the mode you want to test.
- API starts successfully.

---

## A) Quick Mode Matrix

- **JSON mode**
  - `DataSources:JsonFile:Enabled = true`
  - `DataSources:SqlServer:Enabled = false`
- **SQL no-auth mode**
  - `DataSources:JsonFile:Enabled = false`
  - `DataSources:SqlServer:Enabled = true`
  - `Auth:Enabled = false`
- **SQL auth mode**
  - `DataSources:JsonFile:Enabled = false`
  - `DataSources:SqlServer:Enabled = true`
  - `Auth:Enabled = true`

---

## B) SQL Migration Commands

Run from solution root when needed.

### No-auth SQL database

```powershell
dotnet ef database update --context HorseDbContext --project horseapispring26/horseapispring26.csproj --startup-project horseapispring26/horseapispring26.csproj
```

### Auth SQL database

```powershell
dotnet ef database update --context AuthHorseDbContext --project horseapispring26/horseapispring26.csproj --startup-project horseapispring26/horseapispring26.csproj
```

---

## C) Core API Tests (all modes)

Use Postman/HTTP client.

1. **Get all horses**
   - `GET /api/horse`
   - Expect `200 OK`.

2. **Get horse by id**
   - `GET /api/horse/{id}`
   - Expect `200 OK` for existing id, `404` for missing.

3. **Create horse**
   - `POST /api/horse` with valid JSON body
   - Expect success (`201` or configured success code).

4. **Update horse**
   - `PUT /api/horse/{id}`
   - Expect success for existing record.

5. **Delete horse (soft delete)**
   - `DELETE /api/horse/{id}`
   - Verify horse no longer appears in normal GET results.

---

## D) SQL Startup Import Test

When in SQL mode:

- Set `DataSources:ImportFromJsonOnStart = true`
- Ensure `Horses` table is empty
- Start API

Expected:

- Horses from `data/horses.json` are imported once.
- Subsequent starts skip import if data exists.

---

## E) Auth Mode Tests

Only for `Auth:Enabled = true`.

1. Register user:
   - `POST /register`
2. Login:
   - `POST /login`
   - Capture access token.
3. Call protected endpoint:
   - Add `Authorization: Bearer <token>`
4. Role checks:
   - Test admin-only endpoint (e.g., delete if role-protected).

In development, admin seed user/role is created by initializer.

---

## F) Common Failure Cases

1. **PendingModelChangesWarning**
   - Wrong context migration set or missing migration update.

2. **SQL mode enabled but wrong connection string**
   - Verify `HorseNoAuthConnection` vs `HorseAuthConnection` based on `Auth:Enabled`.

3. **Auth enabled with SQL disabled**
   - Not supported; auth mode requires SQL.

4. **No data imported**
   - Check JSON path and `ImportFromJsonOnStart`.

---

## G) Suggested Student Demo Sequence

1. Start in JSON mode, test CRUD basics.
2. Switch to SQL no-auth mode, apply no-auth migration, test CRUD + JSON import-on-start.
3. Switch to SQL auth mode, apply auth migration, test register/login and role-based behavior.

This sequence highlights why the project uses two contexts and two SQL databases.
