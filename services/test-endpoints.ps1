# MovieHub API Gateway Endpoint Tests
Write-Host "`n=== MovieHub API Gateway Endpoint Tests ===" -ForegroundColor Cyan
Write-Host "Testing all endpoints through http://localhost:5000`n" -ForegroundColor Gray

# 1. Test API Gateway Health
Write-Host "1. Testing API Gateway Health..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/health" -Method GET -UseBasicParsing
    Write-Host "   ✅ Gateway Health: $($response.StatusCode) - $($response.Content)" -ForegroundColor Green
} catch {
    Write-Host "   ❌ Gateway Health Error: $($_.Exception.Message)" -ForegroundColor Red
}

# 2. Test Auth Service - Register
Write-Host "`n2. Testing Auth Service - Register..." -ForegroundColor Yellow
$registerBody = @{
    username = "testuser$(Get-Random -Maximum 9999)"
    email = "test$(Get-Random -Maximum 9999)@example.com"
    password = "Test@123456"
} | ConvertTo-Json

try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/api/auth/register" -Method POST -Body $registerBody -ContentType "application/json" -UseBasicParsing
    $content = $response.Content | ConvertFrom-Json
    Write-Host "   ✅ Register: $($response.StatusCode)" -ForegroundColor Green
    Write-Host "   User ID: $($content.userId)" -ForegroundColor Gray
    $global:testEmail = ($registerBody | ConvertFrom-Json).email
} catch {
    Write-Host "   ❌ Register Error: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "   Details: $($_.ErrorDetails.Message)" -ForegroundColor Red
    }
}

# 3. Test Auth Service - Login
Write-Host "`n3. Testing Auth Service - Login..." -ForegroundColor Yellow
$loginBody = @{
    email = $global:testEmail
    password = "Test@123456"
} | ConvertTo-Json

try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/api/auth/login" -Method POST -Body $loginBody -ContentType "application/json" -UseBasicParsing
    $content = $response.Content | ConvertFrom-Json
    Write-Host "   ✅ Login: $($response.StatusCode)" -ForegroundColor Green
    Write-Host "   AccessToken: $($content.accessToken.Substring(0, [Math]::Min(50, $content.accessToken.Length)))..." -ForegroundColor Gray
    $global:token = $content.accessToken
} catch {
    Write-Host "   ❌ Login Error: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
}

# 4. Test User Service - Get Profile (NEW ENDPOINT /api/me)
Write-Host "`n4. Testing User Service - GET /api/me..." -ForegroundColor Yellow
try {
    $headers = @{
        "Authorization" = "Bearer $global:token"
    }
    $response = Invoke-WebRequest -Uri "http://localhost:5000/api/me" -Method GET -Headers $headers -UseBasicParsing
    Write-Host "   ✅ Get Profile: $($response.StatusCode)" -ForegroundColor Green
    $content = $response.Content | ConvertFrom-Json
    Write-Host "   Email: $($content.email)" -ForegroundColor Gray
} catch {
    $statusCode = $_.Exception.Response.StatusCode.value__
    Write-Host "   ❌ Get Profile Error: $statusCode" -ForegroundColor Red
    if ($statusCode -eq 404) {
        Write-Host "   Profile not found - Creating profile..." -ForegroundColor Yellow
        
        # Create profile first - POST to user-service directly
        $createBody = @{
            displayName = "Test User"
        } | ConvertTo-Json
        
        try {
            # Note: Creating profile via user-service POST /api endpoint
            $response = Invoke-WebRequest -Uri "http://localhost:5002/api" -Method POST -Headers $headers -Body $createBody -ContentType "application/json" -UseBasicParsing
            Write-Host "   ✅ Profile Created: $($response.StatusCode)" -ForegroundColor Green
            
            # Try get again through gateway
            $response = Invoke-WebRequest -Uri "http://localhost:5000/api/me" -Method GET -Headers $headers -UseBasicParsing
            Write-Host "   ✅ Get Profile (retry): $($response.StatusCode)" -ForegroundColor Green
            $content = $response.Content | ConvertFrom-Json
            Write-Host "   DisplayName: $($content.displayName)" -ForegroundColor Gray
        } catch {
            Write-Host "   ❌ Create Profile Error: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
        }
    }
}

# 5. Test User Service - Update Profile
Write-Host "`n5. Testing User Service - PUT /api/me..." -ForegroundColor Yellow
$updateBody = @{
    displayName = "Updated Test User"
} | ConvertTo-Json

try {
    $headers = @{
        "Authorization" = "Bearer $global:token"
    }
    $response = Invoke-WebRequest -Uri "http://localhost:5000/api/me" -Method PUT -Headers $headers -Body $updateBody -ContentType "application/json" -UseBasicParsing
    Write-Host "   ✅ Update Profile: $($response.StatusCode)" -ForegroundColor Green
    $content = $response.Content | ConvertFrom-Json
    Write-Host "   DisplayName: $($content.displayName)" -ForegroundColor Gray
} catch {
    Write-Host "   ❌ Update Profile Error: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "   Details: $($_.ErrorDetails.Message)" -ForegroundColor Red
    }
}

# 6. Test User Service - Get Library
Write-Host "`n6. Testing User Service - GET /api/me/library..." -ForegroundColor Yellow
try {
    $headers = @{
        "Authorization" = "Bearer $global:token"
    }
    $response = Invoke-WebRequest -Uri "http://localhost:5000/api/me/library" -Method GET -Headers $headers -UseBasicParsing
    Write-Host "   ✅ Get Library: $($response.StatusCode)" -ForegroundColor Green
    Write-Host "   Content: $($response.Content)" -ForegroundColor Gray
} catch {
    Write-Host "   ❌ Get Library Error: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
}

# 7. Test Catalog Service - Get Movies
Write-Host "`n7. Testing Catalog Service - GET /api/movies..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/api/movies" -Method GET -UseBasicParsing
    Write-Host "   ✅ Get Movies: $($response.StatusCode)" -ForegroundColor Green
    $content = $response.Content | ConvertFrom-Json
    if ($content) {
        Write-Host "   Movies count: $($content.Count)" -ForegroundColor Gray
    }
} catch {
    Write-Host "   ❌ Get Movies Error: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
}

# 8. Test Search Service - Search Movies
Write-Host "`n8. Testing Search Service - GET /api/search?query=test..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/api/search?query=test" -Method GET -UseBasicParsing
    Write-Host "   ✅ Search Movies: $($response.StatusCode)" -ForegroundColor Green
} catch {
    Write-Host "   ❌ Search Movies Error: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
}

# Summary
Write-Host "`n=== Test Summary ===" -ForegroundColor Cyan
Write-Host "All endpoint tests completed!" -ForegroundColor Green
Write-Host "`nKey Endpoints:" -ForegroundColor Yellow
Write-Host "  - API Gateway: http://localhost:5000/health" -ForegroundColor Gray
Write-Host "  - Auth: http://localhost:5000/api/auth/*" -ForegroundColor Gray
Write-Host "  - User: http://localhost:5000/api/me (NEW!)" -ForegroundColor Gray
Write-Host "  - Movies: http://localhost:5000/api/movies" -ForegroundColor Gray
Write-Host "  - Search: http://localhost:5000/api/search" -ForegroundColor Gray
