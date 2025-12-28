# MovieHub Frontend

Modern, responsive Next.js application for the MovieHub streaming platform with full microservices backend integration.

## ⚡ Quick Troubleshooting

**Seeing ERR_NAME_NOT_RESOLVED or requests to `user-service:5445`?**

This is a browser cache issue. Fix with these steps:

```bash
# 1. Stop the dev server (Ctrl+C)

# 2. Clear Next.js cache
cd front-end
rm -rf .next   # On Windows: Remove-Item -Recurse -Force .next

# 3. Clear browser cache
# In Chrome/Edge: Ctrl+Shift+Delete → Clear cached images and files

# 4. Restart dev server
npm run dev

# 5. Hard refresh browser
# Windows: Ctrl+F5
# Mac: Cmd+Shift+R
```

**All API requests should go to:** `http://localhost:5000/api/*`  
**Never to:** `https://user-service:5445` or any direct service URLs

---

## 🚀 Quick Start

```bash
# Install dependencies
npm install

# Set up environment
cp .env.example .env.local
# Edit .env.local with your configuration

# Run development server
npm run dev

# Build for production
npm run build

# Start production server
npm start
```

**Access:** http://localhost:3000

---

## 🏗️ Architecture Overview

Production-grade Next.js 16 frontend using the App Router, TypeScript, Redux Toolkit, and modern React patterns.

### Core Principles
- **API Gateway-Only Communication**: All requests go through API Gateway (`http://localhost:5000`)
- **Clean Separation of Concerns**: Components → Hooks → Services → Redux
- **Type Safety**: Strong TypeScript types for all data
- **Session Management**: Secure token storage with auto-refresh
- **Error Resilience**: Graceful degradation when services are unavailable

### Project Structure

```
front-end/
├── src/
│   ├── app/                    # Next.js App Router pages
│   │   ├── auth/               # Login, Register, Forgot Password
│   │   ├── browse/             # Home/Browse movies
│   │   ├── search/             # Search with filters
│   │   ├── movie/[id]/         # Movie details + player
│   │   ├── library/            # Personal library
│   │   ├── watch/[id]/         # Full-screen player
│   │   ├── upload/             # Movie upload (admin)
│   │   └── settings/           # User settings
│   │
│   ├── components/             # Reusable UI components
│   │   ├── layout/             # Navbar, SessionMonitor
│   │   ├── movie/              # MovieCard, MovieGrid, SearchBar
│   │   ├── player/             # VideoPlayer (HLS.js)
│   │   ├── torrent/            # TorrentSeedWidget (WebTorrent)
│   │   └── ui/                 # Radix UI primitives
│   │
│   ├── hooks/                  # Custom React hooks
│   │   ├── useAuth.ts          # Authentication with session
│   │   ├── useUser.ts          # User data & library
│   │   ├── useMovies.ts        # Movie catalog
│   │   └── useSearch.ts        # Search with debounce
│   │
│   ├── services/               # API clients (Axios)
│   │   ├── auth.service.ts     # /api/auth/*
│   │   ├── user.service.ts     # /api/users/*
│   │   ├── movie.service.ts    # /api/movies/*
│   │   ├── search.service.ts   # /api/search/*
│   │   ├── library.service.ts  # /api/library/*
│   │   └── upload.service.ts   # /api/upload/*
│   │
│   ├── store/                  # Redux Toolkit state
│   │   ├── index.ts            # Store configuration
│   │   └── slices/
│   │       ├── authSlice.ts    # JWT, user, auth state
│   │       ├── userSlice.ts    # Profile, library, favorites
│   │       ├── movieSlice.ts   # Catalog, trending, current
│   │       ├── searchSlice.ts  # Query, filters, results
│   │       ├── playerSlice.ts  # Playback state
│   │       ├── librarySlice.ts # Watch history
│   │       └── torrentSlice.ts # Torrent stats
│   │
│   ├── utils/                  # Utility functions
│   │   └── session.ts          # Session management
│   │
│   ├── lib/
│   │   ├── api.ts              # Configured Axios instance
│   │   └── utils.ts            # Helper functions
│   │
│   └── types/
│       └── index.ts            # TypeScript interfaces
```

---

## 🔐 Authentication & Session Management

