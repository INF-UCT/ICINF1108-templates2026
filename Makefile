.PHONY: install dev build format format-check clean check-tools up down logs db db-reset migrate

check-tools:
	@if ! command -v dotnet >/dev/null 2>&1; then \
		echo "El SDK de .NET no esta instalado. Instalalo aqui: https://dotnet.microsoft.com/download"; \
		exit 1; \
	fi

install: check-tools
	dotnet restore
	dotnet tool restore

dev:
	dotnet watch run

build:
	dotnet build

format:
	dotnet format

format-check:
	dotnet format --verify-no-changes

clean:
	dotnet clean

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

# Crea una migración nueva a partir de los modelos (con la API en Docker)
# Uso: make migrate name=AgregaCampoX
migrate:
	docker compose exec api dotnet ef migrations add $(name)

# Borra la base de datos (elimina el volumen pgdata)
db-reset:
	docker compose down -v
