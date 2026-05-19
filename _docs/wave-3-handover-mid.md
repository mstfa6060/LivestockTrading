# Wave 3 Mid-Handover (W3.0 + W3.8 + Grup 1 push sonrası)

**Durum:** Wave 3 ORTA NOKTA — Catalog.Infrastructure W3.0 (foundation) + W3.8 (dep-cleanup) tamam, Grup 1 push origin'de. Kalan 7 sub-batch (W3.1→W3.7) yeni Backend session'da.
**Tarih:** 2026-05-19
**Sebep:** Backend context dolma — handover paketi (Wave 0/1/2 emsali).
**Canonical state-of-truth:** bu doc + `_docs/wave-2-handover.md` (Wave 2 kapanış) + memory `wave3_plan1_decisions.md`.

## 1. Repo Durumu

| | SHA |
|---|---|
| HEAD (rebuild/v2) | `304c133c232b283bc0ad32616de68e77624e00a0` (W3.8) |
| main | `44416138b978774146f992f9e0756b829ba541e0` (**INVARIANT, dokunulmaz**) |
| origin/main | `44416138...` (= lokal main, Backend fetch cross-check) |
| origin/rebuild/v2 | `304c133...` (= HEAD, tam senkron) |

- `main..HEAD` = **42** commit · working tree clean · ahead/behind **0/0**
- Tag'ler (origin intact): `wave-0-complete` obj `40927c8`→`dd50923` · `wave-1-complete` obj `deaadb7`→`33d058d` · `wave-2-complete` obj `e7e7fec`→`1f2c7dd` · `wave-3-complete` **YOK** (Grup 4 sonrası)
- **Push tatbikatı: 22** (Wave 0+1+2 = 20, Wave 3 Grup 1 = 22.). **main INVARIANT 22/22 korundu** (her push Backend bağımsız fetch cross-check).

## 2. Wave 3 Commit Zinciri (mid-state)

