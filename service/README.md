# MovieHub Microservices Backend

A complete microservices architecture for MovieHub built with ASP.NET Core, featuring an API Gateway, multiple specialized services, and containerized deployment.

## 🏗️ Architecture Overview

```
┌─────────────────┐    ┌─────────────────┐
│   Frontend      │────│   API Gateway   │
│   (Next.js)     │    │   (Port 5000)   │
└─────────────────┘    └─────────────────┘
                              │
                    ┌─────────┼─────────┐
                    │         │         │
            ┌───────▼───┐ ┌───▼───┐ ┌───▼───┐
            │Auth Service│ │Movie │ │User   │
            │(Port 5001) │ │Svc   │ │Service│
            └────────────┘ │(5002)│ │(5003) │
                           └──────┘ └───────┘
                    │         │         │
            ┌───────▼───┐ ┌───▼───┐ ┌───▼───┐
            │Library Svc │ │File  │ │Databases│
            │(Port 5004) │ │Svc   │ │(Postgres)|
            └────────────┘ │(5005)│ └────────┘
                           └──────┘
```

## 📋 Services

### 🔐 Authentication Service (Port 5001)
- User registration and login
- JWT token generation and validation
- Password hashing with BCrypt
- PostgreSQL database storage

### 🎬 Movie Service (Port 5002)
- Movie CRUD operations
- Movie search and filtering
- TMDB API integration
- PostgreSQL database storage

### 👤 User Service (Port 5003)
- User profile management
- User preferences and settings
- PostgreSQL database storage

### 📚 Library Service (Port 5004)
- User movie libraries
- Library item management
- Watchlist functionality
- PostgreSQL database storage

### 📁 File Service (Port 5005)
- File upload and storage
- Image and video file handling
- Public URL generation
- Local file system storage

### 🚪 API Gateway (Port 5000)
- Single entry point for all services
- Request routing with YARP
- JWT token forwarding
- Centralized CORS handling

## 🚀 Quick Start

### Prerequisites
- Docker and Docker Compose
- .NET 8.0 SDK (for local development)

### Using Docker Compose

1. **Clone and navigate to the service directory:**
   ```bash
   cd service/
   ```

2. **Start all services:**
   ```bash
   docker-compose up --build
   ```

3. **Access the API Gateway:**
   - API Gateway: `http://localhost:5000`
   - All services accessible through the gateway

### Local Development

1. **Start databases:**
   ```bash
   docker-compose up auth-db movie-db user-db library-db
   ```

2. **Run individual services:**
   ```bash
   # Terminal 1 - Auth Service
   cd auth-service/Auth.API && dotnet run

   # Terminal 2 - Movie Service
   cd movie-service/Movie.API && dotnet run

   # Terminal 3 - User Service
   cd user-service/User.API && dotnet run

   # Terminal 4 - Library Service
   cd library-service/Library.API && dotnet run

   # Terminal 5 - File Service
   cd file-service/File.API && dotnet run

   # Terminal 6 - API Gateway
   cd api-gateway/ApiGateway && dotnet run
   ```

## 📡 API Endpoints

All requests go through the API Gateway at `http://localhost:5000`:

### Authentication
```
POST /api/auth/register     - User registration
POST /api/auth/login        - User login
POST /api/auth/refresh      - Token refresh
```

### Movies
```
GET    /api/movies          - Get all movies
GET    /api/movies/{id}     - Get movie by ID
POST   /api/movies          - Create movie
PUT    /api/movies/{id}     - Update movie
DELETE /api/movies/{id}     - Delete movie
GET    /api/movies/search   - Search movies
```

### User Profiles
```
GET    /api/userprofiles    - Get all profiles
GET    /api/userprofiles/{id} - Get profile by ID
PUT    /api/userprofiles/{id} - Update profile
```

### Libraries
```
GET    /api/libraries       - Get user libraries
POST   /api/libraries       - Create library
GET    /api/libraries/{id}  - Get library by ID
PUT    /api/libraries/{id}  - Update library
DELETE /api/libraries/{id}  - Delete library
POST   /api/libraries/{id}/items - Add item to library
```

