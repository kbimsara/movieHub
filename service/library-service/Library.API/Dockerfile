# Use the official .NET 8 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["library-service/", "./library-service/"]
COPY ["shared/", "./shared/"]
RUN dotnet restore "library-service/Library.API/Library.API.csproj"

# Copy the source code (already done above, but ensure everything is copied)
# COPY ["library-service/Library.API/", "library-service/Library.API/"]

# Build the application
RUN dotnet build "library-service/Library.API/Library.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "library-service/Library.API/Library.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET 8 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose the port the app runs on
EXPOSE 5004

# Set the entry point
ENTRYPOINT ["dotnet", "Library.API.dll"]