# =========================
# BUILD STAGE
# =========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# =========================
# ARG per GitHub Packages
# =========================
ARG MIRKO_NUGET_USERNAME
ARG MIRKO_NUGET_PWD

# =========================
# REGISTRA FEED NUGET PRIVATO
# =========================
RUN dotnet nuget add source https://nuget.pkg.github.com/Mirko627/index.json \
    --name github \
    --username $MIRKO_NUGET_USERNAME \
    --password $MIRKO_NUGET_PWD \
    --store-password-in-clear-text

# =========================
# COPY csproj (layer caching)
# =========================
COPY NuGet.config . 
COPY OfferService.API/OfferService.API.csproj OfferService.API/
COPY OfferService.Business/OfferService.Business.csproj OfferService.Business/
COPY OfferService.Repository/OfferService.Repository.csproj OfferService.Repository/
COPY OfferService.Shared/OfferService.Shared.csproj OfferService.Shared/

# =========================
# RESTORE
# =========================
RUN dotnet restore OfferService.API/OfferService.API.csproj

# =========================
# COPY FULL SOURCE
# =========================
COPY . .

WORKDIR /src/OfferService.API

# =========================
# PUBLISH
# =========================
RUN dotnet publish OfferService.API.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# =========================
# RUNTIME STAGE
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "OfferService.API.dll"]