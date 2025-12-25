# MovieHub Database Services

This directory contains Docker Compose configuration for all database services used by MovieHub microservices.

## Services

### PostgreSQL (auth-db)
- **Container Name**: `moviehub-auth-db`
- **Port**: `5432`
- **Database**: `authdb`
- **User**: `postgres`
- **Password**: `postgres` (change in production!)
- **Purpose**: Authentication service database

### pgAdmin (optional)
- **Container Name**: `moviehub-pgadmin`
- **Port**: `5050`
- **URL**: `http://localhost:5050`
- **Email**: `admin@moviehub.local`
- **Password**: `admin`
- **Purpose**: Database management UI

## Quick Start

### 1. Start All Databases
```bash
cd f:\Github\movieHub\services\DB
docker-compose up -d
```

### 2. Start Specific Database
```bash
docker-compose up -d auth-db
```

### 3. View Logs
```bash
docker-compose logs -f auth-db
```

### 4. Stop All Databases
```bash
docker-compose down
```

### 5. Stop and Remove Volumes (⚠️ Data Loss!)
```bash
docker-compose down -v
```

## Access Database

### Using psql
```bash
docker exec -it moviehub-auth-db psql -U postgres -d authdb
```

### Using pgAdmin
1. Open browser: `http://localhost:5050`
2. Login with `admin@moviehub.local` / `admin`
3. Add new server:
   - Name: `Auth DB`
   - Host: `auth-db` (or `host.docker.internal` if from host)
   - Port: `5432`
   - Username: `postgres`
   - Password: `postgres`

### From .NET Application
Update your `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=authdb;Username=postgres;Password=postgres"
  }
}
```

## Connection Strings

### From Host Machine (Development)
```
Host=localhost;Port=5432;Database=authdb;Username=postgres;Password=postgres
```

### From Docker Container (Same Network)
```
Host=auth-db;Port=5432;Database=authdb;Username=postgres;Password=postgres
```

## Volume Management

### List Volumes
```bash
docker volume ls | findstr moviehub
```

### Inspect Volume
```bash
docker volume inspect db_auth-db-data
```

### Backup Database
```bash
docker exec moviehub-auth-db pg_dump -U postgres authdb > backup_$(Get-Date -Format 'yyyyMMdd_HHmmss').sql
```

### Restore Database
```bash
Get-Content backup_20251225_120000.sql | docker exec -i moviehub-auth-db psql -U postgres -d authdb
```

## Health Check

The PostgreSQL container includes a health check that runs every 10 seconds:
```bash
docker-compose ps
```

Look for `healthy` status.

## Troubleshooting

### Container Won't Start
```bash
# Check logs
docker-compose logs auth-db

# Check if port is already in use
netstat -ano | findstr :5432
```

### Cannot Connect from Application
1. Ensure container is running: `docker-compose ps`
2. Check connection string uses correct host (`localhost` from host, `auth-db` from container)
3. Verify firewall settings
4. Check Docker network: `docker network inspect db_moviehub-network`

### Reset Database
```bash
docker-compose down -v
docker-compose up -d auth-db
```

## Production Considerations

⚠️ **Before deploying to production:**

1. **Change Default Passwords**: Update `.env` file with strong passwords
2. **Remove pgAdmin**: Comment out or remove pgAdmin service
3. **Use Secrets**: Store credentials in Docker secrets or environment variables
4. **Enable SSL**: Configure PostgreSQL to use SSL/TLS
5. **Backup Strategy**: Implement automated backups
6. **Resource Limits**: Add memory and CPU limits to services
7. **Persistent Volumes**: Use named volumes or bind mounts to specific paths

## Adding More Databases

When adding new microservices, add their databases here:

```yaml
# Example: Movie Service Database
movie-db:
  image: postgres:16-alpine
  container_name: moviehub-movie-db
  restart: unless-stopped
  environment:
    POSTGRES_DB: moviedb
    POSTGRES_USER: postgres
    POSTGRES_PASSWORD: postgres
  ports:
    - "5433:5432"
  volumes:
    - movie-db-data:/var/lib/postgresql/data
  networks:
    - moviehub-network
```

Don't forget to add the volume:
```yaml
volumes:
  movie-db-data:
    driver: local
```
