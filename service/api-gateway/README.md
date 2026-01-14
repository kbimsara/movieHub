# API Gateway - MovieHub Microservices

A reverse proxy API Gateway built with ASP.NET Core and YARP that provides a single entry point for all MovieHub microservices.

## Overview

The API Gateway serves as the single entry point for frontend applications, routing requests to appropriate microservices while handling cross-cutting concerns like CORS and JWT token forwarding.

## Architecture

```
Frontend → API Gateway → Microservices
                    ↓
            ┌─────────────┐
            │ Auth Service│
            │ Port: 5001  │
            └─────────────┘
                    ↓
            ┌─────────────┐
            │Movie Service│
            │ Port: 5002  │
            └─────────────┘
                    ↓
            ┌─────────────┐
            │User Service │
            │ Port: 5003  │
            └─────────────┘
                    ↓
            ┌─────────────┐
            │Library Svc  │
            │ Port: 5004  │
            └─────────────┘
                    ↓
            ┌─────────────┐
            │File Service │
            │ Port: 5005  │
            └─────────────┘
```

## Features

- **Single Entry Point**: All API calls go through `http://localhost:5000`
- **Request Routing**: Automatic routing to appropriate microservices
- **CORS Handling**: Centralized CORS configuration
- **JWT Forwarding**: Automatically forwards authentication headers
- **Load Balancing**: Ready for horizontal scaling of services
- **Path Transformation**: Clean URL paths for frontend consumption

## API Routes

| Frontend Request | Target Service | Target URL |
|------------------|----------------|------------|
| `POST /api/auth/login` | Auth Service | `http://localhost:5001/api/auth/login` |
| `GET /api/movies` | Movie Service | `http://localhost:5002/api/movies` |
| `GET /api/userprofiles/123` | User Service | `http://localhost:5003/api/userprofiles/123` |
| `GET /api/libraries` | Library Service | `http://localhost:5004/api/libraries` |
| `POST /api/files/upload` | File Service | `http://localhost:5005/api/files/upload` |
| `GET /uploads/image.jpg` | File Service | `http://localhost:5005/uploads/image.jpg` |

## Configuration

### Reverse Proxy Routes

The gateway uses YARP (Yet Another Reverse Proxy) with configuration in `appsettings.json`:

```json
{
  "ReverseProxy": {
    "Routes": {
      "auth-route": {
        "ClusterId": "auth-cluster",
        "Match": { "Path": "/api/auth/{**catch-all}" }
      }
    },
    "Clusters": {
      "auth-cluster": {
        "Destinations": {
          "auth-service": { "Address": "http://localhost:5001" }
        }
      }
    }
  }
}
```

### CORS Configuration

```csharp
app.UseCors(policy =>
{
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader();
});
```

## Request Flow Example

### User Authentication Flow

1. **Frontend** → `POST http://localhost:5000/api/auth/login`
2. **Gateway** routes to Auth Service → `POST http://localhost:5001/api/auth/login`
3. **Auth Service** validates credentials and returns JWT
4. **Gateway** forwards response back to Frontend

### Movie Data Flow

1. **Frontend** → `GET http://localhost:5000/api/movies` (with JWT header)
2. **Gateway** routes to Movie Service → `GET http://localhost:5002/api/movies`
3. **Movie Service** validates JWT and returns movie data
4. **Gateway** forwards response back to Frontend

## Development Setup

### Prerequisites

- .NET 8.0 SDK
- All microservices running on their respective ports

### Running the Gateway

```bash
cd service/api-gateway/ApiGateway
dotnet run
```

The gateway will be available at `http://localhost:5000`

### Service Ports

Ensure all services are running on their configured ports:

- API Gateway: `http://localhost:5000`
- Auth Service: `http://localhost:5001`
- Movie Service: `http://localhost:5002`
- User Service: `http://localhost:5003`
- Library Service: `http://localhost:5004`
- File Service: `http://localhost:5005`

## Production Considerations

### Load Balancing

For production, configure multiple instances of each service:

```json
"Clusters": {
  "auth-cluster": {
    "Destinations": {
      "auth-service-1": { "Address": "http://auth-svc-1:5001" },
      "auth-service-2": { "Address": "http://auth-svc-2:5001" }
    }
  }
}
```

### Security

- Implement rate limiting
- Add request/response logging
- Configure proper CORS policies for production domains
- Add authentication/authorization at gateway level if needed

### Monitoring

- Add health checks for all downstream services
- Implement distributed tracing
- Add metrics collection

## Testing

Test the gateway by making requests to `http://localhost:5000`:

```bash
# Test auth endpoint
curl -X POST "http://localhost:5000/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"password"}'

# Test movie endpoint (with JWT)
curl "http://localhost:5000/api/movies" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

The API Gateway provides a clean, scalable entry point for your microservices architecture! 🚀