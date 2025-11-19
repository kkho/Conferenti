FROM python:3.12 AS base

# Default build args (can be overridden at runtime via Key Vault)
ARG PROJECT_ENDPOINT=http://localhost:11434/v1
ARG MODEL_DEPLOYMENT_NAME=llama3.2
ARG LOG_LEVEL=INFO

# Azure Key Vault configuration (provided at runtime)
ARG AZURE_KEY_VAULT_NAME=""
ARG AZURE_TENANT_ID=""
ARG AZURE_CLIENT_ID=""
ARG AZURE_CLIENT_SECRET=""
ARG LOCAL=false
ARG AUTH0_DOMAIN
ARG AUTH0_AUDIENCE
ARG COSMOSDB_ENDPOINT
ARG COSMOSDB_DATABASE="ConferentiDatabase"
ARG COSMOSDB_SPEAKER_CONTAINER="SpeakerContainer"
ARG COSMOSDB_SESSION_CONTAINER="SessionContainer"
# Secrets (for development only - use Key Vault in production)
ARG API_KEY=""
ARG COSMOSDB_KEY=""
ARG CONFERENTI_API_KEY=""

ARG AUTH0_CLIENT_ID
ARG AUTH0_CLIENT_SECRET

ENV PYTHONUNBUFFERED=1 \
  PYTHONDONTWRITEBYTECODE=1

WORKDIR /app

# Update system packages to reduce vulnerabilities
RUN apt-get update && \
  apt-get upgrade -y && \
  apt-get install -y --no-install-recommends \
  gcc \
  g++ \
  && apt-get clean \
  && rm -rf /var/lib/apt/lists/*

COPY requirements.txt .
RUN pip install --no-cache-dir --upgrade pip && \
  pip install --no-cache-dir -r requirements.txt

# Copy source code and install package
COPY pyproject.toml .
COPY src/ ./src/
RUN pip install --no-cache-dir -e .

RUN echo "MODEL_DEPLOYMENT_NAME=${MODEL_DEPLOYMENT_NAME}" >> .env && \
  echo "LOG_LEVEL=${LOG_LEVEL}" >> .env

ARG LOCAL=false

# Environment variables for runtime (non-sensitive)
ENV LOCAL=${LOCAL}
ENV PROJECT_ENDPOINT=${PROJECT_ENDPOINT}
ENV MODEL_DEPLOYMENT_NAME=${MODEL_DEPLOYMENT_NAME}
ENV LOG_LEVEL=${LOG_LEVEL}
ENV AUTH0_DOMAIN=${AUTH0_DOMAIN}
ENV AUTH0_AUDIENCE=${AUTH0_AUDIENCE}
ENV COSMOSDB_ENDPOINT=${COSMOSDB_ENDPOINT}
ENV COSMOSDB_DATABASE=${COSMOSDB_DATABASE}
ENV COSMOSDB_SPEAKER_CONTAINER=${COSMOSDB_SPEAKER_CONTAINER}
ENV COSMOSDB_SESSION_CONTAINER=${COSMOSDB_SESSION_CONTAINER}
ENV AZURE_KEY_VAULT_NAME=${AZURE_KEY_VAULT_NAME}
ENV AZURE_TENANT_ID=${AZURE_TENANT_ID}
ENV AZURE_CLIENT_ID=${AZURE_CLIENT_ID}
ENV AUTH0_CLIENT_ID=${AUTH0_CLIENT_ID}

# Copy remaining files
COPY . .

# Create non-root user
RUN addgroup --gid 1001 application-group && \
  adduser --uid 1001 --gid 1001 --disabled-password --gecos "" application-user && \
  chown -R application-user:application-group /app

USER application-user

EXPOSE 8000

# Run the application with secret loading
# Secrets should be mounted at /run/secrets/ by Docker/Kubernetes
CMD if [ -f /run/secrets/AZURE_CLIENT_SECRET ]; then export AZURE_CLIENT_SECRET=$(cat /run/secrets/AZURE_CLIENT_SECRET); fi && \
  if [ -f /run/secrets/API_KEY ]; then export API_KEY=$(cat /run/secrets/API_KEY); fi && \
  if [ -f /run/secrets/COSMOSDB_KEY ]; then export COSMOSDB_KEY=$(cat /run/secrets/COSMOSDB_KEY); fi && \
  if [ -f /run/secrets/AUTH0_CLIENT_SECRET ]; then export AUTH0_CLIENT_SECRET=$(cat /run/secrets/AUTH0_CLIENT_SECRET); fi && \
  uvicorn conferenti_agent.services.api_client:app --host 0.0.0.0 --port 8000