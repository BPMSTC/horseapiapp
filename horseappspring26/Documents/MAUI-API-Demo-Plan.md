# MAUI API Demo Implementation Plan

This document outlines exactly what students will build in the MAUI app to consume the Horse API with authentication and full CRUD support on Android emulator.

## Demo target
- Platform: Android emulator first (Windows machine can still be used for development workflow).
- API usage: GET, POST, PUT, DELETE on horses.
- Auth: login flow required; delete uses admin role.
- MVVM pattern: `CommunityToolkit.Mvvm`.

## Build checklist
1. Project setup
   - Add MVVM toolkit package.
   - Add folders: `Models`, `Services`, `ViewModels`, `Views`, `Configuration`.
   - Register dependencies in `MauiProgram.cs`.

2. API configuration
   - Add a single source of truth for base URL.
   - Use emulator-compatible host (`https://10.0.2.2:<api-port>`).

3. Authentication flow
   - Create login model + service call to Identity login endpoint.
   - Store token for authenticated requests.
   - Ensure auth header is sent for `POST`, `PUT`, and `DELETE`.

4. Horse service layer
   - Implement calls for:
     - `GET /api/horse`
     - `GET /api/horse/{id}`
     - `POST /api/horse`
     - `PUT /api/horse/{id}`
     - `DELETE /api/horse/{id}`
   - Map API DTOs to app models.

5. ViewModels
   - `LoginViewModel`
   - `HorseListViewModel`
   - `HorseDetailViewModel`
   - `HorseEditViewModel`
   - Use commands for load, save, update, delete, and refresh.

6. Views
   - Login page.
   - Horse list page.
   - Horse detail page.
   - Add/edit page.
   - Add basic search/filter on list page.

7. Error handling and logging
   - Handle 400/401/403/404 and network exceptions.
   - Show user-friendly error messages.
   - Log failures and key API operations for debugging/teaching.

8. End-to-end validation
   - Login with seeded admin account for delete demonstration.
   - Verify create/update/delete outcomes against API data.
   - Confirm behavior on invalid input and failed auth.

## Demo path (live class flow)
1. Start API and confirm reachable URL.
2. Launch MAUI emulator app.
3. Log in.
4. Load horse list.
5. Open detail.
6. Create horse.
7. Edit horse.
8. Delete horse (admin role).
9. Trigger and explain one error scenario.
