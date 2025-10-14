## Components

### Conferenti API

- Built with .NET 9 Minimal APIs
- Features: API versioning, JWT authentication, OpenAPI/Swagger, Azure Key Vault integration, Application Insights telemetry
- [API README & Source](https://github.com/kkho/Conferenti/tree/main/converenti-api)

### Conferenti Single Page Application (SPA)

- Built with Next.js (React)
- NPM-based workflow
- [SPA README & Source](https://github.com/kkho/Conferenti/tree/main/conferenti-web)

### Mobile Apps

- **Android** and **iOS** apps (under development)

---

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js & npm](https://nodejs.org/) (for frontend)
- (Optional) [Azure Key Vault Emulator](https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/keyvault/Azure.Security.KeyVault.Emulator)
- (Optional) [Azure Cosmos DB Emulator](https://learn.microsoft.com/azure/cosmos-db/local-emulator) (for local Cosmos DB development)
- Docker (if using containerized services)

---

## Getting Started

1. **Clone the repository**
    ```sh
    git clone https://github.com/kkho/Conferenti.git
    cd Conferenti
    ```

2. **Restore and build the solution**
    ```sh
    dotnet restore
    dotnet build
    ```

3. **Run the distributed application with Aspire**
    ```sh
    cd Conferenti.AppHost/Conferenti.AppHost.AppHost
    dotnet run
    ```

    This will:
    - Start the API backend
    - Start the Next.js frontend (with `npm install` and `npm run dev`)
    - Launch the Key Vault emulator

4. **Access the applications**
    - **API**: [http://localhost:5000/swagger](http://localhost:5000/swagger) (or the port assigned by Aspire)
    - **Frontend**: [http://localhost:3000](http://localhost:3000) (or the port assigned by Aspire)

---

## Troubleshooting

- **Node.js/NPM errors**: Ensure Node.js is installed and available in your system `PATH`.
- **Directory errors**: Verify the relative path to `conferenti-web` in `.AddNpmApp` matches your directory structure.
- **Key Vault issues**: Make sure the emulator is running or configure for real Azure Key Vault.

---

## Contributing

Contributions are welcome! Please open issues or submit pull requests for improvements and bug fixes.

---

## License

This project is licensed under the [Apache License 2.0](LICENSE).