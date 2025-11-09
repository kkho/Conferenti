# Conferenti Admin Web App

A Blazor Web App for managing Conferenti conference administration, built with .NET 9 and FluentUI components.

## Features

- **Auth0 Authentication**: Secure authentication with role-based authorization
- **Interactive Server Rendering**: Blazor Server components for rich interactivity
- **FluentUI Design**: Modern UI components from Microsoft FluentUI
- **Role-Based Access**: Support for `AdminRole` and `AdminRead` roles
- **Service Bus Integration**: Azure Service Bus for messaging

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (17.8 or later) or [VS Code](https://code.visualstudio.com/)
- Auth0 Account (for authentication)
- Azure Service Bus (for messaging features)

## Configuration

### 1. Auth0 Setup

Create an `appsettings.Development.json` file in the project root:
```
{ "Auth0": { "Domain": "your-tenant.auth0.com", "ClientId": "your-client-id", "ClientSecret": "your-client-secret", "Audience": "https://admin.conferenti.com/api/" } }
```

**Note:** Never commit `appsettings.Development.json` or `appsettings.Local.json` to version control.

### 2. Service Bus Configuration

Add Service Bus settings to your configuration file:

```
{ "ServiceBus": { "ConnectionString": "your-service-bus-connection-string", "QueueName": "your-queue-name" } }
```

### 3. User Secrets (Recommended for Development)

Instead of modifying `appsettings.Development.json`, use User Secrets:

```
dotnet user-secrets init
dotnet user-secrets set "Auth0:Domain" "your-tenant.auth0.com"
dotnet user-secrets set "Auth0:ClientId" "your-client-id"
dotnet user-secrets set "Auth0:ClientSecret" "your-client-secret"
dotnet user-secrets set "ServiceBus:ConnectionString" "your-connection-string"
```

## Running the Application

### Using Visual Studio

1. Open `Conferenti.Admin.WebApp.sln`
2. Press `F5` or click the Run button
3. Navigate to `https://localhost:5001`

### Using .NET CLI

```bash
# Run the application
dotnet run --project Conferenti.Admin.WebApp.csproj

# Open in browser
start https://localhost:5001

cd src/Conferenti.Admin.WebApp
dotnet run
```

The application will be available at `https://localhost:5001`

## Authentication & Authorization

### Roles

- **AdminRole**: Required to access the application

### Login Flow

1. Navigate to the home page
2. If not authenticated, click "Logg inn" button
3. Complete Auth0 authentication
4. You'll be redirected back to the application

### Logout

Click your profile or navigate to `/Account/Logout`

## Development

### Adding New Pages

1. Create a new `.razor` file in `Components/Pages/`
2. Add the `@page` directive with the route
3. Add `@attribute [Authorize(Roles = "AdminRole")]` if authentication is required
4. Add `@rendermode InteractiveServer` if interactivity is needed
5. Update `NavMenu.razor` to add navigation link

Example:

```
@page "/mypage" @attribute [Authorize(Roles = "AdminRole")] @rendermode InteractiveServer
<PageTitle>My Page</PageTitle>
<h1>My Page</h1>
@code { // Component logic }
```

### Styling

- Global styles: `wwwroot/app.css`
- Component-specific styles: Create a `.razor.css` file next to your component

## Testing

```
dotnet test
```

## Building for Production

```
dotnet publish -c Release -o ./publish
```

## Deployment

### Azure App Service

1. Create an Azure App Service (Windows or Linux)
2. Configure App Settings in Azure Portal:
   - Add Auth0 configuration
   - Add Service Bus connection string
3. Deploy using Visual Studio, Azure CLI, or GitHub Actions

### Docker (Optional)

```
docker build -t conferenti-admin-web . docker run -p 8080:80 conferenti-admin-web
```

## Troubleshooting

### NavigationException during OnInitializedAsync

Use `OnAfterRenderAsync` instead of `OnInitializedAsync` for navigation, or add `@rendermode InteractiveServer` to the component.

### 404 Page Not Showing

Ensure `MapFallback` is configured in `Program.cs` after `MapRazorComponents`.

### Auth0 Login Issues

1. Verify Auth0 configuration in `appsettings.json`
2. Check that callback URLs are configured in Auth0 dashboard
3. Ensure roles are properly assigned in Auth0

## Resources

- [Blazor Documentation](https://learn.microsoft.com/aspnet/core/blazor/)
- [FluentUI Blazor](https://www.fluentui-blazor.net/)
- [Auth0 Blazor Quickstart](https://auth0.com/docs/quickstart/webapp/aspnet-core)

## License

This project is licensed under the [Apache License 2.0](LICENSE).