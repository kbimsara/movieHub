FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY ["MovieSearchService.API/MovieSearchService.API.csproj", "MovieSearchService.API/"]
COPY ["MovieSearchService.Application/MovieSearchService.Application.csproj", "MovieSearchService.Application/"]
COPY ["MovieSearchService.Domain/MovieSearchService.Domain.csproj", "MovieSearchService.Domain/"]
COPY ["MovieSearchService.Infrastructure/MovieSearchService.Infrastructure.csproj", "MovieSearchService.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "MovieSearchService.API/MovieSearchService.API.csproj"

# Copy all source files
COPY . .

# Build
WORKDIR "/src/MovieSearchService.API"
RUN dotnet build "MovieSearchService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MovieSearchService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MovieSearchService.API.dll"]
