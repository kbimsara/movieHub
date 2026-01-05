# Movie Management Service - Setup Guide

## Overview
The Movie Management Service handles movie uploads, file management, and video streaming for the MovieHub platform. It's fully integrated with the API Gateway and ready to use.

## What's Been Added

### 1. New Microservice Structure
```
movie-management-service/
├── MovieManagementService.API/          # API layer with controllers
├── MovieManagementService.Application/  # Business logic and DTOs
├── MovieManagementService.Domain/       # Core entities
├── MovieManagementService.Infrastructure/ # Data access
├── Dockerfile
└── README.md
```

### 2. API Gateway Routes
The following routes have been added to API Gateway (`appsettings.json`):
- `/api/upload` → Movie Management Service
- `/api/files` → Movie Management Service  
- `/api/manage/movies` → Movie Management Service

### 3. Docker Configuration
- New PostgreSQL database: `movie-management-db` on port 5436
- New service: `movie-management-service` on port 5005
- Persistent volume for movie uploads: `movie-uploads`

### 4. Postman Collection
Added complete API testing collection with:
- Movie CRUD operations (8 endpoints)
- Upload endpoints (2 endpoints)
- File management endpoints (4 endpoints)

## Quick Start

### Option 1: Run with Docker Compose (Recommended)

```powershell
# Navigate to services directory
cd e:\Github\movieHub\services

# Build and start all services including the new one
docker-compose up -d --build

# Check service health
docker-compose ps

# View logs for movie management service
docker-compose logs -f movie-management-service
```

### Option 2: Run Locally for Development

```powershell
# Navigate to the API project
cd e:\Github\movieHub\services\movie-management-service\MovieManagementService.API

# Restore dependencies
dotnet restore

# Update database connection in appsettings.Development.json if needed
# Default: localhost:5436

# Run the service
dotnet run

# Service will be available at http://localhost:5005
# Swagger UI: http://localhost:5005/swagger
```

## Testing the Service

### 1. Using Postman

1. Import the updated collection:
   - File: `services/Request-postman/MovieHub-ApiGateWay.postman_collection.json`

2. Set up variables:
   - `auth_token`: Get this from Auth Service login
   - `movie_id`: Use after creating a movie
   - `file_id`: Use after uploading a file
   - `session_id`: Use after starting an upload

3. Test the endpoints in this order:
   - Register & Login (Auth Service)
   - Create Movie (Movie Management)
   - Upload Movie File (Upload endpoint)
   - Get Movie Files
   - Stream File

### 2. Using cURL

**Create a Movie:**
```bash
curl -X POST http://localhost:5000/api/manage/movies \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "The Matrix",
    "description": "A computer hacker learns about reality",
    "genre": "Sci-Fi",
    "releaseYear": 1999,
    "durationMinutes": 136,
    "rating": 8.7,
    "quality": "1080p"
  }'
```

**Upload a Movie File:**
```bash
curl -X POST http://localhost:5000/api/upload \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -F "VideoFile=@/path/to/movie.mp4" \
  -F "PosterFile=@/path/to/poster.jpg" \
  -F "Title=Inception" \
  -F "Genre=Sci-Fi" \
  -F "ReleaseYear=2010" \
  -F "DurationMinutes=148" \
  -F "Rating=8.8" \
  -F "Quality=1080p"
```

**Get All Movies:**
```bash
curl http://localhost:5000/api/manage/movies
```

**Stream a Video:**
```bash
curl http://localhost:5000/api/files/{file-id}/stream \
  -H "Range: bytes=0-1024" \
  --output video.mp4
```

### 3. Using Swagger UI

Navigate to: http://localhost:5005/swagger

- Click "Authorize" and enter: `Bearer YOUR_TOKEN`
- Try the endpoints interactively
- View request/response schemas

## Database Migrations

The service automatically runs migrations on startup. To manually create migrations:

```powershell
cd MovieManagementService.Infrastructure

# Add a new migration
dotnet ef migrations add InitialCreate -s ..\MovieManagementService.API

# Update database
dotnet ef database update -s ..\MovieManagementService.API
```

## Architecture

### Entities
- **ManagedMovie**: Movie metadata (title, genre, rating, etc.)
- **MovieFile**: File information (video, poster, subtitle)
- **UploadSession**: Upload progress tracking

### Controllers
- **MoviesController**: Movie CRUD operations
- **UploadController**: File upload handling
- **FilesController**: File retrieval and streaming
- **HealthController**: Service health check

### Services
- **MovieService**: Business logic for movies
- **FileService**: File upload/download/streaming logic

### Repositories
- **MovieRepository**: Database access for movies
- **FileRepository**: Database access for files

## Environment Variables

Key variables in `.env`:

```env
# Service Port
MOVIE_MANAGEMENT_HTTP_PORT=5005
MOVIE_MANAGEMENT_HTTPS_PORT=5448

# Database
MOVIE_MANAGEMENT_DB_NAME=moviemanagementdb
MOVIE_MANAGEMENT_DB_USER=postgres
MOVIE_MANAGEMENT_DB_PASSWORD=postgres
MOVIE_MANAGEMENT_DB_PORT=5436

# JWT (shared with other services)
JWT_SECRET=your-secret-key
JWT_ISSUER=moviehub-auth
JWT_AUDIENCE=moviehub-api
```

## File Storage

### Current Implementation
Files are stored locally in the `/app/uploads` directory inside the container, mapped to the `movie-uploads` Docker volume.

### Future Enhancements
- Cloud storage integration (AWS S3, Azure Blob)
- Video transcoding pipeline
- Thumbnail generation
- CDN integration for streaming

## Integration with Frontend

The frontend at `front-end/src` has been configured to work with this service:

**Upload Service** (`src/services/upload.service.ts`):
- Connects to `/api/upload`
- Handles file upload with progress tracking

**File Service** (`src/services/file.service.ts`):
- Connects to `/api/files`
- Manages file metadata and downloads

**Upload Page** (`src/app/upload/page.tsx`):
- Ready-to-use upload form
- Integrated with the backend

## Troubleshooting

### Service won't start
```powershell
# Check database is ready
docker-compose ps movie-management-db

# Check logs
docker-compose logs movie-management-service

# Restart service
docker-compose restart movie-management-service
```

### Cannot upload files
- Check JWT token is valid
- Ensure file size is under 5GB limit
- Verify disk space in uploads volume

### Database connection errors
- Confirm database credentials in `.env`
- Check if database container is running
- Verify network connectivity

### 404 errors from frontend
- Ensure API Gateway is routing correctly
- Check service is registered in `docker-compose.yml`
- Verify API Gateway `appsettings.json` has the routes

## Next Steps

1. **Test the Upload Flow**
   - Register a user
   - Upload a movie with poster
   - View the movie on the frontend

2. **Add Video Processing**
   - Integrate FFmpeg for transcoding
   - Generate thumbnails
   - Create multiple quality versions

3. **Implement Streaming**
   - Add HLS/DASH support
   - Implement adaptive bitrate streaming
   - Add subtitle support

4. **Add Storage Options**
   - Configure AWS S3 or Azure Blob
   - Implement CDN integration
   - Add file cleanup policies

## Support

For issues or questions:
1. Check service logs: `docker-compose logs movie-management-service`
2. Verify database: `docker-compose exec movie-management-db psql -U postgres -d moviemanagementdb`
3. Test health: `curl http://localhost:5005/health`
