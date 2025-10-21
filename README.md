# Conferenti – The intelligent platform for modern conferences

<p align="center">
      <img src="public/Peer_Hexagon_Lys.svg" alt="peer consulting logo" width="50" height="50" style="text-align:center;"/>
   </p>

I would like to thank my company [Peer Consulting](https://peerconsulting.no/) for sponsoring this project, it would have never been possible without them!

| Api 1 |  Frontend |
|---|---|
| [![Api](https://github.com/kkho/Conferenti/actions/workflows/api-test-build-deploy.yaml/badge.svg?branch=main)](https://github.com/kkho/Conferenti/actions/workflows/api-test-build-deploy.yaml) | [![Frontend](https://github.com/kkho/Conferenti/actions/workflows/frontend-test-build-deploy.yaml/badge.svg?branch=main)](https://github.com/kkho/Conferenti/actions/workflows/frontend-test-build-deploy.yaml) |



<p style="margin-bottom: '32px'">
Conferenti is a modern open-source platform developed to revolutionize the way conferences and events are organized and executed.
The goal of the project is to provide a flexible, scalable, and user-friendly solution that can be used by all stakeholders – from small seminars to large international conferences – without the limitations that characterize many of today’s solutions.

The platform is built on modern technology: a Next.js application with React v18+ (using React Hooks, Zustand and React Query for optimized API calls) for the frontend, a robust .NET 9 API backend, and a reliable infrastructure running in Kubernetes. The entire development process is automated through an agile CI/CD framework, ensuring frequent, stable, and high-quality deliveries.

The work is organized according to Kanban methodology, with focusing on their respective areas of responsibility within infrastructure, backend, mobile, and frontend. This way of working provides both high development speed and great flexibility, while also facilitating continuous improvements.

Through the open-source principle, Conferenti invites collaboration and further development from the community. In this way, a solution is created that not only meets today’s needs but is also continuously shaped to address the challenges and opportunities of tomorrow.
</p>


<img width="1643" height="1020" alt="image" src="https://github.com/user-attachments/assets/febcbf70-5843-4dfb-a731-0e9cc1c713c3" />

## Components

### Conferenti API

- Built with .NET 9 Minimal APIs
- Features: API versioning, JWT authentication, OpenAPI/Swagger, Azure Key Vault integration, Application Insights telemetry
- [API README & Source](https://github.com/kkho/Conferenti/tree/main/conferenti-api)

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
