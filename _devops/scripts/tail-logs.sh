#!/usr/bin/env bash
# ============================================================================
# Livestock Trading — Tail Docker Compose Logs
# ============================================================================
# Wrapper for: docker compose logs -f [service]
#
# Usage:
#   ./tail-logs.sh              # All services
#   ./tail-logs.sh postgres     # Single service
#   ./tail-logs.sh postgres redis  # Multiple services
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

cd "$COMPOSE_DIR"
exec docker compose -f docker-compose.yml -f docker-compose.dev.yml \
  --env-file .env.dev logs -f "$@"
