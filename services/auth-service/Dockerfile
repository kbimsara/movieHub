# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["AuthService.API/AuthService.API.csproj", "AuthService.API/"]

# Restore dependencies
RUN dotnet restore "AuthService.API/AuthService.API.csproj"

# Copy source code
COPY . .

# Build
WORKDIR "/src/AuthService.API"
RUN dotnet build "AuthService.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "AuthService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0.0 AS final

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

WORKDIR /app
EXPOSE 5001
EXPOSE 5444

# Copy published files
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5001

ENTRYPOINT ["dotnet", "AuthService.API.dll"]
