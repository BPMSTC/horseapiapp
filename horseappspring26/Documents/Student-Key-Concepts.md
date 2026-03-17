# Student Key Concepts: Theory + Where It Appears

This guide explains the core concepts students should learn in the MAUI/API demo and points to where each concept is implemented in the solution.

## 1) MVVM separation of concerns
- Theory:
  - View = UI markup and bindings.
  - ViewModel = state + commands.
  - Service = API communication logic.
- Where in solution:
  - `horseappspring26/horseappspring26/Views/*`
  - `horseappspring26/horseappspring26/ViewModels/*`
  - `horseappspring26/horseappspring26/Services/*`

## 2) Dependency Injection in MAUI
- Theory:
  - DI decouples object creation from usage and makes testing easier.
- Where in solution:
  - `horseappspring26/horseappspring26/MauiProgram.cs` (service and viewmodel registration)

## 3) API contracts (DTO thinking)
- Theory:
  - Client should shape requests/responses around API contracts, not UI controls.
- Where in solution:
  - MAUI app models/DTOs under `horseappspring26/horseappspring26/Models/*`
  - API contract reference:
    - `horseapispring26/horseapispring26/Models/DTOs/CreateHorseRequestDto.cs`
    - `horseapispring26/horseapispring26/Models/DTOs/UpdateHorseRequestDto.cs`
    - `horseapispring26/horseapispring26/Models/DTOs/HorseResponseDto.cs`

## 4) Authentication and authorization
- Theory:
  - Authentication verifies identity (login/token).
  - Authorization enforces access rules (roles/claims).
- Where in solution:
  - MAUI token handling in `horseappspring26/horseappspring26/Services/*`
  - API auth setup in `horseapispring26/horseapispring26/Program.cs`
  - Endpoint access rules in `horseapispring26/horseapispring26/Controllers/HorseController.cs`

## 5) Role-based access (Admin delete)
- Theory:
  - Some actions are allowed only for specific roles.
- Where in solution:
  - API delete endpoint role requirement:
    - `horseapispring26/horseapispring26/Controllers/HorseController.cs` (`[Authorize(Roles = "Admin")]`)
  - Seeded admin user setup:
    - `horseapispring26/horseapispring26/Data/DbInitializer.cs`

## 6) Android emulator networking
- Theory:
  - `localhost` in emulator points to emulator itself, not the developer machine.
  - `10.0.2.2` maps to host machine loopback for Android emulator.
- Where in solution:
  - MAUI API base URL configuration:
    - `horseappspring26/horseappspring26/Configuration/*` or equivalent settings class

## 7) Error handling strategy
- Theory:
  - Anticipate and classify errors: validation, auth, not found, server, network.
  - Present clear user messages and keep technical detail in logs.
- Where in solution:
  - MAUI: request/command try-catch and message mapping in `Services/*` and `ViewModels/*`
  - API global exception handling:
    - `horseapispring26/horseapispring26/Infrastructure/GlobalExceptionHandler.cs`

## 8) Logging for troubleshooting
- Theory:
  - Logs capture technical context needed to diagnose failures and teach runtime behavior.
- Where in solution:
  - MAUI logging configured in `horseappspring26/horseappspring26/MauiProgram.cs`
  - Service/viewmodel log statements in MAUI app layers
  - API logs in middleware and repositories

## 9) End-to-end request lifecycle
- Theory:
  - UI action -> command -> service call -> HTTP request -> API controller -> repository/db -> response -> UI update.
- Where in solution:
  - MAUI views/viewmodels/services
  - API controller and repository layers (`HorseController`, `IHorseRepository` implementations)

## 10) Practical API design notes students should observe
- Theory:
  - Correct verb usage (`GET/POST/PUT/DELETE`), status codes, and pagination are core interoperability skills.
- Where in solution:
  - API endpoint definitions in `horseapispring26/horseapispring26/Controllers/HorseController.cs`
  - Broader reference in `horseapispring26/API_DOCUMENTATION.md`
