#!/bin/bash

# LivestockTrading DEV Deployment Script
# Bu script sunucuda çalıştırılır

set -e

DOCKER_USERNAME="mstfaock"
TAG="dev-latest"
PROJECT_DIR="/opt/livestocktrading"
COMPOSE_PROJECT="livestock_dev"
NETWORK_NAME="livestock_dev_livestocktrading-network"

echo "=========================================="
echo "🚀 LivestockTrading DEV Deployment"
echo "=========================================="

cd $PROJECT_DIR

echo ""
echo "📥 [1/6] Git repository güncelleniyor..."
git fetch origin dev
git reset --hard origin/dev
git submodule update --init --recursive

echo ""
echo "🔑 [2/6] Docker Hub'a giriş yapılıyor..."
# Docker login (credential helper veya manual)
# docker login

echo ""
echo "🐳 [3/6] Docker image'ları pull ediliyor..."
docker compose -p $COMPOSE_PROJECT \
  -f docker-compose.yml \
  -f docker-compose.dev.yml \
  --env-file .env.dev pull

echo ""
echo "🌱 [4/6] ResourceSeeder çalıştırılıyor..."
docker run --rm \
  --network $NETWORK_NAME \
  --env-file .env.dev \
  ${DOCKER_USERNAME}/livestocktrading-resource-seeder:${TAG} \
  development || echo "⚠️ ResourceSeeder failed but continuing..."

echo ""
echo "🚀 [5/6] Servisler başlatılıyor..."
docker compose -p $COMPOSE_PROJECT \
  -f docker-compose.yml \
  -f docker-compose.dev.yml \
  --env-file .env.dev up -d

echo ""
echo "🧹 [6/6] Temizlik yapılıyor..."
docker image prune -f

echo ""
echo "=========================================="
echo "✅ DEV deployment tamamlandı!"
echo "=========================================="
echo ""
echo "📊 Container durumu:"
docker compose -p $COMPOSE_PROJECT ps
