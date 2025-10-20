# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
LABEL maintainer="conferenti@converenti.com"

# Add non-root user and group
RUN addgroup application-group --gid 1001 \
    && adduser application-user --uid 1001 \
    --ingroup application-group \
    --disabled-password

USER application-user 
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Update packages to latest version to ensure we get voulnerabilities as quick as possible.
RUN apt-get update
RUN apt-get upgrade --assume-yes --no-install-recommends

# Clean up after updating and installing packages.
RUN apt-get clean
RUN rm -rf /var/cache/apt/archives

WORKDIR /src
COPY . .
RUN dotnet restore \
    ./src/Conferenti.Api.csproj \
    && dotnet build \
    ./src/Conferenti.Api.csproj \
    && dotnet publish \
    ./src/Conferenti.Api.csproj \
    --output ./out \
    --configuration Release

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=build /app/out .
RUN chown --recursive application-user .
ENTRYPOINT ["dotnet", "Conferenti.Api.dll"]