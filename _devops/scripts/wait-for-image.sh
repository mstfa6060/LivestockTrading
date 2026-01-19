#!/bin/bash

IMAGE_NAME="mstfaock/hirovo-api"
TAG="latest"

echo "🕒 DockerHub'da $IMAGE_NAME:$TAG imajının oluşmasını bekliyorum..."

for i in {1..30}; do
  docker manifest inspect $IMAGE_NAME:$TAG > /dev/null 2>&1
  if [ $? -eq 0 ]; then
    echo " Imaj bulundu! Devam ediliyor..."
    exit 0
  fi
  echo "⌛ Henüz gelmedi, tekrar denenecek... ($i/30)"
  sleep 10
done

echo "⛔ 5 dakika içinde imaj hazır olmadı. Pipeline durduruluyor."
exit 1
