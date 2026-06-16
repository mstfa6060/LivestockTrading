#!/bin/bash
set -e

REPO_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$REPO_ROOT"

echo "🔨 Building Livestock.Host..."
dotnet build src/Bootstrapper/Livestock.Host/Livestock.Host.csproj --nologo -v minimal

echo "📝 Generating OpenAPI spec + TypeScript client..."
# NSWAG_GENERATE=true tells Program.cs to skip live infra registrations
# (NATS connect, FusionCache Redis backplane, etc.) when NSwag boots the app.
export NSWAG_GENERATE=true
nswag run nswag.json

echo "📦 Copying api-client.ts to web + mobile..."

WEB_DIR="$REPO_ROOT/../livestock-frontend/src/api/generated"
MOBILE_DIR="$REPO_ROOT/../livestock-mobile/src/api/generated"

mkdir -p "$WEB_DIR"
mkdir -p "$MOBILE_DIR"

cp generated/api-client.ts "$WEB_DIR/api-client.ts"
echo "  ✅ → $WEB_DIR/api-client.ts"

cp generated/api-client.ts "$MOBILE_DIR/api-client.ts"
echo "  ✅ → $MOBILE_DIR/api-client.ts"

echo "🎉 Done."
