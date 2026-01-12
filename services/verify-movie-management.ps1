# Movie Management Service - Verification Script
# Run this to verify the service is properly set up

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Movie Management Service Verification" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

$servicePath = "e:\Github\movieHub\services\movie-management-service"
$checksPassed = 0
$totalChecks = 0

# Check project structure
Write-Host "Checking project structure..." -ForegroundColor Yellow
$totalChecks += 4

$requiredDirs = @(
    "MovieManagementService.API",
    "MovieManagementService.Application",
    "MovieManagementService.Domain",
    "MovieManagementService.Infrastructure"
)

foreach ($dir in $requiredDirs) {
    $path = Join-Path $servicePath $dir
    if (Test-Path $path) {
        Write-Host "  [OK] $dir exists" -ForegroundColor Green
        $checksPassed++
    }
    else {
        Write-Host "  [FAIL] $dir missing" -ForegroundColor Red
    }
}

# Check key files
Write-Host "`nChecking key files..." -ForegroundColor Yellow
$totalChecks += 6

$requiredFiles = @(
    "Dockerfile",
    "README.md",
    "SETUP_GUIDE.md",
    "MovieManagementService.API\Program.cs",
    "MovieManagementService.API\appsettings.json",
    "MovieManagementService.API\.env"
)

foreach ($file in $requiredFiles) {
    $path = Join-Path $servicePath $file
    if (Test-Path $path) {
        Write-Host "  [OK] $file exists" -ForegroundColor Green
        $checksPassed++
    }
    else {
        Write-Host "  [FAIL] $file missing" -ForegroundColor Red
    }
}

# Check API Gateway integration
Write-Host "`nChecking API Gateway integration..." -ForegroundColor Yellow
$totalChecks++

$gatewaySettings = Get-Content "e:\Github\movieHub\services\api-gateway\appsettings.json" -Raw
if ($gatewaySettings -match "upload-route" -and $gatewaySettings -match "movie-management-cluster") {
    Write-Host "  [OK] API Gateway routes configured" -ForegroundColor Green
    $checksPassed++
}
else {
    Write-Host "  [WARN] API Gateway routes not found" -ForegroundColor Yellow
}

# Check docker-compose
Write-Host "`nChecking docker-compose configuration..." -ForegroundColor Yellow
$totalChecks++

$dockerCompose = Get-Content "e:\Github\movieHub\services\docker-compose.yml" -Raw
if ($dockerCompose -match "movie-management-service" -and $dockerCompose -match "movie-management-db") {
    Write-Host "  [OK] Docker services configured" -ForegroundColor Green
    $checksPassed++
}
else {
    Write-Host "  [FAIL] Docker services not found" -ForegroundColor Red
}

# Check .env file
Write-Host "`nChecking environment variables..." -ForegroundColor Yellow
$totalChecks++

$envContent = Get-Content "e:\Github\movieHub\services\.env" -Raw
if ($envContent -match "MOVIE_MANAGEMENT_HTTP_PORT" -and $envContent -match "MOVIE_MANAGEMENT_DB") {
    Write-Host "  [OK] Environment variables configured" -ForegroundColor Green
    $checksPassed++
}
else {
    Write-Host "  [WARN] Some environment variables not found" -ForegroundColor Yellow
}

# Check Postman collection
Write-Host "`nChecking Postman collection..." -ForegroundColor Yellow
$totalChecks++

$postmanPath = "e:\Github\movieHub\services\Request-postman\MovieHub-ApiGateWay.postman_collection.json"
if (Test-Path $postmanPath) {
    $postmanContent = Get-Content $postmanPath -Raw
    if ($postmanContent -match "Movie Management Service") {
        Write-Host "  [OK] Postman collection updated" -ForegroundColor Green
        $checksPassed++
    }
    else {
        Write-Host "  [WARN] Movie Management endpoints not found in Postman" -ForegroundColor Yellow
    }
}
else {
    Write-Host "  [WARN] Postman collection not found" -ForegroundColor Yellow
}

# Summary
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Verification Summary" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

Write-Host "Checks passed: $checksPassed / $totalChecks" -ForegroundColor $(if ($checksPassed -eq $totalChecks) { "Green" } else { "Yellow" })

if ($checksPassed -eq $totalChecks) {
    Write-Host "`nSUCCESS: All checks passed! Service is ready to use.`n" -ForegroundColor Green
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "  1. cd e:\Github\movieHub\services" -ForegroundColor White
    Write-Host "  2. docker-compose up -d --build" -ForegroundColor White
    Write-Host "  3. curl http://localhost:5005/health" -ForegroundColor White
    Write-Host "  4. Open http://localhost:5005/swagger in browser`n" -ForegroundColor White
}
else {
    Write-Host "`nSome checks failed. Please review the output above.`n" -ForegroundColor Yellow
}

# Check Docker
Write-Host "Checking Docker..." -ForegroundColor Yellow
$dockerCheck = $null
try {
    $dockerCheck = docker --version 2>&1
}
catch {
    # Ignore
}

if ($dockerCheck -and $dockerCheck -match "Docker version") {
    Write-Host "  [OK] Docker is installed" -ForegroundColor Green
    Write-Host "`nTo start the service:" -ForegroundColor Cyan
    Write-Host "  docker-compose up -d movie-management-service`n" -ForegroundColor White
}
else {
    Write-Host "  [WARN] Docker not found or not running`n" -ForegroundColor Yellow
}

Write-Host "========================================`n" -ForegroundColor Cyan

