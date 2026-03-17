# Authentication and Authorization Guide

## Summary

This API supports two auth modes configured via `Auth:Enabled`.

- `Auth:Enabled = false` -> **Demo authentication mode**
- `Auth:Enabled = true` -> **ASP.NET Core Identity mode**

Authorization attributes on endpoints stay the same; authentication source changes by mode.

---

## Mode 1: Auth Disabled (Demo Mode)

When auth is off, the app registers a custom demo authentication handler:

- `DemoAuthenticationHandler`

Purpose:

- Keep `[Authorize]` endpoints testable during no-auth labs.
- Avoid Identity complexity when students focus on repository/API fundamentals.

In this mode, Identity endpoints (`/register`, `/login`, `/refresh`) are **not** mapped.

---

## Mode 2: Auth Enabled (Identity Mode)

When auth is on:

- `AddIdentityApiEndpoints<IdentityUser>()`
- `AddRoles<IdentityRole>()`
- `AddEntityFrameworkStores<AuthHorseDbContext>()`
- `app.MapIdentityApi<IdentityUser>()`

This enables built-in Identity endpoints and role-aware authorization.

Common endpoints:

- `POST /register`
- `POST /login`
- `POST /refresh`

The app seeds admin role/user in development through `DbInitializer`.

---

## Authorization in Controllers

Controllers should use attributes such as:

- `[AllowAnonymous]` for public endpoints
- `[Authorize]` for authenticated access
- `[Authorize(Roles = "Admin")]` for role-restricted operations

These rules apply in both modes; only the identity source differs.

---

## Why Auth Mode Requires Separate Context/Database

Auth mode includes Identity schema.
No-auth mode excludes Identity schema.

To avoid EF migration/model mismatch, the app uses:

- `HorseDbContext` + `HorseNoAuthConnection`
- `AuthHorseDbContext` + `HorseAuthConnection`

This is intentionally more verbose (“bloated”) to keep mode switching reliable and teachable.

---

## Configuration Checklist

## No-auth mode

- `Auth:Enabled = false`
- `DataSources:SqlServer:Enabled = true` (if using SQL)
- `ConnectionStrings:HorseNoAuthConnection` valid

## Auth mode

- `Auth:Enabled = true`
- `DataSources:SqlServer:Enabled = true`
- `ConnectionStrings:HorseAuthConnection` valid
- Auth migrations applied for `AuthHorseDbContext`

---

## Security Notes for Students

- Use HTTPS in real deployments.
- Do not keep dev seed passwords in production.
- Demo mode is for classroom/dev convenience only.
- Role assignments determine access for restricted endpoints.
