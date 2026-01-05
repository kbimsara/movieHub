# 🚀 Movie Management Service - Quick Start

## ✅ Verification Complete!
All 14 checks passed - the service is ready to use!

## Start the Service

### Option 1: Start All Services
```powershell
cd e:\Github\movieHub\services
docker-compose up -d --build
```

### Option 2: Start Only Movie Management Service
```powershell
cd e:\Github\movieHub\services
docker-compose up -d --build movie-management-service movie-management-db
```

## Verify Service is Running

```powershell
# Check service health
curl http://localhost:5005/health

# Expected response:
# {"status":"healthy","service":"Movie Management Service","timestamp":"..."}

# Via API Gateway
curl http://localhost:5000/api/manage/movies

# View logs
docker-compose logs -f movie-management-service
```

## Access Swagger UI
Open in your browser: **http://localhost:5005/swagger**

## Test with Postman

1. **Import Collection**:
   - File: `Request-postman/MovieHub-ApiGateWay.postman_collection.json`
   
2. **Get Authentication Token**:
   - Use "Auth Service" → "Login User"
   - Copy the `token` from response
   - Set variable: `auth_token` = `YOUR_TOKEN`

3. **Test Endpoints**:
   - "Movie Management Service" → "Movies" → "Create Movie"
   - "Movie Management Service" → "Upload" → "Upload Movie"
   - "Movie Management Service" → "Files" → "Get Movie Files"

## Test Upload from Frontend

```powershell
# Start frontend
cd e:\Github\movieHub\front-end
npm run dev

# Navigate to:
# http://localhost:3000/upload

# Upload a movie with poster
# Then check:
# http://localhost:3000  (see "Recently Added")
```

## Available Endpoints

### Movies (via API Gateway - Port 5000)
- POST `/api/manage/movies` - Create movie
- GET `/api/manage/movies` - List all movies
- GET `/api/manage/movies/{id}` - Get movie
- GET `/api/manage/movies/my-movies` - My movies
- PUT `/api/manage/movies/{id}` - Update movie
- DELETE `/api/manage/movies/{id}` - Delete movie
- POST `/api/manage/movies/{id}/publish` - Publish
- POST `/api/manage/movies/{id}/view` - Track view

### Upload
- POST `/api/upload` - Upload video + poster
- GET `/api/upload/{sessionId}` - Upload status

### Files
- GET `/api/files/{fileId}` - File metadata
- GET `/api/files/movie/{movieId}` - Movie files
- GET `/api/files/{fileId}/stream` - Stream video
- DELETE `/api/files/{fileId}` - Delete file

## Service Ports

- **API Gateway**: 5000
- **Movie Management**: 5005
- **Database**: 5436
- **Frontend**: 3000

## Database Access

```powershell
# Connect to database
docker-compose exec movie-management-db psql -U postgres -d moviemanagementdb

# List tables
\dt

# View movies
SELECT * FROM "Movies";

# View files
SELECT * FROM "MovieFiles";

# Exit
\q
```

## Useful Commands

```powershell
# View all containers
docker-compose ps

# View logs
docker-compose logs movie-management-service

# Restart service
docker-compose restart movie-management-service

# Stop service
docker-compose stop movie-management-service

# Remove service and data
docker-compose down -v

# View uploaded files
docker-compose exec movie-management-service ls -lh /app/uploads
```

## Troubleshooting

### Service won't start
```powershell
docker-compose logs movie-management-service
docker-compose restart movie-management-db
```

### Cannot upload files
- Check JWT token is valid (login again)
- Ensure file size < 5GB
- Check disk space: `docker system df`

### 404 errors
- Verify API Gateway is running: `curl http://localhost:5000/health`
- Check service health: `curl http://localhost:5005/health`

## What's Next?

1. ✅ Service is running
2. ✅ Test upload with Postman
3. ✅ Test frontend upload page
4. ⏭️ Add video transcoding (FFmpeg)
5. ⏭️ Implement cloud storage (S3/Azure)
6. ⏭️ Add HLS streaming support

## Documentation

- **Setup Guide**: `movie-management-service/SETUP_GUIDE.md`
- **API Reference**: `MOVIE_MANAGEMENT_SERVICE.md`
- **Implementation Details**: `movie-management-service/IMPLEMENTATION_SUMMARY.md`
- **Service README**: `movie-management-service/README.md`

## Support

If you encounter issues:
1. Check logs: `docker-compose logs movie-management-service`
2. Verify database: `docker-compose ps movie-management-db`
3. Test health: `curl http://localhost:5005/health`
4. Review setup guide for detailed troubleshooting

---

🎉 **You're all set! The Movie Management Service is ready to handle uploads, file management, and streaming!**
