# Wave 0 → Wave 1 Devir Teslim

**Durum:** Wave 0 KAPANDI + C-serisi cleanup tamam, rebuild/v2 = 4d5780c
**Tarih:** 2026-05-16 (Sat)
**Sonraki:** Wave 1 (Catalog modülü)

## Repo Durumu

| Branch | SHA | Durum |
|---|---|---|
| `main` | `44416138...` | ArfBlocks production, Wave 0 + cleanup boyunca DOKUNULMADI |
| `rebuild/v2` | `4d5780c...` | Wave 0 isi + handover docs + cleanup, FF merge ile geldi |
| `feature/wave-0-infra` | `cc8d51b...` | Wave 0 isi origin (referans olarak korunuyor) |
| `feature/wave-0-cleanup` | `4d5780c...` | C-serisi origin (referans olarak korunuyor) |
| tag `wave-0-complete` | → `dd50923` | Wave 0 isi bitisi (annotated, cleanup dışı) |

## Wave 0 Commit Zinciri (10 isi + 1 handover + 2 cleanup = 13 commit)

### Wave 0 isi (rebuild/v2'de tag wave-0-complete ile isaretli)

dd50923 fix(ci): Wave 0 CI pluginsiz - docker run pattern (B' fix)
84de827 ci(skeleton): Phase 7 - Jenkinsfile.ci + 15 runbook index
488ab26 chore(dx): Phase 6 - dev workflow scripts + editorconfig
7750d26 feat(infra): Phase 5 - docker compose
9a7f3d3 feat(solution): Phase 4 C3 - Api host + 3 Tools
a0b0c56 feat(solution): Phase 4 C2 - 10 modul x 3 katman = 30 csproj
605a1d0 feat(solution): Phase 4 C1 - Shared kernel + slnx
985730c feat(db): Phase 3 - init.sql.template + init.sh
e1497a9 chore(cleanup): Phase 2 - ArfBlocks nuke (2536 dosya)
201ef3f docs(decisions): Phase 1 - 22 plan dokumani

### Wave 0 retrospektif (rebuild/v2'de, tag disi)

cc8d51b docs(wave-0): kapanis - devir teslim paketi + sapma defteri
06a61ac chore(repo): add .gitattributes for LF normalization
4d5780c chore(repo): remove dead ArfBlocks paths from .gitignore

### main intact (Wave 0 boyunca dokunulmadi)

4441613 fix(signalr): ArfBlocks parent (production main)

## Doğrulanmış İskelet

- **37 .csproj** (3 Shared + 30 modül + 1 Api + 3 Tools), hepsi `net10.0`, `TreatWarningsAsErrors=true`
- **0 NuGet paketi**, 0 cross-module ref ihlali, 0 circular dependency
- **LivestockTrading.slnx** parse oluyor, MSBuild 37 projeyi tanıyor
- `dotnet restore` 37/37 başarılı (lokal + Jenkins CI'da doğrulandı)
- `dotnet build -c Release` Senaryo Y: 33 class library success, 3 executable CS5001 (Api + SeedRunner + AdminBootstrap), OpenApiGen dependency-skip
- `.gitignore` `bin/obj` pattern'i aktif, sızıntı yok

## CI Altyapısı

- **Jenkins:** `https://jenkins.hirovo.com`, paylaşımlı server `45.143.4.64`
- **Job:** `LivestockTrading Backend Rebuild CI`
- **Pattern:** flow-definition + Pipeline script from SCM (Jenkinsfile.ci)
- **Pipeline:** agent any + sh `docker run mcr.microsoft.com/dotnet/sdk:10.0`
- **NuGet cache:** named volume `livestock-nuget-cache`
- **Branch:** `*/feature/wave-0-infra` (Wave 1 başında `*/rebuild/v2`'ye geçirilecek)
- **Webhook:** GitHub push trigger config'de var ama gerçekte tetiklemiyor (backlog WAVE-1-CI-01)

## Açık Backlog

| ID | Açıklama | Öncelik |
|---|---|---|
| `WAVE-1-CI-01` | GitHub→Jenkins webhook neden bu job'u tetiklemiyor? Tanı + düzeltme. Memory exception genişletmesi gerekebilir | Wave 1 başında |
| `WAVE-0-CLEANUP-02` | ✅ KAPANDI - `.gitattributes` LF normalize (commit 06a61ac) | - |
| `WAVE-0-CLEANUP-03` | ✅ KAPANDI - `.gitignore` 12 dead ref + yetim yorum (commit 4d5780c) | - |
| `WAVE-1-ARCHIVE-01` | `claude/*` branch arsivleme: 21 lokal + 11 origin, 19 orphan, gerçek feature işi. Inceleme + archive/migration karari. Ayrı mini-wave | Düşük, Wave 1 sonu veya bağımsız |
| `SENARYO-Y` | Api + SeedRunner + AdminBootstrap (+ OpenApiGen) `Program.cs` eksik | **Wave 1 ilk iş** |

## Wave 1 Başlangıç İçin Hazır

- Catalog modülü plan dokümanı: `_docs/decisions/05-catalog.md`
- Domain patterns: `_docs/decisions/03-domain-patterns.md`
- Migration: `_docs/decisions/04-migration.md` (eski sistemden Catalog data taşıma)
- API contract: `_docs/decisions/06-api-contract.md`

## Üç-Taraflı Çalışma Modeli (Wave 1+ için kalıcı)

- **Frontend Claude:** Plan, talimat üretimi, review. Kod yazmaz, komut çalıştırmaz.
- **Backend Claude:** Komutları çalıştırır, raporlar, proaktif sapma yakalar
- **Kullanıcı (Mustafa):** Stratejik karar verici, mesaj taşıyıcı, prod write işleri (Jenkins UI gibi)

### Frontend Claude Disiplin Kuralları (Wave 0'dan öğrenildi)

1. **Plan-first:** Kod/komut açık plan onayı olmadan üretilmez
2. **Adım adım:** Her mesaj = 1 mantıksal adım + DUR + onay
3. **Sapma yakalama (3 seviye):** Beklenen sonuç / Gerçek sonuç / Fark = tool davranışı mı, talimat hatası mı, içerik hatası mı?
4. **Koşullu talimat yok:** Bir önceki adımın çıktısı review edilmeden sonraki adımın talimatı üretilmez (Sapma 19 dersi)
5. **Prompt'a değil fiili veriye güven:** Hash/SHA/sayım vb. değerler her zaman fiili kaynaktan teyit edilir (Sapma 17, 20, 22 dersleri)
6. **Kullanıcıya soru sormak yerine Backend'e tanı sor:** Kararsızlıkta Backend'e teknik durum sorgulama talimatı ver, sonra direkt karar ver
7. **Talimat başlığı kontrol:** Her talimat bloğu `## Adım X` başlığı ile başlar, kullanıcı zincirde takip için kullanır

### Memory Kayıtları (Backend Claude tarafı)

- `feedback_prod_jenkins_ui_only.md` — paylaşımlı production Jenkins'e otomatik write/sensitive-read yapma, exception: kendi job'umuz için read+log+Jenkinsfile edit
- `feedback_direct_push_main.md` — rebuild/v2 ← feature/wave-N-* FF merge pattern, main asla dokunulmaz
- `reference_devops_containers`, `reference_server_ssh` — SSH erişimi referansları
