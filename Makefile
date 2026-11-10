.PHONY: install dev build lint format format-check clean check-tools up down logs db db-reset

check-tools:
	@if ! command -v node >/dev/null 2>&1; then \
		echo "Node.js no esta instalado. Instalalo aqui: https://nodejs.org/"; \
		exit 1; \
	fi
	@if ! command -v pnpm >/dev/null 2>&1; then \
		echo "pnpm no esta instalado. Instalalo aqui: https://pnpm.io/installation"; \
		exit 1; \
	fi

install: check-tools
	pnpm install

dev:
	pnpm run start:dev

build:
	pnpm run build

lint:
	pnpm run lint

format:
	pnpm run format

format-check:
	pnpm run format:check

clean:
	rm -rf dist coverage node_modules

# --- Docker -----------------------------------------------------------------

# Levanta la API (modo dev) y PostgreSQL
up:
	docker compose up --build

down:
	docker compose down

logs:
	docker compose logs -f

# Levanta solo PostgreSQL (para correr la API con `make dev` fuera de Docker)
db:
	docker compose up -d db

# Borra la base de datos (elimina el volumen pgdata)
db-reset:
	docker compose down -v
