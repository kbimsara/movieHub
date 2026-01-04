# Frontend-Backend Service Connections

This document shows all frontend service files connected to backend microservices.

## Service Architecture

```
Frontend (Next.js) -> API Gateway (5000) -> Microservices
                                          ├─ auth-service (5001)
                                          ├─ user-service (5002)
                                          ├─ catalog-service (5003)
                                          └─ search-service (5004)
```

## 1. Authentication Service (`src/services/auth.service.ts`)

**Backend:** auth-service (port 5001)  
**Routes:** `/api/auth/*`

### Connected Endpoints:

| Method | Endpoint | Frontend Method | Status |
|--------|----------|-----------------|--------|
| POST | `/api/auth/register` | `register()` | ✅ Working |
| POST | `/api/auth/login` | `login()` | ✅ Working |
| POST | `/api/auth/refresh` | `refreshToken()` | ✅ Working |
| POST | `/api/auth/validate` | `validateToken()` | ✅ Working |
| GET | `/api/auth/me` | `me()` | ✅ Working |
| POST | `/api/auth/logout` | `logout(refreshToken)` | ✅ Working |

### Commented Out (Not Implemented):
- `forgotPassword()` - Backend not implemented
- `resetPassword()` - Backend not implemented

---

## 2. User Profile Service (`src/services/user.service.ts`)

**Backend:** user-service (port 5002)  
**Routes:** `/api/me/*`

### Connected Endpoints:

| Method | Endpoint | Frontend Method | Status |
|--------|----------|-----------------|--------|
| GET | `/api/me` | `getProfile()` | ✅ Working |
| PUT | `/api/me` | `updateProfile()` | ✅ Working |
| POST | `/api/me` | `createProfile()` | ✅ Working |
| POST | `/api/me/avatar` | `uploadAvatar()` | ⚠️ Untested |

### Library Endpoints (Stub Implementations):

| Method | Endpoint | Frontend Method | Status |
|--------|----------|-----------------|--------|
| GET | `/api/me/library` | `getLibrary()` | ⚠️ Returns empty array |
| POST | `/api/me/library` | `addToLibrary()` | ⚠️ Stub |
| DELETE | `/api/me/library/{id}` | `removeFromLibrary()` | ⚠️ Stub |
| GET | `/api/me/history` | `getWatchHistory()` | ⚠️ Stub |
| GET | `/api/me/favorites` | `getFavorites()` | ⚠️ Stub |
| POST | `/api/me/favorites/{movieId}` | `addToFavorites()` | ⚠️ Stub |
| DELETE | `/api/me/favorites/{movieId}` | `removeFromFavorites()` | ⚠️ Stub |
| GET | `/api/me/watch-later` | `getWatchLater()` | ⚠️ Stub |
| POST | `/api/me/watch-later/{movieId}` | `addToWatchLater()` | ⚠️ Stub |
| DELETE | `/api/me/watch-later/{movieId}` | `removeFromWatchLater()` | ⚠️ Stub |

**Note:** Library endpoints are defined in the backend but return stub responses (empty arrays). Full implementation needed.

---

## 3. Movie Catalog Service (`src/services/movie.service.ts`)

**Backend:** catalog-service (port 5003)  
**Routes:** `/api/movies/*`

### Connected Endpoints:

| Method | Endpoint | Frontend Method | Status |
|--------|----------|-----------------|--------|
| GET | `/api/movies` | `getMovies(page, pageSize)` | ✅ Working |
| GET | `/api/movies/{id}` | `getMovieById(id)` | ✅ Working |
| POST | `/api/movies` | `createMovie()` | ⚠️ Untested |
| PUT | `/api/movies/{id}` | `updateMovie()` | ⚠️ Untested |
| DELETE | `/api/movies/{id}` | `deleteMovie()` | ⚠️ Untested |

### Commented Out (Not Implemented):
- `getTrendingMovies()` - Backend not implemented
- `getPopularMovies()` - Backend not implemented
- `getTopRatedMovies()` - Backend not implemented
- `getMoviesByGenre()` - Backend not implemented
- `getRelatedMovies()` - Backend not implemented
- `getGenres()` - Backend not implemented

---

## 4. Search Service (`src/services/search.service.ts`)

**Backend:** movie-search-service (port 5004)  
**Routes:** `/api/search/*`

### Connected Endpoints:

| Method | Endpoint | Frontend Method | Status |
|--------|----------|-----------------|--------|
| GET | `/api/search/movies` | `searchMovies(query, filters)` | ✅ Working |
| POST | `/api/search/movies/index` | `indexMovie(movie)` | ⚠️ Untested |

**Query Parameters Supported:**
- `query` - Search text
- `year` - Filter by year
- `genre` - Filter by genre
- `rating` - Minimum rating
- `page` - Page number
- `pageSize` - Results per page

### Commented Out (Not Implemented):
- `getSearchSuggestions()` - Backend not implemented
- `getTrendingSearches()` - Backend not implemented
- `getRecommendations()` - Backend not implemented

---

## Test Results Summary

### Latest Test Run:
- **Total Tests:** 5
- **Passed:** 3 (60%)
- **Failed:** 2

### Working Services:
✅ Health Check  
✅ Movie Catalog - Get Movies  
✅ Search Service - Search Movies  

### Issues:
❌ Auth Register - 400 Bad Request (validation issue)  
❌ Get Movie By ID - 400 Bad Request (invalid movie ID 1)

---

## Frontend Service File Updates

All service files have been updated to:
1. Use correct endpoint URLs matching backend implementation
2. Comment out unimplemented endpoints to prevent 404 errors
3. Include proper TypeScript types
4. Use Authorization headers for protected endpoints

### Files Updated:
- `front-end/src/services/auth.service.ts`
- `front-end/src/services/user.service.ts`
- `front-end/src/services/movie.service.ts`
- `front-end/src/services/search.service.ts`

---

## API Gateway Configuration

The API Gateway (`api-gateway/appsettings.json`) routes requests as follows:

```json
{
  "auth-route": "/api/auth/{**catch-all}" -> "http://auth-service:5001"
  "users-route": "/api/me/{**catch-all}" -> "http://user-service:5002"
  "catalog-route": "/api/movies/{**catch-all}" -> "http://catalog-service:5003"
  "search-route": "/api/search/{**catch-all}" -> "http://search-service:5004"
}
```

---

## Next Steps

1. **Implement Library Features:**
   - Complete the stub implementations in UserController
   - Add database tables for library, favorites, watch-later, watch-history

2. **Implement Missing Catalog Features:**
   - Trending movies endpoint
   - Popular movies endpoint
   - Top-rated movies endpoint
   - Genre management
   - Related movies recommendations

3. **Implement Search Features:**
   - Search suggestions
   - Trending searches
   - User recommendations

4. **Add Missing Auth Features:**
   - Forgot password flow
   - Reset password flow
   - Email verification

5. **Populate Data:**
   - Add sample movies to catalog
   - Index movies in Elasticsearch for search

---

## Testing

Run the comprehensive test script:

```powershell
cd services
.\test-connections.ps1
```

This tests all frontend-backend connections and provides a detailed report.

---

**Last Updated:** $(Get-Date -Format "yyyy-MM-dd HH:mm")  
**Status:** All core endpoints connected. Library and advanced features pending implementation.
