# syntax=docker/dockerfile:1
FROM alpine:latest AS base
LABEL maintainer="conferenti@conferenti.io"

RUN apk update && \
  apk upgrade && \
  apk cache clean

RUN addgroup application-group --gid 1001 \
  && adduser application-user --uid 1001 \
  --ingroup application-group \
  --disabled-password

FROM golang:1.25-alpine AS build
LABEL maintainer="conferenti@conferenti.io"

# Build arguments for non-secret environment variables
ARG LOCAL=false
ARG AUTH0_DOMAIN
ARG AUTH0_AUDIENCE
ARG AUTH0_SCOPE
ARG COSMOSDB_ENDPOINT
ARG COSMOSDB_DATABASE
ARG COSMOSDB_SPEAKER_CONTAINER
ARG COSMOSDB_SESSION_CONTAINER

ENV GO111MODULE=on CGO_ENABLED=0 GOOS=linux GOARCH=amd64

RUN apk add --no-cache make

WORKDIR /app
COPY . .

# Build with secrets mounted (not stored in image layers)
RUN --mount=type=secret,id=auth0_secret \
  --mount=type=secret,id=auth0_client_id \
  --mount=type=secret,id=auth0_client_secret \
  --mount=type=secret,id=auth0_audience \
  export AUTH0_SECRET=$(cat /run/secrets/auth0_secret) && \
  export AUTH0_CLIENT_ID=$(cat /run/secrets/auth0_client_id) && \
  export AUTH0_CLIENT_SECRET=$(cat /run/secrets/auth0_client_secret) && \
  export AUTH0_AUDIENCE=$(cat /run/secrets/auth0_audience) && \
  make all

FROM base AS runtime
LABEL maintainer="conferenti@conferenti.io"

# Set environment variables from build args (non-secrets only)
ARG LOCAL=false
ARG AUTH0_DOMAIN
ARG AUTH0_AUDIENCE
ARG AUTH0_SCOPE
ARG COSMOSDB_ENDPOINT
ARG COSMOSDB_DATABASE
ARG COSMOSDB_SPEAKER_CONTAINER
ARG COSMOSDB_SESSION_CONTAINER

ENV LOCAL=${LOCAL}
ENV AUTH0_DOMAIN=${AUTH0_DOMAIN}
ENV AUTH0_AUDIENCE=${AUTH0_AUDIENCE}
ENV AUTH0_SCOPE=${AUTH0_SCOPE}
ENV COSMOSDB_ENDPOINT=${COSMOSDB_ENDPOINT}
ENV COSMOSDB_DATABASE=${COSMOSDB_DATABASE}
ENV COSMOSDB_SPEAKER_CONTAINER=${COSMOSDB_SPEAKER_CONTAINER}
ENV COSMOSDB_SESSION_CONTAINER=${COSMOSDB_SESSION_CONTAINER}

# Secrets will be provided at runtime via secret files or env vars
ENV AUTH0_SECRET_FILE="/run/secrets/auth0_secret"
ENV AUTH0_CLIENT_ID_FILE="/run/secrets/auth0_client_id"
ENV AUTH0_CLIENT_SECRET_FILE="/run/secrets/auth0_client_secret"
ENV AUTH0_AUDIENCE_FILE="/run/secrets/auth0_audience"

RUN apk add --no-cache ca-certificates apache2-utils

WORKDIR /app
COPY --from=build /app/bin .

# Create secrets directory for runtime secrets
RUN mkdir -p /run/secrets && chown application-user:application-group /run/secrets

RUN chown --recursive application-user .
USER application-user
EXPOSE 8080
ENTRYPOINT ["/app/main"]