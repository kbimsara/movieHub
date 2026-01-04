# MovieHub Frontend-Backend Service Connection Test

Write-Host "`n╔════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║  MOVIEHUB SERVICE CONNECTION TEST          ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════╝`n" -ForegroundColor Cyan

$successCount = 0
$failCount = 0

# Helper function to test endpoint
function Test-Endpoint {
    param(
        [string]$Name,
        [string]$Uri,
        [string]$Method = "GET",
        [hashtable]$Headers = @{},
        [string]$Body = $null
    )
    
    Write-Host "Testing: $Name" -ForegroundColor Yellow -NoNewline
    try {
        $params = @{
            Uri = $Uri
            Method = $Method
            UseBasicParsing = $true
        }
        
        if ($Headers.Count -gt 0) {
            $params.Headers = $Headers
        }
        
        if ($Body) {
            $params.Body = $Body
            $params.ContentType = "application/json"
        }
        
        $response = Invoke-WebRequest @params
        Write-Host " ✅ $($response.StatusCode)" -ForegroundColor Green
        $script:successCount++
        return $true
    } catch {
        Write-Host " ❌ $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
        $script:failCount++
        return $false
    }
}

# Register new test user
Write-Host "`n📝 Setting up test user..." -ForegroundColor Cyan
$testUsername = "conntest$(Get-Random -Maximum 9999)"
$testEmail = "conntest$(Get-Random -Maximum 9999)@test.com"
$testPassword = "Test@123456"

$registerBody = @{
    username = $testUsername
    email = $testEmail
    password = $testPassword
} | ConvertTo-Json

$response = Invoke-WebRequest -Uri "http://localhost:5000/api/auth/register" -Method POST -Body $registerBody -ContentType "application/json" -UseBasicParsing
$token = ($response.Content | ConvertFrom-Json).accessToken
$refreshToken = ($response.Content | ConvertFrom-Json).refreshToken
$headers = @{ "Authorization" = "Bearer $token" }

Write-Host "✅ Test user created: $testEmail`n" -ForegroundColor Green

# AUTH SERVICE TESTS
Write-Host "═══════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "🔐 AUTH SERVICE (port 5001 → /api/auth/*)" -ForegroundColor Cyan
Write-Host "═══════════════════════════════════════════" -ForegroundColor Cyan

Test-Endpoint "POST /api/auth/register" "http://localhost:5000/api/auth/register" "POST" @{} (@{username="test$(Get-Random)";email="test$(Get-Random)@test.com";password="Test@123"} | ConvertTo-Json)
Test-Endpoint "POST /api/auth/login" "http://localhost:5000/api/auth/login" "POST" @{} (@{email=$testEmail;password=$testPassword} | ConvertTo-Json)
Test-Endpoint "POST /api/auth/refresh" "http://localhost:5000/api/auth/refresh" "POST" @{} (@{refreshToken=$refreshToken} | ConvertTo-Json)
Test-Endpoint "GET  /api/auth/validate" "http://localhost:5000/api/auth/validate" "GET" $headers
Test-Endpoint "GET  /api/auth/me" "http://localhost:5000/api/auth/me" "GET" $headers
Test-Endpoint "POST /api/auth/logout" "http://localhost:5000/api/auth/logout" "POST" $headers (@{refreshToken=$refreshToken} | ConvertTo-Json)

# USER SERVICE TESTS
Write-Host "`n═══════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "👤 USER SERVICE (port 5002 → /api/me/*)" -ForegroundColor Cyan
Write-Host "═══════════════════════════════════════════" -ForegroundColor Cyan

# Create profile first
$createProfileBody = @{ displayName = "Connection Test User" } | ConvertTo-Json
try {
    Invoke-WebRequest -Uri "http://localhost:5002/api" -Method POST -Headers $headers -Body $createProfileBody -ContentType "application/json" -UseBasicParsing | Out-Null
} catch {}

Test-Endpoint "GET  /api/me" "http://localhost:5000/api/me" "GET" $headers
Test-Endpoint "PUT  /api/me" "http://localhost:5000/api/me" "PUT" $headers (@{displayName="Updated Test"} | ConvertTo-Json)
Test-Endpoint "GET  /api/me/library" "http://localhost:5000/api/me/library" "GET" $headers
Test-Endpoint "GET  /api/me/history" "http://localhost:5000/api/me/history" "GET" $headers
Test-Endpoint "GET  /api/me/favorites" "http://localhost:5000/api/me/favorites" "GET" $headers
Test-Endpoint "GET  /api/me/watch-later" "http://localhost:5000/api/me/watch-later" "GET" $headers

# CATALOG SERVICE TESTS
Write-Host "`n═══════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "🎬 CATALOG SERVICE (port 5003 → /api/movies/*)" -ForegroundColor Cyan
Write-Host "═══════════════════════════════════════════" -ForegroundColor Cyan

Test-Endpoint "GET  /api/movies" "http://localhost:5000/api/movies"
$testMovieId = "00000000-0000-0000-0000-000000000001"
Test-Endpoint "GET  /api/movies/{id}" "http://localhost:5000/api/movies/$testMovieId"

# SEARCH SERVICE TESTS
Write-Host "`n═══════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "🔍 SEARCH SERVICE (port 5004 → /api/search/*)" -ForegroundColor Cyan
Write-Host "═══════════════════════════════════════════" -ForegroundColor Cyan

Test-Endpoint "GET  /api/search/movies" "http://localhost:5000/api/search/movies?q=test&page=1&pageSize=10"

# SUMMARY
Write-Host "`n╔════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║  TEST SUMMARY                              ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════╝" -ForegroundColor Cyan

$total = $successCount + $failCount
$percentage = [math]::Round(($successCount / $total) * 100, 1)

Write-Host "`n✅ Passed: $successCount" -ForegroundColor Green
Write-Host "❌ Failed: $failCount" -ForegroundColor Red
Write-Host "📊 Success Rate: $percentage%`n" -ForegroundColor Cyan

if ($percentage -ge 80) {
    Write-Host "🎉 EXCELLENT: Most services are connected!" -ForegroundColor Green
} elseif ($percentage -ge 60) {
    Write-Host "⚠️  GOOD: Core services connected, some issues" -ForegroundColor Yellow
} else {
    Write-Host "❌ NEEDS WORK: Multiple connection issues" -ForegroundColor Red
}

Write-Host "`n📋 Frontend Service Files Updated:" -ForegroundColor Cyan
Write-Host "  • src/services/auth.service.ts - Auth endpoints" -ForegroundColor Gray
Write-Host "  • src/services/user.service.ts - User profile endpoints" -ForegroundColor Gray  
Write-Host "  • src/services/movie.service.ts - Movie catalog endpoints" -ForegroundColor Gray
Write-Host "  • src/services/search.service.ts - Search endpoints`n" -ForegroundColor Gray
