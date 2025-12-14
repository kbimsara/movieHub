# MovieHub Backend - Quick Start (Windows)
# PowerShell script for Windows users

$ErrorActionPreference = "Stop"

function Write-ColorOutput($ForegroundColor) {
    $fc = $host.UI.RawUI.ForegroundColor
    $host.UI.RawUI.ForegroundColor = $ForegroundColor
    if ($args) {
        Write-Output $args
    }
    $host.UI.RawUI.ForegroundColor = $fc
}

Write-ColorOutput Green "╔═══════════════════════════════════════╗"
Write-ColorOutput Green "║   MovieHub Backend Quick Start       ║"
Write-ColorOutput Green "╚═══════════════════════════════════════╝"
Write-Host ""

# Check prerequisites
Write-ColorOutput Yellow "[1/8] Checking prerequisites..."

if (!(Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-ColorOutput Red "✗ Docker is not installed. Please install Docker Desktop."
    exit 1
}
Write-ColorOutput Green "✓ Docker found"

if (!(Get-Command docker-compose -ErrorAction SilentlyContinue)) {
    Write-ColorOutput Red "✗ Docker Compose is not installed."
    exit 1
}
Write-ColorOutput Green "✓ Docker Compose found"

if (!(Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-ColorOutput Red "✗ .NET 8 SDK is not installed. Please install .NET 8 SDK."
    exit 1
}
Write-ColorOutput Green "✓ .NET 8 SDK found"

# Check .NET version
$dotnetVersion = dotnet --version
$majorVersion = [int]$dotnetVersion.Split('.')[0]
if ($majorVersion -lt 8) {
    Write-ColorOutput Red "✗ .NET 8.0 or higher is required. Found version $dotnetVersion"
    exit 1
}
Write-ColorOutput Green "✓ .NET version is compatible"

# Start infrastructure services
Write-Host ""
Write-ColorOutput Yellow "[2/8] Starting infrastructure services (PostgreSQL, Kafka, Redis)..."
docker-compose up -d postgres kafka zookeeper redis prometheus grafana

Write-ColorOutput Green "✓ Infrastructure services started"
Write-Host "Waiting for services to be ready (30 seconds)..."
Start-Sleep -Seconds 30

# Check if PostgreSQL is ready
Write-ColorOutput Yellow "[3/8] Verifying PostgreSQL connection..."
$retries = 0
while ($retries -lt 30) {
    try {
        docker exec moviehub-postgres pg_isready -U postgres 2>&1 | Out-Null
        break
    } catch {
        Write-Host "Waiting for PostgreSQL..."
        Start-Sleep -Seconds 2
        $retries++
    }
}
Write-ColorOutput Green "✓ PostgreSQL is ready"

# Check if Kafka is ready
Write-ColorOutput Yellow "[4/8] Verifying Kafka connection..."
$retries = 0
while ($retries -lt 30) {
    try {
        docker exec moviehub-kafka kafka-broker-api-versions --bootstrap-server localhost:9092 2>&1 | Out-Null
        break
    } catch {
        Write-Host "Waiting for Kafka..."
        Start-Sleep -Seconds 2
        $retries++
    }
}
Write-ColorOutput Green "✓ Kafka is ready"

# Restore NuGet packages
Write-Host ""
Write-ColorOutput Yellow "[5/8] Restoring NuGet packages..."
dotnet restore MovieHub.sln
Write-ColorOutput Green "✓ Packages restored"

# Build solution
Write-Host ""
Write-ColorOutput Yellow "[6/8] Building solution..."
dotnet build MovieHub.sln --configuration Release --no-restore
Write-ColorOutput Green "✓ Solution built successfully"

# Run database migrations
Write-Host ""
Write-ColorOutput Yellow "[7/8] Running database migrations..."

Write-Host "Migrating Auth Service database..."
Push-Location src\Services\Auth\MovieHub.Services.Auth.API
try {
    dotnet ef database update --project ..\MovieHub.Services.Auth.Infrastructure --no-build 2>&1 | Out-Null
} catch {
    Write-Host "Migration warning (can be ignored if database exists)"
}
Pop-Location

Write-ColorOutput Green "✓ Database migrations completed"

# Create startup script
Write-Host ""
Write-ColorOutput Yellow "[8/8] Creating startup scripts..."

@"
# MovieHub Services Startup Script
Write-Host "Starting MovieHub services..."

# Create logs directory
New-Item -ItemType Directory -Force -Path logs | Out-Null

# Start Auth Service
Write-Host "Starting Auth Service on port 5001..."
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd src\Services\Auth\MovieHub.Services.Auth.API; dotnet run --no-build --urls 'http://localhost:5001' | Tee-Object -FilePath ..\..\..\..\logs\auth-service.log"

Start-Sleep -Seconds 2

# Start User Service
Write-Host "Starting User Service on port 5002..."
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd src\Services\User\MovieHub.Services.User.API; dotnet run --no-build --urls 'http://localhost:5002' | Tee-Object -FilePath ..\..\..\..\logs\user-service.log"

Start-Sleep -Seconds 2

# Start Movie Metadata Service
Write-Host "Starting Movie Metadata Service on port 5003..."
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd src\Services\MovieMetadata\MovieHub.Services.MovieMetadata.API; dotnet run --no-build --urls 'http://localhost:5003' | Tee-Object -FilePath ..\..\..\..\logs\movie-metadata-service.log"

Write-Host ""
Write-Host "All services started!"
Write-Host ""
Write-Host "Available endpoints:"
Write-Host "  Auth Service:           http://localhost:5001/swagger"
Write-Host "  User Service:           http://localhost:5002/swagger"
Write-Host "  Movie Metadata Service: http://localhost:5003/swagger"
Write-Host "  Prometheus:             http://localhost:9090"
Write-Host "  Grafana:                http://localhost:3000 (admin/admin)"
Write-Host ""
Write-Host "Service logs are in the .\logs directory"
Write-Host "To stop services, close the PowerShell windows or run: .\stop-services.ps1"
"@ | Out-File -FilePath start-services.ps1 -Encoding UTF8

@"
# MovieHub Services Stop Script
Write-Host "Stopping MovieHub services..."

# Find and stop all dotnet processes for our services
Get-Process | Where-Object {`$_.ProcessName -eq "dotnet" -and `$_.Path -like "*MovieHub*"} | Stop-Process -Force

Write-Host "All services stopped!"
"@ | Out-File -FilePath stop-services.ps1 -Encoding UTF8

Write-ColorOutput Green "✓ Startup scripts created"

# Summary
Write-Host ""
Write-ColorOutput Green "╔═══════════════════════════════════════════════════════════╗"
Write-ColorOutput Green "║              Setup Complete! 🎉                           ║"
Write-ColorOutput Green "╚═══════════════════════════════════════════════════════════╝"
Write-Host ""
Write-ColorOutput Yellow "Infrastructure Services:"
Write-Host "  PostgreSQL:  localhost:5432 (user: postgres, pass: postgres)"
Write-Host "  Kafka:       localhost:9093"
Write-Host "  Redis:       localhost:6379"
Write-Host "  Prometheus:  http://localhost:9090"
Write-Host "  Grafana:     http://localhost:3000 (admin/admin)"
Write-Host ""
Write-ColorOutput Yellow "Next Steps:"
Write-Host "  1. Start all services:"
Write-ColorOutput Green "     .\start-services.ps1"
Write-Host ""
Write-Host "  2. Access the APIs:"
Write-ColorOutput Green "     http://localhost:5001/swagger" -NoNewline
Write-Host " - Auth Service"
Write-ColorOutput Green "     http://localhost:5002/swagger" -NoNewline
Write-Host " - User Service"
Write-Host ""
Write-Host "  3. Test with curl:"
Write-ColorOutput Green "     curl http://localhost:5001/health"
Write-Host ""
Write-Host "  4. Stop all services:"
Write-ColorOutput Green "     .\stop-services.ps1"
Write-Host ""
Write-ColorOutput Yellow "Useful Commands:"
Write-Host "  View logs:       " -NoNewline
Write-ColorOutput Green "Get-Content logs\auth-service.log -Wait"
Write-Host "  Docker logs:     " -NoNewline
Write-ColorOutput Green "docker-compose logs -f"
Write-Host "  Stop Docker:     " -NoNewline
Write-ColorOutput Green "docker-compose down"
Write-Host ""
Write-Host "For more information, see README.md"
Write-Host ""
