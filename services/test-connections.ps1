# Service Connection Test Script
# Tests all frontend service connections to backend microservices

$baseUrl = "http://localhost:5000"
$testResults = @{
    Total = 0
    Passed = 0
    Failed = 0
}

function Test-Endpoint {
    param(
        [string]$Method,
        [string]$Url,
        [string]$Description,
        [hashtable]$Headers = @{},
        [object]$Body = $null
    )
    
    $testResults.Total++
    Write-Host "`n[$Method] $Url" -ForegroundColor Yellow
    Write-Host "Description: $Description" -ForegroundColor Gray
    
    try {
        $params = @{
            Uri = $Url
            Method = $Method
            Headers = $Headers
            ContentType = "application/json"
        }
        
        if ($Body) {
            $params.Body = ($Body | ConvertTo-Json)
        }
        
        $response = Invoke-WebRequest @params -UseBasicParsing
        Write-Host "SUCCESS: $($response.StatusCode)" -ForegroundColor Green
        $testResults.Passed++
        return $true
    }
    catch {
        Write-Host "FAILED: $($_.Exception.Message)" -ForegroundColor Red
        $testResults.Failed++
        return $false
    }
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "MOVIEHUB SERVICE CONNECTION TEST" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Base URL: $baseUrl`n" -ForegroundColor Gray

# Test 1: Health Check
Write-Host "`n=== HEALTH CHECK ===" -ForegroundColor Cyan
Test-Endpoint -Method "GET" -Url "$baseUrl/health" -Description "API Gateway health check"

# Test 2-7: AUTH SERVICE (port 5001 -> /api/auth/*)
Write-Host "`n=== AUTH SERVICE (port 5001 -> /api/auth/*)" -ForegroundColor Cyan

# Register test user
$registerBody = @{
    email = "test_$(Get-Random)@test.com"
    password = "Test123!@#"
    confirmPassword = "Test123!@#"
}
$registerSuccess = Test-Endpoint -Method "POST" -Url "$baseUrl/api/auth/register" -Description "Register new user" -Body $registerBody

# Login
$loginBody = @{
    email = $registerBody.email
    password = $registerBody.password
}
$loginResult = $null
if ($registerSuccess) {
    try {
        $loginResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method POST -Body ($loginBody | ConvertTo-Json) -ContentType "application/json"
        $token = $loginResponse.accessToken
        $refreshToken = $loginResponse.refreshToken
        Write-Host "`n[POST] $baseUrl/api/auth/login" -ForegroundColor Yellow
        Write-Host "SUCCESS: Got access token" -ForegroundColor Green
        $testResults.Total++
        $testResults.Passed++
        $loginResult = $true
    }
    catch {
        Write-Host "`n[POST] $baseUrl/api/auth/login" -ForegroundColor Yellow
        Write-Host "FAILED: $($_.Exception.Message)" -ForegroundColor Red
        $testResults.Total++
        $testResults.Failed++
        $loginResult = $false
    }
}

# Test auth endpoints with token
if ($loginResult -and $token) {
    $authHeaders = @{
        "Authorization" = "Bearer $token"
    }
    
    Test-Endpoint -Method "GET" -Url "$baseUrl/api/auth/me" -Description "Get current user info" -Headers $authHeaders
    Test-Endpoint -Method "POST" -Url "$baseUrl/api/auth/validate" -Description "Validate token" -Headers $authHeaders
    
    if ($refreshToken) {
        $refreshBody = @{ refreshToken = $refreshToken }
        Test-Endpoint -Method "POST" -Url "$baseUrl/api/auth/refresh" -Description "Refresh access token" -Body $refreshBody
    }
    
    $logoutBody = @{ refreshToken = $refreshToken }
    Test-Endpoint -Method "POST" -Url "$baseUrl/api/auth/logout" -Description "Logout user" -Headers $authHeaders -Body $logoutBody
}

# Test 8-11: USER SERVICE (port 5002 -> /api/me/*)
Write-Host "`n=== USER SERVICE (port 5002 -> /api/me/*)" -ForegroundColor Cyan

if ($token) {
    $authHeaders = @{
        "Authorization" = "Bearer $token"
    }
    
    Test-Endpoint -Method "GET" -Url "$baseUrl/api/me" -Description "Get user profile" -Headers $authHeaders
    
    $profileBody = @{
        displayName = "Test User"
        bio = "Test bio"
    }
    Test-Endpoint -Method "PUT" -Url "$baseUrl/api/me" -Description "Update user profile" -Headers $authHeaders -Body $profileBody
    Test-Endpoint -Method "GET" -Url "$baseUrl/api/me/library" -Description "Get user library" -Headers $authHeaders
    Test-Endpoint -Method "GET" -Url "$baseUrl/api/me/favorites" -Description "Get user favorites" -Headers $authHeaders
}

# Test 12-13: CATALOG SERVICE (port 5003 -> /api/movies/*)
Write-Host "`n=== CATALOG SERVICE (port 5003 -> /api/movies/*)" -ForegroundColor Cyan

Test-Endpoint -Method "GET" -Url "$baseUrl/api/movies?page=1&pageSize=10" -Description "Get movies list"
Test-Endpoint -Method "GET" -Url "$baseUrl/api/movies/1" -Description "Get movie by ID"

# Test 14: SEARCH SERVICE (port 5004 -> /api/search/*)
Write-Host "`n=== SEARCH SERVICE (port 5004 -> /api/search/*)" -ForegroundColor Cyan

Test-Endpoint -Method "GET" -Url "$baseUrl/api/search/movies?query=test" -Description "Search movies"

# Summary
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "TEST SUMMARY" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Total Tests: $($testResults.Total)" -ForegroundColor White
Write-Host "Passed: $($testResults.Passed)" -ForegroundColor Green
Write-Host "Failed: $($testResults.Failed)" -ForegroundColor Red
$successRate = [math]::Round(($testResults.Passed / $testResults.Total) * 100, 2)
Write-Host "Success Rate: $successRate%" -ForegroundColor $(if ($successRate -ge 80) { "Green" } elseif ($successRate -ge 50) { "Yellow" } else { "Red" })

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "FRONTEND SERVICE FILE STATUS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Updated service files:" -ForegroundColor Gray
Write-Host "  - src/services/auth.service.ts - Auth endpoints" -ForegroundColor Gray
Write-Host "  - src/services/user.service.ts - User profile endpoints" -ForegroundColor Gray
Write-Host "  - src/services/movie.service.ts - Catalog endpoints" -ForegroundColor Gray
Write-Host "  - src/services/search.service.ts - Search endpoints" -ForegroundColor Gray
