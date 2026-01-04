# Quick API Gateway Route Test
Write-Host "Testing API Gateway Routes..." -ForegroundColor Cyan

# Register a new user
Write-Host "`n1. Register new user..." -ForegroundColor Yellow
$registerBody = @{
    username = "quicktest$(Get-Random -Maximum 9999)"
    email = "quicktest$(Get-Random -Maximum 9999)@example.com"
    password = "Test@123456"
} | ConvertTo-Json

$response = Invoke-WebRequest -Uri "http://localhost:5000/api/auth/register" -Method POST -Body $registerBody -ContentType "application/json" -UseBasicParsing
Write-Host "Register Status: $($response.StatusCode)" -ForegroundColor Green
$testEmail = ($registerBody | ConvertFrom-Json).email

# Login
Write-Host "`n2. Login..." -ForegroundColor Yellow
$loginBody = @{
    email = $testEmail
    password = "Test@123456"
} | ConvertTo-Json

$response = Invoke-WebRequest -Uri "http://localhost:5000/api/auth/login" -Method POST -Body $loginBody -ContentType "application/json" -UseBasicParsing
$token = ($response.Content | ConvertFrom-Json).accessToken
Write-Host "Login Status: $($response.StatusCode)" -ForegroundColor Green
Write-Host "Token: $($token.Substring(0, 30))..." -ForegroundColor Gray

# Test direct to user-service
Write-Host "`n3. Test DIRECT to user-service (port 5002)..." -ForegroundColor Yellow
$headers = @{
    "Authorization" = "Bearer $token"
}

try {
    $response = Invoke-WebRequest -Uri "http://localhost:5002/api/me" -Method GET -Headers $headers -UseBasicParsing
    Write-Host "   ✅ Direct to user-service: $($response.StatusCode)" -ForegroundColor Green
    Write-Host "   Content: $($response.Content)" -ForegroundColor Gray
} catch {
    Write-Host "   Status: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Yellow
    if ($_.Exception.Response.StatusCode.value__ -eq 404) {
        Write-Host "   Creating profile..." -ForegroundColor Gray
        $createBody = @{ displayName = "Quick Test" } | ConvertTo-Json
        try {
            $response = Invoke-WebRequest -Uri "http://localhost:5002/api" -Method POST -Headers $headers -Body $createBody -ContentType "application/json" -UseBasicParsing
            Write-Host "   ✅ Profile created: $($response.StatusCode)" -ForegroundColor Green
            
            # Try again
            $response = Invoke-WebRequest -Uri "http://localhost:5002/api/me" -Method GET -Headers $headers -UseBasicParsing
            Write-Host "   ✅ Get profile (retry): $($response.StatusCode)" -ForegroundColor Green
        } catch {
            Write-Host "   ❌ Error: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
        }
    }
}

# Test through API Gateway
Write-Host "`n4. Test THROUGH API Gateway (port 5000)..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/api/me" -Method GET -Headers $headers -UseBasicParsing
    Write-Host "   ✅ Through gateway: $($response.StatusCode)" -ForegroundColor Green
    Write-Host "   Content: $($response.Content)" -ForegroundColor Gray
} catch {
    Write-Host "   ❌ Error: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
    Write-Host "   Message: $($_.ErrorDetails.Message)" -ForegroundColor Red
}

Write-Host "`nTest complete!" -ForegroundColor Cyan
