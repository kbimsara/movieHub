# 🎬 Movie Management Service - Quick Reference

## Service Endpoints

### Via API Gateway (http://localhost:5000)

#### Movies
- `POST /api/manage/movies` - Create movie
- `GET /api/manage/movies` - List all movies
- `GET /api/manage/movies/{id}` - Get movie details
- `GET /api/manage/movies/my-movies` - Get user's movies
- `PUT /api/manage/movies/{id}` - Update movie
- `DELETE /api/manage/movies/{id}` - Delete movie
- `POST /api/manage/movies/{id}/publish` - Publish movie
- `POST /api/manage/movies/{id}/view` - Increment views

#### Upload
- `POST /api/upload` - Upload movie (video + poster)
- `GET /api/upload/{sessionId}` - Get upload status

#### Files
- `GET /api/files/{fileId}` - Get file metadata
- `GET /api/files/movie/{movieId}` - Get movie files
- `GET /api/files/{fileId}/stream` - Stream video
- `DELETE /api/files/{fileId}` - Delete file

### Direct Service (http://localhost:5005)
- `/health` - Health check
- `/swagger` - API documentation

## Quick Commands

```powershell
# Start all services
docker-compose up -d

# Start only movie management service
docker-compose up -d movie-management-service

# View logs
docker-compose logs -f movie-management-service

# Restart service
docker-compose restart movie-management-service

# Stop all services
docker-compose down

# Rebuild and start
docker-compose up -d --build movie-management-service

# Check database
docker-compose exec movie-management-db psql -U postgres -d moviemanagementdb

# View upload files
docker-compose exec movie-management-service ls -lh /app/uploads
```

## Ports

- **Service**: 5005 (HTTP), 5448 (HTTPS)
- **Database**: 5436 (PostgreSQL)
- **API Gateway**: 5000

## Default Credentials

- **Database User**: postgres
- **Database Password**: postgres
- **Database Name**: moviemanagementdb

## Testing with Frontend

1. Start frontend: `cd front-end && npm run dev`
2. Navigate to: http://localhost:3000/upload
3. Upload a movie with poster
4. Check: http://localhost:3000 for "Recently Added"

## Docker Volumes

- `movie-management-db-data` - Database persistence
- `movie-uploads` - Uploaded files storage

## Integration Points

**Frontend** (`front-end/src`):
- Upload page: `/upload`
- My Uploads: `/uploads`
- File Manager: `/files`

**API Gateway** (`api-gateway/appsettings.json`):
- Routes: upload-route, files-route, managed-movies-route
- Cluster: movie-management-cluster
- Timeout: 5 minutes (for large uploads)

**Postman Collection**:
- Import: `Request-postman/MovieHub-ApiGateWay.postman_collection.json`
- Folder: "Movie Management Service"

## File Upload Limits

- Max file size: 5GB
- Supported formats: MP4, MKV, AVI (video), JPG, PNG (poster)
- Quality options: 480p, 720p, 1080p, 4K

## Common Issues

**404 on /api/upload**:
- Check API Gateway is running
- Verify service is healthy: `curl http://localhost:5005/health`

**Upload fails**:
- Check JWT token is valid
- Ensure database is running
- Verify disk space

**Cannot stream video**:
- Check file exists: `GET /api/files/{fileId}`
- Verify file path in database
- Check file permissions in uploads volume
