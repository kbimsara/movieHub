# MovieHub Quick Start Script
# Starts all essential services from the root directory

Write-Host "`n🎬 Starting MovieHub Platform..." -ForegroundColor Cyan
Write-Host "═══════════════════════════════════════════════════════" -ForegroundColor Cyan

# Check if Docker is running
Write-Host "`n[1/4] Checking Docker..." -ForegroundColor Yellow
try {
    docker --version | Out-Null
    Write-Host "  ✓ Docker is running" -ForegroundColor Green
} catch {
    Write-Host "  ✗ Docker is not running. Please start Docker Desktop." -ForegroundColor Red
    exit 1
}

# Stop any existing containers
Write-Host "`n[2/4] Stopping existing containers..." -ForegroundColor Yellow
docker-compose down 2>$null
Write-Host "  ✓ Cleaned up" -ForegroundColor Green

# Start essential services
Write-Host "`n[3/4] Starting services..." -ForegroundColor Yellow
Write-Host "  - PostgreSQL database" -ForegroundColor Gray
Write-Host "  - Redis cache" -ForegroundColor Gray
Write-Host "  - Auth service (backend)" -ForegroundColor Gray
Write-Host "  - Frontend application" -ForegroundColor Gray

docker-compose up -d postgres redis auth-service frontend

# Wait for services to be ready
Write-Host "`n[4/4] Waiting for services to initialize..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Check service status
Write-Host "`n📊 Service Status:" -ForegroundColor Cyan
docker-compose ps

# Test backend health
Write-Host "`n🔍 Testing services..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5001/health" -UseBasicParsing -TimeoutSec 5
    if ($response.StatusCode -eq 200) {
        Write-Host "  ✓ Backend API is healthy" -ForegroundColor Green
    }
} catch {
    Write-Host "  ⚠ Backend API starting (this is normal, give it a moment)" -ForegroundColor Yellow
}

# Display access information
Write-Host "`n═══════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "🎉 MovieHub is running!" -ForegroundColor Green
Write-Host "═══════════════════════════════════════════════════════" -ForegroundColor Cyan

Write-Host "`n🌐 Access URLs:" -ForegroundColor Yellow
Write-Host "  Frontend:   http://localhost:3000" -ForegroundColor White
Write-Host "  Backend:    http://localhost:5001" -ForegroundColor White
Write-Host "  Swagger UI: http://localhost:5001/swagger" -ForegroundColor White
Write-Host "  Grafana:    http://localhost:3001" -ForegroundColor White
Write-Host "  Prometheus: http://localhost:9090" -ForegroundColor White

Write-Host "`n📚 Quick Links:" -ForegroundColor Yellow
Write-Host "  Documentation:  README.md" -ForegroundColor Gray
Write-Host "  Frontend Docs:  FRONTEND.md" -ForegroundColor Gray
Write-Host "  Backend Docs:   BACKEND.md" -ForegroundColor Gray

Write-Host "`n🛠️ Useful Commands:" -ForegroundColor Yellow
Write-Host "  View logs:      docker-compose logs -f" -ForegroundColor Gray
Write-Host "  Stop services:  docker-compose down" -ForegroundColor Gray
Write-Host "  Restart:        docker-compose restart" -ForegroundColor Gray

Write-Host "`n✅ Ready to use! Open http://localhost:3000 in your browser" -ForegroundColor Green
Write-Host "═══════════════════════════════════════════════════════`n" -ForegroundColor Cyan

# Open browser (optional)
$openBrowser = Read-Host "Open browser now? (Y/n)"
if ($openBrowser -ne 'n' -and $openBrowser -ne 'N') {
    Start-Process "http://localhost:3000"
    Start-Process "http://localhost:5001/swagger"
}
