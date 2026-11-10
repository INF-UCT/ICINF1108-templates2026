# BUILD STAGE ========================================================

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /app

# Restore dependencies first so this layer is cached while only code changes
COPY estudiantes_icinf.csproj ./
COPY .config/dotnet-tools.json ./.config/
RUN dotnet restore && dotnet tool restore

COPY . .
RUN dotnet publish -c Release -o /out --no-restore

# DEVELOPMENT STAGE ========================================================

FROM build AS dev

ENV ASPNETCORE_ENVIRONMENT=Development \
    ASPNETCORE_HTTP_PORTS=3000 \
    DOTNET_USE_POLLING_FILE_WATCHER=true

EXPOSE 3000

# `dotnet watch` recompiles and restarts on every code change (hot reload).
# Migrations are applied by the app itself on startup (Program.cs).
CMD ["dotnet", "watch", "run", "--no-launch-profile", "--non-interactive"]

# PRODUCTION STAGE ========================================================

# Runtime-only image: no SDK, no compiler, no source code.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS prod

ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_HTTP_PORTS=3000

WORKDIR /app

COPY --from=build /out ./

# Non-root user provided by the official .NET images
USER $APP_UID

EXPOSE 3000

ENTRYPOINT ["dotnet", "estudiantes_icinf.dll"]
