# syntax=docker/dockerfile:1.7

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY FileService.csproj ./
RUN dotnet restore FileService.csproj
COPY . .
RUN dotnet publish FileService.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:8080 \
    ASPNETCORE_ENVIRONMENT=Development
EXPOSE 8080
ENTRYPOINT ["dotnet", "FileService.dll"]
