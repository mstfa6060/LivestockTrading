#!/bin/bash
# _devops/db/init.sh — Wave 0 PostgreSQL bootstrap wrapper
# Runs once at first container start via /docker-entrypoint-initdb.d/00-init.sh
set -euo pipefail

: "${MIGRATOR_PASSWORD:?MIGRATOR_PASSWORD env var required}"
: "${APP_PASSWORD:?APP_PASSWORD env var required}"

# Substitute env vars in template → temp file
envsubst < /docker-entrypoint-initdb.d/init.sql.template > /tmp/init.sql

# Execute as postgres superuser
psql -v ON_ERROR_STOP=1 \
  --username "$POSTGRES_USER" \
  --dbname "$POSTGRES_DB" \
  -f /tmp/init.sql

# Cleanup substituted file (contains plaintext passwords)
shred -u /tmp/init.sql 2>/dev/null || rm -f /tmp/init.sql
