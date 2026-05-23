# Wave 3 Mid-Handover (W3.5A sonrası state — c523106'daki ilk yazımın W3.1→W3.5A güncellemesi)

**Durum:** Wave 3 ORTA NOKTA-2 — Catalog.Infrastructure W3.0+W3.8+W3.1+W3.2+W3.3+W3.4+W3.5A tamam, Grup 2 push origin'de. Kalan 3 sub-batch (W3.5B + W3.6 + W3.7) + Grup 3+4 push + `wave-3-complete` tag. **W3.5B yeni session devralacak** (Backend Soru 2 (b) öz-değerlendirme + 3-tur F-S39/S40 detour context yorgun + W3.5B agresif scope).
**Tarih:** 2026-05-23
**Sebep:** W3.5A 3-tur compile-test detour sonrası context yorgun + W3.5B agresif scope (~7 dosya, CatalogReadService 25 metot EF projection + AdminRead transition) + W3.1/W3.5A emsali detour riski %30-40 — yeni session güvenli devralma.
**Canonical state-of-truth:** bu doc + `_docs/wave-2-handover.md` (Wave 2 kapanış) + memory `wave3_plan1_decisions.md`.

## 1. Repo Durumu (W3.5A sonrası)

| | SHA |
|---|---|
| HEAD (rebuild/v2) | `aa231281c3a8963b84fdf9e8627f6299a0e7c295` (W3.5A) |
| main | `44416138b978774146f992f9e0756b829ba541e0` (**INVARIANT, dokunulmaz**) |
| origin/main | `44416138...` (= lokal main, Backend fetch cross-check) |
| origin/rebuild/v2 | `a5fa00513247f6950478fef636eed7dc5a96f398` (W3.2, Grup 2 push 24. tatbikat sonrası) |

- `main..HEAD` = **48** commit · working tree clean · **ahead 3** (W3.3+W3.4+W3.5A)
- Tag'ler (origin intact): `wave-0-complete` obj `40927c8`→`dd50923` · `wave-1-complete` obj `deaadb7`→`33d058d` · `wave-2-complete` obj `e7e7fec`→`1f2c7dd` · `wave-3-complete` **YOK** (Grup 4 sonrası)
- **Push tatbikatı: 24** (Wave 0+1+2=20 · Grup 1=22. · handover ara push=23. · Grup 2=24.). **main INVARIANT 24/24 korundu** (her push Backend bağımsız fetch cross-check).

## 2. Wave 3 Commit Zinciri (8 commit, mid-state-2)

