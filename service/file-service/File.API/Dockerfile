# Use the official .NET 8 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["file-service/", "./file-service/"]
COPY ["shared/", "./shared/"]
RUN dotnet restore "file-service/File.API/File.API.csproj"

# Copy the source code (already done above, but ensure everything is copied)
# COPY ["file-service/File.API/", "file-service/File.API/"]

# Build the application
RUN dotnet build "file-service/File.API/File.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "file-service/File.API/File.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET 8 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create uploads directory
RUN mkdir -p /app/uploads

# Expose the port the app runs on
EXPOSE 5005

# Set the entry point
ENTRYPOINT ["dotnet", "File.API.dll"]