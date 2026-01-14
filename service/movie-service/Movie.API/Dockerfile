FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Movie.API.csproj", "."]
COPY ["../Shared.JWT/Shared.JWT.csproj", "../Shared.JWT/"]
RUN dotnet restore "Movie.API.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "Movie.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Movie.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Movie.API.dll"]