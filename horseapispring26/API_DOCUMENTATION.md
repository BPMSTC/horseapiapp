# Horse API - Complete API Documentation

**Purpose**: Complete API reference, endpoints, examples, and testing guide for the Horse API.

## 📁 Documentation Structure
- **PROJECT_SETUP.md** - Setup, installation, database configuration, and basic usage
- **API_DOCUMENTATION.md** (this file) - Complete API reference, endpoints, examples, and testing guide

---

This guide provides comprehensive information about REST API development using .NET Web API, specifically designed for students learning to build APIs that will be consumed by .NET MAUI applications.

## What is a REST API?

REST (Representational State Transfer) is an architectural style for designing networked applications. A REST API is a web service that follows REST principles and uses HTTP methods to perform operations on resources.

### Key REST Principles

1. **Stateless** - Each request contains all necessary information
2. **Client-Server** - Clear separation between client and server
3. **Cacheable** - Responses can be cached for better performance
4. **Uniform Interface** - Consistent way to interact with resources
5. **Layered System** - Architecture can be composed of hierarchical layers

## HTTP Methods and Their Purposes

### GET
- **Purpose**: Retrieve data
- **Idempotent**: Yes (safe to repeat)
- **Example**: `GET /api/weatherforecast` - Get all weather forecasts

### POST
- **Purpose**: Create new resources
- **Idempotent**: No
- **Example**: `POST /api/weatherforecast` - Create a new weather forecast

### PUT
- **Purpose**: Update entire resource
- **Idempotent**: Yes
- **Example**: `PUT /api/weatherforecast/1` - Update weather forecast with ID 1

### PATCH
- **Purpose**: Partial update of resource
- **Idempotent**: No
- **Example**: `PATCH /api/weatherforecast/1` - Update specific fields of weather forecast

### DELETE
- **Purpose**: Remove resource
- **Idempotent**: Yes
- **Example**: `DELETE /api/weatherforecast/1` - Delete weather forecast with ID 1

## HTTP Status Codes

### Success Codes (2xx)
- **200 OK** - Request successful
- **201 Created** - Resource created successfully
- **204 No Content** - Request successful, no content returned

### Client Error Codes (4xx)
- **400 Bad Request** - Invalid request data
- **401 Unauthorized** - Authentication required
- **403 Forbidden** - Access denied
- **404 Not Found** - Resource not found
- **422 Unprocessable Entity** - Valid request but semantic errors

### Server Error Codes (5xx)
- **500 Internal Server Error** - Unexpected server error
- **502 Bad Gateway** - Invalid response from upstream server
- **503 Service Unavailable** - Server temporarily unavailable

## API Design Best Practices

### URL Structure
```
https://api.example.com/v1/resource/{id}/sub-resource
```

- Use nouns, not verbs
- Use plural forms for collections
- Use hierarchical structure
- Include version in URL

### Request/Response Format
- Use JSON for data exchange
- Include proper Content-Type headers
- Use consistent naming conventions (camelCase recommended)

### Error Handling
```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Invalid input data",
    "details": [
      {
        "field": "email",
        "message": "Email format is invalid"
      }
    ]
  }
}
```

## .NET Web API Specifics

### Controller Structure
```csharp
[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<WeatherForecast>> Get()
    {
        // Implementation
    }
}
```

### Attribute Routing
- `[HttpGet]` - Maps to GET requests
- `[HttpPost]` - Maps to POST requests
- `[HttpPut]` - Maps to PUT requests
- `[HttpDelete]` - Maps to DELETE requests
- `[Route("custom-path")]` - Custom route mapping

### Model Binding
- Automatic binding of request data to C# objects
- Support for query parameters, route parameters, and request body
- Validation using Data Annotations

### Dependency Injection
- Built-in DI container
- Register services in Program.cs
- Inject dependencies into controllers

## Testing APIs

### Tools for Testing
1. **Postman** - GUI tool for API testing
2. **curl** - Command-line tool
3. **Swagger/OpenAPI** - Interactive API documentation
4. **Unit Tests** - Automated testing

