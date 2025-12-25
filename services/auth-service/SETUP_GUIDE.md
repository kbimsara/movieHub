# JWT Authentication Service - Setup Guide

## Project Structure (Clean Architecture)

```
AuthService.Domain/           ← Domain Entities (no dependencies)
  └── Entities/
      └── User.cs

AuthService.Application/      ← Business Logic (depends on Domain only)
  ├── DTOs/
  │   ├── RegisterRequestDto.cs
  │   ├── LoginRequestDto.cs
  │   ├── AuthResponseDto.cs
  │   └── TokenClaimsDto.cs
  ├── Interfaces/
  │   ├── IAuthService.cs
  │   ├── IUserRepository.cs
  │   ├── IJwtTokenGenerator.cs
  │   └── IPasswordHasher.cs
  ├── Services/
  │   └── AuthenticationService.cs
  ├── Exceptions/
  │   ├── UserAlreadyExistsException.cs
  │   └── InvalidCredentialsException.cs
  └── DependencyInjection.cs

AuthService.Infrastructure/   ← External Concerns (DB, JWT, BCrypt)
  ├── Persistence/
  │   ├── AuthDbContext.cs
  │   └── Repositories/
  │       └── UserRepository.cs
  ├── Security/
  │   ├── JwtSettings.cs
  │   ├── JwtTokenGenerator.cs
  │   └── BCryptPasswordHasher.cs
  └── DependencyInjection.cs

AuthService.API/              ← Web Entry Point
  ├── Controllers/
  │   └── AuthController.cs
  ├── Extensions/
  │   ├── AuthenticationExtensions.cs
  │   └── SwaggerExtensions.cs
  ├── Middlewares/
  │   └── ExceptionMiddleware.cs
  ├── Program.cs
  └── appsettings.json
```

## Setup Instructions

### 1. Start Database
```bash
cd f:\Github\movieHub\services\DB
docker-compose --env-file .env.local up -d
```

Verify database is running:
```bash
docker ps
```

### 2. Install EF Core Tools (if not already installed)
```bash
dotnet tool install --global dotnet-ef
```

### 3. Restore NuGet Packages
```bash
cd f:\Github\movieHub\services\auth-service\AuthService.API
dotnet restore
```

### 4. Create Database Migration
```bash
cd ..\AuthService.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ..\AuthService.API
dotnet ef database update --startup-project ..\AuthService.API
```

### 5. Run the Application
```bash
cd ..\AuthService.API
dotnet run
```

The API will start on `http://localhost:5294` (or check terminal output for actual port)

**Access Swagger UI:** Navigate to `http://localhost:5294/swagger` in your browser

### 6. Test the API

**Option 1: Using Swagger UI (Recommended)**
- Open `http://localhost:5294/swagger` in your browser
- Click on `/api/auth/register` endpoint
- Click "Try it out"
- Enter request body and execute

**Option 2: Using Postman or curl**

**Register a new user:**
```bash
POST http://localhost:5294/api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Test123!"
}
```

**Successful Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI3ZjkxMjM0NS02Nzg5LTEyMzQtNTY3OC0xMjM0NTY3ODkwYWIiLCJlbWFpbCI6InVzZXJAZXhhbXBsZS5jb20iLCJqdGkiOiJhYmNkMTIzNC01Njc4LTkwYWItY2RlZi0xMjM0NTY3ODkwYWIiLCJleHAiOjE3MzUyMzQ1Njd9.signature",
  "email": "user@example.com"
}
```

**Login:**
```bash
POST http://localhost:5294/api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Test123!"
}
```

**Successful Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "user@example.com"
}
```

**Error Responses:**
- `409 Conflict` - User already exists (during registration)
- `401 Unauthorized` - Invalid credentials (during login)

