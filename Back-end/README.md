# MovieHub Backend - .NET 8 Microservices Platform

## 🎬 Overview

A production-ready, cloud-native movie streaming platform built with .NET 8 microservices architecture, deployed on Google Kubernetes Engine (GKE) with Istio service mesh.

## 🏗️ Architecture

### Microservices

1. **Auth Service** (Port 5001)
   - User authentication & authorization
   - JWT token generation & validation
   - Role-based access control (RBAC)
   - Refresh token rotation

2. **User Service** (Port 5002)
   - User profile management
   - User preferences
   - Account status management

3. **Movie Metadata Service** (Port 5003)
   - CRUD operations for movies
   - Genres, cast, crew management
   - Advanced search & filtering
   - Movie recommendations

4. **Upload Service** (Port 5004)
   - Large file upload handling (multipart)
   - File validation & virus scanning
   - Temporary storage management
   - Publishes `movie-uploaded` events

5. **Video Processing Worker**
   - Kafka consumer for video processing
   - FFmpeg transcoding to HLS
   - Multi-resolution stream generation (1080p, 720p, 480p, 360p)
   - Thumbnail generation
   - GCS upload
   - Publishes `movie-processed` events

6. **Streaming Gateway Service** (Port 5005)
   - Signed URL generation for HLS streams
   - Access control & authorization
   - Rate limiting
   - CDN integration

7. **Library Service** (Port 5006)
   - User movie library management
   - Watch progress tracking
   - Favorites & watchlist
   - Watch history

8. **Torrent Service** (Port 5007)
   - Torrent file generation
   - Magnet link creation
   - Seeding statistics tracking
   - Peer/seeder management

9. **Notification Service** (Port 5008)
   - Event-driven notifications
   - Email notifications
   - In-app notifications
   - Push notifications

### Technology Stack

- **Framework**: .NET 8 (ASP.NET Core Web API + Worker Services)
- **Architecture**: Clean Architecture (Domain, Application, Infrastructure, API)
- **Database**: PostgreSQL 16 (Cloud SQL)
- **Message Broker**: Apache Kafka
- **Caching**: Redis
- **Storage**: Google Cloud Storage (GCS)
- **Container Orchestration**: Kubernetes (GKE)
- **Service Mesh**: Istio
- **Monitoring**: Prometheus + Grafana
- **Logging**: Serilog
- **CI/CD**: GitHub Actions
- **IaC**: Terraform

### Event-Driven Architecture

**Kafka Topics:**
- `movie-uploaded`: Triggered when a movie is uploaded
- `movie-processing-started`: Video processing begins
- `movie-processed`: Video processing completes
- `torrent-created`: Torrent file generated
- `torrent-seeding-started`: Seeding initiated
- `library-updated`: User library modified

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK
- Docker & Docker Compose
- PostgreSQL 16
- Kafka
- Google Cloud SDK (for production deployment)
- Terraform (for infrastructure provisioning)

### Local Development Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-org/moviehub.git
   cd moviehub/Back-end
   ```

2. **Start infrastructure services**
   ```bash
   docker-compose up -d postgres kafka zookeeper redis prometheus grafana
   ```

3. **Run database migrations**
   ```bash
   dotnet ef database update --project src/Services/Auth/MovieHub.Services.Auth.Infrastructure --startup-project src/Services/Auth/MovieHub.Services.Auth.API
   ```

4. **Start services**
   ```bash
   # Auth Service
   cd src/Services/Auth/MovieHub.Services.Auth.API
   dotnet run

   # Or run all services with docker-compose
   docker-compose up
   ```

5. **Access services**
   - Auth Service: http://localhost:5001/swagger
   - User Service: http://localhost:5002/swagger
   - Movie Metadata Service: http://localhost:5003/swagger
   - Prometheus: http://localhost:9090
   - Grafana: http://localhost:3000 (admin/admin)

### Configuration

Update `appsettings.json` in each service:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=moviehub_auth;Username=postgres;Password=postgres"
  },
  "JwtSettings": {
    "Secret": "your-super-secret-key-minimum-32-characters",
    "Issuer": "MovieHub.AuthService",
    "Audience": "MovieHub.Client",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  },
  "Kafka": {
    "BootstrapServers": "localhost:9093"
  }
}
```

## 📦 Docker Deployment

### Build all services
```bash
docker-compose build
```

### Run all services
```bash
docker-compose up -d
```

### View logs
```bash
docker-compose logs -f auth-service
```

### Stop all services
```bash
docker-compose down
```

## ☸️ Kubernetes Deployment

### Prerequisites
- GKE cluster provisioned via Terraform
- kubectl configured
- Istio installed

### Deploy to Kubernetes

1. **Apply namespace**
   ```bash
   kubectl apply -f k8s/namespace.yaml
   ```

2. **Apply configurations**
   ```bash
   kubectl apply -f k8s/configmap.yaml
   kubectl apply -f k8s/secrets.yaml
   ```

3. **Deploy services**
   ```bash
   kubectl apply -f k8s/services/
   ```

