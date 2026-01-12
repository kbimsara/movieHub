# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY MovieManagementService.API/MovieManagementService.API.csproj MovieManagementService.API/
COPY MovieManagementService.Application/MovieManagementService.Application.csproj MovieManagementService.Application/
COPY MovieManagementService.Domain/MovieManagementService.Domain.csproj MovieManagementService.Domain/
COPY MovieManagementService.Infrastructure/MovieManagementService.Infrastructure.csproj MovieManagementService.Infrastructure/

# Restore dependencies
RUN dotnet restore MovieManagementService.API/MovieManagementService.API.csproj

# Copy everything else
COPY . .

# Build
WORKDIR /src/MovieManagementService.API
RUN dotnet build -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Install curl for healthchecks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

# Create uploads directory
RUN mkdir -p /app/uploads

ENTRYPOINT ["dotnet", "MovieManagementService.API.dll"]
