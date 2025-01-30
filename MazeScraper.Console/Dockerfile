FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MazeScraper.Console/MazeScraper.Console.csproj", "MazeScraper.Console/"]
COPY ["MazeScraper.Client/MazeScraper.Client.csproj", "MazeScraper.Client/"]
COPY ["MazeScraper.Common/MazeScraper.Common.csproj", "MazeScraper.Common/"]
COPY ["MazeScraper.Data/MazeScraper.Data.csproj", "MazeScraper.Data/"]
COPY ["MazeScraper.Domain/MazeScraper.Domain.csproj", "MazeScraper.Domain/"]
RUN dotnet restore "MazeScraper.Console/MazeScraper.Console.csproj"
COPY . .
WORKDIR "/src/MazeScraper.Console"
RUN dotnet build "MazeScraper.Console.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MazeScraper.Console.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MazeScraper.Console.dll"]
