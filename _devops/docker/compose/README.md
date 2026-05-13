# Docker Compose - Local Dev

Wave 0 minimum infrastructure: PostgreSQL 17 + PostGIS 3.5, Redis 7, RabbitMQ 3 (management), MinIO (S3).

Gozlem stack (Prometheus, Grafana, Loki, Tempo) Wave 5'te eklenecek. Vault Wave 7'de.

## Ilk Kurulum

```bash
cd _devops/docker/compose

# 1. Env dosyasini hazirla
cp .env.dev.example .env.dev
# Edit .env.dev - change-me-* degerlerini gercek password'lere degistir
# Production: Vault'tan alinacak (Wave 7+)

# 2. Servisleri ayaga kaldir
docker compose -f docker-compose.yml -f docker-compose.dev.yml \
  --env-file .env.dev up -d

# 3. Healthcheck (1-2 dakika)
docker compose -f docker-compose.yml -f docker-compose.dev.yml ps
# Tum servisler "healthy" olmali

# 4. PostgreSQL'in init.sh'i calistigini dogrula
docker exec livestock_dev-postgres psql -U postgres -d livestock_trading -c "\dn"
# 10 schema gorunmeli: identity, accounts, catalog, listings, carrier,
#                      marketplace, messaging, notifications, subscription, admin
```

## Port Haritasi (Dev)

| Servis | Port | Notlar |
|---|---|---|
| PostgreSQL | 5432 | `postgres` / `${POSTGRES_SUPER_PASSWORD}` |
| Redis | 6379 | Password: `${REDIS_PASSWORD}` |
| RabbitMQ AMQP | 5672 | `${RABBITMQ_USER}` / `${RABBITMQ_PASSWORD}` |
| RabbitMQ Mgmt UI | http://localhost:15672 | Ayni kullanici |
| MinIO S3 API | 9000 | `${MINIO_ROOT_USER}` / `${MINIO_ROOT_PASSWORD}` |
| MinIO Console UI | http://localhost:9001 | Ayni kullanici |

## Komutlar

```bash
# Durdur (data korunur - named volumes)
docker compose -f docker-compose.yml -f docker-compose.dev.yml down

# Durdur + data sil (FRESH START)
docker compose -f docker-compose.yml -f docker-compose.dev.yml down -v

# Log akisi
docker compose -f docker-compose.yml -f docker-compose.dev.yml logs -f postgres

# Servis restart
docker compose -f docker-compose.yml -f docker-compose.dev.yml restart redis
```

## Init.sh Davranisi

PostgreSQL container ilk kez basladiginda `_devops/db/init.sh` calisir:
1. `${MIGRATOR_PASSWORD}` ve `${APP_PASSWORD}` env'den okur
2. `init.sql.template`'i envsubst ile substitute eder -> `/tmp/init.sql`
3. `psql -f /tmp/init.sql` calistirir (10 schema + 2 role + extensions + grants)
4. `/tmp/init.sql`'i shred eder (plaintext password guvenligi)

Sonraki container start'larinda init.sh calismaz (PostgreSQL `/var/lib/postgresql/data` bos degilse `/docker-entrypoint-initdb.d/` atlanir).

**Fresh start:** `down -v` + tekrar `up -d` -> init.sh yeniden calisir.

## Bilinen Limitler

- **Vault yok** - Wave 0'da `.env.dev` dosyasi dev secret deposu. Wave 7'de Vault.
- **Backup yok** - Dev container'lar, kaybolur. Production strateji: pg_dump + WAL (Wave 7+).
- **Gozlem yok** - Prometheus/Grafana/Loki/Tempo Wave 5'te.
- **TLS yok** - Internal network plain HTTP. Production: nginx reverse proxy + Let's Encrypt.

## Sonraki Adimlar (Plan Doc'tan)

- Phase 6: `setup-dev.sh` (this compose'u otomatik calistirir)
- Phase 7: CI skeleton + 15 runbook
- Wave 1: Catalog modulu (ilk modul + EF Core migrations + SeedRunner)