4. **Apply Istio configurations**
   ```bash
   kubectl apply -f k8s/istio/
   ```

5. **Verify deployment**
   ```bash
   kubectl get pods -n moviehub
   kubectl get services -n moviehub
   ```

## 🌍 Infrastructure Provisioning (Terraform)

### Initialize Terraform
```bash
cd terraform
terraform init
```

### Plan infrastructure
```bash
terraform plan -var="project_id=your-gcp-project-id"
```

### Apply infrastructure
```bash
terraform apply -var="project_id=your-gcp-project-id"
```

### Destroy infrastructure
```bash
terraform destroy -var="project_id=your-gcp-project-id"
```

## 📊 Monitoring & Observability

### Prometheus Metrics
All services expose Prometheus metrics at `/metrics` endpoint:
- HTTP request duration
- HTTP request count by status code
- Active requests
- Database query performance
- Kafka consumer lag
- Custom business metrics

### Grafana Dashboards
Access Grafana at http://localhost:3000
- Service Overview Dashboard
- Kafka Consumer Dashboard
- Database Performance Dashboard
- API Performance Dashboard

### Health Checks
All services expose health check endpoints:
- `/health`: Basic health check
- `/health/ready`: Readiness probe
- `/health/live`: Liveness probe

## 🔐 Security

- **Authentication**: JWT Bearer tokens
- **Authorization**: Role-based access control (RBAC)
- **Transport Security**: TLS/HTTPS everywhere
- **Service-to-Service**: mTLS via Istio
- **Secrets Management**: Kubernetes Secrets / Google Secret Manager
- **Database**: SSL connections, private VPC
- **Storage**: Signed URLs with expiration

## 🧪 Testing

### Run unit tests
```bash
dotnet test
```

### Run integration tests
```bash
dotnet test --filter Category=Integration
```

### Run load tests
```bash
cd tests/LoadTests
dotnet run
```

## 📈 Performance

- **Horizontal Scaling**: Auto-scaling based on CPU/Memory
- **Database Connection Pooling**: EF Core connection pooling
- **Caching**: Redis for hot data
- **CDN**: GCS with CDN for static assets
- **Rate Limiting**: Per-user and per-IP rate limits

## 🔄 CI/CD Pipeline

GitHub Actions workflow (`.github/workflows/ci-cd.yml`):
1. **Build**: Compile .NET projects
2. **Test**: Run unit and integration tests
3. **Docker Build**: Build Docker images
4. **Push**: Push to GCR
5. **Deploy**: Deploy to GKE
6. **Verify**: Health checks and smoke tests

## 📝 API Documentation

Swagger/OpenAPI documentation available at:
- Auth Service: http://localhost:5001/swagger
- User Service: http://localhost:5002/swagger
- Movie Metadata Service: http://localhost:5003/swagger
- Upload Service: http://localhost:5004/swagger
- Streaming Service: http://localhost:5005/swagger
- Library Service: http://localhost:5006/swagger
- Torrent Service: http://localhost:5007/swagger
- Notification Service: http://localhost:5008/swagger

## 🏛️ Project Structure

```
Back-end/
├── src/
│   ├── Shared/
│   │   └── MovieHub.Shared.Kernel/          # Shared kernel library
│   ├── Services/
│   │   ├── Auth/                             # Authentication service
│   │   │   ├── MovieHub.Services.Auth.API
│   │   │   ├── MovieHub.Services.Auth.Application
│   │   │   ├── MovieHub.Services.Auth.Domain
│   │   │   └── MovieHub.Services.Auth.Infrastructure
│   │   ├── User/                             # User service
│   │   ├── MovieMetadata/                    # Movie metadata service
│   │   ├── Upload/                           # Upload service
│   │   ├── Streaming/                        # Streaming service
│   │   ├── Library/                          # Library service
│   │   ├── Torrent/                          # Torrent service
│   │   └── Notification/                     # Notification service
│   └── Workers/
│       └── VideoProcessing/                  # Video processing worker
├── k8s/                                      # Kubernetes manifests
├── terraform/                                # Infrastructure as Code
├── monitoring/                               # Prometheus & Grafana configs
├── scripts/                                  # Utility scripts
├── docker-compose.yml                        # Local development setup
└── MovieHub.sln                             # Solution file
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License.

## 👥 Team

- Backend Team: .NET Microservices
- DevOps Team: Infrastructure & Deployment
- Frontend Team: React/Next.js

## 📞 Support

- Email: support@moviehub.com
- Slack: #moviehub-backend
- Issues: GitHub Issues

## 🎯 Roadmap

- [ ] Implement API Gateway (Ocelot/YARP)
- [ ] Add gRPC support for inter-service communication
- [ ] Implement CQRS with Event Sourcing
- [ ] Add GraphQL API
- [ ] Implement distributed tracing (Jaeger)
- [ ] Add chaos engineering tests (Chaos Toolkit)
- [ ] Implement feature flags (LaunchDarkly)
- [ ] Add multi-region support
- [ ] Implement A/B testing framework
- [ ] Add machine learning recommendations
