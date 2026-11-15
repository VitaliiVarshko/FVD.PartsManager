
# Build-Phase
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore "src/FVD.PartsManager.Web/FVD.PartsManager.Web.csproj"

RUN dotnet publish "src/FVD.PartsManager.Web/FVD.PartsManager.Web.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

# Runtime-Phase
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

USER root

RUN apt-get update \
    && apt-get install -y --no-install-recommends libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

USER $APP_UID

ENTRYPOINT ["dotnet", "FVD.PartsManager.Web.dll"]

