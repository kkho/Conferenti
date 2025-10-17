# Conferenti API & Aspire Distributed Application

This repository contains the **Conferenti API** (.NET 9, Minimal APIs) and its distributed application setup using [Aspire](https://learn.microsoft.com/dotnet/aspire/). The solution is designed for modern, scalable, and cloud-ready development, featuring secure configuration, API versioning, and a Next.js frontend.

## Features

- **.NET 9 Minimal API**: Fast, lightweight, and modern REST API.
- **Aspire Orchestration**: Distributed application composition and local development.
- **Azure Key Vault Integration**: Secure secrets management (with emulator support).
- **OpenAPI/Swagger**: Interactive API documentation.
- **Authentication & Authorization**: JWT Bearer support.
- **Frontend**: Next.js app with NPM-based workflow.
- **Application Insights**: Telemetry and monitoring.

## Project Structure

- `src/Conferenti.Api`: Main API project (Minimal APIs, OpenAPI, Auth, versioning)
- `src/Conferenti.Domain`: Domain models (e.g., `Speaker`, `Session`)
- `src/Conferenti.Application`: Application logic, CQRS handlers, commands/queries
- `src/Conferenti.Infrastructure`: Data access, repositories (e.g., CosmosDB)
- `aspire/Conferenti.AppHost`: Aspire orchestration and service composition
- `test/`: Unit and integration tests
- `frontend/`: Next.js frontend (if present)

## API Endpoints

- `POST /speakers`: Upsert a list of speakers (requires admin authorization)
- `GET /speakers`: Retrieve all speakers
- (See Swagger UI for full API documentation)

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js & npm](https://nodejs.org/) (for frontend)
- (Optional) [Azure Key Vault Emulator](https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/keyvault/Azure.Security.KeyVault.Emulator)
- Docker (if using containerized services)

## Getting Started

1. **Clone the repository**

```
git clone https://github.com/your-org/conferenti.gi && cd conferenti
```

2. **Restore and build the solution**

```
dotnet restore && dotnet build
```

3. **Run the distributed application with Aspire**

```
cd Conferenti.AppHost/Conferenti.AppHost.AppHost && dotnet run
```

## License

This project is licensed under the [Apache License 2.0](LICENSE).
