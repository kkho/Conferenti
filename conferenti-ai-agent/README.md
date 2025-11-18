# Conferenti AI Agent

AI-powered agent for managing session operations using Azure OpenAI or Ollama.

## 🚀 Quick Start

**New to this project? Start here:** → [**QUICKSTART.md**](./QUICKSTART.md)

Follow the 5-minute guide to get up and running!

---

## 📖 How to Run the Solution

### Prerequisites

1. **Python 3.11 or higher** installed
2. **One of the following**:
   - Ollama installed (for local development) - [Download](https://ollama.com/download)
   - Azure OpenAI access (for production)
   - aoai-api-simulator running (for testing)

### Step 1: Install Dependencies

```powershell
cd d:\Hobby\Conferenti\conferenti-ai-agent

# Install all required packages
pip install -r requirements.txt
```

### Step 2: Choose Your Backend

#### Option A: Using Ollama (Recommended for Development)

```powershell
# 1. Install Ollama
winget install Ollama.Ollama

# 2. Pull a model
ollama pull llama3.2

# 3. Verify Ollama is running
ollama list
```

Create `.env` file:

```env
PROJECT_ENDPOINT=http://localhost:11434/v1
MODEL_DEPLOYMENT_NAME=llama3.2
API_KEY=ollama
LOG_LEVEL=INFO
```

#### Option B: Using Azure OpenAI (Production)

Create `.env` file:

```env
PROJECT_ENDPOINT=https://your-resource.openai.azure.com
MODEL_DEPLOYMENT_NAME=gpt-4
API_KEY=your-azure-api-key
LOG_LEVEL=INFO
```

#### Option C: Using aoai-api-simulator (Testing)

```powershell
# 1. Start the simulator
cd aoai-api-simulator
pip install -r requirements.txt
python -m uvicorn aoai_api_simulator.main:app --host 0.0.0.0 --port 8000

# 2. In another terminal, create .env file
```

Create `.env` file:

```env
PROJECT_ENDPOINT=http://localhost:8000
MODEL_DEPLOYMENT_NAME=gpt-35-turbo-10k-token
API_KEY=<get-from-simulator-startup-logs>
LOG_LEVEL=INFO
```

### Step 3: Run the Agent

#### Interactive Chat Mode

```powershell
# Run the main agent
python -m conferenti_agent.main
```

You'll see:

```
🤖 Starting Conferenti AI Agent...
📍 Endpoint: http://localhost:11434/v1
🎯 Model: llama3.2
✅ Agent 'conferenti-agent' created successfully!

💬 Starting interactive chat (type 'exit' to quit, 'clear' to reset):

You:
```

Now you can chat with the agent!

**Commands:**

- Type your questions naturally
- Type `clear` to reset conversation history
- Type `exit` or `quit` to stop

#### Example Conversations

```
You: What is Conferenti?
Agent: Conferenti is a conference system...

You: Suggest 3 speakers for an AI session
Agent: Here are 3 excellent speaker suggestions...

You: clear
🧹 Conversation history cleared.

You: exit
👋 Goodbye!
```

## Alternative: Running as Package

### Installation

#### Option 1: Development Install (Editable)

```powershell
# Install in editable mode with dev dependencies
pip install -e ".[dev]"
```

#### Option 2: Regular Install

```powershell
# Install the package
pip install .

# Or with specific optional dependencies
pip install ".[test]"
```

#### Option 3: From Built Wheel

```powershell
# Build the package
python -m build

# Install from dist folder
pip install dist/conferenti_agent-0.1.0-py3-none-any.whl
```

### Environment Setup

Create a `.env` file:

```bash
PROJECT_ENDPOINT=https://your-project.openai.azure.com/
MODEL_DEPLOYMENT_NAME=gpt-4
AZURE_TENANT_ID=your-tenant-id
AZURE_CLIENT_ID=your-client-id
```

## 📦 Building the Package

### Install Build Tools

```powershell
pip install build
```

### Build Distribution Files

```powershell
# Creates both wheel (.whl) and source (.tar.gz) in dist/
python -m build
```

### Build Specific Format

```powershell
# Build only wheel
python -m build --wheel

# Build only source distribution
python -m build --sdist
```

## Running Tests

```powershell
# Run all tests
pytest

# Run with coverage
pytest --cov=conferenti_agent --cov-report=html

# Run specific test file
pytest test/test_agent.py

# Run with verbose output
pytest -v
```

## Development Tools

### Code Formatting

```powershell
# Format code with Black
black src/ test/

# Check without modifying
black --check src/ test/
```

### Linting

```powershell
# Run Ruff linter
ruff check src/ test/

# Auto-fix issues
ruff check --fix src/ test/
```

### Type Checking

```powershell
# Run MyPy type checker
mypy src/
```

### Run All Checks

```powershell
# Format, lint, and type check
black src/ test/ && ruff check src/ test/ && mypy src/
```

## Common Commands

### Install Dependencies Only

```powershell
# Production dependencies
pip install -r requirements.txt

# Development dependencies
pip install -e ".[dev]"
```

### Generate Requirements File

```powershell
# From pyproject.toml
pip-compile pyproject.toml -o requirements.txt

# Or manually
pip freeze > requirements.txt
```

### Clean Build Artifacts

```powershell
# Remove build directories
Remove-Item -Recurse -Force dist, build, *.egg-info -ErrorAction SilentlyContinue

# Clean Python cache
Get-ChildItem -Recurse -Directory __pycache__ | Remove-Item -Recurse -Force
Get-ChildItem -Recurse -Filter "*.pyc" | Remove-Item -Force
```

## 🌐 Run API Server

### Run by Environment

The API automatically uses your `.env` file configuration.

**Option 1: Using uvicorn (Recommended)**

```powershell
# From conferenti-ai-agent directory
APP_ENV=dev python -m uvicorn conferenti_agent.services.api_client:app --reload --port 8000
```

**Option 2: Direct Python execution**

```powershell
python -m conferenti_agent.services.api_client
```

> ✅ Both methods automatically load `.env` from the current directory

### Environment-Specific Examples

**Development (Ollama):**

```powershell
# .env file:
# PROJECT_ENDPOINT=http://localhost:11434/v1
# MODEL_DEPLOYMENT_NAME=llama3.2
# API_KEY=ollama

python -m uvicorn conferenti_agent.services.api_client:app --reload --port 8000
# ✅ API uses Ollama backend at localhost:11434
```

**Production (Azure OpenAI):**

```powershell
# .env file:
# PROJECT_ENDPOINT=https://your-azure.openai.azure.com
# MODEL_DEPLOYMENT_NAME=gpt-4
# API_KEY=sk-xxxxx

python -m uvicorn conferenti_agent.services.api_client:app --reload --port 8000
# API uses Azure OpenAI backend
```

**Using Custom .env File:**

```powershell
# If your .env file has a different name or location
# PowerShell:
$env:ENV_FILE=".env.production"; python -m uvicorn conferenti_agent.services.api_client:app --port 8000

# Or load manually:
Get-Content .env.production | ForEach-Object {
    if ($_ -match '^([^=]+)=(.*)$') {
        [Environment]::SetEnvironmentVariable($matches[1], $matches[2])
    }
}
python -m uvicorn conferenti_agent.services.api_client:app --port 8000
```

### Testing with protected endpoints

```powershell
curl --request POST \
  --url https://your-tenant.eu.auth0.com/oauth/token \
  --header 'content-type: application/json' \
  --data '{
    "grant_type": "client_credentials",
    "client_id": "abc123...",
    "client_secret": "xyz789...",
    "audience": "https://api.conferenti.com"
  }'


TOKEN="eyJhbGc..."  # Token from above

curl -X POST http://localhost:8000/api/speakers/suggest-general \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "theme": "Cloud Computing",
    "topics": ["Azure", "Kubernetes"],
    "count": 5
  }'
```

### Access API Documentation

Once running, access:

- **Swagger UI**: http://localhost:8000/docs
- **ReDoc**: http://localhost:8000/redoc
- **Health Check**: http://localhost:8000/health

### Quick API Test

```powershell
# Health check
curl http://localhost:8000/health

# Get speakers
curl http://localhost:8000/api/speakers

# AI: Suggest speakers for session
curl -X POST http://localhost:8000/api/speakers/suggest `
  -H "Content-Type: application/json" `
  -d '{\"session_id\": \"session_123\", \"count\": 3}'
```

---

## 🐳 Running with Docker

### Build Docker Image

```powershell
# From conferenti-ai-agent directory
docker build -t conferenti-ai-agent:latest .
```

### Run by Environment

#### Development (Ollama on Host Machine)

```powershell
# Connect to Ollama running on host
docker run -d \
  --name conferenti-agent \
  -p 8000:8000 \
  -e PROJECT_ENDPOINT=http://host.docker.internal:11434/v1 \
  -e MODEL_DEPLOYMENT_NAME=llama3.2 \
  -e API_KEY=ollama \
  conferenti-ai-agent:latest
```

#### Production (Azure OpenAI)

```powershell
# Use Azure OpenAI
docker run -d \
  --name conferenti-agent \
  -p 8000:8000 \
  -e PROJECT_ENDPOINT=https://your-azure.openai.azure.com \
  -e MODEL_DEPLOYMENT_NAME=gpt-4 \
  -e API_KEY=your-azure-api-key \
  conferenti-ai-agent:latest
```

#### Using .env File

```powershell
# Create .env file first, then:
docker run -d \
  --name conferenti-agent \
  -p 8000:8000 \
  --env-file .env \
  conferenti-ai-agent:latest
```

## Troubleshooting

### Import Issues

If you can't import the package after installation:

```powershell
# Verify installation
pip list | Select-String conferenti

# Reinstall in editable mode
pip uninstall conferenti-agent
pip install -e .
```

### Build Failures

```powershell
# Upgrade build tools
pip install --upgrade build setuptools wheel

# Clear cache and rebuild
Remove-Item -Recurse -Force dist, build
python -m build
```

---

## 🔐 Azure Key Vault Integration

The Python implementation mirrors the C# `KeyVaultSettings` pattern from `conferenti-api` with three modes:

### Mode 1: Bypass (Local Development)

Skip Key Vault entirely and use `.env` file only. This is the **default for local development**.

**C# equivalent:** `BypassKeyVault=true` in `appsettings.json`

```env
# .env file
BYPASS_KEY_VAULT=true
API_KEY=your-api-key
COSMOS_DB_KEY=your-cosmos-key
```

```powershell
# Run normally
python -m uvicorn conferenti_agent.services.api_client:app --reload
```

### Mode 2: Key Vault Emulator (Local Development with Emulator)

Use Azure Key Vault emulator for local testing. Requires running the emulator first.

**C# equivalent:** `UseEmulator=true` in `appsettings.json`

```env
# .env file
BYPASS_KEY_VAULT=false
USE_KEY_VAULT_EMULATOR=true
KEY_VAULT_EMULATOR_ENDPOINT=http://localhost:5000
```

**Setup Azure Key Vault Emulator:**

```powershell
# 1. Run Key Vault emulator (check C# conferenti-api setup)
# Follow your existing emulator setup from conferenti-api

# 2. Add secrets to emulator
# Use your emulator's API or CLI to add:
# - CONFERENTI-API-KEY
# - COSMOS-DB-KEY

# 3. Run the agent
python -m uvicorn conferenti_agent.services.api_client:app --reload
```

### Mode 3: Production Azure Key Vault

Use real Azure Key Vault with Managed Identity or Service Principal.

**C# equivalent:** Real `VaultEndPoint` with `BypassKeyVault=false`

#### Using Managed Identity (Recommended)

```env
# .env file
BYPASS_KEY_VAULT=false
USE_KEY_VAULT_EMULATOR=false
AZURE_KEY_VAULT_NAME=my-vault
```

```powershell
# Deploy to Azure with Managed Identity enabled
# Ensure your App Service/Container App has Managed Identity
# Grant Key Vault access: "Key Vault Secrets User" role
```

#### Using Service Principal (Alternative)

```env
# .env file
BYPASS_KEY_VAULT=false
USE_KEY_VAULT_EMULATOR=false
AZURE_KEY_VAULT_NAME=my-vault
AZURE_TENANT_ID=your-tenant-id
AZURE_CLIENT_ID=your-client-id
AZURE_CLIENT_SECRET=your-client-secret
```

### Key Vault Secrets

The agent fetches these secrets from Key Vault:

| Secret Name          | Purpose                       | Fallback Env Var     |
| -------------------- | ----------------------------- | -------------------- |
| `CONFERENTI-API-KEY` | Conferenti API authentication | `CONFERENTI_API_KEY` |
| `COSMOS-DB-KEY`      | Cosmos DB connection key      | `COSMOS_DB_KEY`      |

### Docker with Key Vault

#### Emulator Mode

```powershell
docker run -d \
  --name conferenti-agent \
  -p 8000:8000 \
  -e BYPASS_KEY_VAULT=false \
  -e USE_KEY_VAULT_EMULATOR=true \
  -e KEY_VAULT_EMULATOR_ENDPOINT=http://host.docker.internal:5000 \
  -e PROJECT_ENDPOINT=http://host.docker.internal:11434/v1 \
  -e MODEL_DEPLOYMENT_NAME=llama3.2 \
  conferenti-ai-agent:latest
```

#### Production with Managed Identity

```powershell
# Deploy to Azure Container Apps with Managed Identity
az containerapp create \
  --name conferenti-agent \
  --resource-group my-rg \
  --image conferenti-ai-agent:latest \
  --environment my-env \
  --ingress external --target-port 8000 \
  --env-vars \
    BYPASS_KEY_VAULT=false \
    USE_KEY_VAULT_EMULATOR=false \
    AZURE_KEY_VAULT_NAME=my-vault \
    PROJECT_ENDPOINT=https://my-azure.openai.azure.com \
    MODEL_DEPLOYMENT_NAME=gpt-4 \
  --system-assigned

# Grant Key Vault access
az keyvault set-policy \
  --name my-vault \
  --object-id <managed-identity-principal-id> \
  --secret-permissions get list
```

#### Production with Service Principal

```powershell
docker run -d \
  --name conferenti-agent \
  -p 8000:8000 \
  -e BYPASS_KEY_VAULT=false \
  -e USE_KEY_VAULT_EMULATOR=false \
  -e AZURE_KEY_VAULT_NAME=my-vault \
  -e AZURE_TENANT_ID=your-tenant-id \
  -e AZURE_CLIENT_ID=your-client-id \
  -e AZURE_CLIENT_SECRET=your-client-secret \
  -e PROJECT_ENDPOINT=https://my-azure.openai.azure.com \
  -e MODEL_DEPLOYMENT_NAME=gpt-4 \
  conferenti-ai-agent:latest
```

### Troubleshooting Key Vault

**"Cannot connect to Key Vault":**

```powershell
# Check network connectivity
curl https://my-vault.vault.azure.net

# Verify authentication
az login
az account show

# Test Key Vault access
az keyvault secret show --vault-name my-vault --name CONFERENTI-API-KEY
```

**"Emulator not working":**

```powershell
# Verify emulator is running
curl http://localhost:5000/health

# Check .env configuration
Get-Content .env | Select-String KEY_VAULT

# Fall back to bypass mode
# Set BYPASS_KEY_VAULT=true in .env
```

---

## 📋 Complete Configuration Reference

### `.env` File Template with Key Vault

```env
# ============================================
# AI Backend Configuration
# ============================================
# Ollama (Local)
PROJECT_ENDPOINT=http://localhost:11434/v1
MODEL_DEPLOYMENT_NAME=llama3.2
API_KEY=ollama

# Azure OpenAI (Production)
# PROJECT_ENDPOINT=https://your-azure.openai.azure.com
# MODEL_DEPLOYMENT_NAME=gpt-4
# API_KEY=sk-xxxxx

# ============================================
# Azure Key Vault Configuration
# Mirrors C# KeyVaultSettings
# ============================================
# Mode 1: Bypass (default for local dev)
BYPASS_KEY_VAULT=true

# Mode 2: Key Vault Emulator
# BYPASS_KEY_VAULT=false
# USE_KEY_VAULT_EMULATOR=true
# KEY_VAULT_EMULATOR_ENDPOINT=http://localhost:5000

# Mode 3: Production Azure Key Vault
# BYPASS_KEY_VAULT=false
# USE_KEY_VAULT_EMULATOR=false
# AZURE_KEY_VAULT_NAME=my-vault

# Service Principal Auth (optional, for non-Managed Identity)
# AZURE_TENANT_ID=your-tenant-id
# AZURE_CLIENT_ID=your-client-id
# AZURE_CLIENT_SECRET=your-client-secret

# ============================================
# Cosmos DB Configuration
# Mirrors C# CosmosDbSettings
# ============================================
COSMOS_DB_USE_LOCAL=true
COSMOS_DB_ENDPOINT=https://localhost:8081
COSMOS_DB_DATABASE_NAME=conferenti

# Local emulator key (public key, safe to commit)
COSMOS_DB_KEY=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==

# Production key (fetch from Key Vault)
# COSMOS_DB_KEY=your-production-key

# ============================================
# Conferenti API
# ============================================
CONFERENTI_API_URL=http://localhost:5000/api
# CONFERENTI_API_KEY=your-api-key

# ============================================
# Logging
# ============================================
LOG_LEVEL=INFO
```

### Key Vault Secret Names

When using Key Vault (emulator or production), ensure these secrets exist:

| Secret Name          | Description                   | Local Dev Value        |
| -------------------- | ----------------------------- | ---------------------- |
| `CONFERENTI-API-KEY` | Conferenti API authentication | Can be empty for local |
| `COSMOS-DB-KEY`      | Cosmos DB connection key      | Emulator public key    |

---

## License

See LICENSE file for details.
