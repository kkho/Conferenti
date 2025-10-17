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

ARG NODE_ENV=development
ENV NODE_ENV=$NODE_ENV
# Set environment for build
ENV NEXT_TELEMETRY_DISABLED=1

# Map NODE_ENV values to correct script names
RUN if [ "$NODE_ENV" = "development" ]; then npm run build:dev; \
  elif [ "$NODE_ENV" = "production" ]; then npm run build:prod; \
  elif [ "$NODE_ENV" = "test" ]; then npm run build:test; \
  else npm run build; fi

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