# Multi-stage build for optimized production image
FROM node:22-alpine AS base

# Install dependencies only when needed
FROM base AS deps
WORKDIR /app

# Copy package files
COPY package.json package-lock.json* ./
RUN npm ci --include=optional

# Rebuild the source code only when needed
FROM base AS builder
WORKDIR /app
COPY package.json package-lock.json* ./
RUN npm ci

COPY . .

# Build arguments for environment variables
ARG NODE_ENV=development
ARG LOCAL=false
ARG CONFERENTI_BASE_URL=""
ARG APP_BASE_URL=""
ARG AUTH0_DOMAIN=""
ARG AUTH0_CLIENT_ID=""
ARG AUTH0_AUDIENCE=""
ARG AUTH0_SCOPE="openid profile email read:shows"

# Set environment variables
ENV NODE_ENV=$NODE_ENV
ENV LOCAL=$LOCAL
ENV CONFERENTI_BASE_URL=$CONFERENTI_BASE_URL
ENV APP_BASE_URL=$APP_BASE_URL
ENV AUTH0_DOMAIN=$AUTH0_DOMAIN
ENV AUTH0_CLIENT_ID=$AUTH0_CLIENT_ID
ENV AUTH0_AUDIENCE=$AUTH0_AUDIENCE
ENV AUTH0_SCOPE=$AUTH0_SCOPE

# Set environment for build
ENV NEXT_TELEMETRY_DISABLED=1

# Use secret mounts instead of build args for sensitive data
RUN --mount=type=secret,id=AUTH0_SECRET \
  --mount=type=secret,id=AUTH0_CLIENT_SECRET \
  # Read secrets securely
  AUTH0_SECRET=$(cat /run/secrets/AUTH0_SECRET) && \
  AUTH0_CLIENT_SECRET=$(cat /run/secrets/AUTH0_CLIENT_SECRET) && \
  # Export as environment variables for the build
  export AUTH0_SECRET && \
  export AUTH0_CLIENT_SECRET && \
  # Build the application with secrets available
  npm run build

# Production image, copy all the files and run next
FROM base AS runner
WORKDIR /app

ARG NODE_ENV=development
ENV NODE_ENV=$NODE_ENV
ENV NEXT_TELEMETRY_DISABLED=1

# Create a non-root user
RUN addgroup --system --gid 1001 nodejs
RUN adduser --system --uid 1001 nextjs

# Copy the public folder
COPY --from=builder /app/public ./public

# Set the correct permission for prerender cache
RUN mkdir .next
RUN chown nextjs:nodejs .next

# Copy built application
COPY --from=builder --chown=nextjs:nodejs /app/.next/standalone ./
COPY --from=builder --chown=nextjs:nodejs /app/.next/static ./.next/static

USER nextjs

EXPOSE 3000

ENV PORT=3000
ENV HOSTNAME="0.0.0.0"

CMD ["node", "server.js"]