### Example curl Commands
```bash
# GET all horses
curl -X GET "https://localhost:7240/api/horses"

# GET horse by ID
curl -X GET "https://localhost:7240/api/horses/1"

# GET horse by registration number
curl -X GET "https://localhost:7240/api/horses/registration/TB-001-2020"

# POST new horse
curl -X POST "https://localhost:7240/api/horses" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Thunder Strike",
    "registrationNumber": "TB-001-2020",
    "dateOfBirth": "2020-03-15T00:00:00Z",
    "gender": 0,
    "color": "Bay",
    "sire": "Lightning Bolt",
    "dam": "Storm Queen",
    "breederName": "Royal Stables",
    "pictureUrl": "https://example.com/horse-image.jpg",
    "totalRacesRun": 12,
    "wins": 4,
    "places": 3,
    "shows": 2,
    "careerEarnings": 125000.00,
    "currentOwner": "John Smith Racing",
    "trainer": "Mike Johnson"
  }'

# PUT update horse
curl -X PUT "https://localhost:7240/api/horses/1" \
  -H "Content-Type: application/json" \
  -d '{
    "id": 1,
    "name": "Thunder Strike Updated",
    "registrationNumber": "TB-001-2020",
    "dateOfBirth": "2020-03-15T00:00:00Z",
    "gender": 0,
    "color": "Bay",
    "sire": "Lightning Bolt",
    "dam": "Storm Queen",
    "breederName": "Royal Stables",
    "pictureUrl": "https://example.com/horse-image.jpg",
    "totalRacesRun": 15,
    "wins": 6,
    "places": 4,
    "shows": 2,
    "careerEarnings": 150000.00,
    "currentOwner": "Updated Owner Racing",
    "trainer": "Updated Trainer"
  }'

# DELETE horse
curl -X DELETE "https://localhost:7240/api/horses/1"

# Search horses
curl -X GET "https://localhost:7240/api/horses/search?searchTerm=thunder"

# Get horses by owner
curl -X GET "https://localhost:7240/api/horses/owner/John%20Smith%20Racing"

# Get horses by trainer
curl -X GET "https://localhost:7240/api/horses/trainer/Mike%20Johnson"

# Get top earners
curl -X GET "https://localhost:7240/api/horses/top-earners?count=5"

# Get horses by gender (0=Stallion, 1=Mare, 2=Gelding)
curl -X GET "https://localhost:7240/api/horses/gender/0"
```

## Security Considerations

### Authentication
- JWT (JSON Web Tokens)
- API Keys
- OAuth 2.0

### Authorization
- Role-based access control
- Policy-based authorization
- Resource-level permissions

### Data Protection
- HTTPS for all communications
- Input validation and sanitization
- SQL injection prevention
- CORS configuration

## CORS (Cross-Origin Resource Sharing)

Essential for MAUI applications consuming web APIs:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMAUI",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
```

## Performance Optimization

### Caching
- Response caching
- Memory caching
- Distributed caching

### Pagination
```json
{
  "data": [...],
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalCount": 100,
    "totalPages": 10
  }
}
```

### Compression
- Enable response compression
- Use appropriate content encoding

## Documentation

### Swagger/OpenAPI
- Automatic API documentation generation
- Interactive testing interface
- Schema definitions

### XML Documentation
```csharp
/// <summary>
/// Gets weather forecast data
/// </summary>
/// <returns>List of weather forecasts</returns>
[HttpGet]
public ActionResult<IEnumerable<WeatherForecast>> Get()
```

## Database Architecture

### SQL Server Integration
This API uses **SQL Server** as the primary data store, which may be new to students:

- **Containerized Database**: SQL Server runs in a Docker container
- **Entity Framework Core**: Object-relational mapping (ORM) for database operations
- **Automatic Schema Management**: Database tables created automatically on first run
- **JSON Import Process**: Sample data imported from `data/horses.json` on startup

### Database Setup Process
1. **Docker Container**: SQL Server runs in `horseapi-database-dev` container
2. **Connection String**: `Server=localhost,1433;Database=HorseDB;User Id=sa;Password=YourPassword123!`
3. **Schema Creation**: Entity Framework creates the `Horses` table automatically
4. **Data Import**: JSON file data imported on first startup
5. **Repository Pattern**: `SqlHorseRepository` handles all database operations

### Understanding the Data Flow
```
JSON File (data/horses.json) 
    ↓ (automatic import on startup)
SQL Server Database (HorseDB.dbo.Horses)
    ↓ (via Entity Framework)
Repository Layer (SqlHorseRepository)
    ↓ (dependency injection)
Controller Layer (HorseController)
    ↓ (HTTP requests/responses)
MAUI Application
```

## Common Patterns

### Repository Pattern
- **Interface-based design** (`IHorseRepository`) for easy data source switching
- **SQL Server implementation** (`SqlHorseRepository`) for production
- **JSON fallback** (`JsonHorseRepository`) for development
- **Dependency injection** for clean architecture
- **Abstract data access** - easier testing and maintenance
- **Clean separation of concerns** between data and business logic

### DTOs (Data Transfer Objects)
- Separate API models from domain models
- Control data exposure
- Version API contracts

### Media Handling
- **Picture URLs** - Store references to external images
- **File uploads** - Handle image uploads for horse pictures
- **CDN integration** - Use content delivery networks for image serving
- **Image optimization** - Resize and compress images for different use cases

### Middleware
- Request/response processing pipeline
- Logging, authentication, error handling
- Custom middleware for specific needs

## Quick Start
For setup and installation instructions, see **PROJECT_SETUP.md**.

This section focuses on API usage and testing.

## 🚀 Quick API Reference

### Base URL
- **Development**: `https://localhost:7240`
- **HTTP Alternative**: `http://localhost:5000`

