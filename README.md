# 🎬 MovieHub - Distributed Movie Streaming Platform

A modern, scalable movie streaming platform built with microservices architecture, featuring Next.js frontend and .NET backend services.

## 🏗️ Architecture Overview

```
┌─────────────────────────────────────────────────────────────────────────┐
│                           User's Browser                                 │
└────────────┬──────────────────────────────────┬─────────────────────────┘
             │                                    │
             │ Port 3000                          │ Port 5001
             ▼                                    ▼
┌─────────────────────────┐          ┌────────────────────────────────────┐
│   Next.js Frontend      │          │      API Gateway (Future)          │
│  ┌──────────────────┐   │          │  ┌──────────────────────────────┐  │
│  │  React UI        │───┼──────────┼─▶│  Ocelot/YARP Router          │  │
│  │  Redux Store     │   │          │  └──────────────────────────────┘  │
│  │  Axios Client    │   │          │              │                      │
│  │  Video Player    │   │          │              │                      │
│  └──────────────────┘   │          └──────────────┼──────────────────────┘
└─────────────────────────┘                         │
                                                    │
                    ┌───────────────────────────────┼─────────────────────┐
                    ▼                               ▼                     ▼
        ┌────────────────────┐          ┌────────────────────┐  ┌─────────────────┐
        │   Auth Service     │          │   Movie Service    │  │  Upload Service │
        │   ✅ Running       │          │   🔄 Coming Soon   │  │  🔄 Coming Soon │
        │   Port: 5001       │          │   Port: 5003       │  │  Port: 5004     │
        │ ┌────────────────┐ │          │ ┌────────────────┐ │  │ ┌─────────────┐ │
        │ │ • Registration │ │          │ │ • TMDB API     │ │  │ │ • GCS Upload│ │
        │ │ • Login        │ │          │ │ • Search       │ │  │ │ • Transcoding│ │
        │ │ • JWT Auth     │ │          │ │ • Metadata     │ │  │ │ • Validation│ │
        │ │ • Refresh      │ │          │ │ • Categories   │ │  │ │             │ │
        │ └────────────────┘ │          │ └────────────────┘ │  │ └─────────────┘ │
        └──────────┬─────────┘          └──────────┬─────────┘  └────────┬────────┘
                   │                               │                      │
    ┌──────────────┴──────────┬────────────────────┴──────────────────────┴─────┐
    ▼                         ▼                                 ▼                 ▼
┌──────────────┐    ┌──────────────────┐         ┌──────────────────┐  ┌──────────────┐
│ PostgreSQL   │    │  Redis Cache     │         │  Kafka/Events    │  │  Monitoring  │
│ Port: 5432   │    │  Port: 6379      │         │  Port: 9092      │  │              │
│              │    │                  │         │                  │  │ Prometheus   │
│ • Users      │    │ • Session Cache  │         │ • Event Bus      │  │ Grafana      │
│ • Movies     │    │ • API Cache      │         │ • Pub/Sub        │  │              │
│ • Library    │    │ • Rate Limiting  │         │                  │  │              │
└──────────────┘    └──────────────────┘         └──────────────────┘  └──────────────┘
```

## 🎯 Project Overview

MovieHub is a comprehensive movie streaming platform that allows users to:

- **Browse & Search** movies from TMDB database
- **Upload** their own content with automatic transcoding
- **Stream** movies with adaptive bitrate (HLS)
- **Manage** personal libraries and watchlists
- **Seed & Download** using WebTorrent P2P technology
- **Track** watch history and progress

### Key Features

✅ **Implemented:**
- User Authentication (Register, Login, JWT)
- Responsive Next.js Frontend
- Docker Containerization
- PostgreSQL Database
- Redis Caching
- Health Checks & Monitoring

🔄 **In Progress:**
- Movie Metadata Service
- Video Upload & Processing
- Streaming Service
- Library Management
- Torrent Integration

## 🚀 Quick Start

### Prerequisites

- Docker Desktop
- Node.js 20+ (for local development)
- .NET 8.0 SDK (for local development)

### Start with Docker (Recommended)

```bash
# Clone the repository
git clone <repository-url>
cd movieHub

# Start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop all services
docker-compose down
```

**Access the application:**
- Frontend: http://localhost:3000
- Backend API: http://localhost:5001
- Swagger UI: http://localhost:5001/swagger
- Grafana: http://localhost:3001
- Prometheus: http://localhost:9090

## 📁 Project Structure

