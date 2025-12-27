# Quick Reference: API Integration

## Environment Setup

Create/update `.env.local`:
```env
NEXT_PUBLIC_API_BASE_URL=http://localhost:5000
```

## API Endpoints Reference

All endpoints route through API Gateway at `http://localhost:5000`

### Authentication (`/api/auth`)
```typescript
POST   /api/auth/login           // Login user
POST   /api/auth/register        // Register new user
POST   /api/auth/logout          // Logout user
GET    /api/auth/me              // Get current user
POST   /api/auth/refresh         // Refresh access token
```

### Movies (`/api/movies`)
```typescript
GET    /api/movies               // Get all movies (with filters)
GET    /api/movies/{id}          // Get movie by ID
GET    /api/movies/trending      // Get trending movies
GET    /api/movies/popular       // Get popular movies
GET    /api/movies/top-rated     // Get top rated movies
GET    /api/movies/genres        // Get all genres
GET    /api/movies/genre/{genre} // Get movies by genre
GET    /api/movies/{id}/related  // Get related movies
```

### Search (`/api/search`)
```typescript
GET    /api/search/movies        // Search movies
GET    /api/search/suggestions   // Get search suggestions
GET    /api/search/trending      // Get trending searches
GET    /api/search/recommendations // Get recommendations
GET    /api/search/similar/{id}  // Get similar movies
```

### Library (`/api/library`)
```typescript
GET    /api/library              // Get user library
POST   /api/library              // Add movie to library
DELETE /api/library/{movieId}    // Remove from library
PUT    /api/library/{movieId}/favorite // Toggle favorite
PUT    /api/library/{movieId}/progress // Update watch progress
GET    /api/library/continue-watching  // Get continue watching
GET    /api/library/favorites    // Get favorites
GET    /api/library/history      // Get watch history
DELETE /api/library/history      // Clear watch history
```

### User (`/api/users`)
```typescript
GET    /api/users/me             // Get user profile
PUT    /api/users/me             // Update user profile
POST   /api/users/me/avatar      // Upload avatar
GET    /api/users/me/library     // Get user library
POST   /api/users/me/library     // Add to library
DELETE /api/users/me/library/{movieId} // Remove from library
```

## Using Services in Components

### 1. Using Hooks (Recommended)

```typescript
import { useMovies } from '@/hooks/useMovies';

function MyComponent() {
  const { movies, isLoading, error, fetchMovies } = useMovies();

  useEffect(() => {
    fetchMovies();
  }, []);

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error: {error}</div>;

  return <MovieList movies={movies} />;
}
```

### 2. Using Services Directly

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

### 3. Authentication

```typescript
import { useAuth } from '@/hooks/useAuth';

function LoginForm() {
  const { login, isLoading } = useAuth();

  const handleSubmit = async (email: string, password: string) => {
    const result = await login({ email, password });
    if (result.success) {
      router.push('/browse');
    } else {
      alert(result.error);
    }
  };
}
```

## JWT Token Flow

### Automatic Token Management
- ✅ Token automatically attached to all requests
- ✅ Token stored in localStorage
- ✅ Token refresh on 401 responses
- ✅ Redirect to login on refresh failure

### Manual Token Access (if needed)
```typescript
const token = localStorage.getItem('accessToken');
const refreshToken = localStorage.getItem('refreshToken');
```

## Error Handling

### In Components
```typescript
const { error, isLoading } = useMovies();

if (error) {
  return <div className="error">{error}</div>;
}
```

### In Services
```typescript
try {
  const response = await movieService.getMovies();
  // Handle success
} catch (error) {
  // API call failed
  console.error(error);
}
```

## Testing Locally

### 1. Start Backend Services
```bash
cd services
docker-compose up
# or start each service individually
```

### 2. Verify API Gateway
Visit: http://localhost:5000/health

### 3. Start Frontend
```bash
cd front-end
npm run dev
```

### 4. Test Authentication
1. Navigate to http://localhost:3000/auth/login
2. Login with credentials
3. Check localStorage for `accessToken` and `refreshToken`
4. Navigate to http://localhost:3000/browse
5. Check Network tab - all requests should go to `localhost:5000/api/*`

## Common Issues

### Issue: "Failed to fetch"
**Cause:** Backend not running or wrong port  
**Fix:** Ensure API Gateway is running on port 5000

### Issue: "401 Unauthorized"
**Cause:** No token or expired token  
**Fix:** Login again to get new token

### Issue: "404 Not Found"
**Cause:** Endpoint not implemented in backend  
**Fix:** Check API Gateway routing configuration

### Issue: "CORS Error"
**Cause:** API Gateway not configured for CORS  
**Fix:** Ensure API Gateway allows `http://localhost:3000`

## Files Modified

| File | Changes |
|------|---------|
| `src/lib/axios.ts` | JWT handling, 401 interceptor, env variable |
| `src/services/library.service.ts` | Use apiClient, API Gateway routes |
| `src/services/torrent.service.ts` | Added `/api` prefix |
| `src/services/upload.service.ts` | Added `/api` prefix |
| `src/hooks/useMovies.ts` | Removed all mock data fallbacks |
| `src/lib/mockData.ts` | **DELETED** |
| `.env.local` | Single API_BASE_URL variable |

## Files Reviewed (No Changes)

- ✅ `src/services/auth.service.ts` - Already correct
- ✅ `src/services/movie.service.ts` - Already correct
- ✅ `src/services/search.service.ts` - Already correct
- ✅ `src/services/user.service.ts` - Already correct
- ✅ All Redux slices - Already correct

---

**Ready for backend integration testing!** 🚀
