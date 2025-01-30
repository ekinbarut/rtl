FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MazeScraper.Api/MazeScraper.Api.csproj", "MazeScraper.Api/"]
COPY ["MazeScraper.Common/MazeScraper.Common.csproj", "MazeScraper.Common/"]
COPY ["MazeScraper.Data/MazeScraper.Data.csproj", "MazeScraper.Data/"]
COPY ["MazeScraper.Domain/MazeScraper.Domain.csproj", "MazeScraper.Domain/"]
RUN dotnet restore "MazeScraper.Api/MazeScraper.Api.csproj"
COPY . .
WORKDIR "/src/MazeScraper.Api"
RUN dotnet build "MazeScraper.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MazeScraper.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MazeScraper.Api.dll"]