### File Uploads
```
POST   /api/files/upload    - Upload file
GET    /uploads/{filename} - Get uploaded file
```

## 🗄️ Database Setup

Each service uses its own PostgreSQL database:

- **Auth Database**: `localhost:5432/AuthDb`
- **Movie Database**: `localhost:5433/MovieDb`
- **User Database**: `localhost:5434/UserDb`
- **Library Database**: `localhost:5435/LibraryDb`

### Database Migrations

Run migrations for each service:

```bash
# Auth Service
cd auth-service/Auth.API && dotnet ef database update

# Movie Service
cd movie-service/Movie.API && dotnet ef database update

# User Service
cd user-service/User.API && dotnet ef database update

# Library Service
cd library-service/Library.API && dotnet ef database update
```

## 🔧 Configuration

### Environment Variables

Each service can be configured via environment variables:

```bash
# Database connections
ConnectionStrings__DefaultConnection=Host=localhost;Database=AuthDb;Username=postgres;Password=password

# JWT Settings
JWT__Key=your-secret-key-here
JWT__Issuer=your-issuer
JWT__Audience=your-audience

# TMDB API (Movie Service)
TMDB__ApiKey=your-tmdb-api-key
TMDB__BaseUrl=https://api.themoviedb.org/3
```

### CORS Configuration

CORS is handled centrally by the API Gateway. In production, configure specific origins:

```csharp
// In ApiGateway/Program.cs
app.UseCors(policy =>
{
    policy.WithOrigins("https://your-frontend-domain.com")
          .AllowAnyMethod()
          .AllowAnyHeader()
          .AllowCredentials();
});
```

## 🧪 Testing

### API Testing

Use tools like Postman, Insomnia, or curl to test endpoints:

```bash
# Register a new user
curl -X POST "http://localhost:5000/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "password123",
    "firstName": "John",
    "lastName": "Doe"
  }'

# Login and get JWT token
curl -X POST "http://localhost:5000/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "password123"
  }'

# Use JWT token for authenticated requests
curl "http://localhost:5000/api/movies" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### Health Checks

Check service health:

```bash
# API Gateway
curl http://localhost:5000/health

# Individual services
curl http://localhost:5001/health
curl http://localhost:5002/health
# ... etc
```

## 📁 Project Structure

```
service/
├── api-gateway/           # API Gateway service
│   ├── ApiGateway/        # Main project
│   └── README.md
├── auth-service/          # Authentication service
│   └── Auth.API/
├── movie-service/         # Movie management service
│   └── Movie.API/
├── user-service/          # User profile service
│   └── User.API/
├── library-service/       # Library management service
│   └── Library.API/
├── file-service/          # File upload service
│   └── File.API/
├── docker-compose.yml     # Container orchestration
└── README.md             # This file
```

## 🔒 Security Features

- **JWT Authentication**: Token-based authentication across all services
- **Password Hashing**: BCrypt for secure password storage
- **CORS Protection**: Configurable cross-origin policies
- **Input Validation**: Model validation on all endpoints
- **SQL Injection Prevention**: Parameterized queries with EF Core

## 🚀 Deployment

### Production Considerations

1. **Load Balancing**: Configure multiple instances of each service
2. **Database Clustering**: Use PostgreSQL clustering for high availability
3. **Monitoring**: Add logging, metrics, and health checks
4. **Security**: Configure proper firewall rules and SSL/TLS
5. **Backup**: Implement automated database backups

### Docker Production Setup

```yaml
# docker-compose.prod.yml
version: '3.8'
services:
  api-gateway:
    image: moviehub/api-gateway:latest
    ports:
      - "80:5000"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
```

## 🤝 Contributing

1. Follow the existing code structure and patterns
2. Add appropriate tests for new features
3. Update documentation for API changes
4. Ensure all services build and run correctly

## 📝 License

This project is licensed under the MIT License.

---

Built with ❤️ using ASP.NET Core, YARP, and PostgreSQL