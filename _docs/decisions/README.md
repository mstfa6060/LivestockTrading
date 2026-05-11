# Livestock Trading — Planning Documentation

**Status:** Karar 1-7 FINAL + 21 doküman üretildi (planlama tamamlandı, kod yazımına geçiş)
**Planning sessions:** 2026-04 → 2026-05
**Stack:** .NET 10 LTS + PostgreSQL 17 + Redis + RabbitMQ + MinIO + Next.js (frontend, ayrı repo)

---

## Karar Verme Kronolojisi

| Karar | Konu | Status |
|---|---|---|
| **Karar 1** | Solution yapısı — Seçenek B (modüler monolith, .csproj per modül, single host) | ✓ FINAL |
| **Karar 2** | 10 modül + 7 wave (revize: 7 → 10 modül; Carrier + Subscription + Admin ayrıldı) | ✓ FINAL |
| **Karar 3** | Domain pattern'ler (AR + Domain Event + Cross-modül communication + VO inventory) — 5 alt-karar | ✓ FINAL |
| **Karar 4** | Migration planı — 7 alt-karar (wave order, schema-per-modül, seed, EF, PostGIS, connection, init) | ✓ FINAL |
| **Karar 5** | Modül detayları — 10 modül × full design (AR + events + endpoints + cross-modül) | ✓ FINAL |
| **Karar 6** | API contract stratejisi — 10 madde (OpenAPI, MSW, cursor, locale, admin route, realtime, GeoJSON, TS client, error codes, migration timing) | ✓ FINAL |
| **Karar 7** | Operations / Observability — 3 grup × ~5 konu (metrics/logs/errors/traces/health + CI/CD/backup/secrets/integrations/security + perf/process/Faz2/DX) | ✓ FINAL |

---

## Toplam Metrikler (Final)

| Metric | Değer |
|---|---|
| Modül sayısı | 10 |
| Aggregate Root | 30 (+ 5 reference entity = 35 entity family) |
| Public Event (Faz 1 active) | 80 |
| API endpoint | ~417 (10 modül + admin cross-module) |
| Error code | 155 (universal 20 + Identity 16 + 8 modül + field-level 17) |
| Backlog item | 226 (Faz 2 hardening + features + DX iyileştirme) |
| Wave sayısı | 7 (Wave 0 infra + 1-7 modül grupları) |

---

## Doküman Listesi

| Dosya | İçerik |
|---|---|
| [01-architecture.md](01-architecture.md) | Karar 1 — Solution yapısı (Seçenek B), 10 modül class library + single host |
| [02-modules-list.md](02-modules-list.md) | Karar 2 — 10 modül + wave order DAG + Görev 3 communication matrix + Görev 4 AR list |
| [03-domain-patterns.md](03-domain-patterns.md) | Karar 3 — AR (30) + Domain Event (80) + Cross-modül + VO inventory (24) |
| [04-migration.md](04-migration.md) | Karar 4 — Migration sırası + schema-per-modül + seed + EF + PostGIS + connection + init workflow |
| [05-modules/](05-modules/) | Karar 5 — 10 modül detayı (her biri ayrı dosya) |
| [05-patch.md](05-patch.md) | Karar 5 sonu konsolide cross-doc patch (9 patch grubu) |
| [06-api-contract.md](06-api-contract.md) | Karar 6 — API contract (10 madde) |
| [07-operations.md](07-operations.md) | Karar 7 — Operations (3 grup × ~5 konu) |
| [backlog.md](backlog.md) | 226 backlog item — 8 kategori (kapanan + Karar 6/7 transfer + Faz 2 aktivasyon + feature + frontend + DX + anti-abuse) |
| [frontend-api-inventory.md](frontend-api-inventory.md) | Frontend → backend API kullanım envanteri (PLACEHOLDER — Wave 1 başında dolacak) |

---

## Cross-Reference Conventions

Her dokümanın başında "İlişkili Kararlar" bölümü:
- **Üst:** Bu karar hangi üst kararın uygulanması
- **Patch:** Karar 5 sonu patch dokümanında değişiklik var mı (varsa link)
- **Frontend:** İlgili frontend inventory bölümü

Linkler relative; örnek: `[Karar 4b](../04-migration.md#4b-schema-per-modül)`.

---

## Module Index

10 modül Wave order'a göre (Wave 0 infra → Wave 7 admin):

| Wave | Modül | Doc |
|---|---|---|
| 1 | Catalog (revize v2) | [05-catalog.md](05-modules/05-catalog.md) |
| 2 | Identity (v2) | [05-identity.md](05-modules/05-identity.md) |
| 3 | Accounts (v2) | [05-accounts.md](05-modules/05-accounts.md) |
| 4 | Listings | [05-listings.md](05-modules/05-listings.md) |
| 5 (paralel) | Carrier | [05-carrier.md](05-modules/05-carrier.md) |
| 5 (paralel) | Subscription | [05-subscription.md](05-modules/05-subscription.md) |
| 6 (paralel) | Marketplace | [05-marketplace.md](05-modules/05-marketplace.md) |
| 6 (paralel) | Messaging | [05-messaging.md](05-modules/05-messaging.md) |
| 6 (paralel) | Notifications | [05-notifications.md](05-modules/05-notifications.md) |
| 7 | Admin | [05-admin.md](05-modules/05-admin.md) |

---

## Sıradaki Adım — KOD

Wave 0 (init.sql + solution structure + Docker Compose dev profile + CI skeleton + setup-dev.sh) ile başlangıç.

Repo: `c:\workspace\GlobalLivestock\LivestockTrading` (mevcut, legacy kod var — yeni rewrite ile değiştirilecek; veri taşıma yok).

---

## İletişim / Decision Process

- Planlama Anthropic Plan Mode (Claude planning Claude) ile yapıldı
- Tüm kararlar Türkçe iletişim
- Conventional Commits (kod yazımında)
- Plan-first yaklaşım (kod yazmadan önce architecture document)
