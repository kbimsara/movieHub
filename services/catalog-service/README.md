# Catalog Service

Movie Catalog microservice built with .NET 9 and Clean Architecture.

## Architecture

### Domain Layer
- **Movie Entity**: Core business entity with all movie properties
- No dependencies on other layers

### Application Layer
- **DTOs**: Data transfer objects (MovieDto, CreateMovieDto)
- **Interfaces**: IMovieService, IMovieRepository
- **Services**: MovieService contains all business logic
- Depends on: Domain layer only

### Infrastructure Layer
- **MovieDbContext**: EF Core DbContext for PostgreSQL
- **MovieRepository**: Data access implementation
- **DependencyInjection**: Infrastructure service registration
- Depends on: Domain, Application layers

### API Layer
- **Controllers**: MovieController, HealthController
- **Program.cs**: Application configuration, JWT validation, Swagger
- No business logic in controllers
- Depends on: Application, Infrastructure layers

## Features

### Endpoints
- `GET /api/movies` - Get all movies (public)
- `GET /api/movies/{id}` - Get movie by ID (public)
- `POST /api/movies` - Create movie (requires JWT)
- `GET /api/health` - Health check

### JWT Authentication
- Validates JWT tokens issued by Auth Service
- Uses same Secret, Issuer, and Audience as Auth Service
- [Authorize] attribute on POST endpoint only
- Read-only endpoints are public

## Configuration

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=catalogdb;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "Secret": "YourSuperSecretKeyForJWT_MustBeAtLeast32Characters",
    "Issuer": "MovieHubAuthService",
    "Audience": "MovieHubServices"
  }
}
```

**Important**: Use the SAME JWT configuration as Auth Service.

## Database

PostgreSQL with Entity Framework Core:
- Auto-migration on startup (development only)
- Movies table with all required fields

### Run Migrations Manually
```bash
cd CatalogService.API
dotnet ef migrations add InitialCreate --project ../CatalogService.Infrastructure
dotnet ef database update
```

## Running the Service

### Prerequisites
- .NET 9 SDK
- PostgreSQL running on localhost:5432

### Run Locally
```bash
cd CatalogService.API
dotnet restore
dotnet run
```

Service runs on: `http://localhost:5000`
Swagger UI: `http://localhost:5000/swagger`

### Docker
```bash
docker build -t catalog-service .
docker run -p 5000:8080 catalog-service
```

## Testing with Swagger

1. Navigate to `http://localhost:5000/swagger`
2. For POST /api/movies:
   - Click "Authorize" button
   - Enter: `Bearer <your-jwt-token>`
   - Click "Authorize"
3. Test endpoints

## Clean Architecture Rules

✅ **DO**
- Keep business logic in Application layer
- Use interfaces for dependencies
- Repository pattern for data access
- DTOs for API contracts

❌ **DON'T**
- Put business logic in controllers
- Use EF Core directly in Application layer
- Create or manage JWT tokens (only validate)
- Mix layer responsibilities

## Integration with Other Services

- **API Gateway**: Routes `/api/movies/**` to this service
- **Auth Service**: Issues JWT tokens (this service only validates)
- **User Service**: Can be called if user context needed

## Next Steps

1. Update JWT settings to match Auth Service
2. Configure PostgreSQL connection string
3. Run migrations
4. Test endpoints via Swagger
5. Integrate with API Gateway
