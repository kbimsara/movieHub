# Movie Search Service

A high-performance, read-only search microservice built with .NET 10 and Elasticsearch using Clean Architecture principles.

## Overview

This service provides fast, full-text search capabilities for the MovieHub platform. It's optimized for search operations and acts as a search layer on top of the Movie Catalog Service data.

**Key Characteristics:**
- **READ-ONLY**: No write operations to databases, only indexes documents to Elasticsearch
- **Search-Optimized**: Uses Elasticsearch for blazing-fast full-text and filtered searches
- **No Authentication**: Basic search is public (JWT support can be added later)
- **Microservice**: Designed to work behind the API Gateway at `/api/search/**`

## Architecture

### Clean Architecture Layers

```
MovieSearchService.API           → Controllers, Program.cs, Swagger
MovieSearchService.Application   → DTOs, Interfaces, Business Logic
MovieSearchService.Domain         → Domain Entities (SearchMovie)
MovieSearchService.Infrastructure → Elasticsearch Client, Repository
```

### Why Read-Only?

This service is read-only because:
1. **Separation of Concerns**: Movie Catalog Service owns movie data
2. **Search Optimization**: Elasticsearch is optimized for search, not as a primary data store
3. **Eventual Consistency**: Search index is updated asynchronously from Catalog Service
4. **Scalability**: Read-only services can be scaled independently

## Technology Stack

- **.NET 10** - Latest ASP.NET Core Web API
- **Elasticsearch 8.x** - Search and analytics engine
- **Elastic.Clients.Elasticsearch** - Official .NET client
- **Swagger/OpenAPI** - API documentation
- **Docker** - Containerization

## Elasticsearch Index

**Index Name:** `movies`

**Mapping:**
```json
{
  "id": "keyword",
  "title": "text (keyword subfield)",
  "description": "text",
  "genre": "keyword",
  "releaseYear": "integer",
  "rating": "float",
  "createdAt": "date"
}
```

## API Endpoints

### 1. Search Movies
```http
GET /api/search/movies
```

**Query Parameters:**
- `q` (optional) - Search text (searches title and description)
- `genre` (optional) - Filter by genre (exact match)
- `year` (optional) - Filter by release year
- `page` (default: 1) - Page number
- `pageSize` (default: 10, max: 100) - Results per page

**Example:**
```bash
GET /api/search/movies?q=inception&genre=Sci-Fi&page=1&pageSize=20
```

**Response:**
```json
{
  "items": [
    {
      "id": "123",
      "title": "Inception",
      "description": "A thief who steals corporate secrets...",
      "genre": "Sci-Fi",
      "releaseYear": 2010,
      "rating": 8.8,
      "createdAt": "2024-01-01T00:00:00Z",
      "score": 12.5
    }
  ],
  "page": 1,
  "pageSize": 20,
  "totalCount": 1,
  "totalPages": 1
}
```

### 2. Index Movie
```http
POST /api/search/index
```

**Body:**
```json
{
  "id": "123",
  "title": "Inception",
  "description": "A thief who steals corporate secrets through dream-sharing technology",
  "genre": "Sci-Fi",
  "releaseYear": 2010,
  "rating": 8.8
}
```

**Response:**
```json
{
  "message": "Movie indexed successfully",
  "id": "123"
}
```

### 3. Health Check
```http
GET /api/health
```

## How Search Works

### Full-Text Search
The service uses Elasticsearch's **MultiMatchQuery** to search across title and description:
- **Title boost (^2)**: Title matches rank higher than description matches
- **Fuzzy matching**: Handles typos with AUTO fuzziness
- **Relevance scoring**: Results sorted by Elasticsearch's BM25 algorithm

### Filters
- **Genre**: Exact match using `term` query on `genre.keyword`
- **Year**: Exact match using `term` query on `releaseYear`
- Filters are combined with search using `bool` query with `must` clauses

### Pagination
- Efficient pagination using `from` and `size` parameters
- Returns total count for client-side page calculation

## Configuration

### appsettings.json
```json
{
  "Elasticsearch": {
    "Url": "http://localhost:9200",
    "IndexName": "movies",
    "Username": "",
    "Password": ""
  }
}
```

### Environment Variables (Docker)
```bash
Elasticsearch__Url=http://elasticsearch:9200
Elasticsearch__IndexName=movies
```

## Running the Service

### Prerequisites
- .NET 10 SDK
- Elasticsearch 8.x running on port 9200

### Local Development
```bash
# Navigate to solution directory
cd services/movie-search-service

# Restore dependencies
dotnet restore

# Run the service
cd MovieSearchService.API
dotnet run
```

The API will be available at: `http://localhost:5000`  
Swagger UI: `http://localhost:5000`

### Docker
```bash
# Build image
docker build -t movie-search-service .

# Run container
docker run -d -p 5000:80 \
  -e Elasticsearch__Url=http://elasticsearch:9200 \
  movie-search-service
```

## Development

### Project Structure
```
movie-search-service/
├── MovieSearchService.API/
│   ├── Controllers/
│   │   ├── SearchController.cs
│   │   └── HealthController.cs
│   ├── Program.cs
│   └── appsettings.json
├── MovieSearchService.Application/
│   ├── DTOs/
│   ├── Interfaces/
│   ├── Services/
│   └── DependencyInjection.cs
├── MovieSearchService.Domain/
│   └── Entities/
│       └── SearchMovie.cs
├── MovieSearchService.Infrastructure/
│   ├── Configuration/
│   ├── Elasticsearch/
│   ├── Repositories/
│   └── DependencyInjection.cs
└── Dockerfile
```

### Adding New Search Features

1. **Add filter to DTO**: Update `MovieSearchRequestDto`
2. **Update repository**: Add query logic in `SearchRepository.SearchMoviesAsync`
3. **Update controller**: Add query parameter to `SearchController.SearchMovies`

## Integration with MovieHub

### API Gateway Routing
The API Gateway routes `/api/search/**` to this service:
```json
{
  "DownstreamPathTemplate": "/api/search/{everything}",
  "DownstreamScheme": "http",
  "DownstreamHostAndPorts": [
    {
      "Host": "movie-search-service",
      "Port": 80
    }
  ]
}
```

### Data Synchronization
Movie data should be indexed from the Catalog Service:
1. Catalog Service publishes movie create/update events
2. Event handler in this service (or separate worker) indexes to Elasticsearch
3. Search results always reflect latest indexed data

## Future Enhancements

- [ ] JWT authentication support
- [ ] Advanced filters (rating range, date range)
- [ ] Autocomplete/suggestions
- [ ] Aggregations (faceted search)
- [ ] Highlight search terms in results
- [ ] Search analytics
- [ ] Message queue integration for auto-indexing

## License

Part of the MovieHub platform.
