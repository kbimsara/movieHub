# MovieHub

A self-hosted video streaming platform built with a microservices architecture. Upload, catalog, and watch movies with HLS adaptive streaming, direct download, and P2P torrent seeding — all from a single Docker Compose command.

---

## Table of Contents

- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Quick Start (Docker)](#quick-start-docker)
- [Local Development (Without Docker)](#local-development-without-docker)
- [Default Credentials](#default-credentials)
- [Features](#features)
- [API Routes](#api-routes)
- [Environment Variables](#environment-variables)
- [Database](#database)
- [Security Notes](#security-notes)

---

## Architecture

```
Browser
  └── Next.js Frontend  (localhost:3000)
            │
            ▼  all requests → localhost:5000
  ┌─────────────────────────────────┐
  │     API Gateway  (YARP)         │
  │         port 5000               │
  └──┬──────────┬──────────┬────────┘
     │          │          │         │
     ▼          ▼          ▼         ▼
 Auth       Movie       File      User
Service    Service    Service   Service
:5001       :5002      :5003     :5004
     │          │          │         │
     └──────────┴──────────┴─────────┘
                       │
                  PostgreSQL 16
                    :5432
```

All frontend traffic goes through the API Gateway (YARP reverse proxy), which routes to the appropriate microservice based on the URL path.

---

## Tech Stack

### Frontend
| Technology | Version | Purpose |
|---|---|---|
| Next.js | 16.0.10 | React framework (App Router, Turbopack) |
| React | 19.2.1 | UI library |
| TypeScript | 5 | Type safety |
| Redux Toolkit | 2.x | Global state (auth, movies, library, player) |
| Tailwind CSS | 4 | Styling |
| Radix UI | latest | Accessible headless components |
| HLS.js | 1.x | Adaptive HTTP Live Streaming |
| WebTorrent | 2.x | P2P torrent seeding |
| React Hook Form + Zod | latest | Form handling and validation |
| Axios | 1.x | HTTP client |

### Backend
| Technology | Version | Purpose |
|---|---|---|
| ASP.NET Core | .NET 8 / 9 | Microservice APIs |
| YARP | latest | API Gateway / reverse proxy |
| Entity Framework Core | latest | ORM |
| PostgreSQL | 16 | Primary database |
| JWT | — | Authentication tokens |

### Infrastructure
| Technology | Purpose |
|---|---|
| Docker & Docker Compose | Containerisation and orchestration |
| Docker Bridge Network | Internal service-to-service communication |

---

## Project Structure

```
movieHub/
├── docker-compose.yml          # Orchestrates all 7 containers
├── front-end/                  # Next.js 16 application
│   ├── src/
│   │   ├── app/                # Pages (App Router)
│   │   │   ├── auth/           # Login, Register, Password Reset
│   │   │   ├── browse/         # Home / catalog browsing
│   │   │   ├── movie/[id]/     # Movie detail page
│   │   │   ├── watch/[id]/     # Full-screen video player
│   │   │   ├── library/        # Personal library, favourites, history
│   │   │   ├── upload/         # Movie upload with auto-detection
│   │   │   ├── search/         # Full-text search with filters
│   │   │   └── settings/       # User profile & preferences
│   │   ├── components/         # Reusable UI components
│   │   │   ├── movie/          # MovieCard, MovieRow
│   │   │   └── player/         # VideoPlayer (HLS + MP4)
│   │   ├── hooks/              # Custom React hooks
│   │   ├── services/           # API service layer
│   │   ├── store/              # Redux slices
│   │   └── types/              # TypeScript interfaces
│   └── next.config.ts
└── service/                    # .NET microservices
    ├── apiGateway/             # YARP reverse proxy (.NET 8)
    ├── authService/            # Authentication (.NET 8)
    ├── movieService/           # Movie catalog (.NET 8)
    ├── fileService/            # File uploads & serving (.NET 8)
    ├── userService/            # User profiles & library (.NET 9)
    └── MovieHub.Services.sln   # Combined Visual Studio solution
```

---

## Quick Start (Docker)

**Prerequisites:** Docker Desktop

```bash
# Clone the repository
git clone https://github.com/your-username/movieHub.git
cd movieHub

# Build and start all services
docker compose up -d --build
```

Once all containers are healthy, open **http://localhost:3000** in your browser.

| Service | URL |
|---|---|
| Frontend | http://localhost:3000 |
| API Gateway | http://localhost:5000 |
| Auth Service | http://localhost:5001 |
| Movie Service | http://localhost:5002 |
| File Service | http://localhost:5003 |
| User Service | http://localhost:5004 |
| PostgreSQL | localhost:5432 |

### Stop everything
```bash
docker compose down
```

### Stop and remove volumes (full reset)
```bash
docker compose down -v
```

---

## Local Development (Without Docker)

You need **.NET 8 SDK**, **Node.js 20+**, and a running **PostgreSQL 16** instance.

### 1. Start PostgreSQL

```bash
# Via Docker (easiest)
docker run -d \
  --name moviehub-db \
  -e POSTGRES_USER=moviehub \
  -e POSTGRES_PASSWORD=moviehub \
  -e POSTGRES_DB=moviehub \
  -p 5432:5432 \
  postgres:16
```

### 2. Start backend services (separate terminals)

```bash
# API Gateway — port 5000
dotnet run --project service/apiGateway/ApiGateway.csproj

# Auth Service — port 5001
dotnet run --project service/authService/WebApplication1.csproj

# Movie Service — port 5002
dotnet run --project service/movieService/MovieService.csproj

# File Service — port 5003
dotnet run --project service/fileService/FileService.csproj

# User Service — port 5004
dotnet run --project service/userService/UserService.csproj
```

Or open `service/MovieHub.Services.sln` in Visual Studio and use a **multi-start profile**.

### 3. Start the frontend

```bash
cd front-end
npm install
npm run dev       # http://localhost:3000
```

---

## Default Credentials

The auth service automatically seeds a demo admin account on first startup:

| Email | Password | Role |
|---|---|---|
| `demo@moviehub.local` | `Pass@123` | `admin` |

You can sign in with these credentials immediately, or register a new account — registrations are persisted to PostgreSQL.

---

## Features

### 🎥 Video Playback
- **HLS adaptive streaming** — quality adjusts automatically to bandwidth
- **Direct MP4 streaming** — range-request support for seeking in non-HLS files
- **P2P torrent seeding** — via WebTorrent (magnet link support)
- Custom video player with scrub bar, buffering indicator, volume, and fullscreen

### 📤 Movie Upload
- Drag-and-drop video files (MP4, MKV, AVI, MOV, WMV — up to 10 GB)
- **Auto-detection from filename** — title, year, quality, and duration extracted automatically
  - `The.Dark.Knight.2008.1080p.BluRay.x264.mp4` → Title: *The Dark Knight*, Year: *2008*
- **Auto-detection from video file** — duration and resolution read from the browser's video element (10-second timeout fallback)
- Poster image upload with live preview
- Upload checklist shows completion status in real time

### 📚 Personal Library
- Add/remove movies from your library
- Favourites and Watch Later lists
- Watch progress tracking (auto-saved during playback)
- Continue Watching section (in-progress movies)
- Full watch history

### 🔍 Search & Discovery
- Full-text movie search
- Filter by genre, year, quality, and rating
- Sort by title, year, rating, views, or date added
- Recently added, All Movies, and My Library rows on the home page

### 👤 User Accounts
- Register / login / logout with JWT + refresh tokens
- Avatar upload
- Profile editing
- Session persists across browser reloads (localStorage)

---

## API Routes

All routes go through the API Gateway at `http://localhost:5000`.

| Prefix | Forwarded to | Description |
|---|---|---|
| `/api/auth/*` | Auth Service `:5001` | Login, register, token refresh |
| `/api/movies/*` | Movie Service `:5002` | CRUD, search, trending |
| `/api/files/*` | File Service `:5003` | File uploads and serving |
| `/api/upload/*` | File Service `:5003` | Upload status / management |
| `/api/library/*` | User Service `:5004` | Rewritten → `/api/me/*` |
| `/api/search/*` | Movie Service `:5002` | Rewritten → `/api/movies/*` |

---

## Environment Variables

The docker-compose file handles all environment variables automatically. For local development without Docker, the defaults below apply:

### All .NET services
```
ConnectionStrings__Default=Host=localhost;Port=5432;Database=moviehub;Username=moviehub;Password=moviehub
ASPNETCORE_ENVIRONMENT=Development
```

### Auth Service (additional)
```
Jwt__Key=LocalDevelopmentSuperSecretKey123!
Jwt__Issuer=MovieHub.AuthService
Jwt__Audience=MovieHub.Frontend
```

### User Service (additional)
```
Jwt__Key=LocalDevelopmentSuperSecretKey123!
Jwt__Issuer=MovieHub.AuthService
Jwt__Audience=MovieHub.Frontend
MovieService__BaseUrl=http://localhost:5002
```

### File Service (additional)
```
Storage__RootPath=/app/Uploads
Storage__PublicBaseUrl=http://localhost:5000
MovieService__BaseUrl=http://localhost:5002
```

### Frontend
```
NEXT_PUBLIC_API_BASE_URL=http://localhost:5000
```

---

## Database

A single **PostgreSQL 16** database (`moviehub`) is shared by all services. Each service manages its own tables via Entity Framework Core migrations that run automatically on startup.

**Key tables:**

| Table | Service | Description |
|---|---|---|
| `Users` | Auth | Email, username, password hash, role |
| `RefreshTokens` | Auth | JWT refresh token storage |
| `Movies` | Movie | Title, description, genres, URLs, metadata |
| `CastMembers` | Movie | Actor names linked to movies |
| `LibraryItems` | User | User's saved movies + watch progress |
| `WatchHistory` | User | Historical viewing records |

**Docker volume:** `posgraph-data` — data persists across container restarts.
Run `docker compose down -v` to wipe the database and start fresh.

---

## Adding a New Microservice

1. Create the service under `service/<your-service>/`
2. Add the `.csproj` to `service/MovieHub.Services.sln`
3. Add a new route + cluster in `service/apiGateway/appsettings.json`
4. Add the service to `docker-compose.yml` with `depends_on: posgraph`
5. Call it from the frontend via `/api/<your-prefix>/*`

---

## Security Notes

- **JWT key**: The `Jwt__Key` value in `docker-compose.yml` is for local development only. Override it with a strong random secret (32+ characters) before deploying to any shared or production environment.
- **Demo account**: The auth service seeds `demo@moviehub.local / Pass@123` on the very first startup (when the `Users` table is empty). Change or remove this account after initial setup.
- **Swagger UI**: Swagger is only exposed in the `Development` environment. Set `ASPNETCORE_ENVIRONMENT=Production` to disable it.
- **File uploads**: All upload endpoints require a valid JWT. Anonymous upload requests are rejected with `401 Unauthorized`.
- **Database password**: The default PostgreSQL password (`moviehub`) in `docker-compose.yml` is suitable for local development only — use a strong password and secrets management in production.
