#!/bin/bash

# MovieHub Backend - Quick Start Script
# This script sets up a complete local development environment

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${GREEN}╔═══════════════════════════════════════╗${NC}"
echo -e "${GREEN}║   MovieHub Backend Quick Start       ║${NC}"
echo -e "${GREEN}╚═══════════════════════════════════════╝${NC}"
echo ""

# Check prerequisites
echo -e "${YELLOW}[1/8] Checking prerequisites...${NC}"

command -v docker >/dev/null 2>&1 || {
    echo -e "${RED}✗ Docker is not installed. Please install Docker Desktop.${NC}"
    exit 1
}
echo -e "${GREEN}✓ Docker found${NC}"

command -v docker-compose >/dev/null 2>&1 || {
    echo -e "${RED}✗ Docker Compose is not installed.${NC}"
    exit 1
}
echo -e "${GREEN}✓ Docker Compose found${NC}"

command -v dotnet >/dev/null 2>&1 || {
    echo -e "${RED}✗ .NET 8 SDK is not installed. Please install .NET 8 SDK.${NC}"
    exit 1
}
echo -e "${GREEN}✓ .NET 8 SDK found${NC}"

# Check .NET version
DOTNET_VERSION=$(dotnet --version | cut -d. -f1)
if [ "$DOTNET_VERSION" -lt 8 ]; then
    echo -e "${RED}✗ .NET 8.0 or higher is required. Found version $(dotnet --version)${NC}"
    exit 1
fi
echo -e "${GREEN}✓ .NET version is compatible${NC}"

# Start infrastructure services
echo ""
echo -e "${YELLOW}[2/8] Starting infrastructure services (PostgreSQL, Kafka, Redis)...${NC}"
docker-compose up -d postgres kafka zookeeper redis prometheus grafana

echo -e "${GREEN}✓ Infrastructure services started${NC}"
echo "Waiting for services to be ready (30 seconds)..."
sleep 30

# Check if PostgreSQL is ready
echo -e "${YELLOW}[3/8] Verifying PostgreSQL connection...${NC}"
until docker exec moviehub-postgres pg_isready -U postgres >/dev/null 2>&1; do
    echo "Waiting for PostgreSQL..."
    sleep 2
done
echo -e "${GREEN}✓ PostgreSQL is ready${NC}"

# Check if Kafka is ready
echo -e "${YELLOW}[4/8] Verifying Kafka connection...${NC}"
until docker exec moviehub-kafka kafka-broker-api-versions --bootstrap-server localhost:9092 >/dev/null 2>&1; do
    echo "Waiting for Kafka..."
    sleep 2
done
echo -e "${GREEN}✓ Kafka is ready${NC}"

# Restore NuGet packages
echo ""
echo -e "${YELLOW}[5/8] Restoring NuGet packages...${NC}"
dotnet restore MovieHub.sln
echo -e "${GREEN}✓ Packages restored${NC}"

# Build solution
echo ""
echo -e "${YELLOW}[6/8] Building solution...${NC}"
dotnet build MovieHub.sln --configuration Release --no-restore
echo -e "${GREEN}✓ Solution built successfully${NC}"

# Run database migrations
echo ""
echo -e "${YELLOW}[7/8] Running database migrations...${NC}"

# Auth Service
echo "Migrating Auth Service database..."
cd src/Services/Auth/MovieHub.Services.Auth.API
dotnet ef database update --project ../MovieHub.Services.Auth.Infrastructure --no-build >/dev/null 2>&1 || true
cd ../../../../

echo -e "${GREEN}✓ Database migrations completed${NC}"

# Create startup script
echo ""
echo -e "${YELLOW}[8/8] Creating startup scripts...${NC}"

cat > start-services.sh <<'EOF'
#!/bin/bash

echo "Starting MovieHub services..."

# Function to run service in background
start_service() {
    local service_path=$1
    local service_name=$2
    local port=$3
    
    echo "Starting $service_name on port $port..."
    cd "$service_path"
    dotnet run --no-build --urls "http://localhost:$port" > "../../../logs/$service_name.log" 2>&1 &
    echo $! > "../../../logs/$service_name.pid"
    cd - > /dev/null
}

# Create logs directory
mkdir -p logs

# Start services
start_service "src/Services/Auth/MovieHub.Services.Auth.API" "auth-service" 5001
sleep 2
start_service "src/Services/User/MovieHub.Services.User.API" "user-service" 5002
sleep 2
start_service "src/Services/MovieMetadata/MovieHub.Services.MovieMetadata.API" "movie-metadata-service" 5003

echo ""
echo "All services started!"
echo ""
echo "Available endpoints:"
echo "  Auth Service:          http://localhost:5001/swagger"
echo "  User Service:          http://localhost:5002/swagger"
echo "  Movie Metadata Service: http://localhost:5003/swagger"
echo "  Prometheus:            http://localhost:9090"
echo "  Grafana:               http://localhost:3000 (admin/admin)"
echo ""
echo "Service logs are in the ./logs directory"
echo "To stop services, run: ./stop-services.sh"
EOF

cat > stop-services.sh <<'EOF'
#!/bin/bash

echo "Stopping MovieHub services..."

if [ -d "logs" ]; then
    for pidfile in logs/*.pid; do
        if [ -f "$pidfile" ]; then
            pid=$(cat "$pidfile")
            if kill -0 "$pid" 2>/dev/null; then
                echo "Stopping process $pid..."
                kill "$pid"
            fi
            rm "$pidfile"
        fi
    done
fi

echo "All services stopped!"
EOF

chmod +x start-services.sh
chmod +x stop-services.sh

echo -e "${GREEN}✓ Startup scripts created${NC}"

# Summary
echo ""
echo -e "${GREEN}╔═══════════════════════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║              Setup Complete! 🎉                           ║${NC}"
echo -e "${GREEN}╚═══════════════════════════════════════════════════════════╝${NC}"
echo ""
echo -e "${YELLOW}Infrastructure Services:${NC}"
echo "  PostgreSQL:  localhost:5432 (user: postgres, pass: postgres)"
echo "  Kafka:       localhost:9093"
echo "  Redis:       localhost:6379"
echo "  Prometheus:  http://localhost:9090"
echo "  Grafana:     http://localhost:3000 (admin/admin)"
echo ""
echo -e "${YELLOW}Next Steps:${NC}"
echo "  1. Start all services:"
echo "     ${GREEN}./start-services.sh${NC}"
echo ""
echo "  2. Access the APIs:"
echo "     ${GREEN}http://localhost:5001/swagger${NC} - Auth Service"
echo "     ${GREEN}http://localhost:5002/swagger${NC} - User Service"
echo ""
echo "  3. Test with curl:"
echo "     ${GREEN}curl http://localhost:5001/health${NC}"
echo ""
echo "  4. Stop all services:"
echo "     ${GREEN}./stop-services.sh${NC}"
echo ""
echo -e "${YELLOW}Useful Commands:${NC}"
echo "  View logs:       ${GREEN}tail -f logs/auth-service.log${NC}"
echo "  Docker logs:     ${GREEN}docker-compose logs -f${NC}"
echo "  Stop Docker:     ${GREEN}docker-compose down${NC}"
echo ""
echo "For more information, see README.md"
echo ""
