# Use the official .NET 8 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["auth-service/", "./auth-service/"]
COPY ["shared/", "./shared/"]
RUN dotnet restore "auth-service/Auth.API/Auth.API.csproj"

# Copy the source code (already done above, but ensure everything is copied)
# COPY ["auth-service/Auth.API/", "auth-service/Auth.API/"]

# Build the application
RUN dotnet build "auth-service/Auth.API/Auth.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "auth-service/Auth.API/Auth.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET 8 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose the port the app runs on
EXPOSE 5001

# Set the entry point
ENTRYPOINT ["dotnet", "Auth.API.dll"]