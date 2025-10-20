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
ARG BUILD_CONFIGURATION=Release

# Update packages to latest version to ensure we get voulnerabilities as quick as possible.
RUN apt-get update
RUN apt-get upgrade --assume-yes --no-install-recommends

# Clean up after updating and installing packages.
RUN apt-get clean
RUN rm -rf /var/cache/apt/archives

WORKDIR /src
COPY ["Conferenti.Api.csproj", "."]
RUN dotnet restore "./Conferenti.Api.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./Conferenti.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Conferenti.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN chown --recursive application-user .
ENTRYPOINT ["dotnet", "Conferenti.Api.dll"]