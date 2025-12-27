# MovieHub Frontend Architecture

## 🏗️ Architecture Overview

This is a production-grade Next.js 16 frontend using the App Router, TypeScript, Redux Toolkit, and modern React patterns.

### Core Principles
- **API Gateway-Only Communication**: All requests go through `http://localhost:5000` (API Gateway)
- **Clean Separation of Concerns**: Components → Hooks → Services → Redux
- **Type Safety**: Strong TypeScript types for all data
- **Optimistic Updates**: UI updates before API confirmation where appropriate
- **Error Resilience**: Graceful degradation when services are unavailable

---

## 📁 Project Structure

```
front-end/
├── src/
│   ├── app/                    # Next.js App Router pages
│   │   ├── auth/
│   │   │   ├── login/
│   │   │   └── register/
│   │   ├── browse/             # Home/browse movies
│   │   ├── search/             # Search with filters
│   │   ├── movie/[id]/         # Movie details + player
│   │   ├── library/            # Personal library
│   │   ├── watch/[id]/         # Full-screen player
│   │   ├── upload/             # Movie upload (admin)
│   │   └── settings/           # User settings
│   │
│   ├── components/             # Reusable UI components
│   │   ├── layout/
│   │   │   └── Navbar.tsx      # Navigation with auth state
│   │   ├── movie/
│   │   │   ├── MovieCard.tsx
│   │   │   ├── MovieGrid.tsx
│   │   │   ├── MovieRow.tsx
│   │   │   └── SearchBar.tsx
│   │   ├── player/
│   │   │   └── VideoPlayer.tsx # HLS.js integration
│   │   ├── torrent/
│   │   │   └── TorrentSeedWidget.tsx # WebTorrent
│   │   └── ui/                 # Radix UI primitives
│   │
│   ├── hooks/                  # Custom React hooks
│   │   ├── redux.ts            # Typed Redux hooks
│   │   ├── useAuth.ts          # Authentication
│   │   ├── useUser.ts          # User data & library
│   │   ├── useMovies.ts        # Movie catalog
│   │   └── useSearch.ts        # Search with debounce
│   │
│   ├── services/               # API clients (Axios)
│   │   ├── auth.service.ts     # /api/auth/*
│   │   ├── user.service.ts     # /api/users/*
│   │   ├── movie.service.ts    # /api/movies/*
│   │   └── search.service.ts   # /api/search/*
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
│   ├── lib/
│   │   ├── axios.ts            # Configured Axios instance
│   │   └── utils.ts            # Utility functions
│   │
│   └── types/
│       └── index.ts            # TypeScript interfaces
```

---

## 🔐 Authentication Flow

### 1. **Login/Register**
```typescript
// User submits credentials
const result = await login({ email, password });

// On success:
// - JWT tokens stored in localStorage
// - User data in Redux authSlice
// - Axios adds Bearer token to all requests
// - Redirect to /browse
```

### 2. **Token Management**
```typescript
// Axios interceptor automatically:
// - Attaches JWT to all requests
// - Refreshes token on 401
// - Dispatches logout event if refresh fails
```

### 3. **Auto-Logout**
```typescript
// useAuth hook listens for 'auth:logout' event
// Triggered by axios interceptor on 401
// Clears Redux state + localStorage
// Redirects to /auth/login
```

### 4. **Protected Routes**
```typescript
// Hooks check auth state
if (!isAuthenticated) {
  router.push('/auth/login');
}
```

---

## 🎬 Video Streaming (HLS.js)

### Integration
```typescript
<VideoPlayer 
  movie={currentMovie}
  onProgressUpdate={(progress) => updateProgress(movieId, progress)}
/>
```

### Features
- **Adaptive Bitrate**: Automatic quality switching
- **Progress Tracking**: Saves position every few seconds
- **Resume Playback**: Continue from last position
- **Error Recovery**: Network & media error handling

### Player State (Redux)
```typescript
{
  isPlaying: boolean,
  currentTime: number,
  duration: number,
  volume: number,
  isMuted: boolean,
  quality: string,
  selectedSubtitle: string,
  isFullscreen: boolean
}
```

---

## 🧲 WebTorrent Integration

### TorrentSeedWidget
```typescript
<TorrentSeedWidget 
  movieId={movie.id}
  magnetURI={movie.torrentMagnet}
/>
```

### Features
- Seed after watching
- Display peers, upload/download stats
- Torrent health indicators
- Background seeding

---

## 🔍 Search Implementation

### Debounced Search
```typescript
const { searchMovies, updateFilters, loadMore } = useSearch();

// Debounces automatically (300ms)
searchMovies(query);

// Apply filters
updateFilters({ genres: ['Action'], year: 2024 });

// Infinite scroll
loadMore(); // Appends results
```

### Search Filters
- **Text Query**: Full-text search
- **Genres**: Multiple selection
- **Year**: Single select
- **Quality**: 480p, 720p, 1080p, 4K
- **Sort**: Relevance, Title, Year, Rating, Views
- **Pagination**: Page-based with "Load More"

---

## 📚 Library Management

### User Library
```typescript
const { library, addToLibrary, updateProgress } = useUser();

// Add to library
await addToLibrary(movieId);

// Track progress
await updateProgress(movieId, 45); // 45%

// Toggle favorite
await toggleFavorite(movieId);
```

### Library Features
- **Continue Watching**: Movies with progress > 0%
- **Favorites**: Heart icon, filtered view
- **Watch Later**: Bookmarked movies
- **Watch History**: Recently watched, chronological

---

## 🌐 API Client Configuration

### Base Setup (axios.ts)
```typescript
const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 30000,
  headers: { 'Content-Type': 'application/json' },
});
```