### Session Features
- **Secure Token Storage**: Access & refresh tokens in localStorage
- **Auto Session Restore**: Resume session on page reload
- **Session Expiry**: 7-day sessions with auto-logout
- **Activity Monitoring**: Session extends on user activity
- **Remember Me**: Optional persistent login
- **Auto Token Refresh**: Handles expired tokens automatically

### Authentication Flow

```typescript
// 1. Login
const result = await login({ email, password }, rememberMe);
// → JWT stored, user data in Redux, redirect to home

// 2. Auto-restore on page load
// → SessionMonitor validates session
// → User data restored from localStorage

// 3. Token refresh on 401
// → API client auto-refreshes token
// → Retries failed request
// → Logout if refresh fails
```

### Session API

```typescript
import { getSession, saveSession, clearSession } from '@/utils/session';

// Get current session
const session = getSession(); // { user, accessToken, refreshToken, expiresAt }

// Check validity
if (isSessionValid()) {
  // Session is active
}

// Manual logout
clearSession();
```

---

## 📡 API Integration

### Environment Variables

Create `.env.local`:
```env
NEXT_PUBLIC_API_BASE_URL=http://localhost:5000
```

### Available Endpoints

All endpoints route through API Gateway at `http://localhost:5000`

#### Authentication (`/api/auth`)
```
POST   /api/auth/login              # Login user
POST   /api/auth/register           # Register new user
POST   /api/auth/logout             # Logout user
GET    /api/auth/me                 # Get current user
POST   /api/auth/refresh            # Refresh access token
POST   /api/auth/forgot-password    # Request password reset
POST   /api/auth/reset-password     # Reset password
```

#### Movies (`/api/movies`)
```
GET    /api/movies                  # Get all movies (filters, pagination)
GET    /api/movies/{id}             # Get movie by ID
GET    /api/movies/trending         # Get trending movies
GET    /api/movies/popular          # Get popular movies
GET    /api/movies/top-rated        # Get top rated movies
GET    /api/movies/genres           # Get all genres
GET    /api/movies/genre/{genre}    # Get movies by genre
GET    /api/movies/{id}/related     # Get related movies
POST   /api/movies                  # Create movie (admin)
PUT    /api/movies/{id}             # Update movie (admin)
DELETE /api/movies/{id}             # Delete movie (admin)
```

#### Search (`/api/search`)
```
GET    /api/search/movies           # Search movies
GET    /api/search/suggestions      # Get search suggestions
GET    /api/search/trending         # Get trending searches
GET    /api/search/recommendations  # Get recommendations
GET    /api/search/similar/{id}     # Get similar movies
```

#### Library (`/api/library`)
```
GET    /api/library                 # Get user library
POST   /api/library                 # Add movie to library
DELETE /api/library/{movieId}       # Remove from library
PUT    /api/library/{movieId}/favorite        # Toggle favorite
PUT    /api/library/{movieId}/progress        # Update watch progress
GET    /api/library/continue-watching         # Get continue watching
GET    /api/library/favorites                 # Get favorites
GET    /api/library/history                   # Get watch history
DELETE /api/library/history                   # Clear watch history
```

#### User (`/api/users`)
```
GET    /api/users/me                # Get user profile
PUT    /api/users/me                # Update user profile
POST   /api/users/me/avatar         # Upload avatar
GET    /api/users/me/library        # Get user library
```

---

## 🛠️ Tech Stack

### Core
- **Next.js 16** (App Router) - React 19
- **TypeScript** - Type safety
- **Tailwind CSS** - Styling
- **Redux Toolkit** - State management

### Features
- **HLS.js** - Adaptive video streaming
- **WebTorrent** - P2P torrent seeding
- **Radix UI** - Accessible components
- **React Hook Form** - Form handling
- **Zod** - Schema validation
- **Axios** - HTTP client

---

## 💻 Development Guide

### Using Services in Components

#### 1. Using Hooks (Recommended)

```typescript
import { useMovies } from '@/hooks/useMovies';

function MyComponent() {
  const { movies, isLoading, error, fetchMovies } = useMovies();

  useEffect(() => {
    fetchMovies();
  }, []);

  if (isLoading) return <LoadingSpinner />;
  if (error) return <ErrorMessage error={error} />;

  return <MovieList movies={movies} />;
}
```

#### 2. Using Services Directly

