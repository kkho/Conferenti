FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
LABEL maintainer="conferenti@conferenti.com"
WORKDIR /src

#COPY Solutions and project files
COPY ["conferenti-admin-web.sln", "./"]
COPY ["src/Conferenti.Admin.WebApp/Conferenti.Admin.WebApp.csproj", "src/Conferenti.Admin.WebApp/"]

COPY ["src/Conferenti.Infrastructure/Conferenti.Infrastructure.csproj", "src/Conferenti.Infrastructure/"]

Run dotnet restore "src/Conferenti.Admin.WebApp/Conferenti.Admin.WebApp.csproj"

COPY . .

WORKDIR "/src/src/Conferenti.Admin.WebApp"
RUN dotnet build "Conferenti.Admin.WebApp.csproj" -c Release -o /app/build

FROM build AS publish
Run dotnet publish "Conferenti.Admin.WebApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Create non-root user for security
RUN addgroup --system --gid 1001 appuser \
	&& adduser --system --uid 1001 --ingroup appuser appuser

# Copy published application
COPY --from=publish /app/publish .

# Switch ownership
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

#Expose ports
EXPOSE 8080
EXPOSE 8881

# SET environment variables
ENV ASPNETCORE_URLS=http://+:8080;
ENV ASPNETCORE_ENVIRONMENT=Production

# Entry point
ENTRYPOINT ["dotnet", "Conferenti.Admin.WebApp.dll"]

