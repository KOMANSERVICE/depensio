# Consultez https://aka.ms/customizecontainer pour savoir comment personnaliser votre conteneur de débogage et comment Visual Studio utilise ce Dockerfile pour générer vos images afin d’accélérer le débogage.

# Cet index est utilisé lors de l’exécution à partir de VS en mode rapide (par défaut pour la configuration de débogage)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Cette phase est utilisée pour générer le projet de service
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["backend/depensio.Api/Depensio.Api.csproj", "backend/depensio.Api/"]
COPY ["backend/depensio.Application/depensio.Application.csproj", "backend/depensio.Application/"]
COPY ["backend/depensio.Domain/depensio.Domain.csproj", "backend/depensio.Domain/"]
COPY ["backend/BuildingBlocks/BuildingBlocks/BuildingBlocks.csproj", "backend/BuildingBlocks/BuildingBlocks/"]
COPY ["backend/depensio.Infrastructure/depensio.Infrastructure.csproj", "backend/depensio.Infrastructure/"]
RUN dotnet restore "./backend/depensio.Api/Depensio.Api.csproj"
COPY . .
WORKDIR "/src/backend/depensio.Api"
RUN dotnet build "./Depensio.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Cette étape permet de publier le projet de service à copier dans la phase finale
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Depensio.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Cette phase est utilisée en production ou lors de l’exécution à partir de VS en mode normal (par défaut quand la configuration de débogage n’est pas utilisée)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Depensio.Api.dll"]