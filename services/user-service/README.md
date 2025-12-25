# User Service - User Profile Microservice

This is a User Profile microservice built with .NET 10 and Clean Architecture. It **trusts and validates** JWT tokens issued by the Auth Service but does NOT create tokens or handle authentication.

## Architecture

The service follows Clean Architecture with 4 layers:

```
UserService.API          → Controllers, JWT validation, Swagger
UserService.Application  → DTOs, Interfaces, Business logic
UserService.Domain       → Entities (UserProfile)
UserService.Infrastructure → EF Core, PostgreSQL, Repositories
```

## Prerequisites

- .NET 10 SDK
- PostgreSQL (running on port 5434)
- JWT token from Auth Service

## Quick Start

### 1. Start PostgreSQL Database

```bash
cd services/DB
docker-compose --env-file .env.local up -d user-db
```

This starts PostgreSQL on port **5434** with database name **userdb**.

### 2. Update JWT Configuration

Update `appsettings.json` with the **same JWT settings** as your Auth Service:

```json
"JwtSettings": {
  "Secret": "your-super-secret-key-must-match-auth-service-minimum-32-characters",
  "Issuer": "MovieHub.AuthService",
  "Audience": "MovieHub.Client"
}
```

⚠️ **Important**: The Secret, Issuer, and Audience MUST match your Auth Service configuration exactly.

### 3. Run Database Migrations

```bash
cd services/user-service/UserService.API
dotnet ef database update
```

This creates the `UserProfiles` table in the database.

### 4. Run the Service

```bash
cd services/user-service/UserService.API
dotnet run --urls "https://localhost:7002;http://localhost:5002"
```

The service will run on:
- **HTTPS**: https://localhost:7002
- **HTTP**: http://localhost:5002
- **Swagger UI**: https://localhost:7002/swagger

## Configuration

### Database Connection String
Located in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5434;Database=userdb;Username=postgres;Password=postgres"
}
```

### JWT Settings
Must match Auth Service configuration:
```json
"JwtSettings": {
  "Secret": "your-super-secret-key-must-match-auth-service-minimum-32-characters",
  "Issuer": "MovieHub.AuthService",
  "Audience": "MovieHub.Client"
}
```

## API Endpoints

### GET /api/users/me
Returns the current user's profile based on JWT token.

**Authentication Required:** Yes

**Response:**
```json
{
  "id": "guid",
  "userId": "guid",
  "email": "user@example.com",
  "displayName": "John Doe",
  "createdAt": "2025-12-25T00:00:00Z"
}
```

### POST /api/users
Creates a user profile. UserId and Email are extracted from JWT token.
**Get a JWT Token** from Auth Service:
   - Register or login at Auth Service to get a JWT token
   - Copy the `token` value from the response

2. **Navigate to Swagger UI**: https://localhost:7002/swagger

3. **Authorize**:
   - Click the "Authorize" button (🔒 icon)
   - Enter: `Bearer YOUR_JWT_TOKEN`
   - Click "Authorize" then "Close"

4. **Test Endpoints**:
   - **POST /api/users**: Create your user profile (provide `displayName`)
   - **GET /api/users/me**: Retrieve your user profile

## Testing Workflow Example

```bash
# 1. Get JWT token from Auth Service
curl -X POST https://localhost:7001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"YourPassword123"}'

# 2. Create user profile (replace YOUR_TOKEN)
curl -X POST https://localhost:7002/api/users \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"displayName":"John Doe"}'

# 3. Get user profile
curl -X GET https://localhost:7002/api/users/me \
  -H "Authorization: Bearer YOUR_TOKEN"
```
```json
{
  "displayName": "John Doe"
}
```

**Response:**
```json
{

## Database Schema

### UserProfiles Table
| Column | Type | Constraints |
|--------|------|-------------|
| Id | uuid | PRIMARY KEY |
| UserId | uuid | UNIQUE, NOT NULL |
| Email | varchar(255) | NOT NULL |
| DisplayName | varchar(100) | NOT NULL |
| CreatedAt | timestamp | NOT NULL |

**Indexes:**
- Unique index on `UserId` (ensures one profile per user)

## Project Structure

```
UserService/
├── UserService.API/
│   ├── Controllers/
│   │   └── UserController.cs          # Endpoints with [Authorize]
│   ├── Extensions/
│   │   ├── AuthenticationExtensions.cs # JWT validation setup
│   │   └── SwaggerExtensions.cs       # Swagger with JWT
│   ├── Program.cs                     # App configuration
│   └── appsettings.json               # Configuration
├── UserService.Application/
│   ├── DTOs/                          # Request/Response DTOs
│   ├── Interfaces/                    # Service & Repository interfaces
│   ├── Services/
│   │   └── UserProfileService.cs      # Business logic
│   └── DependencyInjection.cs
├── UserService.Domain/
│   └── Entities/
│       └── UserProfile.cs             # Domain entity
└── UserService.Infrastructure/
    ├── Persistence/
    │   ├── UserDbContext.cs           # EF Core DbContext
    │   └── UserProfileRepository.cs   # Repository implementation
    ├── Migrations/                    # EF Core migrations
    └── DependencyInjection.cs
```

## Troubleshooting

### Database Connection Issues
```bash
# Check if PostgreSQL container is running
docker ps | grep moviehub-user-db

# View container logs
docker logs moviehub-user-db

# Restart database
docker-compose --env-file .env.local restart user-db
```

### JWT Validation Errors
- Ensure JWT settings match Auth Service exactly
- Check token hasn't expired
- Verify token includes `sub` (UserId) and `email` claims

### Migration Issues
```bash
# Remove last migration
dotnet ef migrations remove --project ../UserService.Infrastructure

# Recreate migration
dotnet ef migrations add InitialCreate --project ../UserService.Infrastructure

# Apply to database
dotnet ef database update
```
  "id": "guid",
  "userId": "guid",
  "email": "user@example.com",
  "displayName": "John Doe",
  "createdAt": "2025-12-25T00:00:00Z"
}
```

## Testing with Swagger

1. Navigate to https://localhost:7002/swagger
2. Click "Authorize" button
3. Enter: `Bearer YOUR_JWT_TOKEN` (obtained from Auth Service)
4. Test the endpoints

## Key Design Decisions

- **No Token Creation:** This service only validates JWT tokens
- **Claims-based User Info:** UserId and Email come from JWT claims
- **Repository Pattern:** All database access goes through repositories
- **Clean Architecture:** Strict separation of concerns across layers
- **No Business Logic in API:** Controllers are thin, only handle HTTP concerns

## Layer Responsibilities

- **Domain:** Pure entities with no dependencies
- **Application:** Business logic, DTOs, service interfaces
- **Infrastructure:** Database access, EF Core, external services
- **API:** HTTP layer, authentication, routing, Swagger
