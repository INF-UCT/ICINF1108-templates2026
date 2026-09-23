# syntax=docker/dockerfile:1

ARG NODE_VERSION=22-bookworm-slim

# -----------------------------------------------------------------------------
# build (shared): installs dependencies and compiles the app once
# -----------------------------------------------------------------------------
FROM node:${NODE_VERSION} AS build

ENV PNPM_HOME=/pnpm \
    PATH=/pnpm:$PATH

# corepack@latest avoids the outdated signing keys shipped in older Node images
RUN npm install -g corepack@latest && corepack enable

WORKDIR /app

# Build tools for native modules (better-sqlite3). They stay in this stage only
RUN apt-get update \
 && apt-get install -y --no-install-recommends python3 make g++ \
 && rm -rf /var/lib/apt/lists/*

# Manifests first so the dependency layer stays cached when source changes
COPY package.json pnpm-lock.yaml pnpm-workspace.yaml ./

RUN --mount=type=cache,id=pnpm-store,target=/pnpm/store \
    pnpm install --frozen-lockfile --store-dir=/pnpm/store

COPY . .
RUN pnpm run build

# -----------------------------------------------------------------------------
# dev: hot-reload image (source is usually bind-mounted by Compose)
# -----------------------------------------------------------------------------
FROM build AS dev

ENV NODE_ENV=development

EXPOSE 3000

CMD ["pnpm", "run", "start:dev"]

# -----------------------------------------------------------------------------
# prod: minimal runtime image, non-root, with a healthcheck
# -----------------------------------------------------------------------------
FROM node:${NODE_VERSION} AS prod

ENV PNPM_HOME=/pnpm \
    PATH=/pnpm:$PATH \
    NODE_ENV=production

RUN npm install -g corepack@latest && corepack enable

WORKDIR /app

COPY package.json pnpm-lock.yaml pnpm-workspace.yaml ./
COPY --from=build /app/node_modules ./node_modules
COPY --from=build /app/dist ./dist
COPY public ./public

# Drop devDependencies now that dist is already compiled. SQLite writes to
# data.db in the working directory, so keep it on a mountable writable path.
RUN pnpm prune --prod \
 && mkdir -p /data \
 && ln -s /data/data.db /app/data.db \
 && chown -R node:node /data

USER node

EXPOSE 3000

HEALTHCHECK --interval=30s --timeout=5s --start-period=15s --retries=3 \
    CMD node -e "fetch('http://127.0.0.1:3000/docs').then(r => process.exit(r.ok ? 0 : 1)).catch(() => process.exit(1))"

CMD ["node", "dist/main"]