### Main Endpoints
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/horses` | Get all horses |
| GET | `/api/horses/{id}` | Get horse by ID |
| GET | `/api/horses/registration/{regNumber}` | Get horse by registration number |
| POST | `/api/horses` | Create new horse |
| PUT | `/api/horses/{id}` | Update existing horse |
| DELETE | `/api/horses/{id}` | Delete horse (soft delete) |
| GET | `/api/horses/search?searchTerm={term}` | Search horses |
| GET | `/api/horses/owner/{owner}` | Get horses by owner |
| GET | `/api/horses/trainer/{trainer}` | Get horses by trainer |
| GET | `/api/horses/top-earners?count={n}` | Get top earning horses |
| GET | `/api/horses/gender/{gender}` | Get horses by gender |

### Gender Values
- `0` = Stallion
- `1` = Mare  
- `2` = Gelding

## Next Steps for Students

1. **Start Simple** - Begin with basic CRUD operations
2. **Understand Database Setup** - Learn the manual process behind the scripts
3. **Add Validation** - Implement proper input validation
4. **Handle Errors** - Create comprehensive error handling
5. **Add Security** - Implement authentication and authorization
6. **Optimize** - Add caching and performance improvements
7. **Test Thoroughly** - Write unit and integration tests
8. **Document** - Maintain clear API documentation

## Resources for Further Learning

- [Microsoft REST API Guidelines](https://github.com/Microsoft/api-guidelines)
- [ASP.NET Core Web API Documentation](https://docs.microsoft.com/en-us/aspnet/core/web-api/)
- [HTTP Status Codes Reference](https://httpstatuses.com/)
- [JSON API Specification](https://jsonapi.org/)

Remember: Good API design is about making it easy for clients (like your MAUI app) to consume your services effectively and efficiently!

---

## .NET 8 / C# 12 Architecture Updates (Implemented)

This project now standardizes key modern patterns for maintainability, testability, and consistency.

### 1) Primary Constructors for Dependency Injection

Controllers and repositories now use **C# 12 primary constructors**. This reduces constructor boilerplate and keeps dependencies explicit at the type declaration.

#### Applied to controllers
- `HorseController(IHorseRepository horseRepository, IHorseMapper horseMapper)`
- `AuthController(IUserRepository userRepository, ITokenService tokenService, ILogger<AuthController> logger)`

#### Applied to repositories
- `JsonHorseRepository(IConfiguration configuration, ILogger<JsonHorseRepository> logger)`
- `JsonUserRepository(IConfiguration configuration, ILogger<JsonUserRepository> logger)`
- `SqlHorseRepository(HorseDbContext db, ILogger<SqlHorseRepository> logger)`

**Why this helps**
- Cleaner class declarations with less ceremony.
- Explicit dependencies at the top of each class.
- Keeps DI registration unchanged while modernizing class design.

### 2) TimeProvider for Testable Time Logic

Age calculations have been refactored to avoid direct `DateTime.Now` usage:

- Domain model now exposes `Horse.GetAge(DateTime utcNow)`.
- Mapping layer receives `TimeProvider` from DI and computes age using `timeProvider.GetUtcNow().UtcDateTime`.
- `Program.cs` registers `TimeProvider.System`.

**Why this helps**
- Enables deterministic tests by substituting a fake `TimeProvider`.
- Avoids local-time/UTC drift issues from `DateTime.Now`.
- Centralizes time access policy.

### 3) Global Exception Handling with IExceptionHandler

Endpoint-level `try/catch` has been removed in favor of a single global exception pipeline:

- Implemented `GlobalExceptionHandler : IExceptionHandler`.
- Registered with:
  - `builder.Services.AddProblemDetails()`
  - `builder.Services.AddExceptionHandler<GlobalExceptionHandler>()`
  - `app.UseExceptionHandler()`

The handler:
- Logs exception + request method/path.
- Produces standardized RFC 7807 `ProblemDetails` responses.
- Maps `InvalidOperationException` to HTTP 400.
- Uses HTTP 500 for unhandled server exceptions.
- Includes exception details only in Development.

**Why this helps**
- Uniform error responses across all endpoints.
- Single place to evolve error policy and logging.
- Cleaner controller actions focused on business flow.

### 4) DTOs + Mapping Layer

The API uses dedicated request/response DTOs and a mapping abstraction:

- Request DTOs: `CreateHorseRequestDto`, `UpdateHorseRequestDto`, `PaginationQueryDto`
- Response DTOs: `HorseResponseDto`, `PagedResponseDto<T>`
- Mapper abstraction: `IHorseMapper` + `HorseMapper`

Controller actions now rely on `IHorseMapper` to:
- Map request DTOs to domain entities.
- Map domain entities to response DTOs.
- Compute derived response fields (including age via `TimeProvider`).

**Why this helps**
- Prevents direct domain model exposure over API boundaries.
- Supports independent evolution of external contract vs internal model.
- Keeps controllers thin and improves testability of mapping rules.

### Dependency Registration Summary

In `Program.cs`, the following registrations support these patterns:

```csharp
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<IHorseMapper, HorseMapper>();
```