| SHA | Subject |
|---|---|
| `00f2307` | feat(catalog): W3.0 Infrastructure foundation - DbContext + DesignTime + ConnStrBuilder |
| `304c133` | chore(catalog): W3.8 remove unused FluentAssertions dep from tests (retro 20) |
| _(bu handover commit Adım 4'te eklenir)_ | docs(wave-3-mid): … |

Wave 2 sınır: `1f2c7dd` (wave-2-complete, Wave 2 handover). Wave 3 closure-anchor ileride `wave-3-complete` (Grup 4).

## 3. Push Tatbikatı Geçmişi (Wave 3)

- **Grup 1 (W3.0 `00f2307` + W3.8 `304c133`):** dry-run `1f2c7dd..304c133`, 23 obje / 6.71 KiB / delta 9, başarılı. Mustafa eli.
- Backend post-verify: origin/main `44416138` (INVARIANT cross-check Mustafa ls-remote ile birebir), origin/rebuild/v2 `304c133`, 0/0 senkron, tag'ler intact.
- **22. push tatbikatı, main korunma 22/22.**

## 4. Modül Envanteri (Wave 3 mid-state, obj/bin hariç fiili)

**Catalog.Infrastructure — 3 .cs / 89 satır:**
- `CatalogInfrastructureModule.cs` (24) — `AddCatalogInfrastructure(this IServiceCollection, IConfiguration)` placeholder, ns `LivestockTrading.Catalog.Infrastructure`
- `Persistence/CatalogDbContext.cs` (26) — boş model, `HasDefaultSchema("catalog")` + `ApplyConfigurationsFromAssembly`, sealed, ns `…Infrastructure.Persistence`
- `DesignTime/CatalogDbContextDesignTimeFactory.cs` (39) — `IDesignTimeDbContextFactory<CatalogDbContext>`, ns `…Infrastructure.DesignTime`
- csproj: RootNamespace `LivestockTrading.Catalog.Infrastructure`, TWAE=true, +6 NuGet, host-inert (Api'ye wire DEĞİL — W3.7)

**Shared.Infrastructure — 1 .cs / 29 satır:**
- `DatabaseConnectionStringBuilder.cs` (29) — **ns `Shared.Infrastructure` (KISA, Shared.* emsal — csproj RootNamespace `LivestockTrading.Shared.Infrastructure` olsa da file-scoped ezer, Sapma 27)**. `BuildMigrator(IConfiguration, string schema)` only (BuildApp W3.7'ye ertelendi, KAYDET-17 use-case driven)
- csproj: +`Microsoft.Extensions.Configuration.Abstractions` (Npgsql EKLENMEDİ — Karar 1.a minimal, manuel connstr)

**Catalog.Application.Tests:** 4 PackageReference (Microsoft.NET.Test.Sdk/xunit/xunit.runner.visualstudio/NSubstitute; FluentAssertions W3.8'de silindi). **Test 154/154 PASS.**

**Namespace kuralı (kalıcı, KAYDET-9 grep-otorite):** Shared.* projeleri KISA ns (`Shared.Infrastructure`/`Shared.Domain`/`Shared.Results`/`Shared.Contracts.Catalog`); modül projeleri UZUN `LivestockTrading.{Module}.{Layer}`.

## 5. NuGet Durumu (fiili csproj pin)

**Catalog.Infrastructure:** Microsoft.EntityFrameworkCore `10.0.8` · .Relational `10.0.8` · .Design `10.0.8` (PrivateAssets=all) · Npgsql.EntityFrameworkCore.PostgreSQL `10.0.1` · .NetTopologySuite `10.0.1` · EFCore.NamingConventions `10.0.1`
**Shared.Infrastructure:** Microsoft.Extensions.Configuration.Abstractions `10.0.8`

**Wave 3 kalan beklenen NuGet:** Quartz + Quartz.Extensions.Hosting + Microsoft.Extensions.Http (W3.6) · StackExchange.Redis + Microsoft.Extensions.Caching.Memory (W3.5). Dapper YOK (C.5#2 EF projection). KAYDET-10 doc-conditional.

## 6. Sapma Defteri (Wave 3 mid)

**Committed ledger (`deviations.md` `74550a7` immutable, Wave 2 sonu):** **82 distinct** (43 Wave 0+1 + 39 Wave 2), 0 production sızıntısı.

**Handover-only ledger (Wave 3 kapanışında deviations.md update edilecek):**
- F-S23/F-S24/F-S25 — Wave 2 sonu, Aile 4 bayat-state/aritmetik (Sapma 83/84/85 aday)
- F-S26 — W3.0 talimat-transport defect (ardışık-2 + 3. tekrar); Aile 4 durum-farkındalığı + transport. Backend plandan extrapolation YAPMADI.
- F-S27 — Frontend push-emsal grep'siz paraphrase (Seçenek A "her push" Wave 1/2 grep'siz); Aile 3, gözden-geçirme notu (pragmatik karar, yeni Sapma değil)
- 2 bilgi notu: CRLF→LF git autocrlf normalize (Windows csproj, benign) · `dotnet-ef` tool 10.0.5 < runtime 10.0.8 (aynı major, fonksiyonel)
- **F-S28 önleme pattern:** handover commit'leri sub-batch push gruplarına yedirilmez → ayrı ara push (mini-wave emsali)

**Handover-stat (mid):** 82 committed + 5 handover-only (F-S23-S27) = ~87 aday (kesin sınıflama Wave 3 sonu Frontend reconcile, şişirme-önleme pragmatik kararı: yalnız distinct disiplin-ihlali ledger'a; gözden-geçirme notları ayrı).

## 7. Push Stratejisi C (Wave 3 KİLİDİ, dokunulmaz)

Backend operasyonel öneri (7-boyut analiz, Wave 2 #17-20 emsal-sabit):
- **Grup 1: W3.0 + W3.8 ✓** (tamamlandı, 22. push)
- **Grup 2: W3.1 + W3.2** (sırada — "şema materyalize" milestone)
- **Grup 3: W3.3 + W3.4 + W3.5** (behavior katmanı)
- **Grup 4: W3.6 + W3.7 + `wave-3-complete` tag** (host-inert SON, kritik milestone)

**Çapraz-kesen tespit (Backend pozitif inisiyatif, kalıcı):** Backend push ASLA `main`'e atmaz (yalnız `rebuild/v2`); `main` INVARIANT push-hedef disipliniyle korunur, doğrulama-frekansıyla DEĞİL → push sıklığı production-safety değil CI/lokal-kayıp/state-sync trade-off'u.

## 8. Plan-1 Kilitli Kararlar (W3.1+ icra girdileri, yeniden açılmaz)

**C.5 (Plan-1):** #1 Integration test → Wave 4+ ertelendi (WAVE-4-TEST-INFRA backlog); Wave 3 build-invariant = TWAE 0/0 + unit PASS. #2 Read tech → **EF projection** (Dapper-NuGet YOK). #3 **ICurrencyRateRefresher** Application portu izinli (W3.6, `Task<Result> RefreshAsync(...)` ipucu). #4 Migration **offline-only** (W3.2, `migrations add` connect etmez; `database update` ops/Aile 5).

**W3.0 kararları (retro):** NuGet manuel-pin yok (latest stable .NET 10) · CatalogDbContext = empty model + HasDefaultSchema + ApplyConfigurationsFromAssembly · commit scope `feat(catalog)` · NuGet 6 (Catalog) + 1 (Shared, Npgsql HARİÇ).

**W3.8 kararları (retro):** scope yalnız Tests.csproj · gate build 0/0 + test 154/154 birebir · commit scope `chore(catalog)` · FluentAssertions satırı sil, diğer PackageReference korunur.

## 9. Wave 3 Kalan Backlog (7 sub-batch + 1 tag)

| Sub-batch | Kapsam (fiili dosya) | Doc dayanak |
|---|---|---|
| **W3.1** | 12 IEntityTypeConfiguration (4 AR: Category/Breed/Brand/BorderRule + 5 Reference: Country/Currency/Language/CertificationType/Location + 2 Child: CategoryAttribute/BrandCategory + RateLog) + `Persistence/Entities/RateLog.cs` = **13 dosya** | Kural 5:363, §6:702-716, Wave 1 Domain envanteri |
| **W3.2** | InitialCreate migration: `Persistence/Migrations/{ts}_InitialCreate.cs` + `.Designer.cs` + `CatalogDbContextModelSnapshot.cs` = 3 generated. Offline (C.5#4) | §4d:614-630 |
| **W3.3** | `Persistence/Repositories/` 6: Category/Breed/Brand/BorderRule + ReferenceData + UnitOfWork | Kural 5:364, B.7.b Wave 2 portları hazır |
| **W3.4** | `Persistence/DomainEventDispatchInterceptor.cs` + InfraModule M (Catalog-local in-process MassTransit.Mediator) | Karar 1.c, Karar 3b |
| **W3.5** | `Application/Abstractions/ICacheService.cs` + Infra `Caching/Memory+RedisCacheService.cs` + `ReadModels/CatalogReadService.cs`(25 metot EF projection) + `AdminCatalogReadService.cs`(3 metot) + **DELETE** Application NotImpl stub + InfraModule M | §5:481-623, §9:872-879, Karar 4 |
| **W3.6** | `RateProviders/`: IRateProvider+RateFetchResult + Tcmb/Ecb/ExchangeRateHost + CurrencyRateUpdateJob + `Repositories/RateLogRepository.cs` + `Application/Abstractions/ICurrencyRateRefresher.cs` + RefreshExchangeRatesHandler impl + InfraModule M | §6:629-730, C.5#3 |
| **W3.7** | `LivestockTrading.Api/Program.cs` M (AddCatalogApplication+AddCatalogInfrastructure+Map*Endpoints) + InfraModule finalize (AddDbContext+Quartz) + `NotImplemented501ExceptionHandler` | 01-arch:185, retro 19. **host-inert SON erer, Api ilk boot** |
| **tag** | `wave-3-complete` annotated → W3.7 commit, Grup 4 push'a dahil | — |

## 10. Yeni Backend Session Disiplin Kuralları

**KAYDET:** 9 (doc-literal fresh read, extrapolation YASAK) · 10 (NuGet doc-conditional; Shared.* 0-NuGet mutlak) · 13 (Frontend grep'siz varsayım YASAK) · 17 (port amendment use-case driven, ayrı commit yok) · 23 (plan ≠ execution talimatı; eksik talimat → DUR+flag, self-author YASAK) · 25 (grep/build/runtime çıktı otorite, "muhtemelen" YASAK) · 31+alt-varyant (kod-blok template YASAK; Frontend structural+scope, Backend impl tasarımı).

**W1 dersleri:** W1-1 overwrite-guard (mevcut dosya editinde fiili oku) · W1-2 stat reconcile (gevşek aritmetik YASAK, sayım fiili kaynaktan) · W1-4 doc↔commit'li-kod çelişkisinde Kernel/commit baskın.

**Aile taksonomisi (açık küme):** 1 Tool-davranışı · 2 Algı/gerçek · 3 Talimat-tahmin · 4 Disiplin-tutarsızlığı (bayat-state/durum-farkındalığı/transport) · 5 AI self-authorization (prod-Jenkins UI-only, SSH out-of-band) · 6 Plan-doc↔kod çelişkisi · 7 Architectural-invariant evrim · 8 Plan-fazı tip-kimliği (kök Sapma 45).

**Push disiplini:** `main` DOKUNULMAZ (production safety mutlak 22/22) · Backend push YAPMAZ (Mustafa terminal, Aile 5) · model = Backend hazırlık + Mustafa exec + Backend bağımsız post-push verify (fetch cross-check, Mustafa raporuna körlemesine güvenme).

**F-S26 transport dersi:** Frontend talimat tek büyük kod-bloğu (Copy-buton); iç-içe blok YASAK (kesim noktası); Backend eksik talimat → DUR+flag, plandan self-author YASAK (KAYDET-23).
**F-S28 önleme:** handover commit'leri sub-batch push gruplarına yedirilmez, ayrı ara push.

## 11. W3.1 Kickoff Ön-Plan (yeni Backend için)

W3.1 = EF Configurations + RateLog entity (13 dosya). Beklenen yaklaşım (Frontend tam talimat verecek — F-S26 emsali tek-blok; eksikse DUR):
1. Wave 1 Catalog.Domain entity envanteri **fresh read** (KAYDET-9): 4 AR + 5 Reference + 2 Child fiili property/PK/factory listesi
2. Her entity `IEntityTypeConfiguration<T>` impl (`Persistence/Configurations/`)
3. RateLog entity `Persistence/Entities/RateLog.cs` (Karar 5 Infra-local; doc §6:702-713 Id/RateDate/Source/RatesJson/Success/Error/FetchedAt + enum RateProvider)
4. snake_case otomatik (W3.0 EFCore.NamingConventions) · schema "catalog" otomatik (W3.0 HasDefaultSchema)
5. Build TWAE 0/0 host-inert · test 154/154 PASS preserved (config = runtime model, davranış değişmez)
6. Commit `feat(catalog)`, Grup 2 (W3.1+W3.2) push'una hazır

**Yeni Frontend session ilk aksiyon:** (1) bu doc'u oku · (2) Backend handover özetini Mustafa'ya ilet · (3) Mustafa onay · (4) W3.1 tam talimat (tek büyük kod-bloğu, F-S26 emsali).
