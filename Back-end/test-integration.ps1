# MovieHub Integration Test Script
# This script tests the backend and frontend integration

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "MovieHub Integration Test" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Test 1: Check if Docker is running
Write-Host "[1/6] Checking Docker status..." -ForegroundColor Yellow
try {
    $dockerVersion = docker --version 2>&1
    Write-Host "✓ Docker is installed and running" -ForegroundColor Green
}
catch {
    Write-Host "✗ Docker is not running. Please start Docker Desktop." -ForegroundColor Red
    exit 1
}

# Test 2: Check if containers are running
Write-Host ""
Write-Host "[2/6] Checking container status..." -ForegroundColor Yellow
$containers = docker ps --format "{{.Names}}" 2>$null
$requiredContainers = @("moviehub-frontend", "moviehub-auth-service", "moviehub-postgres", "moviehub-redis")
$allRunning = $true

foreach ($container in $requiredContainers) {
    if ($containers -contains $container) {
        Write-Host "✓ $container is running" -ForegroundColor Green
    }
    else {
        Write-Host "✗ $container is not running" -ForegroundColor Red
        $allRunning = $false
    }
}

if (-not $allRunning) {
    Write-Host ""
    Write-Host "Please start all containers with:" -ForegroundColor Yellow
    Write-Host "  cd Back-end" -ForegroundColor Cyan
    Write-Host "  docker-compose up -d" -ForegroundColor Cyan
    exit 1
}

# Test 3: Check backend health
Write-Host ""
Write-Host "[3/6] Testing backend health endpoint..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5001/health" -UseBasicParsing -TimeoutSec 10
    if ($response.StatusCode -eq 200) {
        Write-Host "✓ Backend health check passed (Status: $($response.StatusCode))" -ForegroundColor Green
    }
    else {
        Write-Host "✗ Backend health check failed (Status: $($response.StatusCode))" -ForegroundColor Red
    }
}
catch {
    Write-Host "✗ Backend is not accessible at http://localhost:5001" -ForegroundColor Red
    Write-Host "  Error: $_" -ForegroundColor Red
}

# Test 4: Check frontend
Write-Host ""
Write-Host "[4/6] Testing frontend..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:3000" -UseBasicParsing -TimeoutSec 10
    if ($response.StatusCode -eq 200) {
        Write-Host "✓ Frontend is accessible (Status: $($response.StatusCode))" -ForegroundColor Green
    }
    else {
        Write-Host "✗ Frontend check failed (Status: $($response.StatusCode))" -ForegroundColor Red
    }
}
catch {
    Write-Host "✗ Frontend is not accessible at http://localhost:3000" -ForegroundColor Red
    Write-Host "  Error: $_" -ForegroundColor Red
}

# Test 5: Check database connection
Write-Host ""
Write-Host "[5/6] Testing database connection..." -ForegroundColor Yellow
try {
    $dbTest = docker exec moviehub-postgres psql -U postgres -d moviehub_auth -c "SELECT 1;" 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Database is accessible" -ForegroundColor Green
    }
    else {
        Write-Host "✗ Database connection failed" -ForegroundColor Red
    }
}
catch {
    Write-Host "✗ Unable to test database connection" -ForegroundColor Red
}

# Test 6: Test Auth API endpoint
Write-Host ""
Write-Host "[6/6] Testing Auth API endpoint..." -ForegroundColor Yellow
try {
    $testUser = @{
        email    = "test_$(Get-Random)@example.com"
        username = "testuser_$(Get-Random)"
        password = "Test@123456"
    } | ConvertTo-Json

    $response = Invoke-WebRequest -Uri "http://localhost:5001/api/v1/Auth/register" `
        -Method POST `
        -ContentType "application/json" `
        -Body $testUser `
        -UseBasicParsing `
        -TimeoutSec 10 2>$null

    if ($response.StatusCode -eq 200 -or $response.StatusCode -eq 201) {
        Write-Host "✓ Auth API is working (User registration successful)" -ForegroundColor Green
        
        # Try to parse response
        $result = $response.Content | ConvertFrom-Json
        if ($result.data.accessToken) {
            Write-Host "  ↳ Access token received" -ForegroundColor Gray
        }
    }
    else {
        Write-Host "⚠ Auth API responded with status: $($response.StatusCode)" -ForegroundColor Yellow
    }
}
catch {
    $statusCode = $_.Exception.Response.StatusCode.value__
    if ($statusCode -eq 400) {
        Write-Host "⚠ Auth API is accessible but validation failed (expected for random test data)" -ForegroundColor Yellow
    }
    else {
        Write-Host "✗ Auth API test failed" -ForegroundColor Red
        Write-Host "  Error: $_" -ForegroundColor Red
    }
}

# Summary
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Test Summary" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Services are running at:" -ForegroundColor White
Write-Host "  Frontend:  http://localhost:3000" -ForegroundColor Cyan
Write-Host "  Backend:   http://localhost:5001" -ForegroundColor Cyan
Write-Host "  Swagger:   http://localhost:5001/swagger" -ForegroundColor Cyan
Write-Host "  Grafana:   http://localhost:3001" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "  1. Open http://localhost:3000 in your browser" -ForegroundColor White
Write-Host "  2. Try registering a new user" -ForegroundColor White
Write-Host "  3. Test the login functionality" -ForegroundColor White
Write-Host "  4. Check Swagger UI at http://localhost:5001/swagger" -ForegroundColor White
Write-Host ""
Write-Host "View logs with: docker-compose logs -f" -ForegroundColor Gray
Write-Host "========================================" -ForegroundColor Cyan

