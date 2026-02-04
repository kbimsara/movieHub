# MovieHub Services

This folder hosts the backend microservices for the MovieHub platform. Currently it includes:

- **apiGateway** (`net8.0` YARP reverse proxy) listening on `http://localhost:5000` and proxying `/api/auth/*`
- **authService** (`net8.0` Web API) listening on `http://localhost:5001` and serving the authentication endpoints expected by the Next.js frontend

## Running locally

### Using separate terminals

```bash
# Terminal 1: start API Gateway (port 5000)
dotnet run --project service/apiGateway/ApiGateway.csproj

# Terminal 2: start Auth Service (port 5001)
dotnet run --project service/authService/WebApplication1.csproj
```

With both processes running, the frontend can continue to call `http://localhost:5000/api/auth/*` and the gateway will forward those calls to the auth service.

### Using the combined solution

You can also open the solution file `service/MovieHub.Services.sln` in Visual Studio or run:

```bash
cd service
msbuild MovieHub.Services.sln
```

From Visual Studio, set **ApiGateway** as the startup project (or create a multi-start profile) so it launches alongside `AuthService`.

### Using Docker Compose (gateway + auth + posgraph)

```bash
# From the repo root
docker compose up --build
```

Services included:

- `api-gateway` → http://localhost:5000
- `auth-service` → http://localhost:5001
- `posgraph` (PostgreSQL 16) → localhost:5432 (user/pass/db: `moviehub`)

Both .NET services now have Dockerfiles located beside their csproj files. The compose file wires the gateway to the auth container automatically and exposes Postgres for future persistence work.

## Adding more services

1. Implement the service under `service/<your-service>` and expose the necessary endpoints.
2. Add the project to `MovieHub.Services.sln` for easy builds.
3. Extend `apiGateway/appsettings*.json` with a new `Route` and `Cluster` that forwards the desired `/api/...` path to your new service.

This keeps the frontend talking to a single origin while letting you scale the backend as separate ASP.NET microservices.
