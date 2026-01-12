# Movie Management Service

A comprehensive microservice for handling movie uploads, file management, and streaming in the MovieHub platform.

## Features

- **Movie Management**: Create, read, update, delete movie metadata
- **File Upload**: Support for video files, posters, and subtitles
- **File Streaming**: HTTP range request support for video streaming
- **Quality Selection**: Multiple video quality options (480p, 720p, 1080p, 4K)
- **User Authorization**: JWT-based authentication and authorization
- **Upload Tracking**: Session-based upload progress tracking
- **View Counting**: Track movie view statistics

## Architecture

This service follows Clean Architecture principles with four layers:

- **Domain**: Core entities (ManagedMovie, MovieFile, UploadSession)
- **Application**: Business logic, DTOs, and interfaces
- **Infrastructure**: Data access, repositories, and EF Core
- **API**: Controllers, authentication, and HTTP endpoints

## Endpoints

### Movies
- `POST /api/movies` - Create a new movie
- `GET /api/movies` - Get all published movies
- `GET /api/movies/{id}` - Get movie by ID
- `GET /api/movies/my-movies` - Get current user's movies
- `PUT /api/movies/{id}` - Update movie
- `DELETE /api/movies/{id}` - Delete movie
- `POST /api/movies/{id}/publish` - Publish a movie
- `POST /api/movies/{id}/view` - Increment view count

### Upload
- `POST /api/upload` - Upload movie video and poster files
- `GET /api/upload/{sessionId}` - Get upload session status

### Files
- `GET /api/files/{fileId}` - Get file metadata
- `GET /api/files/movie/{movieId}` - Get all files for a movie
- `GET /api/files/{fileId}/stream` - Stream file content
- `DELETE /api/files/{fileId}` - Delete file

## Database Schema

### ManagedMovie Table
- Id, Title, Description, Genre, ReleaseYear
- DurationMinutes, Rating, PosterUrl, TrailerUrl
- Quality, CreatedAt, UpdatedAt, CreatedBy
- IsPublished, ViewCount

### MovieFile Table
- Id, MovieId, FileName, FilePath, FileType
- FileSize, Quality, MimeType
- UploadedAt, UploadedBy, IsProcessed

### UploadSession Table
- Id, UserId, MovieId, Status, Progress
- TotalBytes, UploadedBytes
- StartedAt, CompletedAt, ErrorMessage

## Running Locally

```bash
cd MovieManagementService.API
dotnet restore
dotnet run
```

Visit `http://localhost:5005/swagger` for API documentation.

## Docker

```bash
docker build -t moviehub-movie-management .
docker run -p 5005:5005 moviehub-movie-management
```

## Environment Variables

See `.env.example` for required configuration.
