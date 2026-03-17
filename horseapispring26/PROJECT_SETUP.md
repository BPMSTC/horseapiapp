# Horse API Fall 2025 - Project Setup Guide

**Purpose**: Complete setup, installation, and basic usage guide for the Horse API project.

## 📁 Documentation Structure
- **PROJECT_SETUP.md** (this file) - Setup, installation, database configuration, and basic usage
- **API_DOCUMENTATION.md** - Complete API reference, endpoints, examples, and testing guide

---

A .NET Web API project designed to be consumed by a .NET MAUI MVVM application.

## Project Structure

This project follows the standard .NET Web API structure with the following key components:

### Core Files
- **Program.cs** - Application entry point and configuration
- **appsettings.json** - Application configuration settings
- **appsettings.Development.json** - Development-specific configuration

### Controllers
- **Controllers/HorseController.cs** - Main API controller for horse management operations
- **Controllers/WeatherForecastController.cs** - Example API controller demonstrating basic REST endpoints

### Models
- **Models/Horse.cs** - Comprehensive horse data model including:
  - Basic Info: Name, Registration Number, Date of Birth, Gender, Color
  - Pedigree: Sire (father), Dam (mother), Breeder Name
  - Visual: Picture URL for horse images
  - Performance: Race record, wins, places, shows, career earnings
  - Team: Current owner and trainer
  - Calculated properties: Win percentage, age, etc.
- **WeatherForecast.cs** - Example weather forecast model

### Configuration
- **Properties/launchSettings.json** - Launch configuration for different environments
- **horseapispring26.csproj** - Project file containing dependencies and build configuration

## Technology Stack

- **.NET 10.0** - Latest .NET framework
- **ASP.NET Core Web API** - RESTful API framework
- **Entity Framework Core** - Object-relational mapping (ORM)
- **SQL Server 2022** - Relational database (containerized)
- **Docker** - Containerization platform
- **C#** - Primary programming language

## Getting Started

### Prerequisites
- .NET 10.0 SDK installed
- Docker Desktop (for SQL Server container)
- Visual Studio, VS Code, or any .NET-compatible IDE
- PowerShell (Windows) or Bash (Linux/macOS) for running setup scripts

### Quick Start (Recommended: Local API + Containerized Database)

#### Option 1: Automated Setup (Recommended)
1. **Navigate to the project directory:**
   ```bash
   cd horseapispring26
   ```

2. **Start the development environment:**
   ```bash
   # Windows PowerShell
   .\start-dev.ps1
   
   # Linux/macOS
   chmod +x start-dev.sh
   ./start-dev.sh
   ```

3. **Access your API:**
   - API: https://localhost:7240
   - SQL Server: localhost:1433

## Database Setup

### Understanding the Database Architecture
Your application uses a **hybrid approach** that may be new to students:

- **SQL Server Database**: Stores the actual horse data in a relational database
- **JSON Import Process**: Automatically populates the database from a JSON file on first run
- **Entity Framework Migrations**: Manages database schema changes
- **Docker Containerization**: SQL Server runs in a container for easy setup

### Automated Setup (Recommended)
Use the provided scripts for easy setup:

**Windows:**
```powershell
.\start-dev.ps1
```

**Linux/macOS:**
```bash
chmod +x start-dev.sh
./start-dev.sh
```

### Manual Setup (For Learning)
If you want to understand each step:

1. **Start SQL Server:**
   ```bash
   docker-compose -f docker-compose.dev.yml up -d
   ```

2. **Verify SQL Server is ready:**
   ```bash
   docker exec horseapi-database-dev /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourPassword123!" -Q "SELECT 1"
   ```

3. **Start the API:**
   ```bash
   cd horseapispring26
   dotnet run
   ```

The API will automatically:
- Create the database schema
- Import sample data from `data/horses.json`
- Start serving requests at `https://localhost:7240`

### Understanding the Scripts (New Concepts for Students)

The `start-dev.ps1` and `start-dev.sh` scripts automate the setup process:

**What the Scripts Do:**
1. **Environment Setup**: Create `.env` file from template
2. **Container Management**: Start SQL Server Docker container
3. **Health Checking**: Wait for SQL Server to be ready (30 attempts, 2-second intervals)
4. **API Launch**: Start the .NET API application

