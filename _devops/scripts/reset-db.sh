#!/usr/bin/env bash
# ============================================================================
# Livestock Trading — Reset Dev Database
# ============================================================================
# DESTRUCTIVE: Drops all 4 service volumes and recreates fresh.
# - postgres: schema + data lost
# - redis: cache lost
# - rabbitmq: queues + messages lost
# - minio: objects lost
#
# Wave 1+ will add: re-apply migrations + re-seed reference data.
#
# Usage:
#   ./reset-db.sh
# ============================================================================
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
COMPOSE_DIR="$REPO_ROOT/_devops/docker/compose"

if [ ! -f "$COMPOSE_DIR/.env.dev" ]; then
  echo "ERROR: $COMPOSE_DIR/.env.dev not found."
  echo "Run setup-dev.sh first."
  exit 1
fi

echo "=========================================="
echo "  DESTRUCTIVE: Reset all 4 service volumes"
echo "=========================================="
echo "  postgres, redis, rabbitmq, minio"
echo "  All data will be LOST."
echo ""
read -r -p "Type 'reset' to confirm: " CONFIRM

if [ "$CONFIRM" != "reset" ]; then
  echo "Cancelled."
  exit 0
fi

cd "$COMPOSE_DIR"

echo ""
echo "[1/3] Stopping services + removing volumes..."
docker compose -f docker-compose.yml -f docker-compose.dev.yml --env-file .env.dev down -v

echo ""
echo "[2/3] Starting fresh..."
docker compose -f docker-compose.yml -f docker-compose.dev.yml --env-file .env.dev up -d

echo ""
echo "[3/3] Waiting for PostgreSQL..."
MAX_WAIT=60
WAIT=0
until docker exec livestock_dev-postgres pg_isready -U postgres >/dev/null 2>&1; do
  sleep 2
  WAIT=$((WAIT + 2))
  if [ $WAIT -ge $MAX_WAIT ]; then
    echo "  ERROR: PostgreSQL did not become ready within ${MAX_WAIT}s"
    exit 1
  fi
done

echo ""
echo "  OK: Fresh DB ready (took ${WAIT}s)"
echo ""
echo "  Wave 1+ TODO:"
echo "    dotnet run --project Tools/SeedRunner -- migrate"
echo "    dotnet run --project Tools/SeedRunner -- seed --scope=all"
