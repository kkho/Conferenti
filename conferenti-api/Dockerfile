FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
LABEL maintainer="conferenti@conferenti.com"

# Update packages to latest version to ensure we get voulnerabilities as quick as possible.
RUN apt-get update
RUN apt-get upgrade --assume-yes --no-install-recommends

# Clean up after updating and installing packages.
RUN apt-get clean
RUN rm -rf /var/cache/apt/archives

WORKDIR /src
COPY . .
RUN dotnet restore \
    ./src/Conferenti.Api/Conferenti.Api.csproj \
    && dotnet build \
    ./src/Conferenti.Api/Conferenti.Api.csproj \
    && dotnet publish \
    ./src/Conferenti.Api/Conferenti.Api.csproj \
    --output ./out \
    --configuration Release


FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
# Workaround
# Unhandled exception. System.NotSupportedException: Globalization Invariant Mode is not supported.
# https://github.com/dotnet/SqlClient/issues/220
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV COMPlus_EnableDiagnostics=0
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

# Workaround
# crit: Microsoft.AspNetCore.Server.Kestrel[0]
#       Unable to start Kestrel.
# System.Net.Sockets.SocketException (13): Permission denied
# https://github.com/dotnet/aspnetcore/issues/4699
ENV ASPNETCORE_URLS=http://*:8080

# Add non-root user and group
RUN addgroup application-group --gid 1001 \
    && adduser application-user --uid 1001 \
    --ingroup application-group \
    --disabled-password

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=build /src/out .
RUN chown --recursive application-user .
User application-user
EXPOSE 8080
EXPOSE 8081
ENTRYPOINT ["dotnet", "Conferenti.Api.dll"]