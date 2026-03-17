# PowerShell script to start local development environment
# Run this from the horseapispring26 directory

Write-Host "🐎 Starting Horse API Development Environment" -ForegroundColor Green

# Check if .env file exists
if (-not (Test-Path ".env")) {
    Write-Host "⚠️  .env file not found. Creating from template..." -ForegroundColor Yellow
    Copy-Item "env.example" ".env"
    Write-Host "✅ Created .env file. You can edit it to change the SQL Server password." -ForegroundColor Green
}

# Start SQL Server container
Write-Host "🐳 Starting SQL Server container..." -ForegroundColor Blue
docker-compose -f docker-compose.dev.yml up -d

# Wait for SQL Server to be ready
Write-Host "⏳ Waiting for SQL Server to be ready..." -ForegroundColor Yellow
$maxAttempts = 30
$attempt = 0

do {
    $attempt++
    Write-Host "Attempt $attempt/$maxAttempts - Checking SQL Server..." -ForegroundColor Gray
    
    try {
        $result = docker exec horseapi-database-dev /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourPassword123!" -Q "SELECT 1" -C 2>$null
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ SQL Server is ready!" -ForegroundColor Green
            break
        }
    }
    catch {
        # Continue waiting
    }
    
    if ($attempt -ge $maxAttempts) {
        Write-Host "❌ SQL Server failed to start after $maxAttempts attempts" -ForegroundColor Red
        exit 1
    }
    
    Start-Sleep -Seconds 2
} while ($true)

# Start the API
Write-Host "🚀 Starting .NET API..." -ForegroundColor Blue
Write-Host "API will be available at: https://localhost:7240" -ForegroundColor Cyan
Write-Host "SQL Server is available at: localhost:1433" -ForegroundColor Cyan
Write-Host ""
Write-Host "Press Ctrl+C to stop both services" -ForegroundColor Yellow

# Change to the API directory and run
Set-Location "horseapispring26"
dotnet run


