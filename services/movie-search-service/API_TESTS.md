# Movie Search Service - API Test Examples

## Prerequisites
- Elasticsearch running on http://localhost:9200
- Movie Search Service running on http://localhost:5193

## Health Check

### Check Service Health
```bash
curl -X GET "http://localhost:5193/api/health"
```

**Expected Response:**
```json
{
  "status": "healthy",
  "service": "Movie Search Service",
  "elasticsearch": "connected"
}
```

---

## Index Movies

### Index a Single Movie
```bash
curl -X POST "http://localhost:5193/api/search/index" \
  -H "Content-Type: application/json" \
  -d '{
    "id": "1",
    "title": "Inception",
    "description": "A thief who steals corporate secrets through the use of dream-sharing technology is given the inverse task of planting an idea into the mind of a C.E.O.",
    "genre": "Sci-Fi",
    "releaseYear": 2010,
    "rating": 8.8
  }'
```

### Index Multiple Test Movies

**The Dark Knight**
```bash
curl -X POST "http://localhost:5193/api/search/index" \
  -H "Content-Type: application/json" \
  -d '{
    "id": "2",
    "title": "The Dark Knight",
    "description": "When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, Batman must accept one of the greatest psychological and physical tests.",
    "genre": "Action",
    "releaseYear": 2008,
    "rating": 9.0
  }'
```

**Interstellar**
```bash
curl -X POST "http://localhost:5193/api/search/index" \
  -H "Content-Type: application/json" \
  -d '{
    "id": "3",
    "title": "Interstellar",
    "description": "A team of explorers travel through a wormhole in space in an attempt to ensure humanity's survival.",
    "genre": "Sci-Fi",
    "releaseYear": 2014,
    "rating": 8.6
  }'
```

**Pulp Fiction**
```bash
curl -X POST "http://localhost:5193/api/search/index" \
  -H "Content-Type: application/json" \
  -d '{
    "id": "4",
    "title": "Pulp Fiction",
    "description": "The lives of two mob hitmen, a boxer, a gangster and his wife intertwine in four tales of violence and redemption.",
    "genre": "Crime",
    "releaseYear": 1994,
    "rating": 8.9
  }'
```

**The Matrix**
```bash
curl -X POST "http://localhost:5193/api/search/index" \
  -H "Content-Type: application/json" \
  -d '{
    "id": "5",
    "title": "The Matrix",
    "description": "A computer hacker learns from mysterious rebels about the true nature of his reality and his role in the war against its controllers.",
    "genre": "Sci-Fi",
    "releaseYear": 1999,
    "rating": 8.7
  }'
```

---

## Search Movies

### Basic Search - All Movies
```bash
curl -X GET "http://localhost:5193/api/search/movies"
```

### Full-Text Search by Query

**Search for "space"**
```bash
curl -X GET "http://localhost:5193/api/search/movies?q=space"
```

**Search for "hacker"**
```bash
curl -X GET "http://localhost:5193/api/search/movies?q=hacker"
```

**Search for "violence"**
```bash
curl -X GET "http://localhost:5193/api/search/movies?q=violence"
```

### Filter by Genre

**All Sci-Fi Movies**
```bash
curl -X GET "http://localhost:5193/api/search/movies?genre=Sci-Fi"
```

**All Action Movies**
```bash
curl -X GET "http://localhost:5193/api/search/movies?genre=Action"
```

### Filter by Year

**Movies from 2010**
```bash
curl -X GET "http://localhost:5193/api/search/movies?year=2010"
```

**Movies from 1990s**
```bash
curl -X GET "http://localhost:5193/api/search/movies?year=1994"
curl -X GET "http://localhost:5193/api/search/movies?year=1999"
```

### Combined Filters

**Search for "dream" in Sci-Fi movies**
```bash
curl -X GET "http://localhost:5193/api/search/movies?q=dream&genre=Sci-Fi"
```

**Sci-Fi movies from 2014**
```bash
curl -X GET "http://localhost:5193/api/search/movies?genre=Sci-Fi&year=2014"
```

### Pagination

**First page (10 results per page)**
```bash
curl -X GET "http://localhost:5193/api/search/movies?page=1&pageSize=10"
```

**Second page (5 results per page)**
```bash
curl -X GET "http://localhost:5193/api/search/movies?page=2&pageSize=5"
```

**Get only 2 results**
```bash
curl -X GET "http://localhost:5193/api/search/movies?pageSize=2"
```

---

## PowerShell Examples

### Index Movie (PowerShell)
```powershell
$body = @{
    id = "6"
    title = "Forrest Gump"
    description = "The presidencies of Kennedy and Johnson unfold through the perspective of an Alabama man with an IQ of 75."
    genre = "Drama"
    releaseYear = 1994
    rating = 8.8
} | ConvertTo-Json

Invoke-RestMethod -Method Post -Uri "http://localhost:5193/api/search/index" `
    -Body $body -ContentType "application/json"
```

### Search Movies (PowerShell)
```powershell
# Basic search
Invoke-RestMethod -Uri "http://localhost:5193/api/search/movies"

# Search with query
Invoke-RestMethod -Uri "http://localhost:5193/api/search/movies?q=batman"

# Search with filters
Invoke-RestMethod -Uri "http://localhost:5193/api/search/movies?genre=Sci-Fi&page=1&pageSize=5"
```

---

## Expected Response Format

```json
{
  "items": [
    {
      "id": "1",
      "title": "Inception",
      "description": "A thief who steals corporate secrets...",
      "genre": "Sci-Fi",
      "releaseYear": 2010,
      "rating": 8.8,
      "createdAt": "2025-12-25T10:30:00Z",
      "score": 12.5
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 1,
  "totalPages": 1
}
```

---

## Testing Fuzzy Search

Elasticsearch supports fuzzy matching for typos:

```bash
# Correct: "inception"
curl -X GET "http://localhost:5193/api/search/movies?q=inception"

# Typo: "inceptoin" (should still find "Inception")
curl -X GET "http://localhost:5193/api/search/movies?q=inceptoin"

# Partial: "incep"
curl -X GET "http://localhost:5193/api/search/movies?q=incep"
```

---

## Relevance Scoring

Title matches score higher than description matches due to the boost (title^2):

```bash
# "dream" appears in Inception's description - lower score
curl -X GET "http://localhost:5193/api/search/movies?q=dream"

# "inception" appears in title - higher score
curl -X GET "http://localhost:5193/api/search/movies?q=inception"
```

---

## Notes

- **Fuzzy Search**: AUTO fuzziness tolerates 1-2 character typos
- **Relevance**: Results sorted by Elasticsearch BM25 score
- **Title Boost**: Title matches are 2x more relevant
- **Pagination**: Max page size is 100
- **Filters**: Genre and Year use exact matching
- **Score**: Higher score = more relevant result
