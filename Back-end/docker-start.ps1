# Docker Compose Quick Start Script for MovieHub

Write-Host "Starting MovieHub with Docker Compose..." -ForegroundColor Green
Write-Host ""

# Change to Back-end directory
Set-Location -Path $PSScriptRoot

# Stop any running containers
Write-Host "Stopping any running containers..." -ForegroundColor Yellow
docker-compose down

# Build and start services
Write-Host ""
Write-Host "Building and starting services..." -ForegroundColor Yellow
Write-Host "This may take a few minutes on the first run..." -ForegroundColor Cyan
docker-compose up --build -d

# Wait for services to be ready
Write-Host ""
Write-Host "Waiting for services to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Check service status
Write-Host ""
Write-Host "Service Status:" -ForegroundColor Green
docker-compose ps

Write-Host ""
Write-Host "===========================================" -ForegroundColor Cyan
Write-Host "MovieHub is starting up!" -ForegroundColor Green
Write-Host "===========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Frontend:  http://localhost:3000" -ForegroundColor Yellow
Write-Host "Backend:   http://localhost:5001" -ForegroundColor Yellow
Write-Host "Swagger:   http://localhost:5001/swagger" -ForegroundColor Yellow
Write-Host "Grafana:   http://localhost:3001 (admin/admin)" -ForegroundColor Yellow
Write-Host ""
Write-Host "To view logs: docker-compose logs -f" -ForegroundColor Cyan
Write-Host "To stop:      docker-compose down" -ForegroundColor Cyan
Write-Host "===========================================" -ForegroundColor Cyan
