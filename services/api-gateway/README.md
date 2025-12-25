# MovieHub API Gateway

A production-ready API Gateway built with **YARP (Yet Another Reverse Proxy)** on **.NET 10** that serves as the single entry point for MovieHub microservices.

## 🏗️ Architecture

The API Gateway acts as a reverse proxy, routing incoming requests to the appropriate downstream microservices:

```
Client → API Gateway (Port 5000) → Auth Service (Port 5001)
                                 → User Service (Port 5002)
```

### Route Configuration

| Route Pattern | Downstream Service | Target URL |
|--------------|-------------------|-----------|
| `/api/auth/**` | Auth Service | `http://localhost:5001` |
| `/api/users/**` | User Service | `http://localhost:5002` |
| `/health` | Gateway Health | (Gateway itself) |

## 🚀 How It Works

### 1. **YARP Reverse Proxy Routing**
The gateway uses YARP's declarative configuration in `appsettings.json`:

- **Routes**: Define URL patterns to match incoming requests
- **Clusters**: Define destination services and their addresses
- **Transforms**: Preserve or modify request paths and headers

When a request arrives at the gateway:
1. YARP matches the request path against configured routes
2. Identifies the target cluster (downstream service)
3. Forwards the request with ALL headers (including `Authorization`)
4. Returns the downstream response to the client

### 2. **JWT Token Handling**

**Important Design Decision**: The gateway does **NOT** validate JWT tokens.

**Why?**
- **Separation of Concerns**: Authentication logic belongs to downstream services
- **Flexibility**: Each service can have its own validation rules
- **Simplicity**: Gateway remains a thin, stateless proxy
- **Performance**: No additional JWT processing overhead

**Flow**:
```
1. Client sends request with JWT token in Authorization header
2. Gateway forwards the entire request (including Authorization header)
3. Downstream service (Auth/User Service) validates the JWT
4. Downstream service returns 401 if token is invalid
5. Gateway forwards the response back to client
```

**Optional Future Enhancement**:
You can add JWT validation in the gateway for:
- Early rejection of invalid tokens
- Centralized authentication
- Token transformation/enrichment

Simply add to `Program.cs`:
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { /* JWT config */ });
app.UseAuthentication();
app.UseAuthorization();
```

### 3. **Header Forwarding**

YARP automatically forwards ALL headers by default, including:
- `Authorization` (JWT tokens)
- `Content-Type`
- `Accept`
- Custom headers

No special configuration needed - this is YARP's default behavior.

## 📦 Project Structure

```
api-gateway/
├── Controllers/
│   └── HealthController.cs       # Health check endpoint
├── appsettings.json               # YARP routes & clusters config
├── appsettings.Development.json   # Dev-specific settings
├── Program.cs                     # Application entry point
├── ApiGateway.csproj             # Project dependencies
├── Dockerfile                     # Docker containerization
└── .dockerignore                 # Docker build exclusions
```

## 🛠️ Local Development

### Prerequisites
- .NET 10 SDK
- Auth Service running on `http://localhost:5001`
- User Service running on `http://localhost:5002`

### Run Locally
```bash
cd services/api-gateway
dotnet restore
dotnet run
```

Gateway will start on: `http://localhost:5000`

### Test Endpoints
```bash
# Health check
curl http://localhost:5000/health

# Auth service (via gateway)
curl http://localhost:5000/api/auth/login

# User service (via gateway)
curl -H "Authorization: Bearer YOUR_TOKEN" http://localhost:5000/api/users/profile
```

### Swagger UI
Available in Development mode: `http://localhost:5000/swagger`

## 🐳 Docker

### Build Image
```bash
docker build -t moviehub-gateway:latest .
```

### Run Container
```bash
docker run -d -p 5000:80 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  moviehub-gateway:latest
```

### Docker Compose (Example)
```yaml
services:
  gateway:
    build: ./services/api-gateway
    ports:
      - "5000:80"
    environment:
      - ReverseProxy__Clusters__auth-cluster__Destinations__auth-service__Address=http://auth-service:80
      - ReverseProxy__Clusters__users-cluster__Destinations__user-service__Address=http://user-service:80
    depends_on:
      - auth-service
      - user-service
```

