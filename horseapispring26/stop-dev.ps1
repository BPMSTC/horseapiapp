# PowerShell script to stop development environment

Write-Host "🛑 Stopping Horse API Development Environment" -ForegroundColor Red

# Stop SQL Server container
Write-Host "🐳 Stopping SQL Server container..." -ForegroundColor Blue
docker-compose -f docker-compose.dev.yml down

Write-Host "✅ Development environment stopped!" -ForegroundColor Green
Write-Host "Note: SQL Server data is preserved in Docker volume" -ForegroundColor Yellow


