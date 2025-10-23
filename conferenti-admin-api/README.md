# Conferenti Admin API (Go)

A REST API built with Go for managing conference speakers and sessions. Features Auth0 authentication, Azure Cosmos DB integration, and comprehensive testing suite.

## 🚀 Features

- **Clean Architecture** - Repository, Service, Handler layers with dependency injection
- **Auth0 JWT Authentication** - Secure API endpoints with scope-based authorization
- **Azure Cosmos DB Integration** - NoSQL database with cross-partition queries
- **Comprehensive Testing** - Unit tests with mocks, coverage reports, and benchmarks
- **Docker Support** - Multi-stage builds with non-root security
- **Request Logging** - Structured middleware for debugging and monitoring
- **CORS Enabled** - Cross-origin resource sharing for web clients
- **Performance Optimized** - Fast routing with Gorilla Mux

## API Endpoints

### Health Check

```http
GET /api/v1/health
```

### Speakers (Auth Required)

```http
# Get all speakers
GET /api/v1/speakers
Authorization: Bearer <jwt_token>
Scope: admin:execute

# Create speaker
POST /api/v1/speakers
Authorization: Bearer <jwt_token>
Scope: admin:execute
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john@example.com",
  "position": "Senior Developer",
  "company": "Tech Corp",
  "bio": "Experienced speaker on cloud technologies",
  "photoUrl": "https://example.com/photo.jpg"
}
```

## 🛠️ Technology Stack

- **Language**: Go 1.25.3
- **Router**: Gorilla Mux
- **Database**: Azure Cosmos DB
- **Authentication**: Auth0 JWT
- **Testing**: stretchr/testify
- **Containerization**: Docker with Alpine Linux
- **Environment**: godotenv for configuration

## 🚀 Getting Started

### Prerequisites

- Go 1.25+ installed
- Docker and Docker Compose
- Azure Cosmos DB account (or local emulator)
- Auth0 application configured

### Environment Setup

1. **Copy environment files:**

```bash
cp .env.common.example .env.common
cp .env.local.example .env.local
```

2. **Configure environment variables:**

```bash
# .env.common or .env.local
AUTH0_DOMAIN=your-domain.eu.auth0.com
AUTH0_CLIENT_ID=your_client_id
AUTH0_CLIENT_SECRET=your_client_secret
AUTH0_AUDIENCE=https://admin.conferenti.com/api/
AUTH0_SCOPE=openid profile email admin:execute

COSMOSDB_ENDPOINT=https://your-account.documents.azure.com:443/
COSMOSDB_KEY=your_cosmos_key
COSMOSDB_DATABASE=ConferentiDatabase
COSMOSDB_SPEAKER_CONTAINER=SpeakerContainer
COSMOSDB_SESSION_CONTAINER=SessionContainer
```

### Local Development

1. **Install dependencies:**

```bash
go mod download
```

2. **Run tests:**

```bash
make test-unit          # Unit tests
make test-coverage      # Coverage reports
make test-race          # Race condition detection
```

3. **Build and run:**

```bash
make build              # Build binary
./conferentiadmin       # Run locally

# Or run directly
go run ./cmd/admin-api
```

### Docker Development

1. **Build and run with Docker Compose:**

```bash
# From project root
docker compose -f conferenti-admin-api/docker-compose.yml up -d --build
```

2. **View logs:**

```bash
docker compose -f conferenti-admin-api/docker-compose.yml logs -f
```

3. **Stop services:**

```bash
docker compose -f conferenti-admin-api/docker-compose.yml down
```

## Testing

The project includes comprehensive testing with multiple targets:

```bash
# Run all tests
make test

# Specific test suites
make test-repos         # Repository layer tests
make test-services      # Service layer tests
make test-handlers      # HTTP handler tests
make test-middleware    # Middleware tests

# Performance and coverage
make benchmark          # Run benchmarks
make test-coverage      # Generate coverage reports
make test-race          # Race condition detection

# Install test dependencies
make test-deps

# Clean test artifacts
make test-clean
```

## Project Structure