## ☸️ Kubernetes Ready

### Deployment Example
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: api-gateway
spec:
  replicas: 3
  selector:
    matchLabels:
      app: api-gateway
  template:
    metadata:
      labels:
        app: api-gateway
    spec:
      containers:
      - name: gateway
        image: moviehub-gateway:latest
        ports:
        - containerPort: 80
        env:
        - name: ReverseProxy__Clusters__auth-cluster__Destinations__auth-service__Address
          value: "http://auth-service:80"
        - name: ReverseProxy__Clusters__users-cluster__Destinations__user-service__Address
          value: "http://user-service:80"
        livenessProbe:
          httpGet:
            path: /health
            port: 80
          initialDelaySeconds: 10
          periodSeconds: 30
        readinessProbe:
          httpGet:
            path: /health
            port: 80
          initialDelaySeconds: 5
          periodSeconds: 10
---
apiVersion: v1
kind: Service
metadata:
  name: api-gateway
spec:
  type: LoadBalancer
  ports:
  - port: 80
    targetPort: 80
  selector:
    app: api-gateway
```

## 🔧 Configuration

### Environment Variables
Override appsettings.json via environment variables:

```bash
# Change downstream service addresses
export ReverseProxy__Clusters__auth-cluster__Destinations__auth-service__Address=http://auth-service:8080
export ReverseProxy__Clusters__users-cluster__Destinations__user-service__Address=http://user-service:8080

# Change timeout
export ReverseProxy__Clusters__auth-cluster__HttpRequest__Timeout=00:02:00
```

### Adding New Routes
Edit `appsettings.json`:

```json
{
  "ReverseProxy": {
    "Routes": {
      "new-service-route": {
        "ClusterId": "new-cluster",
        "Match": {
          "Path": "/api/newservice/{**catch-all}"
        }
      }
    },
    "Clusters": {
      "new-cluster": {
        "Destinations": {
          "new-service": {
            "Address": "http://localhost:5003"
          }
        }
      }
    }
  }
}
```

## 📊 Monitoring & Observability

### Health Checks
- **Endpoint**: `GET /health`
- **Response**: JSON with status, service name, timestamp
- **Use**: Kubernetes liveness/readiness probes, load balancer health checks

### Logging
YARP logs all proxy operations. Check logs for:
- Request routing
- Downstream service errors
- Performance metrics

## 🔐 Security Considerations

**Current State**:
- Gateway forwards JWT tokens without validation
- No rate limiting
- No API key authentication
- CORS configured for development (allow all)

**Production Recommendations**:
1. Add JWT validation in gateway
2. Implement rate limiting (use ASP.NET Core Rate Limiting)
3. Configure CORS properly (specific origins)
4. Add API key authentication for certain routes
5. Enable HTTPS/TLS
6. Add request/response logging
7. Implement circuit breaker pattern

## 📝 Key Design Decisions

1. **No JWT Validation**: Keeps gateway simple; downstream services handle auth
2. **No Business Logic**: Pure routing layer; easy to test and maintain
3. **No Database**: Stateless gateway; scales horizontally
4. **Configuration-Based**: All routing in `appsettings.json`; no code changes needed
5. **Health Endpoint**: Separate from YARP routes; always available

## 🚦 Next Steps

- [ ] Add rate limiting middleware
- [ ] Implement circuit breaker for downstream services
- [ ] Add request/response logging
- [ ] Configure centralized logging (ELK, Seq, Application Insights)
- [ ] Add API versioning support
- [ ] Implement caching layer (Redis)
- [ ] Add OpenTelemetry for distributed tracing

## 📚 Resources

- [YARP Documentation](https://microsoft.github.io/reverse-proxy/)
- [.NET Microservices Architecture](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/)
- [API Gateway Pattern](https://microservices.io/patterns/apigateway.html)
