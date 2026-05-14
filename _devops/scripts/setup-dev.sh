#!/usr/bin/env bash
# ============================================================================
# Livestock Trading — Wave 0 Dev Environment Setup
# ============================================================================
# Bootstraps a fresh dev environment in one command.
#
# Wave 0 scope: tooling check + .NET 10 + compose up + pg_isready + endpoints.
# Wave 1+ adds: EF Core migrations, SeedRunner reference data, appsettings.local.
#
# Usage:
#   ./setup-dev.sh           # Full setup
#   ./setup-dev.sh --help    # Show usage
#
# Plan ref: _docs/decisions/07-operations.md (Grup B.4.1)
# ============================================================================
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
COMPOSE_DIR="$REPO_ROOT/_devops/docker/compose"

echo "=========================================="
echo "  Livestock Trading — Dev Setup"
echo "=========================================="
echo ""

# ----------------------------------------------------------------------------
# Step 1: Tooling check
# ----------------------------------------------------------------------------
echo "[1/6] Tooling check..."

REQUIRED_TOOLS=(dotnet docker git)
MISSING=()

for cmd in "${REQUIRED_TOOLS[@]}"; do
  if ! command -v "$cmd" >/dev/null 2>&1; then
    MISSING+=("$cmd")
  fi
done

if [ ${#MISSING[@]} -gt 0 ]; then
  echo "  ERROR: Missing required tools: ${MISSING[*]}"
  echo "  Please install them and re-run."
  exit 1
fi

echo "  OK: dotnet, docker, git available"

# Check docker compose (v2 plugin or standalone)
if ! docker compose version >/dev/null 2>&1; then
  echo "  ERROR: 'docker compose' (v2) not available."
  echo "  Install Docker Desktop or docker-compose-plugin."
  exit 1
fi
echo "  OK: docker compose v2 available"

# ----------------------------------------------------------------------------
# Step 2: .NET 10 SDK version check
# ----------------------------------------------------------------------------
echo ""
echo "[2/6] .NET 10 SDK version check..."

DOTNET_VERSION=$(dotnet --version)
if [[ ! "$DOTNET_VERSION" =~ ^10\. ]]; then
  echo "  ERROR: .NET 10 SDK required, found: $DOTNET_VERSION"
  echo "  Install from: https://dotnet.microsoft.com/download/dotnet/10.0"
  exit 1
fi
echo "  OK: .NET SDK $DOTNET_VERSION"

# ----------------------------------------------------------------------------
# Step 3: Dev secrets (1Password optional)
# ----------------------------------------------------------------------------
echo ""
echo "[3/6] Dev secrets..."

ENV_FILE="$COMPOSE_DIR/.env.dev"

if [ -f "$ENV_FILE" ]; then
  echo "  OK: $ENV_FILE already exists"
elif command -v op >/dev/null 2>&1; then
  echo "  Attempting 1Password CLI pull..."
  if op signin >/dev/null 2>&1; then
    # Wave 0: 1Password item 'livestock-dev-env' not provisioned yet.
    # When ready (Wave 7+), this block fetches secrets.
    echo "  TODO: 1Password 'livestock-dev-env' item not provisioned yet (Wave 7+)."
    echo "  Falling back to manual .env.dev creation."
  fi
fi

if [ ! -f "$ENV_FILE" ]; then
  echo ""
  echo "  ACTION REQUIRED:"
  echo "    1. cp $COMPOSE_DIR/.env.dev.example $COMPOSE_DIR/.env.dev"
  echo "    2. Edit $COMPOSE_DIR/.env.dev with real passwords"
  echo "    3. Re-run this script"
  exit 1
fi

# ----------------------------------------------------------------------------
# Step 4: Docker compose up
# ----------------------------------------------------------------------------
echo ""
echo "[4/6] Starting infrastructure (postgres + redis + rabbitmq + minio)..."

cd "$COMPOSE_DIR"
docker compose -f docker-compose.yml -f docker-compose.dev.yml --env-file .env.dev up -d

echo "  OK: 4 services started"

# ----------------------------------------------------------------------------
# Step 5: PostgreSQL ready wait
# ----------------------------------------------------------------------------
echo ""
echo "[5/6] Waiting for PostgreSQL..."

MAX_WAIT=60
WAIT=0
until docker exec livestock_dev-postgres pg_isready -U postgres >/dev/null 2>&1; do
  sleep 2
  WAIT=$((WAIT + 2))
  if [ $WAIT -ge $MAX_WAIT ]; then
    echo "  ERROR: PostgreSQL did not become ready within ${MAX_WAIT}s"
    echo "  Check logs: docker compose logs postgres"
    exit 1
  fi
done

echo "  OK: PostgreSQL ready (took ${WAIT}s)"

# Wave 1+: Migrations + SeedRunner come here
# echo "[6/8] Applying migrations..."
# dotnet run --project Tools/SeedRunner -- migrate
# echo "[7/8] Seeding reference data..."
# dotnet run --project Tools/SeedRunner -- seed --scope=all

# ----------------------------------------------------------------------------
# Step 6: Endpoint print
# ----------------------------------------------------------------------------
echo ""
echo "[6/6] Done!"
echo ""
echo "=========================================="
echo "  Services running"
echo "=========================================="
echo "  PostgreSQL:     localhost:5432  (postgres / \$POSTGRES_SUPER_PASSWORD)"
echo "  Redis:          localhost:6379  (password: \$REDIS_PASSWORD)"
echo "  RabbitMQ AMQP:  localhost:5672"
echo "  RabbitMQ Mgmt:  http://localhost:15672 (\$RABBITMQ_USER / \$RABBITMQ_PASSWORD)"
echo "  MinIO S3:       localhost:9000"
echo "  MinIO Console:  http://localhost:9001 (\$MINIO_ROOT_USER / \$MINIO_ROOT_PASSWORD)"
echo "=========================================="
echo ""
echo "  Next steps (Wave 1+):"
echo "    - Apply EF Core migrations"
echo "    - Run SeedRunner for reference data"
echo "    - cd src/LivestockTrading.Api && dotnet watch"
echo ""
echo "  Logs: ./tail-logs.sh [service]"
echo "  Reset: ./reset-db.sh (fresh DB)"
echo ""