| SHA | Subject |
|---|---|
| `00f2307` | feat(catalog): W3.0 Infrastructure foundation - DbContext + DesignTime + ConnStrBuilder |
| `304c133` | chore(catalog): W3.8 remove unused FluentAssertions dep from tests (retro 20) |
| `c523106` | docs(wave-3-mid): Wave 3 mid-handover after W3.0+W3.8+Grup 1 push |
| `1f693b3` | feat(catalog): W3.1 EF Configurations + RateLog + VO converters (12 config + 1 entity + 2 converter) |
| `a5fa005` | feat(catalog): W3.2 InitialCreate migration (12 tables + indexes + FK + jsonb + PostGIS) |
| `a4153dd` | feat(catalog): W3.3 Repositories + UnitOfWork (4 AR + ReferenceData + UoW, porta sadik impl) |
| `13691ba` | feat(catalog): W3.4 Domain Event Dispatcher Interceptor (Catalog-local, in-process MassTransit IPublishEndpoint.Publish) |
| `aa23128` | feat(catalog): W3.5A Cache Foundation (ICacheService + Memory/Redis impls + config-driven DI) |
| _(bu handover commit Adım 5'te eklenir)_ | docs(wave-3-mid): … |

Wave 2 sınır: `1f2c7dd` (wave-2-complete). Wave 3 closure-anchor ileride `wave-3-complete` (Grup 4 sonrası).

## 3. Push Tatbikatı Geçmişi (Wave 3)

- **Grup 1 (W3.0 `00f2307` + W3.8 `304c133`):** dry-run `1f2c7dd..304c133`, 23 obje / 6.71 KiB / delta 9 — **22. push** Mustafa eli.
- **Handover ara push (`c523106`):** mid-handover commit, F-S28 önleme (sub-batch grupları yedirilmez) — **23. push** Mustafa eli.
- **Grup 2 (W3.1 `1f693b3` + W3.2 `a5fa005`):** dry-run `c523106..a5fa005`, 36 obje / 19.82 KiB / delta 19 — **24. push** Mustafa eli. Backend mini-bağımsız verify: origin/main=`44416138` INVARIANT 24/24, origin/rebuild/v2=`a5fa005` 0/0 senkron.
- **Handover ara push 2 (bu doc güncellemesi sonrası):** ⏭ **25. push** Mustafa eli BEKLEYECEK (F-S28 önleme: Grup 3'e yedirilmez)
- **Grup 3 (W3.3+W3.4+W3.5A+W3.5B):** ⏭ **26. push** W3.5B sonrası Mustafa eli
- **Grup 4 (W3.6+W3.7 + `wave-3-complete` tag):** ⏭ **27. push** Wave 3 sonu
- **main INVARIANT 24/24 korundu**, her push Backend bağımsız fetch cross-check (Mustafa raporuna körlemesine güvenme).

## 4. Modül Envanteri (W3.5A sonrası, obj/bin hariç fiili)

**Catalog.Infrastructure — kümülatif (W3.0→W3.5A):**

| Sub-batch | Adds/Modifies | Insertion/Deletion | Kapsam |
|---|---|---|---|
| W3.0 | 4 A + 2 M | 134 / 0 | DbContext + DesignTimeFactory + InfraModule placeholder + ConnStrBuilder + 2 csproj +6 NuGet |
| W3.8 | 1 M | 0 / 1 | Catalog.Application.Tests.csproj FluentAssertions sil (retro 20) |
| W3.1 | 15 A | 637 / 0 | 12 EF Configurations + RateLog entity + Translations/CountryCode VO converters (non-null+nullable simetri) |
| W3.2 | 3 A | 2237 / 0 | InitialCreate migration (12 CreateTable + 8 FK + 18 CreateIndex + jsonb + geometry(Point,4326) + postgis extension) |
| W3.3 | 6 A | 203 / 0 | 4 AR repo (Category/Breed/Brand/BorderRule) + ReferenceDataRepository + UnitOfWork (24 metot porta sadık) |
| W3.4 | 1 A + 1 M | 83 / 5 | DomainEventDispatchInterceptor (SavedChangesAsync post-commit hook + IPublishEndpoint) + InfraModule M (AddScoped) |
| W3.5A | 3 A + 2 M | 126 / 3 | ICacheService Application port + Memory/RedisCacheService Infra (2-call SET+EXPIRE) + InfraModule M cache wire + csproj +2 NuGet |
| **Toplam (Wave 3 mid-state-2)** | **33 A + 6 M** | **3420 / 9** | Catalog.Infrastructure foundation+config+migration+repo+event+cache TAMAM, kalan read+rate+host-wire |

**Catalog.Application — değişmedi (Wave 2 invariant)** + 1 yeni port (ICacheService.cs 17 satır W3.5A).
**Catalog.Domain — değişmedi (Wave 1 invariant, W1-1 her sub-batch'te korundu).**
**Catalog.Application.Tests — değişmedi (W3.8'den beri 154/154 PASS preserved).**

**Namespace kuralı (kalıcı, KAYDET-9 grep-otorite):** Shared.* projeleri KISA ns (`Shared.Infrastructure`/`Shared.Domain`/`Shared.Results`/`Shared.Contracts.Catalog`/`Shared.ValueObjects`); modül projeleri UZUN `LivestockTrading.{Module}.{Layer}` (örn. `LivestockTrading.Catalog.Infrastructure.Persistence.Configurations`, `…Caching`).

## 5. NuGet Durumu (fiili csproj pin, W3.5A sonrası)

**Catalog.Infrastructure (8 PackageReference):**
- EFCore.NamingConventions `10.0.1` · Microsoft.EntityFrameworkCore `10.0.8` · .Design `10.0.8` (PrivateAssets=all) · .Relational `10.0.8` · Npgsql.EntityFrameworkCore.PostgreSQL `10.0.1` · .NetTopologySuite `10.0.1`
- **W3.5A eklemeleri:** Microsoft.Extensions.Caching.Memory `10.0.8` · StackExchange.Redis `[2.*, 3.0)`

**Catalog.Application (3 PackageReference):** MassTransit `[8.*, 9.0)` · FluentValidation `12.1.1` · FluentValidation.DependencyInjectionExtensions `12.1.1` (Wave 2 invariant).
**Shared.Infrastructure:** Microsoft.Extensions.Configuration.Abstractions `10.0.8`.

**Wave 3 kalan beklenen NuGet:** Quartz + Quartz.Extensions.Hosting + Microsoft.Extensions.Http (W3.6 RateProviders). Dapper YOK (C.5#2 EF projection).

## 6. Sapma Defteri (Wave 3 mid-state-2)

**Committed ledger (`deviations.md` `74550a7` immutable, Wave 2 sonu):** **82 distinct** (43 Wave 0+1 + 39 Wave 2), 0 production sızıntısı.

**Handover-only ledger (Wave 3 sonu deviations.md reconcile, F-S23-S40 = 18 distinct aday + 2 pozitif önleme pattern):**

| F-S | Aile | Bağlam | Kategori |
|---|---|---|---|
| S23/S24/S25 | 4 | Wave 2 sonu bayat-state/aritmetik | distinct |
| S26 | 4 | W3.0 talimat-transport defect (ardışık-2+3. tekrar) | distinct |
| S27 | 3 | Frontend push-emsal grep'siz paraphrase (Wave 1/2) | gözden-geçirme |
| S28 | önleme | handover commit'leri sub-batch push gruplarına yedirilmez (ayrı ara push) | **pattern** |
| S29 | bilgi | (CRLF→LF + tool 10.0.5<10.0.8 benign notları) | bilgi |
| S30 | 3 | Frontend KARAR 2/4 örnek yolu+VO adı Wave 1 fiili koddan farklı (W3.1 Adım 1 fresh read) | gözden-geçirme |
| **S31** | 6 | **W3.1 Adım 4 build-fail 5× CS8620 NRT variance Translations nullable variant tasarım eksikliği**; Seçenek A düzeltme | distinct |
| **S32** | 3/6 | **W3.1 Adım 5 talimat-premise `dotnet ef migrations script` tool semantiği yanlış varsayım**; Seçenek (a) düzeltme + W3.2 InitialCreate gate W3.2'de KAPANDI (CreateTable=12 birebir) | distinct |
| S33 | önleme | Backend pattern sorgusuz almama + fresh read disipline (W3.3 Adım 2 6 port grep, 3 anlamlı sapma flag) | **pattern** |
| **S34** | 3 | **W3.3 talimat KARAR 2/3/4 örnek pattern Wave 2 port imzalarıyla cross-check edilmedi** (4. ardışık Aile 3 Frontend defekt); porta sadık impl reconcile | distinct |
| **S35** | 3 | **W3.4 KARAR 4 (a) Frontend tercihi Karar 1.a + KAYDET-17 + W3.0 yorumla çelişti**; Backend (b) gerekçeli önerdi, Frontend onay | gözden-geçirme |
| **S36** | 6 | **W3.4 Adım 4 build-fail 2× CS0246 IMediator namespace; G1 compile-test G4 amendment**; Seçenek C IPublishEndpoint tek-vuruşta yeşil | distinct |
| S37 | önleme | transport-tekrarı 3+ eşik Backend aktif disambiguation sorgu (1/2/3 cevap, iki yol göster), kör retry yasak | **pattern** |
| **S38** | 3 | **W3.5A Frontend talimat KARAR 3 config key "CacheMode" Plan-1 lock `Catalog:CacheProvider` ile çelişti**; W1-4 doc-literal baskın | gözden-geçirme |
| **S39** | 6 | **W3.5A R2-A `StringSetAsync` method-signature TimeSpan? positional Wave 2.8+ Expiration mismatch**; named-arg fail | distinct |
| **S40** | 6 | **W3.5A R2-B `Expiration.For/Never` struct üye isim assumption invalid (CS0117)**; R3-A 2-call SET+EXPIRE pragmatik final yeşil | distinct |

**Wave 3 talimat-pattern defekt sayım:**
- **6 Frontend Aile 3** (S30+S31[Frontend nullable variant atlama]+S32+S34+S35+S38) — Frontend pattern↔fiili-veri cross-check eksiği yoğunlaşma
- **3 Backend Aile 6** (S36+S39+S40) — 3rd-party API (IMediator namespace + StringSetAsync method-signature + Expiration struct üye) çok-katmanlı assumption
- **Toplam 9 talimat-pattern defekt** + **3 pozitif önleme pattern** (S28 + S33 + S37) kalıcı disipline

**G3 amendment KALICI (Wave 4+):** 3rd-party API knowledge **3 katmanda ayrı doğrulama** gerek — (a) namespace · (b) method-signature · (c) class/struct üye isimleri. G1 compile-test tek-katman API yeterli; **3+ katmanlı API'lerde G2 (assembly reflection) veya G4 (alternatif pattern decomposition, örn. W3.5A 2-call SET+EXPIRE) erken tercih**, iterasyon-tasarım-hatırlatma maliyeti optimize.

## 7. Push Stratejisi C (Wave 3 KİLİDİ, dokunulmaz)

- **Grup 1: W3.0 + W3.8 ✅** (22. push)
- **Handover ara push 1: c523106 ✅** (23. push, F-S28 önleme)
- **Grup 2: W3.1 + W3.2 ✅** (24. push, "şema materyalize" milestone)
- **Handover ara push 2: bu doc commit ⏭** (25. push Mustafa eli BEKLEYECEK, F-S28 önleme)
- **Grup 3: W3.3 + W3.4 + W3.5A + W3.5B** (⏭ 26. push, "behavior+cache+read katmanı", W3.5B sonrası Mustafa eli)
- **Grup 4: W3.6 + W3.7 + `wave-3-complete` tag** (⏭ 27. push, "rate+host-wiring host-inert SON" + kritik milestone, Wave 3 sonu)

**Çapraz-kesen tespit (kalıcı):** Backend push ASLA `main`'e atmaz (yalnız `rebuild/v2`); `main` INVARIANT push-hedef disipliniyle korunur, doğrulama-frekansıyla DEĞİL → push sıklığı production-safety değil CI/lokal-kayıp/state-sync trade-off'u.

## 8. Plan-1 Kilitli Kararlar (W3.5B+ icra girdileri, yeniden açılmaz)

**C.5 (Plan-1):** #1 Integration test → Wave 4+ (WAVE-4-TEST-INFRA). #2 Read tech = **EF projection** (Dapper YOK; W3.5B uygulanacak). #3 **ICurrencyRateRefresher** Application portu izinli (W3.6). #4 Migration offline-only (W3.2 KAPANDI, gate 12 CreateTable kanıt).

**W3.0 kararları (retro):** NuGet manuel-pin yok latest stable 10.x · CatalogDbContext empty model + HasDefaultSchema + ApplyConfigurationsFromAssembly · commit scope `feat(catalog)` · NuGet 6 Catalog + 1 Shared.
**W3.8:** Tests.csproj FluentAssertions sil (retro 20 KAPANDI), test 154/154 birebir.
**W3.1 6 Açık Karar (locked):** MaxLength (Code=50/URL=500/Slug=80/ISO sabit) · JSON kolonlar jsonb · Currency.RateToUsd numeric(18,6) · Centroid SRID 4326 · Code uniqueness (Category global / Breed composite) · CountryCodeConverter ayrı sınıf.
**W3.1 mimari simetri:** VO converter non-null+nullable iki variant (CountryCode NonNull/Nullable + Translations Class+Nullable static); F-S31 retro pattern → Adım 2 onayında her VO nullable variant cross-check Frontend disiplini kalıcı.
**W3.2 F-S32 gate KAPANDI:** CreateTable=12 birebir, W3.1 model-validity tam doğrulandı.
**W3.3 W1-4 port baskın:** Wave 2 Application port imzalarına birebir sadık (24 metot 4+4+3+3+10), Brand/BorderRule GetByCode YOK (read W3.5 EF projection KAYDET-14 grounded).
**W3.4 KARAR 4 (b) onaylı:** DbContext registration W3.7 host-wire'a ertelendi (Karar 1.a "BuildMigrator only Faz 1" + KAYDET-17 use-case driven). W3.7'de `opts.AddInterceptors(sp.GetRequiredService<DomainEventDispatchInterceptor>())` + AddDbContext tek noktada. SaveChangesInterceptor Snapshot→Clear→Publish (DDD-doğru handler-induced Raise bir sonraki UoW'a) + per-event try/catch swallow (Faz 1 outbox YOK Wave 5+).
**W3.5A 4 Açık Karar (locked):** ICacheService minimum 3-metot (GetOrSetAsync HARİÇ race-condition impl-leak) · JSON System.Text.Json BCL · IConnectionMultiplexer Singleton (resmi pattern) · NuGet aligned (Memory 10.0.8 + Redis SemVer-range).
**W3.5A R3-A pattern (F-S39+S40 sonrası pragmatik):** RedisCacheService 2-call `StringSetAsync(key,json)` + `if (ttl.HasValue) await KeyExpireAsync(key, ttl.Value)`; atomicity loss cache layer'da kabul (Faz 1 best-effort, miss = factory tekrar, no data corruption).

## 9. Wave 3 Kalan Backlog (3 sub-batch + Grup 3+4 push + tag + final handover)

| Sub-batch | Kapsam (fiili dosya tahmini) | Doc dayanak | Notlar |
|---|---|---|---|
| **W3.5B** | CatalogReadService 25 metot EF projection (Translations VO→DTO + nullable + jsonb cast) · AdminCatalogReadService Infra impl (3 metot BulkImport/MissingTranslationsReport/RateLogList) · Application AdminCatalogReadService.cs NotImpl stub **DELETE** · InfraModule M (ICatalogReadService + IAdminCatalogReadService DI) · (W3.5A ICacheService cache-aside kullanım W3.5B impl içinde) = **~7 dosya** | §5:481-623 + §10:883 (preserveAdminEdits flag) + Wave 0/1 Shared.Contracts/Catalog/* 11 DTO + ICatalogReadService cross-module port | **Yoğun**, F-S31/F-S30/F-S40 emsali %30-40 detour riski; ReadModels/ klasör Catalog.Infrastructure/ReadModels/ |
| **W3.6** | RateProviders 3-tier (IRateProvider + RateFetchResult + TcmbRateProvider + EcbRateProvider + ExchangeRateHostRateProvider) · CurrencyRateUpdateJob (Quartz IJob) · `Repositories/RateLogRepository.cs` · `Application/Abstractions/ICurrencyRateRefresher.cs` (C.5#3 port) · RefreshExchangeRatesHandler impl swap · InfraModule M (Quartz + IRateProvider chain DI) = **~7-8 dosya** | §6:629-730 (RateProviders/Quartz/RateLog ns `Catalog.Infrastructure.RateProviders`), C.5#3 | **Yoğun**, Quartz job pattern + 3-tier fallback strategy + NuGet ekleme (Quartz + Quartz.Extensions.Hosting + Microsoft.Extensions.Http) |
| **W3.7** | LivestockTrading.Api Program.cs M (`AddCatalogApplication()` + `AddCatalogInfrastructure(builder.Configuration)` + `MapCategoryEndpoints` vb. 10 modül skeleton emsal + `NotImplemented501ExceptionHandler` retro 19 + DbContext registration **W3.4 KARAR 4 (b)** `AddDbContext<CatalogDbContext>((sp, opts) => UseNpgsql(DatabaseConnectionStringBuilder.BuildApp(...)) + UseSnakeCaseNamingConvention + UseNetTopologySuite + opts.AddInterceptors(sp.GetRequiredService<DomainEventDispatchInterceptor>())`) + Shared.Infrastructure `BuildApp` varyantı (Karar 1.a use-case driven) + InfraModule finalize | 01-arch:185 + retro 19 (501 IExceptionHandler) + Karar 1.a (BuildApp use-case driven W3.7) | **Orta + KRİTİK**, host-inert SON erer, Api ilk boot; DatabaseConnectionStringBuilder.BuildApp eklenir (Shared.Infrastructure M) |
| **Grup 3 push** | W3.3 + W3.4 + W3.5A + W3.5B birlikte | 26. push tatbikatı W3.5B commit sonrası Mustafa eli | |
| **Grup 4 push** | W3.6 + W3.7 birlikte + `wave-3-complete` annotated tag | 27. push tatbikatı Wave 3 sonu Mustafa eli | tag obj→W3.7 commit |
| **Wave 3 final handover** | `_docs/wave-3-handover.md` (kapanış emsali Wave 2 1f2c7dd) + deviations.md reconcile (F-S23-S40 + Wave 3 retro pattern'leri) | | Wave 4 kickoff devir teslim |

## 10. Yeni Backend Session Disiplin Kuralları

**KAYDET:** 9 (doc-literal fresh read, extrapolation YASAK — **3rd-party API çok-katmanlı için G2/G4 erken tercih, G1 tek-katman**) · 10 (NuGet doc-conditional; Shared.* 0-NuGet mutlak) · 13 (Frontend grep'siz varsayım YASAK) · 14 (W1-4 port baskın, KAYDET emsali) · 17 (W3.0 retro use-case driven, BuildApp W3.7) · 23 (plan ≠ execution; eksik talimat → DUR+flag, self-author YASAK) · 25 (grep/build/runtime çıktı otorite, "muhtemelen" YASAK; **IDE diagnostic stale, derleyici otoritedir**) · 31+alt-varyant (kod-blok template YASAK; Frontend structural+scope, Backend impl tasarımı).

**W1 dersleri:** W1-1 overwrite-guard (mevcut dosya edit'inde fresh read) · W1-2 stat reconcile · W1-4 doc↔commit'li-kod çelişkisinde commit'li kod baskın.

**Aile taksonomisi (açık küme, Wave 3 retro 9 defekt):** 1 Tool-davranışı · 2 Algı/gerçek · 3 Talimat-tahmin (Wave 3 yoğun: F-S30/S31/S32/S34/S35/S38) · 4 Disiplin-tutarsızlığı · 5 AI self-authorization (prod-Jenkins UI-only, SSH out-of-band) · 6 Plan-doc↔kod / 3rd-party API çelişkisi (Wave 3 yoğun: F-S36/S39/S40) · 7 Architectural-invariant evrim · 8 Plan-fazı tip-kimliği.

**Pozitif önleme pattern (KALICI, Wave 4+):**
- **S28:** handover commit'leri ayrı ara push (sub-batch gruplarına yedirilmez)
- **S33:** Backend pattern sorgusuz almama + fresh read disipline + flag+DUR Frontend reconcile
- **S37:** transport-tekrarı 3+ eşik Backend aktif disambiguation sorgu (1/2/3 sorulu cevap, iki yol göster, kör retry yasak) — bu session'da **3 tekrar** yaşandı (W3.3 reconcile + W3.4 fix 1+2), 4. tekrar varyantı (self-rebroadcast W3.5A öncesi) da yakalandı

**G compile-test/doğrulama amendment'ları (KALICI, Wave 4+):**
- **G1:** tek-katman API namespace compile-test (W3.4 IMediator→IPublishEndpoint emsali)
- **G3:** 3+ katman API erken G2 (assembly reflection) veya G4 (alternatif pattern decomposition) tercih — W3.5A F-S40 sonrası kalıcı; Backend tasarım önerisinde 3+ katman API tespiti → erken G2/G4 flag

**Push disiplini:** `main` DOKUNULMAZ (production safety mutlak 24/24) · Backend push YAPMAZ (Mustafa terminal, Aile 5) · model = Backend hazırlık + Mustafa exec + Backend bağımsız post-push verify (fetch cross-check, Mustafa raporuna körlemesine güvenme; Grup 2'de mini-bağımsız fetch+rev-parse yeterli oldu — F-S29 emsali, aynı session devam ediyorsa).

**F-S37 transport-tekrarı disiplini:** Aynı talimat 3+ kez geliyorsa Backend kör retry YAPMAZ → mevcut DUR durumunu kısa özetle + (1)/(2)/(3) sorulu cevap iki yol göster, Mustafa'dan explicit disambiguation iste. Aynı kural Backend raporu self-echo'da da geçerli (W3.5A öncesi 4. tekrar varyantı emsali).

## 11. W3.5B Kickoff Ön-Plan (yeni Backend session için, eski W3.1 ön-plan'ın güncellemesi)

**W3.5B = Read Service + AdminRead Transition (~7 dosya, YOĞUN W3.1 büyüklüğünde, %30-40 detour riski).**

Beklenen yaklaşım (Frontend tam talimat verecek — F-S26 emsali tek-blok; eksikse DUR):

1. **Adım 1 fresh read (KAYDET-9):**
   - Wave 0/1 `Shared.Contracts/Catalog/*.cs` 11 DTO + `ICatalogReadService.cs` cross-module port imzaları fiili (Translations alanları DTO'da nasıl — string lang-specific vs full map?)
   - Wave 2 `IAdminCatalogReadService.cs` + Application `AdminCatalogReadService.cs` NotImpl stub fiili (silinecek dosya envanteri)
   - W3.5A `ICacheService` cache-aside pattern kullanım (W3.5B `CatalogReadService` impl içinde `_cache.GetAsync` + `_cache.SetAsync` Faz 1 cache-aside)
   - **F-S30/S31/S34/S38 emsali Frontend pattern↔fiili-kod cross-check zorunlu** (Frontend KARAR'larında DTO/port imza Wave 2 grep'le doğrulanmadıysa Backend flag)

2. **Adım 2 tasarım önerisi:**
   - CatalogReadService 25 metot EF projection (Translations VO → DTO mapping: `_db.Set<Category>().Select(c => new CategoryDto { Name = c.Name.Map.ToDictionary(...) ... })` — **jsonb client-side projection vs server-side, F-S38 olasılığı**)
   - AdminCatalogReadService 3 metot (BulkImport/MissingTranslationsReport/RateLogList) Infra impl
   - Cache-aside her metotta veya seçili metot (frequency-driven)
   - Application stub DELETE (W1-1 çevresel: yeni dosya değil, kasıtlı silme Plan-1 onaylı)
   - InfraModule M (ICatalogReadService + IAdminCatalogReadService DI Scoped)
   - **3+ katman API tespiti (EF projection Translations VO)** → G3 amendment erken G4 (alternatif pattern: jsonb raw extraction vs Translations Map projection) flag olasılığı

3. **Adım 3-5 kod yazımı + build/test + commit hazırlığı** (W3.1/W3.4/W3.5A emsali, detour beklenir)

4. **Adım 6 commit + Adım 7 verify + memory checkpoint** (mini-tur)

5. **Sonra Grup 3 push** (W3.3+W3.4+W3.5A+W3.5B birlikte, 26. tatbikat Mustafa eli) → W3.6 → W3.7 → Grup 4 → wave-3-complete tag → Wave 3 final handover

**Yeni Frontend session ilk aksiyon:** (1) bu doc'u oku · (2) Backend handover özetini Mustafa'ya ilet · (3) Mustafa onay · (4) W3.5B sub-batch tam talimat (tek büyük kod-bloğu, F-S26 emsali; pattern referansları MİNİMUM F-S34/S35/S38 dersi; Wave 0/1/2 fiili kod fresh grep zorunlu KAYDET-9 Frontend için de).

**Yeni Backend session ilk aksiyon:** (1) bu doc'u oku + ham özet rapor · (2) fresh fetch + 4-SHA sanity (HEAD/main/origin-main/origin-rebuild-v2) · (3) memory `wave3_plan1_decisions.md` + `MEMORY.md` hook kontrol · (4) Frontend W3.5B tam talimat bekle, kendiliğinden W3.5B hazırlık YAPMA.

**Canonical SoT (handover sonrası):** bu doc (`_docs/wave-3-handover-mid.md`) + `_docs/wave-2-handover.md` + memory `wave3_plan1_decisions.md` + `MEMORY.md` hook + Wave 3 commit zinciri (W3.0→W3.5A 8 commit).