**Why Use Scripts:**
- **Consistency**: Same setup process across different machines
- **Error Handling**: Built-in retry logic and error checking
- **Time Saving**: Automates repetitive setup tasks
- **Student-Friendly**: Reduces setup complexity

### Database Architecture
- **Database**: SQL Server 2022 (Express edition)
- **Container**: `horseapi-database-dev`
- **Schema**: Managed by Entity Framework migrations
- **Data**: 11 sample horses (5 modern + 6 legendary racehorses)
- **Persistence**: Data survives container restarts via Docker volumes

### Troubleshooting Database Issues

**Common Problems and Solutions:**

1. **"Cannot connect to SQL Server"**
   ```bash
   # Check if container is running
   docker ps
   
   # Check container logs
   docker logs horseapi-database-dev
   
   # Restart container
   docker-compose -f docker-compose.dev.yml restart
   ```

2. **"Database not populated"**
   - Check `appsettings.Development.json` has `"ImportFromJsonOnStart": true`
   - Verify `data/horses.json` exists and has valid data
   - Check application logs for import errors

3. **"Migration failed"**
   - The application falls back to `EnsureCreated()` if migrations fail
   - This is normal and expected behavior

### Understanding the Data Flow

```
JSON File (data/horses.json) 
    ↓ (on first startup)
Database Initializer (DbInitializer.cs)
    ↓ (imports data)
SQL Server Database (HorseDB.dbo.Horses)
    ↓ (via Entity Framework)
SQL Repository (SqlHorseRepository.cs)
    ↓ (handles CRUD operations)
API Controller (HorseController.cs)
    ↓ (serves HTTP requests)
MAUI Application (your mobile app)
```

#### Option 2: Manual Setup
1. **Start SQL Server container:**
   ```bash
   docker-compose -f docker-compose.dev.yml up -d
   ```

2. **Start the API:**
   ```bash
   cd horseapispring26
   dotnet run
   ```

3. **Stop when done:**
   ```bash
   # Windows PowerShell
   .\stop-dev.ps1
   
   # Or manually
   docker-compose -f docker-compose.dev.yml down
   ```

### Testing with Native OpenAPI Support
This API uses .NET 10.0's **native OpenAPI support** instead of external Swagger packages. This provides several benefits:

- **Built-in support** - No additional NuGet packages required
- **Better performance** - Native integration with ASP.NET Core
- **Automatic documentation** - OpenAPI spec generated automatically
- **Future-proof** - Uses Microsoft's recommended approach

#### Available Testing Methods:

**1. OpenAPI JSON Specification**
- **URL:** `https://localhost:7240/openapi/v1.json`
- **Purpose:** Machine-readable API specification
- **Use:** Import into Postman, generate client code, or view raw API structure

**2. Direct HTTP Testing with curl**
```bash
# Get all horses
curl -X GET "https://localhost:7240/api/horse"

# Get specific horse by ID
curl -X GET "https://localhost:7240/api/horse/1"

# Search horses by name
curl -X GET "https://localhost:7240/api/horse/search?searchTerm=thunder"

# Get horses by owner
curl -X GET "https://localhost:7240/api/horse/owner/John%20Smith%20Racing"

# Get top earning horses
curl -X GET "https://localhost:7240/api/horse/top-earners?count=3"

# Get horses by gender (0=Stallion, 1=Mare, 2=Gelding)
curl -X GET "https://localhost:7240/api/horse/gender/0"
```

**3. Testing with Postman**
1. Import the OpenAPI spec from `https://localhost:7240/openapi/v1.json`
2. Create a new collection
3. Test all endpoints with the provided sample data

**4. Testing with Browser**
- **GET endpoints** can be tested directly in the browser
- **Example:** `https://localhost:7240/api/horse` (shows all horses)
- **Example:** `https://localhost:7240/api/horse/1` (shows Thunder Strike)