```
conferenti-admin-api/
├── cmd/admin-api/       # Application entry point
│   └── main.go
├── api/                 # API routing and dependency injection
│   └── router.go
├── config/              # Configuration management
├── database/            # Database connection and setup
├── handlers/            # HTTP request handlers
│   ├── health_handler.go
│   ├── speaker_handler.go
│   └── *_handler_test.go
├── middleware/          # HTTP middleware
│   ├── auth.go
│   ├── cors.go
│   ├── logging.go
│   └── *_test.go
├── models/              # Data models and DTOs
│   ├── models.go
│   └── dto/
├── repositories/        # Data access layer
│   ├── speaker_repository.go
│   ├── session_repository.go
│   └── *_repository_test.go
├── services/            # Business logic layer
│   ├── speaker_service.go
│   ├── session_service.go
│   └── *_service_test.go
├── interfaces/          # Interface definitions for DI
├── helpers/             # Utility functions
├── Dockerfile           # Multi-stage Docker build
├── docker-compose.yml   # Local development setup
├── Makefile            # Build and test automation
└── .env.*              # Environment configuration
```

## Development Workflow

### Making Changes

1. **Create feature branch:**

```bash
git checkout -b feature/your-feature-name
```

2. **Write tests first (TDD approach):**

```bash
# Create test file
touch handlers/new_handler_test.go
make test-unit
```

3. **Implement feature:**

```bash
# Add implementation
make test-unit          # Verify tests pass
make test-coverage      # Check coverage
```

4. **Validate build:**

```bash
make build              # Build locally
docker compose up --build  # Test Docker build
```

### Debugging

1. **VS Code Debug Configuration:**

   - Launch configuration available for debugging
   - Breakpoints supported in handlers, services, repositories

2. **Environment Variable Loading:**

   - `.env.local` loads automatically in debug mode
   - Database connections tested on startup

3. **Logging:**
   - Request/response logging middleware
   - Structured error messages
   - Debug endpoints for database connectivity

## Authentication & Authorization

### Auth0 Setup

1. **Create Auth0 Application:**

   - Type: Machine to Machine API
   - Audience: `https://admin.conferenti.com/api/`
   - Scopes: `admin:execute`

2. **Configure Middleware:**

   - JWT validation on protected routes
   - Scope-based authorization
   - Automatic token verification

3. **Testing with Auth Token:**

```bash
# Get token from Auth0
TOKEN="your_jwt_token"

# Test authenticated endpoint
curl -H "Authorization: Bearer $TOKEN" \
     http://localhost:8080/api/v1/speakers
```

## 🗄️ Database Schema

### Speaker Model

```json
{
  "id": "uuid",
  "name": "string",
  "email": "string",
  "position": "string",
  "company": "string",
  "bio": "string",
  "photoUrl": "string",
  "sessions": [],
  "createdAt": "timestamp",
  "updatedAt": "timestamp"
}
```

### Session Model

```json
{
  "id": "uuid",
  "title": "string",
  "slug": "string",
  "tags": ["string"],
  "description": "string",
  "room": "string",
  "level": "beginner|intermediate|advanced",
  "format": "Lecture|Workshop|Panel|Keynote",
  "language": "string",
  "speakerIds": ["uuid"],
  "createdAt": "timestamp",
  "updatedAt": "timestamp"
}
```

### Production Build

```bash
# Build optimized Docker image
docker build -t conferenti/admin-api:latest .

# Run production container
docker run -d \
  --name conferenti-admin-api \
  -p 8080:8080 \
  --env-file .env.production \
  conferenti/admin-api:latest
```

### Environment Variables (Production)

```bash
# Required
AUTH0_DOMAIN=production-domain.auth0.com
COSMOSDB_ENDPOINT=https://prod-cosmos.documents.azure.com:443/
COSMOSDB_KEY=production_key

# Optional
PORT=8080
LOG_LEVEL=info
```

## Contributing

1. Fork the repository
2. Create feature branch (`git checkout -b feature/amazing-feature`)
3. Write tests for your changes
4. Commit changes (`git commit -m 'Add amazing feature'`)
5. Push to branch (`git push origin feature/amazing-feature`)
6. Open Pull Request

## License

This project is licensed under the [Apache License 2.0](LICENSE).

### Debug Commands

```bash
# Check environment loading
go run ./cmd/admin-api --debug

# Test database connection
curl http://localhost:8080/api/v1/health

# View container logs
docker logs admin-api-test
```
