# 🎬 MovieHub Microservices

A modern .NET 10 microservices architecture for a movie streaming platform with production-ready DevOps configuration following 12-Factor App methodology.

> **Quick Start**: Run `.\setup-env.ps1` (Windows) or `./setup-env.sh` (Linux/Mac), then `docker-compose up -d`

## 📋 Table of Contents
- [Architecture](#-architecture)
- [Service Overview](#-service-overview)
- [Quick Start](#-quick-start)
- [Configuration](#-configuration)
- [Development](#-local-development)
- [Docker](#-docker)
- [Kubernetes](#-kubernetes-deployment)
- [API Documentation](#-api-documentation-swagger)
- [Troubleshooting](#-troubleshooting)
- [Security](#-security)
- [DevOps Best Practices](#-devops-best-practices)

## 🏗️ Architecture

```
┌─────────────────┐
│   Frontend      │
│   (Next.js)     │
└────────┬────────┘
         │
┌────────▼────────────────────────────────────────┐
│          API Gateway (YARP)                     │
│          Port: 5000                             │
└─┬────────┬──────────┬──────────┬───────────────┘
  │        │          │          │
  │        │          │          │
┌─▼──────┐ │ ┌───────▼┐ ┌──────▼─┐ ┌──────────▼─┐
│ Auth   │ │ │ User   │ │Catalog │ │   Search   │
│Service │ │ │Service │ │Service │ │  Service   │
│:5001   │ │ │ :5002  │ │ :5003  │ │   :5004    │
└─┬──────┘ │ └───┬────┘ └───┬────┘ └─────┬──────┘
  │        │     │          │            │
┌─▼────────▼─────▼──────────▼────┐   ┌───▼─────────┐
│     PostgreSQL Databases        │   │Elasticsearch│
│  (Auth, User, Catalog DBs)      │   │    :9200    │
└─────────────────────────────────┘   └─────────────┘
```

## 📦 Service Overview

| Service | HTTP Port | HTTPS Port | Database Port | Purpose | Swagger |
|---------|-----------|------------|---------------|---------|---------|
| **API Gateway** | 5000 | 5443 | N/A | Reverse proxy, routing, load balancing | [/swagger](http://localhost:5000/swagger) |
| **Auth Service** | 5001 | 5444 | 5432 | Authentication, JWT token generation | [/swagger](http://localhost:5001/swagger) |
| **User Service** | 5002 | 5445 | 5434 | User profile management | [/swagger](http://localhost:5002/swagger) |
| **Catalog Service** | 5003 | 5446 | 5435 | Movie catalog CRUD operations | [/swagger](http://localhost:5003/swagger) |
| **Search Service** | 5004 | 5447 | 9200 (ES) | Full-text search, recommendations | [/swagger](http://localhost:5004/swagger) |

## 🚀 Quick Start

### Prerequisites
- Docker & Docker Compose
- .NET 10 SDK (for local development)
- PowerShell (Windows) or Bash (Linux/Mac)

### 1. Initial Setup
```bash
# Windows
.\setup-env.ps1

# Linux/Mac
chmod +x setup-env.sh
./setup-env.sh
```

This creates `.env` files for all services with development defaults.

### 2. Start All Services
```bash
# Start everything (databases + microservices)
docker-compose up -d

# Check status
docker-compose ps

# View logs
docker-compose logs -f
```

### 3. Verify Services
```bash
# Health checks
curl http://localhost:5000/health  # API Gateway
curl http://localhost:5001/health  # Auth Service
curl http://localhost:5002/health  # User Service
curl http://localhost:5003/health  # Catalog Service
curl http://localhost:5004/health  # Search Service

# Swagger UI (API Documentation)
# Open in browser:
http://localhost:5000/swagger  # API Gateway
http://localhost:5001/swagger  # Auth Service
http://localhost:5002/swagger  # User Service
http://localhost:5003/swagger  # Catalog Service
http://localhost:5004/swagger  # Search Service
```

### 4. Stop Services
```bash
docker-compose down           # Stop and remove containers
docker-compose down -v        # Also remove volumes (deletes data)
```

## 🔧 Local Development

### Run Single Service Locally
```bash
cd auth-service/AuthService.API
dotnet restore
dotnet run
```

The service will load configuration from:
1. `.env` file (highest priority)
2. `appsettings.Development.json`
3. `appsettings.json` (lowest priority)

### Hot Reload
```bash
dotnet watch run
```

## 🔐 Configuration

### Environment Variable System

This project uses **12-Factor App** methodology with environment variables for all configuration:
- ✅ **Secrets never in source control** - All sensitive data in `.env` files
- ✅ **Multi-environment ready** - Same code, different `.env` per environment
- ✅ **Works everywhere** - Local dev, Docker, Kubernetes
- ✅ **No configuration drift** - Consistent approach across all services

### .NET Environment Variable Naming Convention

.NET uses **double underscores (`__`)** for nested configuration:

| appsettings.json | Environment Variable | Example |
|------------------|---------------------|---------|
| `ConnectionStrings:DefaultConnection` | `ConnectionStrings__DefaultConnection` | `Host=db;Port=5432;...` |
| `Jwt:Secret` | `Jwt__Secret` | `your-secret-here` |
| `Logging:LogLevel:Default` | `Logging__LogLevel__Default` | `Information` |
| `Elasticsearch:Url` | `Elasticsearch__Url` | `http://elasticsearch:9200` |

**Rule**: Replace `:` with `__` (double underscore)

### Critical Settings

#### JWT Configuration (Must Match Across Services)
```bash
Jwt__Secret=YourSuperSecretKeyForJWTTokenGeneration123!@#
Jwt__Issuer=MovieHub.AuthService
Jwt__Audience=MovieHub.Client
```

#### Database Connections
```bash
# Pattern
ConnectionStrings__DefaultConnection=Host=db-host;Port=5432;Database=dbname;Username=user;Password=pass

# Example (Docker)
ConnectionStrings__DefaultConnection=Host=auth-db;Port=5432;Database=authdb;Username=postgres;Password=postgres
```

### Configuration Files

| File | Purpose | Commit to Git? | Contains |
|------|---------|---------------|----------|
| `.env` | Development values with **real secrets** | ❌ **NO** | Actual passwords, JWT secrets |
| `.env.example` | Template **without secrets** | ✅ **YES** | Placeholder structure |
| `appsettings.json` | Placeholder structure only | ✅ **YES** | Empty strings, no secrets |
| `appsettings.Development.json` | Local dev overrides (non-sensitive) | ✅ **YES** | Logging levels, dev URLs |

### Configuration Priority (Highest to Lowest)

1. **Environment Variables** ⬅️ Highest priority (Docker, K8s, OS)
2. `.env` file (loaded by .NET or Docker)
3. `appsettings.{Environment}.json`
4. `appsettings.json` ⬅️ Lowest priority

**This means**: Environment variables always win and can override everything else.

## 📚 Documentation

- **This README** - Complete reference for everything
- **Setup Scripts**: `setup-env.ps1` (Windows) | `setup-env.sh` (Linux/Mac)
- **Postman Collection**: `Request-postman/MovieHub.postman_collection.json`
- **Service READMEs**: Each service has specific documentation

## 🌐 API Documentation (Swagger)

All services include Swagger UI for interactive API documentation and testing:

| Service | Swagger URL | Authentication |
|---------|-------------|----------------|
| **API Gateway** | http://localhost:5000/swagger | None (proxy layer) |
| **Auth Service** | http://localhost:5001/swagger | JWT (after login) |
| **User Service** | http://localhost:5002/swagger | JWT required |
| **Catalog Service** | http://localhost:5003/swagger | JWT required |
| **Search Service** | http://localhost:5004/swagger | None (read-only) |

### How to Use Swagger with Authentication

1. **Get JWT Token**:
   - Go to http://localhost:5001/swagger
   - Use `/api/auth/login` endpoint
   - Copy the token from response

2. **Authorize in Swagger**:
   - Click the 🔒 **Authorize** button (top right)
   - Enter: `Bearer YOUR_TOKEN_HERE`
   - Click **Authorize**

3. **Test Protected Endpoints**:
   - All requests now include the JWT token automatically

## 🏗️ Project Structure

```
services/
├── .env                          # Docker Compose environment variables
├── docker-compose.yml            # Service orchestration
├── setup-env.ps1                 # Windows setup script
├── setup-env.sh                  # Linux/Mac setup script
│
├── api-gateway/
│   ├── .env                      # Service-specific config (not in git)
│   ├── .env.example              # Template (in git)
│   ├── appsettings.json          # Placeholder config
│   ├── Program.cs
│   ├── Dockerfile
│   └── Controllers/
│
├── auth-service/
│   └── AuthService.API/
│       ├── .env
│       ├── .env.example
│       ├── appsettings.json
│       ├── Program.cs
│       └── Controllers/
│
├── user-service/
│   └── UserService.API/
│       └── [same structure]
│
├── catalog-service/
│   └── CatalogService.API/
│       └── [same structure]
│
└── movie-search-service/
    └── MovieSearchService.API/
        └── [same structure]
```

## 🔒 Security

### Production Security Checklist

Before deploying to production, complete this checklist:

- [ ] ✅ **Generate strong JWT secrets** (min 32 characters)
  ```bash
  # Linux/Mac/WSL
  openssl rand -base64 32
  
  # PowerShell
  [Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Maximum 256 }))
  ```
- [ ] ✅ **Different secrets per environment** (dev ≠ staging ≠ production)
- [ ] ✅ **Database passwords changed** from defaults
- [ ] ✅ **`.env` files in `.gitignore`** (verify with `git status`)
- [ ] ✅ **HTTPS enabled** in production
- [ ] ✅ **Secret management system** (Azure Key Vault, AWS Secrets Manager, HashiCorp Vault)
- [ ] ✅ **Secrets rotation policy** established
- [ ] ✅ **Access logs monitored** for suspicious activity
- [ ] ✅ **JWT secret identical** across all services
- [ ] ✅ **No secrets in code** or appsettings.json

### Secret Management for Production

**Development**: `.env` files (gitignored)  
**Staging/Production**: Use enterprise secret management

#### Azure Key Vault Example
```bash
# Store secrets in Azure Key Vault
az keyvault secret set --vault-name MovieHubVault --name JwtSecret --value "your-secret"

# Reference in deployment
# Environment variable loaded from Key Vault
```

#### Kubernetes Secrets Example
```bash
# Create secret
kubectl create secret generic jwt-secret \
  --from-literal=Jwt__Secret='production-secret-here'

# Reference in deployment
envFrom:
  - secretRef:
      name: jwt-secret
```

### Security Best Practices Applied

✅ **12-Factor App** - Config in environment, never in code  
✅ **Principle of Least Privilege** - Services get only needed secrets  
✅ **Defense in Depth** - Multiple layers (network, auth, encryption)  
✅ **Secrets Never Logged** - Environment variables not in logs  
✅ **Immutable Infrastructure** - Secrets injected at runtime

## 🐳 Docker

### Build Individual Service
```bash
cd auth-service/AuthService.API
docker build -t moviehub-auth-service .
docker run --env-file .env -p 5001:5001 moviehub-auth-service
```

### Rebuild All Services
```bash
docker-compose build --no-cache
docker-compose up -d
```

### View Logs
```bash
docker-compose logs -f auth-service
docker-compose logs -f --tail=100
```

### Database Access
```bash
# Connect to Auth DB
docker exec -it moviehub-auth-db psql -U postgres -d authdb

# Connect to Elasticsearch
curl http://localhost:9200/_cluster/health?pretty
```

## ☸️ Kubernetes Deployment

### Create ConfigMaps
```bash
kubectl create configmap auth-config \
  --from-env-file=auth-service/AuthService.API/.env

kubectl create configmap user-config \
  --from-env-file=user-service/UserService.API/.env
```

### Create Secrets
```bash
kubectl create secret generic jwt-secret \
  --from-literal=Jwt__Secret='your-production-secret'

kubectl create secret generic db-secrets \
  --from-literal=DB_PASSWORD='production-db-password'
```

### Deployment Example
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: auth-service
spec:
  template:
    spec:
      containers:
      - name: auth-service
        image: moviehub/auth-service:latest
        envFrom:
        - configMapRef:
            name: auth-config
        - secretRef:
            name: jwt-secret
        - secretRef:
            name: db-secrets
```

## 🧪 Testing

### Run Tests
```bash
cd auth-service
dotnet test
```

### API Testing
Use the Postman collection:
```bash
services/Request-postman/MovieHub.postman_collection.json
```

### Health Checks
```bash
# All services
curl http://localhost:5000/health
curl http://localhost:5001/health
curl http://localhost:5002/health
curl http://localhost:5003/health
curl http://localhost:5004/health

# Databases
docker-compose ps | grep healthy
```

## 🐛 Troubleshooting

### Common Issues & Quick Fixes

#### Services Won't Start
```bash
# Check logs
docker-compose logs [service-name]

# Check .env files exist
ls -la api-gateway/.env
ls -la auth-service/AuthService.API/.env

# Verify configuration
docker-compose config

# Rebuild if needed
docker-compose build --no-cache
docker-compose up -d
```

#### Environment Variables Not Loading
**Problem**: Service can't find configuration  
**Solutions**:
- ✅ Check `.env` file exists in correct directory
- ✅ Verify variable names use `__` not `:`
- ✅ Ensure no spaces around `=` in `.env` files
- ✅ Restart service after changing `.env`
- ✅ For docker-compose: `docker-compose down && docker-compose up -d`

#### Database Connection Errors
**Problem**: Service can't connect to database  
**Solutions**:
```bash
# Verify database is running
docker-compose ps | grep db

# Check database is healthy
docker-compose ps

# Test connection directly
docker exec -it moviehub-auth-db psql -U postgres -d authdb -c '\dt'

# For Docker: Use service name (e.g., auth-db) not localhost
ConnectionStrings__DefaultConnection=Host=auth-db;Port=5432;...
```

#### JWT Validation Fails
**Problem**: Token validation errors between services  
**Solutions**:
```bash
# All services MUST use identical JWT secret
docker exec moviehub-auth-service printenv | grep Jwt__Secret
docker exec moviehub-user-service printenv | grep Jwt__Secret
docker exec moviehub-catalog-service printenv | grep Jwt__Secret

# These MUST match exactly!

# Also verify Issuer and Audience match:
Jwt__Issuer=MovieHub.AuthService
Jwt__Audience=MovieHub.Client
```

#### Port Already in Use
**Problem**: Cannot bind to port  
**Solutions**:
```bash
# Find process using port (Windows)
netstat -ano | findstr :5001

# Find process using port (Linux/Mac)
lsof -i :5001

# Kill the process or change port in .env file
ASPNETCORE_HTTP_PORT=5011
```

#### Docker Build Fails
**Problem**: Dockerfile errors  
**Solutions**:
```bash
# Clear Docker build cache
docker system prune -a

# Rebuild specific service
docker-compose build --no-cache auth-service

# Check Dockerfile exists
ls -la auth-service/Dockerfile
ls -la user-service/Dockerfile
```

## 📊 Monitoring

### Application Logs
```bash
docker-compose logs -f --tail=100 [service-name]
```

### Resource Usage
```bash
docker stats
```

### Database Size
```bash
docker exec moviehub-auth-db psql -U postgres -d authdb -c "SELECT pg_size_pretty(pg_database_size('authdb'));"
```

## 🔄 CI/CD Integration

### GitHub Actions Example
```yaml
name: Deploy to Production

on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Deploy Services
        env:
          JWT_SECRET: ${{ secrets.JWT_SECRET }}
          DB_PASSWORD: ${{ secrets.DB_PASSWORD }}
          ELASTIC_PASSWORD: ${{ secrets.ELASTIC_PASSWORD }}
        run: |
          docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

### Azure DevOps Pipeline
```yaml
trigger:
  - main

pool:
  vmImage: 'ubuntu-latest'

variables:
  - group: MovieHub-Production  # Link to Key Vault

steps:
  - task: Docker@2
    inputs:
      command: 'up'
      dockerComposeFile: 'docker-compose.yml'
      dockerComposeFileArgs: '-f docker-compose.prod.yml'
    env:
      JWT_SECRET: $(JwtSecret)
      DB_PASSWORD: $(DbPassword)
```

## 🎓 DevOps Best Practices

This project implements industry-standard DevOps practices:

### 1. **12-Factor App Methodology** ✅
- **III. Config** - Store config in environment (not code)
- **VI. Processes** - Execute app as stateless processes
- **VIII. Concurrency** - Scale via process model
- **IX. Disposability** - Fast startup/shutdown
- **XI. Logs** - Treat logs as event streams

### 2. **Infrastructure as Code** ✅
- `docker-compose.yml` defines entire infrastructure
- Version controlled, reproducible, testable
- Same structure for dev/staging/production

### 3. **Configuration Management** ✅
- Environment-specific variables separate from code
- No configuration drift between environments
- Easy secret rotation without code changes

### 4. **Security by Design** ✅
- Secrets never in source control
- Environment isolation (dev/staging/prod)
- Ready for enterprise secret management
- Audit trail for configuration changes

### 5. **Immutable Infrastructure** ✅
- Docker images built once, deployed everywhere
- Configuration injected at runtime
- No manual server changes

### 6. **Observability** ✅
- Health checks for all services
- Structured logging with configurable levels
- Ready for centralized monitoring (ELK, Prometheus)

### 7. **Developer Experience** ✅
- One-command setup (`setup-env.ps1`)
- One-command run (`docker-compose up -d`)
- Clear documentation and error messages
- Fast feedback loops

## 📈 What Makes This Production-Ready

| Aspect | Implementation | Benefit |
|--------|---------------|---------|
| **Configuration** | Environment variables, .env files | No secrets in code, multi-environment |
| **Secrets** | Azure Key Vault / K8s Secrets ready | Enterprise-grade security |
| **Deployment** | Docker + docker-compose | Consistent across environments |
| **Documentation** | Complete README, Swagger | Easy onboarding, API discovery |
| **Health Checks** | All services + databases | Auto-recovery, monitoring ready |
| **Networking** | Service discovery, named networks | Microservice communication |
| **Persistence** | Named volumes | Data survives restarts |
| **Logging** | Configurable levels per environment | Debug dev, optimized prod |
| **API Docs** | Swagger UI on all services | Interactive testing, developer UX |
| **Scalability** | Stateless services, external state | Horizontal scaling ready |

## 📞 Support

### Quick Help
- **Can't start services?** → See [Troubleshooting](#-troubleshooting)
- **Configuration issues?** → See [Configuration](#-configuration)
- **Need examples?** → Check service `.env.example` files
- **API questions?** → Visit Swagger UI at `/swagger` endpoints

### Useful Commands Reference

```bash
# Setup
.\setup-env.ps1              # Windows setup
./setup-env.sh               # Linux/Mac setup

# Docker Compose
docker-compose up -d         # Start all services
docker-compose ps            # Check status
docker-compose logs -f       # View all logs
docker-compose logs -f auth-service  # View specific service
docker-compose down          # Stop all services
docker-compose down -v       # Stop + remove volumes (data deleted!)
docker-compose build --no-cache  # Rebuild images
docker-compose restart auth-service  # Restart specific service

# Verification
curl http://localhost:5000/health  # Check health
docker-compose ps | grep healthy   # Check healthy services
docker-compose config              # Validate compose file

# Database Access
docker exec -it moviehub-auth-db psql -U postgres -d authdb
docker exec -it moviehub-user-db psql -U postgres -d userdb
docker exec -it moviehub-catalog-db psql -U postgres -d catalogdb

# Elasticsearch
curl http://localhost:9200/_cluster/health?pretty
curl http://localhost:9200/movies/_search?pretty

# Debugging
docker exec -it moviehub-auth-service printenv | grep Jwt
docker stats                 # Resource usage
docker system prune -a       # Clean up (careful!)
```

## 🎯 Quick Reference Card

### Service Ports
| Service | HTTP | HTTPS | DB | Swagger |
|---------|------|-------|----|---------| 
| Gateway | 5000 | 5443 | - | [:5000/swagger](http://localhost:5000/swagger) |
| Auth | 5001 | 5444 | 5432 | [:5001/swagger](http://localhost:5001/swagger) |
| User | 5002 | 5445 | 5434 | [:5002/swagger](http://localhost:5002/swagger) |
| Catalog | 5003 | 5446 | 5435 | [:5003/swagger](http://localhost:5003/swagger) |
| Search | 5004 | 5447 | 9200 | [:5004/swagger](http://localhost:5004/swagger) |

### Critical Environment Variables
```bash
# JWT (MUST be identical across all services)
Jwt__Secret=YOUR_32_CHAR_SECRET
Jwt__Issuer=MovieHub.AuthService
Jwt__Audience=MovieHub.Client

# Database Connection Pattern
ConnectionStrings__DefaultConnection=Host=db-host;Port=5432;Database=dbname;Username=user;Password=pass

# Service URLs (Docker)
AuthService__BaseUrl=http://auth-service:5001
SearchService__BaseUrl=http://search-service:5004
```

### File Locations
```
services/
├── .env                                 # Main compose vars
├── docker-compose.yml                   # Orchestration
├── api-gateway/.env                     # Gateway config
├── auth-service/AuthService.API/.env    # Auth config
├── user-service/UserService.API/.env    # User config
├── catalog-service/CatalogService.API/.env  # Catalog config
└── movie-search-service/MovieSearchService.API/.env  # Search config
```

## 🤝 Contributing

1. Create `.env` from `.env.example`
2. Run `docker-compose up -d`
3. Make changes
4. Test locally
5. Submit PR

## 📄 License

[Your License Here]

---

**Tech Stack**: .NET 10, PostgreSQL, Elasticsearch, Docker, YARP
**Architecture**: Microservices, API Gateway Pattern, JWT Authentication
**DevOps**: Docker Compose, Environment Variables, Health Checks
