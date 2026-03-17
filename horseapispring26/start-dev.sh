#!/bin/bash

# Bash script to start local development environment
# Run this from the horseapispring26 directory

echo "🐎 Starting Horse API Development Environment"

# Check if .env file exists
if [ ! -f ".env" ]; then
    echo "⚠️  .env file not found. Creating from template..."
    cp env.example .env
    echo "✅ Created .env file. You can edit it to change the SQL Server password."
fi

# Start SQL Server container
echo "🐳 Starting SQL Server container..."
docker-compose -f docker-compose.dev.yml up -d

# Wait for SQL Server to be ready
echo "⏳ Waiting for SQL Server to be ready..."
max_attempts=30
attempt=0

while [ $attempt -lt $max_attempts ]; do
    attempt=$((attempt + 1))
    echo "Attempt $attempt/$max_attempts - Checking SQL Server..."
    
    if docker exec horseapi-database-dev /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourPassword123!" -Q "SELECT 1" -C >/dev/null 2>&1; then
        echo "✅ SQL Server is ready!"
        break
    fi
    
    if [ $attempt -eq $max_attempts ]; then
        echo "❌ SQL Server failed to start after $max_attempts attempts"
        exit 1
    fi
    
    sleep 2
done

# Start the API
echo "🚀 Starting .NET API..."
echo "API will be available at: https://localhost:7240"
echo "SQL Server is available at: localhost:1433"
echo ""
echo "Press Ctrl+C to stop both services"

# Change to the API directory and run
cd horseapispring26
dotnet run