### Request Interceptor
```typescript
// Attach JWT to all requests
config.headers.Authorization = `Bearer ${localStorage.getItem('accessToken')}`;
```

### Response Interceptor
```typescript
// On 401: Try refresh token
// On refresh fail: Dispatch logout event
// On 404: Service not implemented (log warning)
// On ECONNREFUSED: Service unavailable (fallback)
```

---

## 🎨 UI Components (Radix UI + Tailwind)

### Component Library
- **Button**: Primary, secondary, outline variants
- **Input**: Text, password, email with validation
- **Dialog**: Modals, confirmations
- **Dropdown**: Menus, selects
- **Tabs**: Content organization
- **Progress**: Video playback, upload status
- **Skeleton**: Loading states
- **Toast**: Notifications

### Design System
- **Dark Mode**: Default theme
- **Responsive**: Mobile-first design
- **Accessible**: ARIA labels, keyboard navigation
- **Consistent**: Shared spacing, colors, typography

---

## 📤 Upload Flow (UI Ready)

### Upload Page
```typescript
// Form with:
// - File dropzone (react-dropzone)
// - Metadata inputs (title, description, genres, etc.)
// - Poster upload
// - Progress bar

// Backend integration pending
```

---

## 🚀 Redux Toolkit Slices

### authSlice
```typescript
{
  user: User | null,
  accessToken: string | null,
  refreshToken: string | null,
  isAuthenticated: boolean,
  isLoading: boolean,
  error: string | null
}
```

### userSlice
```typescript
{
  profile: UserProfile | null,
  library: LibraryItem[],
  watchHistory: WatchHistory[],
  favoriteMovies: Movie[],
  watchLater: Movie[]
}
```

### searchSlice
```typescript
{
  query: string,
  results: Movie[],
  filters: SearchFilters,
  page: number,
  totalPages: number,
  hasMore: boolean
}
```

### movieSlice
```typescript
{
  movies: Movie[],
  currentMovie: Movie | null,
  trending: Movie[],
  popular: Movie[],
  topRated: Movie[]
}
```

---

## 🔒 Security Best Practices

1. **JWT Storage**: localStorage (for demo; use HttpOnly cookies in prod)
2. **XSS Prevention**: React auto-escapes, no dangerouslySetInnerHTML
3. **CSRF**: API Gateway handles CORS
4. **Token Refresh**: Automatic, seamless
5. **Logout on 401**: Prevents stale sessions

---

## 🧪 Error Handling Strategy

### Service Layer
```typescript
try {
  const response = await apiClient.get('/api/movies');
  return response.data;
} catch (error) {
  if (error.response?.status === 404) {
    console.warn('Endpoint not implemented');
    return mockData; // Optional fallback
  }
  throw error;
}
```

### Component Layer
```typescript
const { data, error, isLoading } = useMovies();

if (error) return <ErrorMessage />;
if (isLoading) return <LoadingSkeleton />;
return <MovieGrid movies={data} />;
```

---

## 🎯 Key Features Implemented

✅ JWT authentication with auto-refresh  
✅ API Gateway-only communication  
✅ Redux Toolkit state management  
✅ HLS.js video player with controls  
✅ WebTorrent seeding widget  
✅ Debounced search with filters  
✅ Personal library with progress tracking  
✅ Movie details page with tabs  
✅ Responsive, accessible UI  
✅ TypeScript throughout  

---

## 🔮 Integration Points for Backend

### Auth Service
- `POST /api/auth/login` → Returns user + tokens
- `POST /api/auth/register` → Creates user
- `POST /api/auth/refresh` → Refreshes access token
- `GET /api/auth/me` → Gets current user

### User Service
- `GET /api/users/me` → User profile
- `GET /api/users/me/library` → Library items
- `POST /api/users/me/library` → Add to library
- `PUT /api/users/me/library/{id}/progress` → Update progress

### Catalog Service
- `GET /api/movies` → List movies (with filters)
- `GET /api/movies/{id}` → Movie details
- `GET /api/movies/trending` → Trending movies
- `POST /api/movies` → Create movie (admin)

### Search Service
- `GET /api/search/movies?q=...&genre=...&year=...` → Search results
- `GET /api/search/suggestions?q=...` → Autocomplete

---

## 🛠️ Environment Variables

```env
# .env.local
NEXT_PUBLIC_API_URL=http://localhost:5000
```

---

## 🚦 Running the Frontend

```bash
npm install
npm run dev
# → http://localhost:3000
```

---

## 📝 Notes for Backend Integration

1. **API Response Format**: All services should return:
   ```json
   {
     "success": true,
     "data": { ... },
     "message": "Optional message"
   }
   ```

2. **Error Format**:
   ```json
   {
     "success": false,
     "error": "Error message",
     "statusCode": 400
   }
   ```

3. **Pagination Format**:
   ```json
   {
     "data": [...],
     "total": 100,
     "page": 1,
     "limit": 20,
     "totalPages": 5
   }
   ```

4. **JWT Format**: Standard Bearer token in Authorization header

---

## 🎓 Architecture Decisions

### Why Redux Toolkit?
- Centralized state for auth, user data, movies
- Easy to debug with Redux DevTools
- Persistent state across page navigation
- Optimistic updates

### Why Axios over Fetch?
- Interceptors for auth token injection
- Better error handling
- Request/response transformation
- Timeout support

### Why HLS.js?
- Industry standard for adaptive streaming
- Works in all browsers
- Supports multiple quality levels
- Built-in error recovery

### Why API Gateway?
- Single entry point for frontend
- Simplified CORS configuration
- Service discovery abstraction
- Centralized rate limiting

---

This architecture is production-ready, scalable, and follows modern React/Next.js best practices. All components are strongly typed, error-resilient, and optimized for performance.