```typescript
import { movieService } from '@/services/movie.service';

async function loadMovie(id: string) {
  try {
    const response = await movieService.getMovieById(id);
    if (response.success && response.data) {
      console.log('Movie:', response.data);
    }
  } catch (error) {
    console.error('Failed to load movie:', error);
  }
}
```

#### 3. Using Redux

```typescript
import { useAppDispatch, useAppSelector } from '@/hooks/redux';
import { fetchMovies } from '@/store/slices/movieSlice';

function MyComponent() {
  const dispatch = useAppDispatch();
  const { movies, isLoading } = useAppSelector(state => state.movie);

  useEffect(() => {
    dispatch(fetchMovies());
  }, [dispatch]);

  return <MovieGrid movies={movies} />;
}
```

### Custom Hooks

#### `useAuth()`
```typescript
const { 
  user,                    // Current user data
  isAuthenticated,         // Auth status
  isLoading,              // Loading state
  login,                  // Login function
  register,               // Register function
  logout                  // Logout function
} = useAuth();

// Login with Remember Me
await login({ email, password }, rememberMe);
```

#### `useUser()`
```typescript
const {
  profile,                // User profile
  library,                // User library
  favorites,              // Favorite movies
  watchLater,             // Watch later list
  addToLibrary,           // Add to library
  removeFromLibrary,      // Remove from library
  toggleFavorite          // Toggle favorite
} = useUser();
```

#### `useSearch()`
```typescript
const {
  query,                  // Search query
  results,                // Search results
  isLoading,              // Loading state
  setQuery,               // Set query (debounced)
  applyFilters,           // Apply filters
  loadMore                // Load more results
} = useSearch();
```

---

## 🐳 Docker

```bash
# Build
docker build -t moviehub-frontend .

# Run
docker run -p 3000:3000 moviehub-frontend
```

---

## 🧪 Testing

```bash
# Run tests
npm test

# Run tests with coverage
npm run test:coverage

# Run E2E tests
npm run test:e2e
```

---

## 📦 Build & Deploy

```bash
# Build for production
npm run build

# Start production server
npm start

# Export static site (if applicable)
npm run export
```

---

## 🔧 Troubleshooting

### Session Issues
- **Session not persisting**: Check localStorage availability
- **Auto-logout**: Verify session expiry time in localStorage
- **Token refresh fails**: Check API Gateway and auth service

### API Connection
- **CORS errors**: Ensure API Gateway CORS is configured
- **401 errors**: Check token validity and refresh endpoint
- **Network errors**: Verify API Gateway is running on port 5000

### Common Errors

```typescript
// Error: Session expired
// Solution: Session auto-expires after 7 days or on inactivity
// User will be automatically logged out

// Error: Token refresh failed
// Solution: User needs to log in again
// Session will be cleared and redirect to login

// Error: API Gateway not responding
// Solution: Ensure docker-compose services are running:
docker-compose up -d
```

---

## 📝 Configuration

### Session Settings

```typescript
// In utils/session.ts
const SESSION_DURATION = 7 * 24 * 60 * 60 * 1000; // 7 days

// In components/layout/SessionMonitor.tsx
setInterval(checkSession, 60 * 1000);           // Check every 1 min
setTimeout(handleUserActivity, 5 * 60 * 1000);  // Extend every 5 min
```

### API Client

```typescript
// In lib/api.ts
const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL || 'http://localhost:5000';
timeout: 30000,  // 30 seconds
```

---

## 🚦 Features

### ✅ Implemented
- ✅ Authentication (Login, Register, Logout)
- ✅ Session Management (Auto-restore, expiry, refresh)
- ✅ Movie Browsing (Trending, Popular, Top-rated)
- ✅ Search & Filters
- ✅ Movie Details
- ✅ Personal Library (Favorites, Watch Later)
- ✅ HLS Video Player
- ✅ WebTorrent Seeding
- ✅ Responsive Design
- ✅ Dark Mode

### 🔄 In Progress
- Password Reset Flow
- User Profile Settings
- Avatar Upload
- Admin Panel

---

## 📄 License

Copyright © 2025 MovieHub. All rights reserved.

---

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

---

## 📞 Support

For issues and questions:
- Create an issue on GitHub
- Check existing documentation
- Review API integration guide

---

**Built with ❤️ using Next.js, TypeScript, and Redux Toolkit**