```
movieHub/
├── docker-compose.yml          # Single file to run everything
├── README.md                   # This file
├── FRONTEND.md                 # Frontend documentation
├── BACKEND.md                  # Backend services documentation
│
├── front-end/                  # Next.js Application
│   ├── src/
│   │   ├── app/               # Next.js 13+ App Router
│   │   ├── components/        # React Components
│   │   ├── hooks/            # Custom React Hooks
│   │   ├── services/         # API Service Layer
│   │   ├── store/            # Redux Store
│   │   └── types/            # TypeScript Types
│   ├── Dockerfile
│   └── package.json
│
└── Back-end/                   # .NET Microservices
    ├── src/
    │   ├── Services/
    │   │   └── Auth/          # ✅ Authentication Service
    │   │       ├── API/       # REST Controllers
    │   │       ├── Application/ # CQRS Handlers
    │   │       ├── Domain/    # Business Logic
    │   │       └── Infrastructure/ # Data Access
    │   │
    │   └── Shared/
    │       └── MovieHub.Shared.Kernel/  # Common utilities
    │
    ├── monitoring/             # Prometheus & Grafana configs
    ├── scripts/               # Database init scripts
    └── MovieHub.sln
```

## 🛠️ Technology Stack

### Frontend
- **Framework:** Next.js 16 (React 19)
- **Language:** TypeScript
- **State Management:** Redux Toolkit
- **Styling:** Tailwind CSS
- **UI Components:** Radix UI, Shadcn/ui
- **Video Player:** HLS.js
- **P2P:** WebTorrent
- **HTTP Client:** Axios
- **Form Validation:** React Hook Form + Zod

### Backend
- **Framework:** ASP.NET Core 8.0
- **Architecture:** Clean Architecture + DDD + CQRS
- **Language:** C# 12
- **Database:** PostgreSQL 16
- **Cache:** Redis 7
- **Message Broker:** Apache Kafka
- **Authentication:** JWT Bearer
- **Validation:** FluentValidation
- **Mapping:** Automapper
- **ORM:** Entity Framework Core
- **Logging:** Serilog
- **Monitoring:** Prometheus + Grafana
- **API Documentation:** Swagger/OpenAPI

### Infrastructure
- **Containerization:** Docker & Docker Compose
- **Orchestration (Planned):** Kubernetes
- **Cloud (Planned):** Google Cloud Platform
- **Storage (Planned):** Google Cloud Storage
- **CDN (Planned):** Google Cloud CDN

## 📡 API Endpoints

### Authentication Service (✅ Running)

| Method | Endpoint | Description | Status |
|--------|----------|-------------|--------|
| POST | `/api/v1/Auth/register` | Register new user | ✅ |
| POST | `/api/v1/Auth/login` | User login | ✅ |
| POST | `/api/v1/Auth/refresh` | Refresh access token | ✅ |
| POST | `/api/v1/Auth/logout` | User logout | ✅ |
| GET | `/health` | Health check | ✅ |

**Example: Register User**
```json
POST http://localhost:5001/api/v1/Auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePass@123",
  "firstName": "John",
  "lastName": "Doe"
}

Response: 200 OK
{
  "success": true,
  "data": {
    "userId": "ba419212-c052-4c33-a24c-5aec0cd4434f",
    "email": "user@example.com",
    "accessToken": "eyJhbGc...",
    "refreshToken": "3f2504e0..."
  }
}
```

**Example: Login**
```json
POST http://localhost:5001/api/v1/Auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePass@123"
}

Response: 200 OK
{
  "success": true,
  "data": {
    "userId": "ba419212-c052-4c33-a24c-5aec0cd4434f",
    "email": "user@example.com",
    "accessToken": "eyJhbGc...",
    "refreshToken": "3f2504e0..."
  }
}
```

### Future Services (🔄 Planned)

#### Movie Service
- `GET /api/v1/Movies` - List movies
- `GET /api/v1/Movies/{id}` - Get movie details
- `GET /api/v1/Movies/search` - Search movies
- `GET /api/v1/Movies/trending` - Get trending movies

#### Upload Service
- `POST /api/v1/Upload` - Upload video file
- `GET /api/v1/Upload/{id}/status` - Check upload status
- `DELETE /api/v1/Upload/{id}` - Cancel upload

#### Streaming Service
- `GET /api/v1/Stream/{id}/master.m3u8` - HLS master playlist
- `GET /api/v1/Stream/{id}/quality/{quality}.m3u8` - Quality playlist
- `GET /api/v1/Stream/{id}/segment/{segment}.ts` - Video segment

#### Library Service
- `GET /api/v1/Library` - Get user library
- `POST /api/v1/Library/{movieId}` - Add to library
- `DELETE /api/v1/Library/{movieId}` - Remove from library

## 🔒 Authentication

The platform uses JWT (JSON Web Tokens) for authentication:

