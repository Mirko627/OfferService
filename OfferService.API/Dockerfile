# =========================
# STAGE 1 - BUILD
# =========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Argomenti per NuGet (GitHub Packages)
ARG MIRKO_NUGET_USERNAME
ARG MIRKO_NUGET_PWD

WORKDIR /src

# Copio SOLO ciò che serve al restore
COPY OfferService/NuGet.config ./NuGet.config
COPY OfferService/OfferService.API/OfferService.API.csproj OfferService.API/
COPY OfferService/OfferService.Business/OfferService.Business.csproj OfferService.Business/
COPY OfferService/OfferService.Repository/OfferService.Repository.csproj OfferService.Repository/
COPY OfferService/OfferService.Shared/OfferService.Shared.csproj OfferService.Shared/

# Restore
RUN dotnet restore OfferService.API/OfferService.API.csproj

# Ora copio tutto il resto
COPY OfferService/. ./OfferService/

WORKDIR /src/OfferService/OfferService.API

# Build + Publish
RUN dotnet publish OfferService.API.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# =========================
# STAGE 2 - RUNTIME
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "OfferService.API.dll"]
