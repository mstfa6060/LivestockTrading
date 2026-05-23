# Wave 3 Mid-Handover (W3.5B sonrası state — mid-handover-2 `f2972e8` üstüne W3.5B 4 sub-batch + Grup 3 push + W3.6 ön-plan güncellemesi)

**Durum:** Wave 3 ORTA NOKTA-3 — Catalog.Infrastructure W3.0+W3.8+W3.1+W3.2+W3.3+W3.4+W3.5A+**W3.5B (4 alt-batch atomik)** tamam, Grup 3 push origin'de. Kalan 2 sub-batch (W3.6 + W3.7) + Grup 4 push + `wave-3-complete` tag. **W3.6 yeni session devralacak** (4 tematik blok yoğun: Cache decorator + Rate providers 3-tier + Quartz scheduler + Domain event handlers).
**Tarih:** 2026-05-23
**Sebep:** W3.5B 4 sub-batch ardışık icra (3 atomik impl + 1 DELETE) context yorgun + W3.6 4-tematik-blok geniş kapsam (HttpClient retry/circuit + Quartz NuGet + cache decorator pattern + domain event subscriber'lar). Yeni session güvenli devralma, mid-handover-3 canonical SoT.
**Canonical state-of-truth:** bu doc + `_docs/wave-2-handover.md` (Wave 2 kapanış) + memory `wave3_plan1_decisions.md`.

## 1. Repo Durumu (W3.5B sonrası)

| | SHA |
|---|---|
| HEAD (rebuild/v2) | `9d7000c583aa2dc50f94d5d6a4999c146572cc45` (W3.5B.4 DI register) |
| main | `44416138b978774146f992f9e0756b829ba541e0` (**INVARIANT, dokunulmaz**) |
| origin/main | `44416138...` (= lokal main, Backend fetch cross-check) |
| origin/rebuild/v2 | `9d7000c...` (Grup 3 push 26. tatbikat sonrası, lokal = origin 0/0) |

- `main..HEAD` = **53** commit · working tree clean · **ahead 0** (Grup 3 push tamam; bu handover commit Adım 5'te +1 ekler)
- Tag'ler (origin intact): `wave-0-complete` obj `40927c8`→`dd50923` · `wave-1-complete` obj `deaadb7`→`33d058d` · `wave-2-complete` obj `e7e7fec`→`1f2c7dd` · `wave-3-complete` **YOK** (Grup 4 sonrası)
- **Push tatbikatı: 30** (Wave 0+1+2=20 · Grup 1=22. · ara push 1=23. · Grup 2=24. · ara push 2=25. · **Grup 3=26.**). **main INVARIANT 30/30 korundu** (her push Backend bağımsız fetch + ls-remote cross-check, Mustafa raporuna körlemesine güvenme — F-S25→F-S50 emsali kalıcı disiplin).

## 2. Wave 3 Commit Zinciri (13 commit, mid-state-3)

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
| `f2972e8` | docs(wave-3-mid): W3.5A sonrasi state guncelleme (W3.1-W3.5A + sapma F-S29-S40 + G3 amendment) |
| `ec84f70` | refactor(catalog) W3.5B.1 application admin read stub + test sil |
| `09917e4` | feat(catalog) W3.5B.2 catalog read service infra impl 22 metot |
| `a6b4a3b` | feat(catalog) W3.5B.3 admin catalog read service infra impl 3 metot |
| `9d7000c` | feat(catalog) W3.5B.4 catalog read service di register |
| _(bu handover commit Adım 5'te eklenir)_ | docs(wave-3-mid): W3.5B kapanis state guncelleme |

Wave 2 sınır: `1f2c7dd` (wave-2-complete). Wave 3 closure-anchor ileride `wave-3-complete` (Grup 4 sonrası).

## 3. Push Tatbikatı Geçmişi (Wave 3)

- **Grup 1 (W3.0 `00f2307` + W3.8 `304c133`):** dry-run `1f2c7dd..304c133`, 23 obje / 6.71 KiB / delta 9 — **22. push** Mustafa eli.
- **Handover ara push 1 (`c523106`):** mid-handover-1 commit, F-S28 önleme (sub-batch grupları yedirilmez) — **23. push** Mustafa eli.
- **Grup 2 (W3.1 `1f693b3` + W3.2 `a5fa005`):** dry-run `c523106..a5fa005`, 36 obje / 19.82 KiB / delta 19 — **24. push** Mustafa eli. Backend mini-bağımsız verify: origin/main=`44416138` INVARIANT 24/24, origin/rebuild/v2=`a5fa005` 0/0 senkron.
- **Handover ara push 2 (`f2972e8`):** mid-handover-2 commit, F-S28 önleme — **25. push** Mustafa eli. Bağımsız fetch + ls-remote cross-check.
- **Grup 3 (W3.3 + W3.4 + W3.5A + W3.5B.1 + W3.5B.2 + W3.5B.3 + W3.5B.4):** dry-run `f2972e8..9d7000c`, 4 commit (W3.5B chain; W3.3/W3.4/W3.5A 25. push'a kadar lokal'di, dry-run yalnız W3.5B) — **26. push** Mustafa eli. **F-S50 yakalama:** Mustafa "push tamamlandı" ilk raporda fiili push yapılmamıştı (origin/rebuild/v2 hâlâ `f2972e8`); Backend `git fetch + ls-remote` bağımsız doğrulama ile tespit etti, Mustafa düzeltici push sonrası ikinci fetch ile teyit. F-S25 emsali kalıcı disiplin: Mustafa raporuna körlemesine güven YASAK.
- **Handover ara push 3 (bu doc commit ⏭):** **27. push** Mustafa eli BEKLEYECEK (F-S28 önleme: Grup 4'e yedirilmez).
- **Grup 4 (W3.6 + W3.7 + `wave-3-complete` tag):** ⏭ **28. push** Wave 3 sonu Mustafa eli.
- **main INVARIANT 30/30 korundu**, her push Backend bağımsız fetch+ls-remote cross-check (Mustafa raporuna körlemesine güvenme, F-S50 kalıcı ders).

## 4. Modül Envanteri (W3.5B sonrası, obj/bin hariç fiili)

**Catalog.Infrastructure — kümülatif (W3.0→W3.5B):**

| Sub-batch | Adds/Modifies/Deletes | Insertion/Deletion | Kapsam |
|---|---|---|---|
| W3.0 | 4 A + 2 M | 134 / 0 | DbContext + DesignTimeFactory + InfraModule placeholder + ConnStrBuilder + 2 csproj +6 NuGet |
| W3.8 | 1 M | 0 / 1 | Catalog.Application.Tests.csproj FluentAssertions sil (retro 20) |
| W3.1 | 15 A | 637 / 0 | 12 EF Configurations + RateLog entity + Translations/CountryCode VO converters (non-null+nullable simetri) |
| W3.2 | 3 A | 2237 / 0 | InitialCreate migration (12 CreateTable + 8 FK + 18 CreateIndex + jsonb + geometry(Point,4326) + postgis extension) |
| W3.3 | 6 A | 203 / 0 | 4 AR repo (Category/Breed/Brand/BorderRule) + ReferenceDataRepository + UnitOfWork (24 metot porta sadık) |
| W3.4 | 1 A + 1 M | 83 / 5 | DomainEventDispatchInterceptor (SavedChangesAsync post-commit hook + IPublishEndpoint) + InfraModule M (AddScoped) |
| W3.5A | 3 A + 2 M | 126 / 3 | ICacheService Application port + Memory/RedisCacheService Infra (2-call SET+EXPIRE) + InfraModule M cache wire + csproj +2 NuGet |
| **W3.5B.1** | **2 D** | **0 / 64** | **Application AdminCatalogReadService stub + test atomik DELETE (F-S42 yakalama, test baseline 154→151)** |
| **W3.5B.2** | **1 A** | **404 / 0** | **CatalogReadService 22 metot EF projection (%91 (A) Select-in-query patron + 1 (B) tree + 1 (A.2) brand two-step + 2 fail-fallback (A) tuttu)** |
| **W3.5B.3** | **1 A** | **139 / 0** | **AdminCatalogReadService 3 metot (ListBrands cursor + MissingTranslations 4 bucket (B) + GetRateLogs)** |
| **W3.5B.4** | **1 M** | **12 / 2** | **CatalogInfrastructureModule M (AddScoped × 2 ICatalogReadService + IAdminCatalogReadService + 2 using güncelleme)** |
| **Toplam (Wave 3 mid-state-3)** | **34 A + 7 M + 2 D** | **3975 / 75** | Catalog.Infrastructure foundation+config+migration+repo+event+cache+**read+adminread+DI** TAMAM, kalan rate+host-wire |

**Catalog.Application — Wave 2 invariant + 1 yeni port** (`ICacheService.cs` 17 satır W3.5A); **stub silme W3.5B.1** (`Features/AdminCatalogRead/AdminCatalogReadService.cs` 33 satır — gerçek impl Infrastructure'a taşındı). Endpoint dosyaları (`AdminCatalogReadEndpoints.cs` + 3 individual) `IAdminCatalogReadService` interface DI tüketici, invariant.
**Catalog.Domain — değişmedi (Wave 1 invariant, W1-1 her sub-batch'te korundu).**
**Catalog.Application.Tests — 154 → 151 PASS** (W3.5B.1 ölü stub-davranış 3 test silindi, Karar 6 gate "preserved minus 3 ölü stub test = 151 yeni baseline W3.5B.1 sonrası" yorumu).

**Namespace kuralı (kalıcı, KAYDET-9 grep-otorite):** Shared.* projeleri KISA ns (`Shared.Infrastructure`/`Shared.Domain`/`Shared.Results`/`Shared.Contracts.Catalog`/`Shared.ValueObjects`/`Shared.Pagination`); modül projeleri UZUN `LivestockTrading.{Module}.{Layer}` (örn. `LivestockTrading.Catalog.Infrastructure.Persistence`, `…Caching`, `…Configurations`).

## 5. NuGet Durumu (fiili csproj pin, W3.5B sonrası)

**Catalog.Infrastructure (8 PackageReference, W3.5B değişmedi):**
- EFCore.NamingConventions `10.0.1` · Microsoft.EntityFrameworkCore `10.0.8` · .Design `10.0.8` (PrivateAssets=all) · .Relational `10.0.8` · Npgsql.EntityFrameworkCore.PostgreSQL `10.0.1` · .NetTopologySuite `10.0.1`
- **W3.5A eklemeleri (invariant):** Microsoft.Extensions.Caching.Memory `10.0.8` · StackExchange.Redis `[2.*, 3.0)`

**Catalog.Application (3 PackageReference):** MassTransit `[8.*, 9.0)` · FluentValidation `12.1.1` · FluentValidation.DependencyInjectionExtensions `12.1.1` (Wave 2 invariant).
**Shared.Infrastructure:** Microsoft.Extensions.Configuration.Abstractions `10.0.8`.

**Wave 3 kalan beklenen NuGet:** Quartz + Quartz.Extensions.Hosting + Microsoft.Extensions.Http (W3.6 RateProviders + Quartz scheduler) · opsiyonel Scrutor (W3.6.A cache decorator için, manuel factory alternatifi). Dapper YOK (C.5#2 EF projection).

## 6. Sapma Defteri (Wave 3 mid-state-3)

**Committed ledger (`deviations.md` `74550a7` immutable, Wave 2 sonu):** **82 distinct** (43 Wave 0+1 + 39 Wave 2), 0 production sızıntısı.

**Handover-only ledger (Wave 3 sonu deviations.md reconcile, F-S23-F-S50 = 28 distinct aday + 5+ pozitif önleme/ders pattern):**

| F-S | Aile | Bağlam | Kategori |
|---|---|---|---|
| S23/S24/S25 | 4 | Wave 2 sonu bayat-state/aritmetik | distinct |
| S26 | 4 | W3.0 talimat-transport defect (ardışık-2+3. tekrar) | distinct |
| S27 | 3 | Frontend push-emsal grep'siz paraphrase (Wave 1/2) | gözden-geçirme |
| S28 | önleme | handover commit'leri sub-batch push gruplarına yedirilmez (ayrı ara push) | **pattern** |
| S29 | bilgi | (CRLF→LF + tool 10.0.5<10.0.8 benign notları) | bilgi |
| S30 | 3 | Frontend KARAR 2/4 örnek yolu+VO adı Wave 1 fiili koddan farklı | gözden-geçirme |
| **S31** | 6 | **W3.1 Adım 4 build-fail 5× CS8620 NRT variance Translations nullable variant tasarım eksikliği** | distinct |
| **S32** | 3/6 | **W3.1 Adım 5 talimat-premise `dotnet ef migrations script` tool semantiği yanlış varsayım** | distinct |
| S33 | önleme | Backend pattern sorgusuz almama + fresh read disipline (W3.3 Adım 2 6 port grep, 3 anlamlı sapma flag) | **pattern** |
| **S34** | 3 | **W3.3 talimat KARAR 2/3/4 örnek pattern Wave 2 port imzalarıyla cross-check edilmedi** | distinct |
| **S35** | 3 | **W3.4 KARAR 4 (a) Frontend tercihi Karar 1.a + KAYDET-17 + W3.0 yorumla çelişti** | gözden-geçirme |
| **S36** | 6 | **W3.4 Adım 4 build-fail 2× CS0246 IMediator namespace; G1 compile-test G4 amendment** | distinct |
| S37 | önleme | transport-tekrarı 3+ eşik Backend aktif disambiguation sorgu, kör retry yasak | **pattern** |
| **S38** | 3 | **W3.5A Frontend talimat KARAR 3 config key "CacheMode" Plan-1 lock ile çelişti** | gözden-geçirme |
| **S39** | 6 | **W3.5A R2-A `StringSetAsync` method-signature TimeSpan? positional Wave 2.8+ Expiration mismatch** | distinct |
| **S40** | 6 | **W3.5A R2-B `Expiration.For/Never` struct üye isim assumption invalid (CS0117)**; R3-A 2-call SET+EXPIRE pragmatik | distinct |
| **F-S41** | 3 | **W3.5B.1 talimat ICatalogReadService "25 metot" ezber; fiili port 22 metot** (W1-4 port baskın); Frontend pattern↔fiili-port grep eksikliği | gözden-geçirme |
| **F-S42** | 3+1 | **W3.5B.1 stub DELETE talimat-eksik test envanteri** (AdminCatalogReadServiceTests 3 NotImpl assert); Backend KRİTİK DUR ile build-fail önlendi (Seçenek A atomik prod+test sil onaylı, 154→151) | distinct |
| **F-S43** | 3 | **W3.5B talimat path `src/Shared/Shared.Contracts/` yanlış; fiili `src/Shared/LivestockTrading.Shared.Contracts/`** (modül-bazlı klasör adlandırma ezber); Backend fresh path baskın | gözden-geçirme |
| **F-S44** | 3 | **W3.5B.3 talimat MissingTranslationsReport 4. bucket "Locations? CertificationTypes?" sorulu cevap**; fiili port `Certifications` (Locations DAHİL DEĞİL doc-literal); Frontend port grep'siz öneri | gözden-geçirme |
| **F-S45** | 6 | **W3.5B.2 enum dublication 4 cast fix iterasyonu**: LocationLevel + BrandStatus×2 + AttributeValueType (Domain dosya-içi enum + Shared.Contracts.Catalog.Enums dublication, S12 iç-numaralı); R-A patron tek-shot fix yeşil | distinct |
| **F-S47** | 6 | **W3.5B.3 Adım 1 port doğrulama** 4. bucket Certifications (Locations YOK); Backend port-fiili grep proaktif, F-S44 dilek-Frontend talimat ezberini reddetti (S13 iç-numaralı) | gözden-geçirme |
| **F-S49** | 6 | **W3.5B.3 jsonb ContainsKey LINQ emsal yok** (`grep` Catalog.Infrastructure 0 sonuç); GetMissingTranslationsAsync (B) ToList+in-memory zorunlu (S15 iç-numaralı); EF Core 10 + Npgsql 10.0.1 belgesiz | distinct |
| **F-S50** | 5+1 | **W3.5B Grup 3 push tamamlandı raporu fiili push yapılmadan geldi**; Backend `git fetch + ls-remote` bağımsız doğrulama tespit, Mustafa düzeltici push sonrası teyit; F-S25 emsali Mustafa raporuna körlemesine güven YASAK kalıcı disipline | distinct |

**Atlanan numaralar (sapma değil, ders/pozitif kayıt):**
- **F-S46** (S11 muafiyeti, interface impl sub-build CS0535 yapısal kaçınılmaz tek-shot doğal — talimat-pattern güncelleme, defekt değil)
- **F-S48** (S14 proaktif önleme, BrandStatus enum dublication W3.5B.2 dersinden W3.5B.3 Adım 1'de proaktif yakalama — S12 emsali tekrar olmadı, pozitif Aile 6 önleme)

**Wave 3 talimat-pattern defekt sayım (mid-state-3):**
- **10 Frontend Aile 3** (S30+S31[Frontend nullable variant atlama]+S32+S34+S35+S38+F-S41+F-S43+F-S44) + **F-S42 (3+1 transport)** = 10 distinct Frontend defekt
- **6 Backend Aile 6** (S36+S39+S40+F-S45+F-S47+F-S49) — 3rd-party API + plan-doc↔fiili-kod
- **F-S50 (5+1, Mustafa transport sapması, Aile 5+1)** — ayrı kategoride
- **Toplam 16 talimat-pattern defekt** + **5+ pozitif önleme/ders pattern** kalıcı disipline (S28+S33+S37+S14 emsali+F-S25→F-S50 emsali + Translations.FirstOrEmpty() VO native keşif + helper vs inline EF translate kararı + S11 muafiyeti)

**G3 amendment KALICI (Wave 4+):** 3rd-party API knowledge **3 katmanda ayrı doğrulama** gerek — (a) namespace · (b) method-signature · (c) class/struct üye isimleri. G1 compile-test tek-katman API yeterli; **3+ katmanlı API'lerde G2 (assembly reflection) veya G4 (alternatif pattern decomposition, örn. W3.5A 2-call SET+EXPIRE) erken tercih**. W3.5B.2 enum dublication F-S45 emsali 4 cast fix R-A patron uygulandı; W3.5B.3 S14 emsali Adım 1 proaktif check ile S12 tekrarı önlendi (Aile 6 önleme pattern kanıtı).

## 7. Push Stratejisi C (Wave 3 KİLİDİ, dokunulmaz)

- **Grup 1: W3.0 + W3.8 ✅** (22. push)
- **Handover ara push 1: `c523106` ✅** (23. push, F-S28 önleme)
- **Grup 2: W3.1 + W3.2 ✅** (24. push, "şema materyalize" milestone)
- **Handover ara push 2: `f2972e8` ✅** (25. push, F-S28 önleme)
- **Grup 3: W3.3 + W3.4 + W3.5A + W3.5B.1 + W3.5B.2 + W3.5B.3 + W3.5B.4 ✅** (26. push, "behavior+cache+read katmanı", F-S50 yakalama Backend bağımsız doğrulama emsali)
- **Handover ara push 3: bu doc commit ⏭** (27. push Mustafa eli BEKLEYECEK, F-S28 önleme: Grup 4'e yedirilmez)
- **Grup 4: W3.6 + W3.7 + `wave-3-complete` tag** (⏭ 28. push, "rate+host-wiring host-inert SON" + kritik milestone, Wave 3 sonu)

**Çapraz-kesen tespit (kalıcı):** Backend push ASLA `main`'e atmaz (yalnız `rebuild/v2`); `main` INVARIANT push-hedef disipliniyle korunur, doğrulama-frekansıyla DEĞİL → push sıklığı production-safety değil CI/lokal-kayıp/state-sync trade-off'u. F-S50 emsali (Mustafa transport sapması) sıfır production riski (main INVARIANT zaten korundu), yalnız tatbikat sayım/canonical SoT lag — Backend bağımsız fetch doğrulama disipline süresi az pozitif önleme.

## 8. Plan-1 Kilitli Kararlar (W3.6+ icra girdileri, yeniden açılmaz)

**C.5 (Plan-1):** #1 Integration test → Wave 4+ (WAVE-4-TEST-INFRA). #2 Read tech = **EF projection** (Dapper YOK; W3.5B uygulandı, %91 (A) patron oranı). #3 **ICurrencyRateRefresher** Application portu izinli (W3.6). #4 Migration offline-only (W3.2 KAPANDI, gate 12 CreateTable kanıt).

**W3.0 kararları (retro):** NuGet manuel-pin yok latest stable 10.x · CatalogDbContext empty model + HasDefaultSchema + ApplyConfigurationsFromAssembly · commit scope `feat(catalog)` · NuGet 6 Catalog + 1 Shared.
**W3.8:** Tests.csproj FluentAssertions sil (retro 20 KAPANDI), test 154 birebir.
**W3.1 6 Açık Karar (locked):** MaxLength (Code=50/URL=500/Slug=80/ISO sabit) · JSON kolonlar jsonb · Currency.RateToUsd numeric(18,6) · Centroid SRID 4326 · Code uniqueness (Category global / Breed composite) · CountryCodeConverter ayrı sınıf.
**W3.1 mimari simetri:** VO converter non-null+nullable iki variant (CountryCode NonNull/Nullable + Translations Class+Nullable static); F-S31 retro pattern → Adım 2 onayında her VO nullable variant cross-check Frontend disiplini kalıcı.
**W3.2 F-S32 gate KAPANDI:** CreateTable=12 birebir, W3.1 model-validity tam doğrulandı.
**W3.3 W1-4 port baskın:** Wave 2 Application port imzalarına birebir sadık (24 metot 4+4+3+3+10), Brand/BorderRule GetByCode YOK (read W3.5 EF projection KAYDET-14 grounded).
**W3.4 KARAR 4 (b) onaylı:** DbContext registration W3.7 host-wire'a ertelendi. W3.7'de `opts.AddInterceptors(sp.GetRequiredService<DomainEventDispatchInterceptor>())` + AddDbContext tek noktada. SaveChangesInterceptor Snapshot→Clear→Publish + per-event try/catch swallow (Faz 1 outbox YOK Wave 5+).
**W3.5A 4 Açık Karar (locked):** ICacheService minimum 3-metot (GetOrSetAsync HARİÇ race-condition impl-leak) · JSON System.Text.Json BCL · IConnectionMultiplexer Singleton (resmi pattern) · NuGet aligned (Memory 10.0.8 + Redis SemVer-range).
**W3.5A R3-A pattern:** RedisCacheService 2-call `StringSetAsync(key,json)` + `if (ttl.HasValue) await KeyExpireAsync(key, ttl.Value)`; atomicity loss cache layer'da kabul (Faz 1 best-effort).

**W3.5B kararları (yeni, locked):**
- **B.1 atomik DELETE pattern (F-S42 yakalama emsali):** Stub + davranış test tek mantıksal birim — stub silininca test semantiği kaybolur, atomik silme zorunlu. Wave 4+ benzer DELETE talimatlarında 4'lü grep self-check (prod + test + DI + endpoint usage) zorunlu.
- **B.2 EF projection patron dağılımı (%91 (A) onaylı, Wave 4+ emsali):** 18 saf (A) Select-in-query + 1 (B) ToList+in-memory tree (Children navigation YOK — Category aggregate boundary saf, Kural 1) + 1 (A.2) two-step (BrandCategory.Category navigation YOK — junction-only int FK) + 2 (A→A.2 fail-fallback) (A) tuttu (correlated sub-query EF Core 10 + Npgsql 10.0.1 başarılı translate). G3 amendment EF projection 3+ katman kapsamında (jsonb + Translations ValueConverter + provider materialize) — W3.5B kanıtı runtime-test edilmeli W3.7 host-wire sonrası.
- **B.2 enum dublication R-A patron (F-S45 emsali):** LocationLevel + BrandStatus + AttributeValueType Domain dosya-içi enum + Shared.Contracts.Catalog.Enums dublication (Wave 1 Sapma 25 invariant); projection `(Shared.Contracts.Catalog.X)entity.Field` cast veya namespace-qualified inline (W3.5B.3 BrandStatus emsali: `(Shared.Contracts.Catalog.BrandStatus)b.Status` cast + filter compare). using ekleme CS0104 ambiguous tetikler.
- **B.3 cursor pattern (Guid v7 chronological):** `OrderBy(b => b.Id).Where(b => b.Id > cursorValue).Take(PageSize + 1)` HasMore detect. Guid v7 monoton sıralı (UUID v7 high-48 bit ms timestamp). Npgsql uuid `>` operator translate — compile-time güvenli, runtime W3.7 test.
- **B.3 (B) ToList+in-memory zorunlu (F-S49 emsali):** jsonb dictionary `ContainsKey` LINQ emsal yok, EF Core 10 + Npgsql 10.0.1 belgesiz. Translations.Map.ContainsKey C# tarafında VO struct equality ile çalışır (LanguageCode readonly record struct). Dataset Faz 1 ~770 entity acceptable.
- **B.3 FallbackNamePreview VO native:** `Translations.FirstOrEmpty()` (Wave 1 invariant metot, Translations.cs:31) — TranslationHelper.Resolve dependency yok, dictionary boş ise empty string. Wave 4+ MissingTranslations emsali kanonik.
- **B.3 helper vs inline kararı:** `Func<T, Translations>` veya `Expression<...>` helper EF translate uyumsuz (lambda invocation projection içinde dispatch edilemez). Inline 4 bucket pattern (Categories/Breeds/Brands/Certifications) kod tekrarı ~16 satır kabul (W3.5B.2 emsali premature abstraction kaçınma, CS-Linq Expression vs Func semantik ayrımı).
- **B.4 lifetime Scoped × 2:** DbContext Scoped (EF Core default) + ReadService DbContext dependency → Scoped zorunlu (Singleton-in-Scoped capture memory leak + thread-safety). Cache decorator wrap W3.6 (Decorate<ICatalogReadService, CachedCatalogReadService>, Scrutor veya manuel factory).

## 9. Wave 3 Kalan Backlog (2 sub-batch + Grup 4 push + tag + final handover)

| Sub-batch | Kapsam (fiili dosya tahmini) | Doc dayanak | Notlar |
|---|---|---|---|
| **W3.6.A Cache Decorator** | `CachedCatalogReadService : ICatalogReadService` (22 metot read-through, W3.5A ICacheService wrap) · DI register decorator (Scrutor `Decorate<>` opsiyon veya manuel factory) · NuGet karar (Scrutor `[6.*, 7.0)` opsiyonel) · cache key pattern `"catalog:<entity>:<code>"` + TTL = 5-30 dk varsayılan = **~2 dosya** | §5:481-623 cache-aside Faz 1 | **1 batch tahminî**, decorator pattern Wave 4+ Listings/Marketplace emsali |
| **W3.6.B Rate Providers 3-tier** | `IRateProvider` (Application port) + `RateFetchResult` (record) + `TcmbRateProvider` (primary, TR cb) + `EcbRateProvider` (fallback, EU cb) + `ExchangeRateHostRateProvider` (tier 3, free API) · HttpClient retry+circuit breaker (Polly opsiyonel veya manuel) · NuGet `Microsoft.Extensions.Http` = **~5-6 dosya** | §6:629-730 RateProviders ns `Catalog.Infrastructure.RateProviders` | **1-2 batch tahminî**, 3-tier fallback strategy + HttpClient registration emsali |
| **W3.6.C Quartz Scheduler** | NuGet `Quartz` + `Quartz.Extensions.Hosting` · `CurrencyRateUpdateJob : IJob` (cron pattern, varsayılan `0 0 6 * * ?` günlük 06:00) · DI register Quartz scheduler + JobDataMap = **~2-3 dosya** | §6:629-730 + retention/cleanup §6:716 | **1 batch tahminî**, Quartz job pattern + DI hosting wire |
| **W3.6.D Domain Event Handlers** | In-process MassTransit.Mediator (Karar 3b) Brand/Category/Breed event subscriber'lar (Wave 1 events 8 + Wave 2 events 3 = 11 event handler) · `RateLogRepository.cs` (Append-only entity W3.1 RateLog + repository thin) · `RefreshExchangeRatesHandler` impl swap (Wave 2 NotImpl stub → IRateProvider chain + RateLogRepository persist) · `ICurrencyRateRefresher` Application portu (C.5#3 izinli imza Backend Adım 1 önerir) = **~5-7 dosya** | §6:629-730 + Wave 1/2 event/handler emsalleri | **1-2 batch tahminî**, event handler katmanı + rate refresh pipeline |
| **W3.7 Host-wire SON** | LivestockTrading.Api Program.cs M (`AddCatalogApplication()` + `AddCatalogInfrastructure(builder.Configuration)` + `MapAdminCatalogReadEndpoints` + 10 modül skeleton emsal + `NotImplemented501ExceptionHandler` retro 19 + DbContext registration W3.4 KARAR 4 (b)) + Shared.Infrastructure `BuildApp` varyantı (Karar 1.a use-case driven) + InfraModule finalize | 01-arch:185 + retro 19 (501 IExceptionHandler) + Karar 1.a (BuildApp use-case driven W3.7) | **Orta + KRİTİK**, host-inert SON erer, Api ilk boot; DI graph runtime resolve test (boot-time integration); cross-modül ICatalogReadService tüketici **0** (Wave 4+ Listings/Marketplace açar) |
| **Grup 4 push** | W3.6 (A+B+C+D) + W3.7 birlikte + `wave-3-complete` annotated tag | 28. push tatbikatı Wave 3 sonu Mustafa eli | tag obj→W3.7 commit |
| **Wave 3 final handover** | `_docs/wave-3-handover.md` (kapanış emsali Wave 2 `1f2c7dd`) + deviations.md reconcile (F-S23-F-S50 28 distinct + Wave 3 retro pattern'leri) | | Wave 4 kickoff devir teslim |

**W3.6 alt-batch tahmini toplam: 4-6 sub-batch** (W3.5B 4 emsali). Net Backend Adım 1 fresh read sonrası kesinleşir (yeni session).

## 10. Yeni Backend Session Disiplin Kuralları

**KAYDET:** 9 (doc-literal fresh read, extrapolation YASAK — **3rd-party API çok-katmanlı için G2/G4 erken tercih, G1 tek-katman**) · 10 (NuGet doc-conditional; Shared.* 0-NuGet mutlak) · 13 (Frontend grep'siz varsayım YASAK) · 14 (W1-4 port baskın, KAYDET emsali) · 17 (W3.0 retro use-case driven, BuildApp W3.7) · 23 (plan ≠ execution; eksik talimat → DUR+flag, self-author YASAK) · 25 (grep/build/runtime çıktı otorite, "muhtemelen" YASAK; **IDE diagnostic stale, derleyici otoritedir**) · 31+alt-varyant (kod-blok template YASAK; Frontend structural+scope, Backend impl tasarımı).

**W1 dersleri:** W1-1 overwrite-guard (mevcut dosya edit'inde fresh read) · W1-2 stat reconcile · W1-4 doc↔commit'li-kod çelişkisinde commit'li kod baskın.

**Aile taksonomisi (açık küme, Wave 3 retro 16 defekt):** 1 Tool-davranışı · 2 Algı/gerçek · 3 Talimat-tahmin (Wave 3 yoğun: F-S30/S31/S32/S34/S35/S38/F-S41/F-S43/F-S44 + F-S42 transport alt-tür) · 4 Disiplin-tutarsızlığı · 5 AI self-authorization + Mustafa transport sapması (F-S50 yeni alt-tür) · 6 Plan-doc↔kod / 3rd-party API çelişkisi (Wave 3 yoğun: F-S36/S39/S40/F-S45/F-S47/F-S49) · 7 Architectural-invariant evrim · 8 Plan-fazı tip-kimliği.

**Pozitif önleme/ders pattern (KALICI, Wave 4+):**
- **S28:** handover commit'leri ayrı ara push (sub-batch gruplarına yedirilmez)
- **S33:** Backend pattern sorgusuz almama + fresh read disipline + flag+DUR Frontend reconcile
- **S37:** transport-tekrarı 3+ eşik Backend aktif disambiguation sorgu (1/2/3 sorulu cevap, iki yol göster, kör retry yasak)
- **S14 emsali (W3.5B.3 atlanan F-S48):** Backend Adım 1'de Aile 6 önleme (enum dublication / VO accessor / namespace-qualified rutini proaktif check); S12→F-S45 dersinden tekrar olmadı, tek-shot 0/0 build başarısı
- **F-S25→F-S50 emsali (W3.5B Grup 3 push):** Mustafa raporuna körlemesine güven YASAK, Backend bağımsız `git fetch + ls-remote + rev-parse` cross-check zorunlu; tatbikat sayım/canonical SoT lag yakalanır, production safety zaten korunur (main INVARIANT push-hedef disiplini)
- **S11 muafiyeti (W3.5B.2 atlanan F-S46):** Interface impl sub-build CS0535 yapısal kaçınılmaz, tek-shot 22 metot yazılır + final build (sub-adım disipline literal uygulamak self-author riski yaratır; Frontend pragmatik yorum onayı)
- **Translations.FirstOrEmpty() VO native fallback (W3.5B.3 pozitif keşif):** Wave 1 Translations.cs:31 metot, dictionary boş ise empty string. Wave 4+ MissingTranslations / locale fallback emsali kanonik (TranslationHelper.Resolve dependency yok)
- **Helper vs inline EF translate kararı (W3.5B.3):** `Func<T, X>` / `Expression<...>` helper EF translate uyumsuz (lambda invocation projection içinde dispatch edilemez). Inline tekrar 16 satır kabul; CS-Linq Expression vs Func semantik ayrımı Wave 4+ kalıcı bilgi.

**G compile-test/doğrulama amendment'ları (KALICI, Wave 4+):**
- **G1:** tek-katman API namespace compile-test (W3.4 IMediator→IPublishEndpoint emsali)
- **G3:** 3+ katman API erken G2 (assembly reflection) veya G4 (alternatif pattern decomposition) tercih — W3.5A F-S40 sonrası kalıcı; W3.5B F-S45 enum dublication R-A patron uygulandı; W3.5B.3 S14 emsali Adım 1 proaktif check ile tekrar önlendi.

**Push disiplini:** `main` DOKUNULMAZ (production safety mutlak 30/30) · Backend push YAPMAZ (Mustafa terminal, Aile 5) · model = Backend hazırlık + Mustafa exec + Backend bağımsız post-push verify (fetch+ls-remote+rev-parse cross-check). **F-S50 kalıcı ders:** Mustafa raporuna körlemesine güven YASAK — Backend doğrulamasında uyumsuzluk varsa diagnostik komut bloğu sun + DUR + Mustafa düzeltici aksiyon bekle; production safety zaten korunduğu için (main INVARIANT push-hedef disiplini) tatbikat sayım/canonical SoT lag düşük-öncelik yakalanır.

**F-S37 transport-tekrarı disiplini:** Aynı talimat 3+ kez geliyorsa Backend kör retry YAPMAZ → mevcut DUR durumunu kısa özetle + (1)/(2)/(3) sorulu cevap iki yol göster, Mustafa'dan explicit disambiguation iste. Aynı kural Backend raporu self-echo'da da geçerli (W3.5A öncesi 4. tekrar varyantı emsali) ve Mustafa-Backend transport için de uygulanır (F-S50 emsali).

**Karar 6 gate güncellemesi (W3.5B.1 sonrası):** Test baseline 154 → **151** (3 ölü stub-davranış test silindi, NotImplementedException assert anlam kaybı). Yorum: "154 minus 3 ölü stub test = 151 yeni baseline W3.5B.1 sonrası". W3.6+ build gate test ≥151 PASS preserved.

## 11. W3.6 Kickoff Ön-Plan (yeni Backend session için, eski W3.5B ön-plan'ın yerine)

**W3.6 = Rate Refresh + Cache Decorator + Quartz + Event Handlers (~4-6 alt-batch, W3.5B emsali yoğun).**

Beklenen yaklaşım (Frontend tam talimat verecek — F-S26 emsali tek-blok; eksikse DUR):

1. **Adım 1 fresh read (KAYDET-9):**
   - W3.5A `ICacheService` fiili imzası (Application port + Memory/RedisCacheService Infra impl, 3 metot GetAsync/SetAsync/RemoveAsync) — W3.6.A cache decorator wrap için bağımlılık zinciri
   - W3.5B `CatalogReadService` 22 metot + `AdminCatalogReadService` 3 metot mevcut impl (decorator hedef interface)
   - `RateProvider` enum fiili (Shared.Contracts.Catalog.Enums/, TEK ns — F-S45 S12 emsali yok)
   - `RateLog` Infra entity + RateLogConfiguration (W3.1+W3.2 jsonb timestamptz PK Guid v7 ValueGeneratedNever index (RateDate,Source))
   - `Translations.FirstOrEmpty()` VO native (Wave 1 invariant, MissingTranslations emsali tekrar W3.6.D'de)
   - HttpClient registration emsali (Wave 2 modüllerinde IHttpClientFactory pattern var mı, Adım 1 grep zorunlu)
   - Quartz NuGet emsali (Wave 0+1+2'de Quartz usage YOK, ilk Wave 3'te eklenir — NuGet manual-pin Plan-1 lock + Quartz `[3.*, 4.0)` SemVer-range tercih)
   - **F-S30/S31/S34/S38/F-S41/F-S44 emsali Frontend pattern↔fiili-kod cross-check zorunlu** (Frontend KARAR'larında DTO/port/NuGet versiyon Wave 0/1/2 grep'le doğrulanmadıysa Backend flag — proaktif F-S33 + F-S37 + S14 patron)

2. **Adım 2 tasarım önerisi (alt-batch dekompozisyon Backend önerir, Frontend onay):**
   - **W3.6.A** Cache decorator (`CachedCatalogReadService` 22 metot read-through + DI Decorate<> veya manuel factory + cache key/TTL kararı)
   - **W3.6.B** Rate providers 3-tier (IRateProvider Application port + 3 concrete + HttpClient retry + RateFetchResult record)
   - **W3.6.C** Quartz scheduler (NuGet karar + IJob impl + cron pattern + DI hosting wire)
   - **W3.6.D** Domain event handlers + RateLogRepository + RefreshExchangeRatesHandler swap + ICurrencyRateRefresher port
   - Her alt-batch için ayrı Adım 2 tasarım onayı (F-S26 emsali tek-blok talimat)

3. **Adım 3-5 kod yazımı + build/test + commit hazırlığı** (W3.5B.2/3 emsali, S14 proaktif önleme rutini, helper vs inline EF translate dersi, S11 muafiyeti tek-shot interface impl)

4. **Adım 6 commit + Adım 7 verify + memory checkpoint** her alt-batch sonrası (mini-tur)

5. **Sonra Grup 4 push** (W3.6 + W3.7 birlikte, 28. tatbikat Mustafa eli) → Wave 3 final handover → Wave 4 kickoff devir teslim → `wave-3-complete` annotated tag

**Yeni Frontend session ilk aksiyon:** (1) bu doc'u (`_docs/wave-3-handover-mid.md` mid-handover-3) oku · (2) Backend handover özetini Mustafa'ya ilet · (3) Mustafa onay · (4) W3.6.A sub-batch tam talimat (tek büyük kod-bloğu, F-S26 emsali; pattern referansları MİNİMUM F-S34/S35/S38/F-S41/F-S44 dersi; Wave 0/1/2/3 fiili kod fresh grep zorunlu KAYDET-9 Frontend için de; path/klasör adlandırma F-S43 dersi her klasör fiili teyit).

**Yeni Backend session ilk aksiyon:** (1) bu doc'u oku + ham özet rapor · (2) fresh fetch + 4-SHA sanity (HEAD/main/origin-main/origin-rebuild-v2; main INVARIANT 30/30 veya 31/31 27. push sonrası) · (3) memory `wave3_plan1_decisions.md` + `MEMORY.md` hook kontrol (W3.5B kapanış kayıtları persist) · (4) Frontend W3.6.A tam talimat bekle, kendiliğinden W3.6 hazırlık YAPMA · (5) S14 emsali proaktif Adım 1 check rutini (enum dublication / VO accessor / NuGet version / namespace-qualified usage / 3rd-party API katman sayım) Aile 6 önleme kalıcı.

**Canonical SoT (handover sonrası):** bu doc (`_docs/wave-3-handover-mid.md` mid-handover-3) + `_docs/wave-2-handover.md` + memory `wave3_plan1_decisions.md` + `MEMORY.md` hook + Wave 3 commit zinciri (W3.0→W3.5B.4 13 commit + bu handover ara push 14. commit).
