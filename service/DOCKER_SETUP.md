# MovieHub Microservices Docker Setup

This Docker Compose setup provides a complete microservices backend for MovieHub with the following services:

## Services

- **auth-service** (Port 5001): Handles user authentication and JWT token management
- **movie-service** (Port 5002): Manages movie data and integrates with TMDB API
- **file-service** (Port 5005): Handles file uploads and storage
- **postgres-auth-db** (Port 5432): PostgreSQL database for authentication service
- **postgres-movie-db** (Port 5433): PostgreSQL database for movie service

## Prerequisites

- Docker and Docker Compose installed
- .NET 8.0 SDK (for building services locally if needed)

## Quick Start

1. **Clone the repository and navigate to the service directory:**
   ```bash
   cd service/
   ```

2. **Copy the environment file and configure your variables:**
   ```bash
   cp .env.example .env
   # Edit .env with your actual values
   ```

3. **Start all services:**
   ```bash
   docker-compose up --build
   ```

4. **Access the services:**
   - Auth Service: `http://localhost:5001`
   - Movie Service: `http://localhost:5002`
   - File Service: `http://localhost:5005`
   - Auth DB: `localhost:5432`
   - Movie DB: `localhost:5433`

## Environment Variables

Create a `.env` file in the service directory with the following variables:

```bash
# Database password (used by both DB services)
POSTGRES_PASSWORD=your-secure-password

# JWT configuration for auth-service
JWT_KEY=your-32-character-or-longer-secret-key
JWT_ISSUER=https://your-domain.com
JWT_AUDIENCE=https://your-domain.com

# TMDB API key for movie-service
TMDB_API_KEY=your-tmdb-api-key
```

## Database Migrations

After starting the services, run Entity Framework migrations for each service:

```bash
# Auth Service
docker-compose exec auth-service dotnet ef database update

# Movie Service
docker-compose exec movie-service dotnet ef database update
```

## Persistent Data

- Database data is persisted in Docker volumes (`auth_db_data`, `movie_db_data`)
- File uploads are stored in the `uploads_data` volume

## Development

For development, you can run services individually:

```bash
# Run only databases
docker-compose up postgres-auth-db postgres-movie-db

# Run specific service
docker-compose up auth-service
```

## Stopping Services

```bash
docker-compose down
```

## Troubleshooting

- **Port conflicts**: Ensure ports 5001, 5002, 5005, 5432, 5433 are available
- **Database connection issues**: Check POSTGRES_PASSWORD in .env file
- **JWT issues**: Ensure JWT_KEY is at least 32 characters long
- **TMDB API**: Verify TMDB_API_KEY is valid

## Production Considerations

- Use secrets management instead of .env file
- Configure proper CORS policies
- Add health checks and monitoring
- Use production-grade PostgreSQL configuration
- Implement load balancing for services