**5. Creating and Updating Horses with curl**
```bash
# Create a new horse
curl -X POST "https://localhost:7240/api/horse" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Lightning Bolt",
    "registrationNumber": "TB-006-2023",
    "dateOfBirth": "2023-04-15T00:00:00Z",
    "gender": 0,
    "color": "Chestnut",
    "sire": "Thunder Strike",
    "dam": "Storm Runner",
    "breederName": "Lightning Stables",
    "pictureUrl": "https://images.unsplash.com/photo-1544966503-7cc4bb7c9e0b?w=400&h=300&fit=crop&crop=center",
    "totalRacesRun": 0,
    "wins": 0,
    "places": 0,
    "shows": 0,
    "careerEarnings": 0.00,
    "currentOwner": "New Owner Racing",
    "trainer": "New Trainer"
  }'

# Update an existing horse (replace {id} with actual ID)
curl -X PUT "https://localhost:7240/api/horse/1" \
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
    "pictureUrl": "https://images.unsplash.com/photo-1544966503-7cc4bb7c9e0b?w=400&h=300&fit=crop&crop=center",
    "totalRacesRun": 15,
    "wins": 6,
    "places": 4,
    "shows": 2,
    "careerEarnings": 150000.00,
    "currentOwner": "Updated Owner Racing",
    "trainer": "Updated Trainer"
  }'

# Delete a horse
curl -X DELETE "https://localhost:7240/api/horse/5"
```

### Available Endpoints
- **GET /api/horse** - View all horses
- **GET /api/horse/{id}** - Get specific horse by ID
- **POST /api/horse** - Create new horse
- **PUT /api/horse/{id}** - Update existing horse
- **DELETE /api/horse/{id}** - Delete horse
- **GET /api/horse/search** - Search horses
- **GET /api/horse/top-earners** - Get top earning horses
- And more...

### Sample Data
The API comes with 11 sample horses loaded from `data/horses.json`, including both modern and legendary horses:

**Modern Horses:**
- Thunder Strike (Bay Thoroughbred)
- Midnight Express (Black Gelding)
- Golden Arrow (Chestnut Mare)
- Silver Bullet (Gray Stallion)
- Wind Dancer (Palomino Mare)

**Legendary Racehorses:**
- **Secretariat** (1970) - Triple Crown winner, $1.37M earnings
- **Man o' War** (1917) - 20 wins in 21 races, $249K earnings
- **Citation** (1945) - Triple Crown winner, $1.09M earnings
- **Kelso** (1957) - 5-time Horse of the Year, $1.98M earnings
- **Seattle Slew** (1974) - Triple Crown winner, $1.21M earnings
- **Dr. Fager** (1964) - 4-time champion, $1.00M earnings

Each horse includes complete information with picture URLs for testing.

### Troubleshooting

**If the API won't start:**
- Ensure you're in the `horseapispring26` directory
- Check that .NET 10.0 SDK is installed: `dotnet --version`
- Try running `dotnet clean` then `dotnet build`

**If Swagger doesn't load:**
- Make sure you're using the correct URL: `https://localhost:7240/swagger`
- Check that the API is running (you should see console output)
- Try the HTTP version: `http://localhost:5102/swagger`

**If you get SSL certificate errors:**
- Click "Advanced" in your browser and "Proceed to localhost"
- Or use the HTTP version: `http://localhost:5102/api/horse`

**If OpenAPI JSON doesn't load:**
- Check that the API is running: `https://localhost:7240/api/horse`
- Try the HTTP version: `http://localhost:5102/openapi/v1.json`
- Ensure you're using the correct URL format

**If curl commands fail:**
- Make sure the API is running (`dotnet run`)
- Check the correct port (7240 for HTTPS, 5102 for HTTP)
- Use `-k` flag for curl if you get SSL errors: `curl -k -X GET "https://localhost:7240/api/horse"`

## API Endpoints

### Horse Management API
The API provides comprehensive horse management endpoints:

**Basic CRUD Operations:**
- `GET /api/horse` - Get all horses
- `GET /api/horse/{id}` - Get horse by ID
- `POST /api/horse` - Create new horse
- `PUT /api/horse/{id}` - Update existing horse
- `DELETE /api/horse/{id}` - Delete horse

**Advanced Search & Filtering:**
- `GET /api/horse/registration/{registrationNumber}` - Get horse by registration number
- `GET /api/horse/search?searchTerm={term}` - Search horses across multiple fields
- `GET /api/horse/owner/{owner}` - Get horses by owner
- `GET /api/horse/trainer/{trainer}` - Get horses by trainer
- `GET /api/horse/top-earners?count={number}` - Get top earning horses
- `GET /api/horse/gender/{gender}` - Get horses by gender (0=Stallion, 1=Mare, 2=Gelding)

