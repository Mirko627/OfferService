FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

ARG MIRKO_NUGET_USERNAME
ARG MIRKO_NUGET_PWD

ENV NUGET_USERNAME=$MIRKO_NUGET_USERNAME
ENV NUGET_PASSWORD=$MIRKO_NUGET_PWD

WORKDIR /src

# NuGet
COPY NuGet.config .

# Copio i csproj
COPY OfferService.API/OfferService.API.csproj OfferService.API/
COPY OfferService.Business/OfferService.Business.csproj OfferService.Business/
COPY OfferService.Repository/OfferService.Repository.csproj OfferService.Repository/
COPY OfferService.Shared/OfferService.Shared.csproj OfferService.Shared/

# Restore
RUN dotnet restore OfferService.API/OfferService.API.csproj

# Copio tutto il resto
COPY . .

WORKDIR /src/OfferService.API

RUN dotnet publish OfferService.API.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "OfferService.API.dll"]