1. **Register** or **Login** to receive an access token
2. Include the token in subsequent requests:
   ```
   Authorization: Bearer <access-token>
   ```
3. Access tokens expire after 60 minutes
4. Use refresh token to get a new access token without re-login

## 🧪 Testing

### Test the Backend API

```bash
# Health check
curl http://localhost:5001/health

# Register a user
curl -X POST http://localhost:5001/api/v1/Auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test@123456",
    "firstName": "Test",
    "lastName": "User"
  }'

# Login
curl -X POST http://localhost:5001/api/v1/Auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test@123456"
  }'
```

### Test the Frontend

1. Open http://localhost:3000
2. Navigate to `/auth/register`
3. Create a new account
4. Login and explore the UI

## 📊 Monitoring

- **Prometheus**: http://localhost:9090
  - Metrics collection and querying
  - Service health monitoring
  
- **Grafana**: http://localhost:3001
  - Username: `admin`
  - Password: `admin`
  - Visualize metrics and create dashboards

## 🔧 Configuration

### Environment Variables

**Frontend** (`.env.local`):
```env
NEXT_PUBLIC_API_URL=http://localhost:5001/api/v1
```

**Backend** (`.env`):
```env
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=moviehub_auth;Username=postgres;Password=postgres
JwtSettings__Secret=your-secret-key-min-32-characters
JwtSettings__Issuer=MovieHub.AuthService
JwtSettings__Audience=MovieHub.Client
JwtSettings__AccessTokenExpirationMinutes=60
JwtSettings__RefreshTokenExpirationDays=7
```

## 📦 Docker Services

| Service | Image | Port | Purpose |
|---------|-------|------|---------|
| frontend | custom | 3000 | Next.js Web UI |
| auth-service | custom | 5001 | Authentication API |
| postgres | postgres:16-alpine | 5432 | Database |
| redis | redis:7-alpine | 6379 | Cache |
| kafka | cp-kafka:7.5.3 | 9092 | Message Broker |
| zookeeper | cp-zookeeper:7.5.3 | 2181 | Kafka Coordinator |
| prometheus | prom/prometheus | 9090 | Metrics |
| grafana | grafana/grafana | 3001 | Monitoring UI |

## 🚧 Roadmap

### Phase 1: Core Infrastructure ✅
- [x] Docker setup
- [x] Authentication service
- [x] Database configuration
- [x] Frontend scaffolding
- [x] Monitoring setup

### Phase 2: Movie Features 🔄
- [ ] Movie metadata service (TMDB integration)
- [ ] Search and browse functionality
- [ ] Movie details page
- [ ] Categories and filters

### Phase 3: Streaming 📅
- [ ] Video upload service
- [ ] Video transcoding (FFmpeg)
- [ ] HLS streaming
- [ ] Adaptive bitrate
- [ ] Google Cloud Storage integration

### Phase 4: User Features 📅
- [ ] Personal library
- [ ] Watch history
- [ ] Favorites and watchlist
- [ ] User profiles

### Phase 5: P2P & Advanced 📅
- [ ] WebTorrent integration
- [ ] Torrent seeding
- [ ] Bandwidth optimization
- [ ] CDN integration

### Phase 6: Production 📅
- [ ] Kubernetes deployment
- [ ] CI/CD pipeline
- [ ] SSL/TLS certificates
- [ ] Load balancing
- [ ] Horizontal scaling

## 🐛 Troubleshooting

### Services won't start
```bash
# Check Docker daemon
docker ps

# View service logs
docker-compose logs -f [service-name]

# Rebuild services
docker-compose build --no-cache
docker-compose up -d
```

### Database connection issues
```bash
# Check PostgreSQL
docker exec moviehub-postgres psql -U postgres -c "\l"

# Check database tables
docker exec moviehub-postgres psql -U postgres -d moviehub_auth -c "\dt"
```

### Port conflicts
```bash
# Find process using port
netstat -ano | findstr :3000
netstat -ano | findstr :5001

# Kill process (Windows)
taskkill /PID <PID> /F
```

## 📚 Documentation

- **[FRONTEND.md](FRONTEND.md)** - Detailed frontend documentation
- **[BACKEND.md](BACKEND.md)** - Backend services and architecture
- **[Swagger UI](http://localhost:5001/swagger)** - Interactive API docs (when running)

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License.

## 👥 Authors

- Your Name - Initial work

## 🙏 Acknowledgments

- TMDB for movie metadata API
- Radix UI for accessible components
- The open-source community

---

**Status**: 🟢 Development Active | ✅ Core Features Working | 🚀 Ready for Local Testing

**Last Updated**: December 14, 2025