**Data Sources:**
- **Primary**: SQL Server database (HorseDB) running in Docker container
- **Sample Data**: Automatically imported from `data/horses.json` on first startup
- **Repository Pattern**: `SqlHorseRepository` handles all database operations
- **Entity Framework**: Manages database schema and migrations
- **Connection**: `localhost:1433` (SQL Server), `sa` user, password from `.env` file

## Development Notes

This API is designed to be consumed by a .NET MAUI MVVM application. The structure will be updated and expanded throughout the development process to support the mobile application's requirements.

## Architecture Features

### Native OpenAPI Support (.NET 10.0)
This project demonstrates **modern .NET API development** using native OpenAPI support:

- **No External Dependencies** - Uses built-in ASP.NET Core OpenAPI features
- **Automatic Documentation** - API spec generated automatically from code
- **Better Performance** - Native integration without third-party overhead
- **Future-Proof** - Follows Microsoft's recommended approach for .NET 10.0+
- **Simplified Configuration** - Minimal setup required in Program.cs

**Why This Matters for Students:**
- Learn the **latest .NET patterns** and best practices
- Understand **native API documentation** generation
- Experience **modern development workflows** without external dependencies
- Prepare for **production-ready** API development

### Repository Pattern
- **Interface-based design** for easy data source switching
- **SQL Server implementation** (`SqlHorseRepository`) for production
- **JSON fallback** (`JsonHorseRepository`) for development
- **Dependency injection** for clean architecture

### Data Management
- **SQL Server database** (`HorseDB`) for persistent storage
- **JSON import process** automatically populates database on first run
- **Entity Framework Core** for object-relational mapping
- **Database migrations** for schema management
- **Sample data** with realistic horse information and picture URLs

### MAUI Integration Ready
- **CORS configured** for cross-origin requests
- **RESTful API design** following industry standards
- **Comprehensive error handling** with proper HTTP status codes
- **Swagger documentation** for easy testing and integration

## Next Steps

- Add authentication and authorization
- Implement SQL Server repository with Entity Framework Core
- Add data validation and business rules
- Implement file upload for horse pictures
- Add comprehensive logging and monitoring
- Create Docker Compose configuration for production deployment

## Testing with Postman (Complete Beginner's Guide)

Postman is a powerful tool for testing APIs. This section will guide you through setting up Postman and testing the Horse API from scratch.

### Step 1: Download and Install Postman