## Configuration (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=authdb;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "Secret": "YourSuperSecretKeyForJWTTokenGeneration123!@#",
    "Issuer": "AuthService",
    "Audience": "MovieHubUsers",
    "ExpiryMinutes": 60
  }
}
```

**Application Layer (AuthService.Application)**
- `DTOs/`: Data transfer objects for API communication
- `Interfaces/`: Contracts that Infrastructure must implement
- `AuthenticationService.cs`: Pure business logic (register, login)
- `Exceptions/`: Business-specific exceptions (UserAlreadyExistsException, InvalidCredentialsException)
- `DependencyInjection.cs`: Registers Application services
- NO database, JWT, or BCrypt code here (only interfaces)

**Infrastructure Layer (AuthService.Infrastructure)**
- `AuthDbContext.cs`: Entity Framework Core database context with PostgreSQL
- `UserRepository.cs`: PostgreSQL implementation of IUserRepository
- `JwtTokenGenerator.cs`: JWT token creation using System.IdentityModel.Tokens.Jwt
- `BCryptPasswordHasher.cs`: Password hashing using BCrypt.Net-Next
- `JwtSettings.cs`: Configuration model for JWT settings
- `DependencyInjection.cs`: Registers Infrastructure services (DbContext, repositories, security)tityModel.Tokens.Jwt
- `BCryptPasswordHasher.cs`: Password hashing using BCrypt.Net-Next
- `JwtSettings.cs`: Configuration model for JWT settings
- `DependencyInjection.cs`: Registers Infrastructure services (DbContext, repositories, security)
**Infrastructure Layer (AuthService.Infrastructure)**
- `AuthDbContext.cs`: Entity Framework Core database context
- `UserRepository.cs`: PostgreSQL implementation of IUserRepository
- `JwtTokenGenerator.cs`: JWT token creation using System.IdentityModel.Tokens.Jwt
- `BCryptPasswordHasher.cs`: Password hashing using BCrypt.Net
- Implements interfaces defined in Application layer

**API Layer (AuthService.API)**
- `AuthController.cs`: HTTP endpoints (no business logic)
- `Program.cs`: Dependency injection setup
- `ExceptionMiddleware.cs`: Global error handling
- `AuthenticationExtensions.cs`: JWT middleware configuration
- Only coordinates between layers, no business rules

## Key Design Decisions

1. **No Business Logic in API**: Controllers just call IAuthService
2. **No Direct EF Core in Application**: Uses IUserRepository abstraction
3. **JWT in Infrastructure**: Application doesn't know about JWT implementation
4. **BCrypt in Infrastructure**: Application just uses IPasswordHasher
5. **Domain Factory Pattern**: User.Create() encapsulates entity creation
6. **Custom Exceptions**: Better error handling than generic Exception
7. **DTO for JWT Claims**: Interface doesn't depend on User entity
 to test AuthenticationService
## Testing Strategy

- **Unit Tests**: Mock IUserRepository, IPasswordHasher, IJwtTokenGenerator to test AuthenticationService
- **Integration Tests**: Test against real PostgreSQL database with TestContainers
- **API Tests**: Use WebApplicationFactory for end-to-end tests

## Quick Start (All-in-One)

```bash
# 1. Start database
cd f:\Github\movieHub\services\DB
docker-compose --env-file .env.local up -d

# 2. Install EF tools (first time only)
dotnet tool install --global dotnet-ef

# 3. Navigate to Infrastructure
cd ..\auth-service\AuthService.Infrastructure

# 4. Create and apply migration
dotnet ef migrations add InitialCreate --startup-project ..\AuthService.API
dotnet ef database update --startup-project ..\AuthService.API

# 5. Run the API
cd ..\AuthService.API
dotnet run

# 6. Open Swagger in browser
# http://localhost:5294/swagger
```
## Endpoints

| Method | Endpoint | Description | Request Body | Success Response |
|--------|----------|-------------|--------------|------------------|
| POST | `/api/auth/register` | Register new user | `{ "email": "string", "password": "string" }` | 200 OK with JWT token |
| POST | `/api/auth/login` | Login existing user | `{ "email": "string", "password": "string" }` | 200 OK with JWT token |
| GET | `/api/auth/health` | Health check | - | 200 OK |

## Troubleshooting

**Issue: Cannot connect to PostgreSQL**
- Ensure "dotnet-ef not found"**
```bash
dotnet tool install --global dotnet-ef
```

- **Database**: PostgreSQL 16-alpine (Docker)
- **Port**: Configured dynamically (check terminal output)local
- Check firewall settings for port 5432

**Issue: Port already in use**
- Check what's running on the port in launchSettings.json
- Kill the process or change the port in Properties/launchSettings.json

**Issue: Migration fails**
- Ensure you're in the Infrastructure project directory
- Use `--startup-project ..\AuthService.API` flag
- Verify database is running and connection string is correct

**Issue: JWT token validation fails**
- Verify JWT Secret, Issuer, and Audience match in appsettings.json
- Check token expiry time (Development: 1440 minutes, Production
## Docker Deployment

```bash
# Build Docker image (create Dockerfile in AuthService.API)
docker build -t auth-service:latest .

# Run with environment variables
docker run -d -p 5000:8080 \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Port=5432;Database=authdb;Username=postgres;Password=postgres" \
  -e Jwt__Secret="YourSuperSecretKeyForProduction" \
  auth-service:latest
```

## Project Statistics

- **Total Projects**: 4 (Domain, Application, Infrastructure, API)
- **Design Pattern**: Clean Architecture + Repository Pattern + Factory Pattern
- **Dependencies**: EF Core 10.0, Npgsql, BCrypt.Net-Next, JWT Bearer
- **Build Status**: ✅ Successful (with 1 minor EF Core version warning)
- **Unit Tests**: Mock IUserRepository, IPasswordHasher, IJwtTokenGenerator
- **Integration Tests**: Test against real PostgreSQL database
- **API Tests**: Use WebApplicationFactory for end-to-end tests