1. **Go to:** [https://www.postman.com/downloads/](https://www.postman.com/downloads/)
2. **Download** the version for your operating system
3. **Install** Postman following the installation wizard
4. **Create a free account** (optional but recommended for syncing)

### Step 2: Import the API Specification

1. **Start your Horse API:**
   ```bash
   cd horseapispring26
   dotnet run
   ```

2. **Open Postman** and click **"Import"** (top left)

3. **Choose "Link"** tab and enter:
   ```
   https://localhost:7240/openapi/v1.json
   ```

4. **Click "Continue"** and then **"Import"**

5. **You should see** a new collection called "horseapispring26" with all the endpoints

### Step 3: Set Up Environment Variables

1. **Click the gear icon** (⚙️) in the top right
2. **Click "Add"** to create a new environment
3. **Name it:** "Horse API Local"
4. **Add these variables:**
   - **Variable:** `base_url`
   - **Initial Value:** `https://localhost:7240`
   - **Current Value:** `https://localhost:7240`

5. **Click "Save"** and select your new environment

### Step 4: Test Basic Endpoints

#### Test 1: Get All Horses
1. **Find** "GET /api/horse" in your collection
2. **Click on it** to open the request
3. **Click "Send"**
4. **You should see** a list of 11 horses including legendary racehorses like Secretariat and Man o' War

#### Test 2: Get Specific Horse
1. **Find** "GET /api/horse/{id}" in your collection
2. **Click on it** to open the request
3. **Change the URL** from `/api/horse/{id}` to `/api/horse/6` (Secretariat)
4. **Click "Send"**
5. **You should see** Secretariat's legendary racing record and $1.37M earnings

#### Test 3: Search Horses
1. **Find** "GET /api/horse/search" in your collection
2. **Click on it** to open the request
3. **Add query parameter:**
   - **Key:** `searchTerm`
   - **Value:** `thunder`
4. **Click "Send"**
5. **You should see** horses matching "thunder"

### Step 5: Test Creating a New Horse

1. **Find** "POST /api/horse" in your collection
2. **Click on it** to open the request
3. **Go to the "Body" tab**
4. **Select "raw"** and **"JSON"** from the dropdown
5. **Paste this JSON:**
   ```json
   {
     "name": "Lightning Bolt",
     "registrationNumber": "TB-006-2023",
     "dateOfBirth": "2023-04-15T00:00:00Z",
     "gender": 0,
     "color": "Chestnut",
     "sire": "Thunder Strike",
     "dam": "Storm Runner",
     "breederName": "Lightning Stables",
     "pictureUrl": "https://images.unsplash.com/photo-1544966503-7cc4bb7c9e0b?w=400&h=300&fit=crop&crop=center",
     "totalRacesRun": 0,
     "wins": 0,
     "places": 0,
     "shows": 0,
     "careerEarnings": 0.00,
     "currentOwner": "New Owner Racing",
     "trainer": "New Trainer"
   }
   ```
6. **Click "Send"**
7. **You should see** the new horse created with an ID

### Step 6: Test Updating a Horse

1. **Find** "PUT /api/horse/{id}" in your collection
2. **Click on it** to open the request
3. **Change the URL** to `/api/horse/1` (or any existing horse ID)
4. **Go to the "Body" tab**
5. **Select "raw"** and **"JSON"**
6. **Paste this JSON** (updating Thunder Strike):
   ```json
   {
     "id": 1,
     "name": "Thunder Strike Updated",
     "registrationNumber": "TB-001-2020",
     "dateOfBirth": "2020-03-15T00:00:00Z",
     "gender": 0,
     "color": "Bay",
     "sire": "Lightning Bolt",
     "dam": "Storm Queen",
     "breederName": "Royal Stables",
     "pictureUrl": "https://images.unsplash.com/photo-1544966503-7cc4bb7c9e0b?w=400&h=300&fit=crop&crop=center",
     "totalRacesRun": 15,
     "wins": 6,
     "places": 4,
     "shows": 2,
     "careerEarnings": 150000.00,
     "currentOwner": "Updated Owner Racing",
     "trainer": "Updated Trainer"
   }
   ```
7. **Click "Send"**
8. **You should see** the updated horse details

### Step 7: Test Advanced Endpoints

#### Get Top Earning Horses
1. **Find** "GET /api/horse/top-earners" in your collection
2. **Add query parameter:**
   - **Key:** `count`
   - **Value:** `3`
3. **Click "Send"**

#### Get Horses by Gender
1. **Find** "GET /api/horse/gender/{gender}" in your collection
2. **Change URL** to `/api/horse/gender/0` (0=Stallion, 1=Mare, 2=Gelding)
3. **Click "Send"**

#### Get Horses by Owner
1. **Find** "GET /api/horse/owner/{owner}" in your collection
2. **Change URL** to `/api/horse/owner/John%20Smith%20Racing`
3. **Click "Send"**

### Step 8: Understanding the Results

**Successful Response (200 OK):**
- **Status:** 200 OK
- **Body:** JSON data with horse information
- **Headers:** Content-Type: application/json

**Error Response (404 Not Found):**
- **Status:** 404 Not Found
- **Body:** Error message like "Horse with ID 999 not found"

**Error Response (400 Bad Request):**
- **Status:** 400 Bad Request
- **Body:** Validation error details

### Step 9: Troubleshooting Common Issues

**"Could not get any response"**
- Make sure the API is running (`dotnet run`)
- Check the URL is correct (`https://localhost:7240`)
- Try the HTTP version (`http://localhost:5102`)

**SSL Certificate Error**
- Click "Settings" (⚙️) in Postman
- Go to "General" tab
- Turn **OFF** "SSL certificate verification"

**"Connection refused"**
- Ensure the API is running
- Check the correct port (7240 for HTTPS, 5102 for HTTP)
- Verify you're in the correct directory (`horseapispring26`)

### Step 10: Save Your Work

1. **Click "Save"** after each successful request
2. **Add descriptions** to your requests for future reference
3. **Create folders** to organize different types of tests
4. **Export your collection** to share with others

### Pro Tips for Students

- **Use the History tab** to see all your previous requests
- **Create different environments** for different API versions
- **Use variables** in your requests (like `{{base_url}}`)
- **Test edge cases** like invalid IDs or missing data
- **Save successful requests** as examples for your MAUI app

This Postman setup will help you understand how your MAUI app will interact with the API!
